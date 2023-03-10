using System;
using System.Collections.Generic;
using System.Text;
using CommonLibrary;
using DevExpress.XtraBars;
using DevExpress.XtraGrid;
using System.Windows.Forms;
using System.Data;
using System.Text.RegularExpressions;
using Payroll.DAL;

namespace Payroll.BLogic
{
    public class BaseMasterBase : Payroll.BLogic.BaseBL
    {
        #region Main Overrides

        public override int Add(System.Data.DataRow row)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override int Update(System.Data.DataRow row)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override int Delete(string code, string userLogin)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override System.Data.DataSet FetchAll()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override System.Data.DataRow Fetch(string code)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        #region masterfile structure

        public struct MasterFile
        { 
            public string iColumnName;
            public CommonEnum.DataEntryType iDataEntry;
            public CommonEnum.DataEntryControlType iControlType;
            public bool iIsNullable;
            public bool iIsAccountDetail;
            public string iAccountDetailTypeName;
            public MasterFile(string ColName, CommonEnum.DataEntryType DataEntry
                , CommonEnum.DataEntryControlType ControlType, bool IsNullable
                , bool IsAccountDetail, string AccountDetailTypeName)
            {
                iColumnName = ColName;
                iDataEntry = DataEntry;
                iControlType = ControlType;
                iIsNullable = IsNullable;
                iIsAccountDetail = IsAccountDetail;
                iAccountDetailTypeName = AccountDetailTypeName;
            }
        }

        #endregion

        #region Column list structure

        public struct ColumnLists
        {
            public string iGridColumnName;
            public string iDBColumnName;

            public ColumnLists(string ColumnName, string DBColumnName)
            {
                iGridColumnName = ColumnName;
                iDBColumnName = DBColumnName;
            }

        }

        #endregion

        #region Primary keys

        public string[] Primarykeys;

        #endregion

        #region Retrieve Data from structures and Primary key

        public virtual MasterFile GetMasterfileRecordFromMasterFileList(string ColumnName)
        {
            MasterFile mf = new MasterFile();
            for (int idx = 0; idx < MasterFiles.Count; idx++)
            {
                if (MasterFiles[idx].iColumnName == ColumnName)
                    return MasterFiles[idx];
            }
            return mf;
        }

        public virtual ColumnLists GetDataColumnRecordFromDataColumnList(string ColName)
        {
            ColumnLists clist = new ColumnLists();
            for (int idx = 0; idx < DataColumns.Count; idx++)
            {
                if (DataColumns[idx].iDBColumnName == ColName)
                    return DataColumns[idx];
            }
            return clist;
        }

        public virtual bool CheckIfColumnNameIsPrimaryKey(string Columnname)
        {
            bool ret = false;
            for (int idx = 0; idx < Primarykeys.Length; idx++)
            {
                if (Primarykeys[idx] == Columnname)
                {
                    ret = true;
                }
            }
            return ret;
        }

        #endregion

        #region variables

        public List<ColumnLists> DataColumns;

        public List<MasterFile> MasterFiles;

        public string MasterFileTableName = string.Empty;
        public string MasterFileQuery = string.Empty;
        public string userLogColumnName = string.Empty;
        public string userLogin = string.Empty;
        public string StatusColumnName = string.Empty;

        public HRCReportsBL HRCReportsBL = new HRCReportsBL();
        public HRCStatisticsBL HRCStatisticsBL = new HRCStatisticsBL();
        #endregion

        #region Initialization of objects

        public virtual DataSet InitializeGrid()
        {
            DataSet ds = GetResultFromMasterFileQuery();
            if (ds == null)
            {
                CommonProcedures.showMessageError("No data retrieved!");
            }
            else if (ds.Tables[0].Rows.Count == 0)
            {
                CommonProcedures.showMessageError("No Record retrived!");
            }
            return ds;

        }

        public virtual void SetUpInitialValuesForPanelObjects(System.Windows.Forms.Panel pnl)
        {
            if (this.MasterFiles != null && this.MasterFiles.Count > 0)
            {
                foreach (Control ctrl in pnl.Controls)
                {
                    if (ctrl is TextBox)
                    {
                        if (((TextBox)ctrl).Tag.ToString().Trim() != string.Empty)
                        {
                            InitializeTextBox(((TextBox)ctrl));
                        }
                    }
                    else if (ctrl is ComboBox)
                    {
                        if (((ComboBox)ctrl).Tag.ToString().Trim() != string.Empty)
                        {
                            InitializeComboBox(((ComboBox)ctrl));
                        }
                    }
                }
            }
            else
            {
                CommonProcedures.showMessageError("Master File Object must is not yet set up!");
            }
        }

        // Initializes the combobox objects in the Panel objects
        // the T_AccountDetail Adt_AccountType is very important
        public virtual void InitializeComboBox(ComboBox cmb)
        {
            bool flag = true;
            for (int idx = 0; idx < this.MasterFiles.Count && flag; idx++)
            {
                if (this.MasterFiles[idx].iColumnName == cmb.Tag
                    && this.MasterFiles[idx].iControlType == CommonEnum.DataEntryControlType.DROPDOWN)
                {
                    string query = string.Format(
                            @"
                                select 
                                    Adt_AccountDesc [Display]
                                    ,Adt_AccountCode [Value]
                                    from T_AccountDetail
                                    where Adt_AccountType = '{0}'
                                    and Adt_Status = 'A'
                            "
                            , this.MasterFiles[idx].iAccountDetailTypeName
                            );
                    
                    using (DALHelper dal = new DALHelper())
                    {
                        try
                        {
                            cmb.Items.Clear();
                            dal.OpenDB();
                            DataSet ds = dal.ExecuteDataSet(query, CommandType.Text);
                            cmb.DisplayMember = "Display";
                            cmb.ValueMember = "Value";
                            cmb.DataSource = ds.Tables[0];
                            if (cmb.Items.Count > 0)
                                cmb.SelectedIndex = 0;
                        }
                        catch (Exception er)
                        {
                            CommonProcedures.showMessageError(er.Message);
                        }
                        finally
                        {
                            dal.CloseDB();
                        }
                    }

                }
            }
        }

        // Initializes the textbox objects in the Panel objects
        // Sets the Key press properties
        public virtual void InitializeTextBox(TextBox txt)
        {
            bool flag = true;
            for (int idx = 0; idx < this.MasterFiles.Count && flag; idx++)
            {
                if (
                    this.MasterFiles[idx].iColumnName == txt.Tag
                    && this.MasterFiles[idx].iControlType == CommonEnum.DataEntryControlType.TEXTBOX
                    )
                {
                    switch (this.MasterFiles[idx].iDataEntry)
                    {
                        case CommonEnum.DataEntryType.ALLKEYTYPES:
                            txt.CharacterCasing = CharacterCasing.Upper;
                            break;
                        case CommonEnum.DataEntryType.NUMERIC:
                            txt.KeyPress += new KeyPressEventHandler(TextBox_KeyPress_NUMERIC);
                            break;
                        case CommonEnum.DataEntryType.ALPHANUMERIC:
                            txt.KeyPress += new KeyPressEventHandler(TextBox_KeyPress_ALPHANUMERIC);
                            break;
                        case CommonEnum.DataEntryType.STRINGNUMERIC:
                            txt.KeyPress += new KeyPressEventHandler(TextBox_KeyPress_STRINGNUMERIC);
                            break;
                        case CommonEnum.DataEntryType.DECIMALAMOUNT:
                            txt.KeyPress += new KeyPressEventHandler(TextBox_KeyPress_DECIMALAMOUNT);
                            break;
                    }


                    flag = false;
                }
            }
        }

        #region Textbox key presses

        public void TextBox_KeyPress_DECIMALAMOUNT(object sender, KeyPressEventArgs e)
        {
            string strToCheck = Convert.ToChar(e.KeyChar).ToString();
            if (strToCheck.Trim().Equals(string.Empty) || strToCheck.StartsWith("-") || strToCheck.StartsWith(".") || strToCheck.StartsWith(","))//edited by louie 20070227
                e.Handled = true;

            Regex objNumercPattern = new Regex("^([0-9]*(, *)?)*[.]?[0-9]*$");
            if (!objNumercPattern.IsMatch(strToCheck))
                e.Handled = true;
        }

        public void TextBox_KeyPress_STRINGNUMERIC(object sender, KeyPressEventArgs e)
        {
            Regex objAlphaNumericPattern = new Regex("^[A-Za-z0-9 ]*$");
            if (!objAlphaNumericPattern.IsMatch(Convert.ToChar(e.KeyChar).ToString()))
                e.Handled = true;
        }

        public void TextBox_KeyPress_ALPHANUMERIC(object sender, KeyPressEventArgs e)
        {
            Regex objAlphaNumericPattern = new Regex("^[A-Za-z0-9 ]*$");
            if (!objAlphaNumericPattern.IsMatch(Convert.ToChar(e.KeyChar).ToString()))
                e.Handled = true;
        }

        public void TextBox_KeyPress_NUMERIC(object sender, KeyPressEventArgs e)
        {
            Regex objNumericPattern = new Regex("^([0-9]{0,7})[^0-9]([0-9]*)?$");
            if (!objNumericPattern.IsMatch(Convert.ToChar(e.KeyChar).ToString()))
                e.Handled = true;
        }

        #endregion

        #endregion

        #region Event when grid selection changed

        // This will change all the values of objects in pnlObjects according to the selected gridview row
        // (on grid view click, assign values to panel)
        public virtual void GridViewSelectionChangeEvents(DataRow dr, System.Windows.Forms.Panel pnl)
        {
            for (int idx2 = 0; idx2 < dr.ItemArray.Length ; idx2++)
            {
                string val = dr[idx2].ToString();
                string GridColName = dr.Table.Columns[idx2].Caption;
                string ObjectTagName = GetObjectTagName(GridColName);
                if (ObjectTagName != string.Empty)
                    SetupDataForPanelObjects(pnl, val, ObjectTagName);
            }
        }

        // This will get the corresponding Tag name of an object from the given column name
        // from the grid row  (Para mag match ang data)
        public virtual string GetObjectTagName(string GridColName)
        {
            string DBTagName = string.Empty;
            if (this.DataColumns.Count > 0)
            {
                for (int idx = 0; idx < this.DataColumns.Count; idx++)
                {
                    if (this.DataColumns[idx].iGridColumnName == GridColName)
                    {
                        DBTagName = this.DataColumns[idx].iDBColumnName;
                    }
                }
            }
            return DBTagName;
        }

        // This will put the values from the grid to the corresponding object in the panel
        // using the Tag name
        public virtual void SetupDataForPanelObjects(System.Windows.Forms.Panel pnl, string val, string ObjectTagName)
        {
            foreach (Control ctrl in pnl.Controls)
            {
                if (ctrl is TextBox)
                {
                    #region for textbox

                    if (((TextBox)ctrl).Tag.ToString().Trim() != string.Empty
                        && ((TextBox)ctrl).Tag == ObjectTagName)
                    {
                        ((TextBox)ctrl).Text = val;
                    }

                    #endregion
                }
                else if (ctrl is ComboBox)
                {
                    #region for combobox

                    if (((ComboBox)ctrl).Tag.ToString().Trim() != string.Empty
                        && ((ComboBox)ctrl).Tag.ToString() == ObjectTagName)
                    {
                        ((ComboBox)ctrl).SelectedIndex = 0;
                        int cmbidx = 0;
                        cmbidx = ((ComboBox)ctrl).FindString(val);
                        ((ComboBox)ctrl).SelectedIndex = cmbidx;

                    }

                    #endregion
                }
                
            }
        }

        #endregion

        #region Queries

        public virtual bool InsertRecord(Panel pnl)
        {
            bool isSuccess = false;

            #region Formulate Query

            string strInsertheader = "Start";
            string strInsertBody = "Start";
            foreach (Control ctrl in pnl.Controls)
            {
                if (Convert.ToString(ctrl.Tag) != string.Empty)
                {
                    string[] values = GetValuesOfControl(ctrl);
                    strInsertheader += "," + values[1].Trim() + "\n";
                    strInsertBody += "," + values[0].Trim() + "\n";
                }
            }
            if (strInsertheader != "Start" && strInsertBody != "Start")
            {
                strInsertheader = strInsertheader.Replace("Start," , "");
                strInsertBody = strInsertBody.Replace("Start,", "");
                strInsertheader += "\n ," + userLogColumnName;
                strInsertheader += "\n ,Ludatetime";
                strInsertBody += "\n ,'" + userLogin + "'";
                strInsertBody += "\n , GETDATE()";

                using (DALHelper dal = new DALHelper())
                {
                    try
                    {
                        dal.OpenDB();
                        dal.BeginTransaction();
                        string query = @"
                            insert into {0} (
                                     {1}
                                    )
                               values(
                                     {2}
                                    )";
                        query = string.Format(query, MasterFileTableName, strInsertheader, strInsertBody);
                        dal.ExecuteNonQuery(query, CommandType.Text);
                        dal.CommitTransaction();
                        CommonProcedures.showMessageInformation("Successfully Inserted New Record!");
                        isSuccess = true;
                    }
                    catch(Exception er)
                    {
                        isSuccess = false;
                        dal.RollBackTransaction();
                        CommonProcedures.showMessageError(er.Message);
                    }
                    finally
                    {
                        dal.CloseDB();
                    }
                }
            }
            

            #endregion

            return isSuccess;
        }

        public virtual bool UpdateRecord(Panel pnl)
        {
            bool isSuccess = false;

            #region Formulate Query

            string strUpdateHeader = "Start";
            string strUpdateBody = "Start";
            foreach (Control ctrl in pnl.Controls)
            {
                if (Convert.ToString(ctrl.Tag) != string.Empty)
                {
                    string[] values = GetValuesOfControl(ctrl);
                    if (!CheckIfColumnNameIsPrimaryKey(values[1]))
                    {
                        strUpdateHeader += ", " + values[1] + " = " + values[0] + @"
                                    ";
                    }
                    else
                    {
                        strUpdateBody += "And " + values[1] + " = " + values[0] + @"
                                    ";
                    }
                }
            }
            if (strUpdateHeader != "Start")
            {
                strUpdateHeader = strUpdateHeader.Replace("Start,", "Set ");
                strUpdateHeader += "\n ," + userLogColumnName + " = '" + userLogin + "'";
                strUpdateHeader += "\n , Ludatetime = GETDATE()";
                if (strUpdateBody == "Start")
                {
                    strUpdateBody = string.Empty;
                }
                else
                { 
                    strUpdateBody = strUpdateBody.Replace("StartAnd", "Where ");
                }

                    using (DALHelper dal = new DALHelper())
                    {
                        try
                        {
                            dal.OpenDB();
                            dal.BeginTransaction();
                            string query = @"
                                Update {0}
                                    {1}
                                    {2}
                            ";
                            query = string.Format(query, MasterFileTableName, strUpdateHeader, strUpdateBody);
                            dal.ExecuteNonQuery(query, CommandType.Text);
                            dal.CommitTransaction();
                            CommonProcedures.showMessageInformation("Successfully updated record!");
                            isSuccess = true;
                        }
                        catch (Exception er)
                        {
                            isSuccess = false;
                            dal.RollBackTransaction();
                            CommonProcedures.showMessageError(er.Message);
                        }
                        finally
                        {
                            dal.CloseDB();
                        }
                    }
            }


            #endregion

            return isSuccess;
        }

        public virtual bool DeleteRecord(Panel pnl)
        {
            bool isSuccess = false;

            #region Formulate Query

            string strDeleteheader = "Start";
            foreach (Control ctrl in pnl.Controls)
            {
                if (Convert.ToString(ctrl.Tag) != string.Empty)
                {
                    string[] values = GetValuesOfControl(ctrl);
                    strDeleteheader += "And " + values[1].Trim() + " = " + values[0].Trim() + "\n";
                }
            }
            if (strDeleteheader != "Start")
            {
                strDeleteheader = strDeleteheader.Replace("StartAnd", "Where ");

                using (DALHelper dal = new DALHelper())
                {
                    try
                    {
                        dal.OpenDB();
                        dal.BeginTransaction();
                        string query = @"
                            delete from {0}
                            {1}
                                    ";
                        query = string.Format(query, MasterFileTableName, strDeleteheader);
                        dal.ExecuteNonQuery(query, CommandType.Text);
                        dal.CommitTransaction();
                        CommonProcedures.showMessageInformation("Successfully Deleted Record!");
                        isSuccess = true;
                    }
                    catch (Exception er)
                    {
                        isSuccess = false;
                        dal.RollBackTransaction();
                        CommonProcedures.showMessageError(er.Message);
                    }
                    finally
                    {
                        dal.CloseDB();
                    }
                }
            }


            #endregion

            return isSuccess;
        }

        public virtual bool CancelStatusRecord(Panel pnl)
        {
            bool isSuccess = false;

            #region Formulate Query

            string strUpdateHeader = "Set " + StatusColumnName + " = 'C'";
            string strUpdateBody = "Start";
            foreach (Control ctrl in pnl.Controls)
            {
                if (Convert.ToString(ctrl.Tag) != string.Empty)
                {
                    string[] values = GetValuesOfControl(ctrl);
                    strUpdateBody += "And " + values[1] + " = " + values[0] + @"
                                    ";
                }
            }
            if (strUpdateBody != "Start")
            {
                strUpdateHeader += "\n ," + userLogColumnName + " = '" + userLogin + "'";
                strUpdateHeader += "\n , Ludatetime = GETDATE()";
                if (strUpdateBody == "Start")
                {
                    strUpdateBody = string.Empty;
                }
                else
                {
                    strUpdateBody = strUpdateBody.Replace("StartAnd", "Where ");
                }

                using (DALHelper dal = new DALHelper())
                {
                    try
                    {
                        dal.OpenDB();
                        dal.BeginTransaction();
                        string query = @"
                                Update {0}
                                    {1}
                                    {2}
                            ";
                        query = string.Format(query, MasterFileTableName, strUpdateHeader, strUpdateBody);
                        dal.ExecuteNonQuery(query, CommandType.Text);
                        dal.CommitTransaction();
                        CommonProcedures.showMessageInformation("Successfully deleted record!");
                        isSuccess = true;
                    }
                    catch (Exception er)
                    {
                        isSuccess = false;
                        dal.RollBackTransaction();
                        CommonProcedures.showMessageError(er.Message);
                    }
                    finally
                    {
                        dal.CloseDB();
                    }
                }
            }


            #endregion

            return isSuccess;
        }

        public virtual string[] GetValuesOfControl(Control ctrl)
        { 
            string [] str = new string[2];
            if (ctrl is TextBox)
            {
                #region If Textbox

                str[0] = ((TextBox)ctrl).Text.Trim();
                str[0] = str[0].Replace("'", "''");
                str[1] = ((TextBox)ctrl).Tag.ToString().Trim();
                MasterFile mf = GetMasterfileRecordFromMasterFileList(str[1]);
                if (mf.iDataEntry != CommonEnum.DataEntryType.DECIMALAMOUNT
                    && mf.iDataEntry != CommonEnum.DataEntryType.NULL
                    && mf.iDataEntry != CommonEnum.DataEntryType.BOOL)
                {
                    str[0] = "'" + str[0] + "'";
                }

                #endregion
            }
            else if (ctrl is ComboBox)
            {
                str[0] = "'" + ((ComboBox)ctrl).SelectedValue.ToString().Trim() + "'";
                str[1] = ((ComboBox)ctrl).Tag.ToString().Trim();
            }
            return str;
        }

        #endregion

        #region Additional Queries

        protected virtual void AdditionalInsertQuery(DALHelper dal)
        {
            
        }

        protected virtual void AdditionalUpdateQuery(DALHelper dal)
        {
            
        }

        protected virtual void AdditonalDeleteQuery(DALHelper dal)
        {
            
        }

        protected virtual void AdditionalCancelQuery(DALHelper dal)
        { 
        
        }
        #endregion

        #region Add Record

        #region Initialization

        public virtual void SetFormToNewState(System.Windows.Forms.Panel pnl)
        {
            ClearFieldofPanel(pnl);
        }

        public virtual void ClearFieldofPanel(System.Windows.Forms.Panel pnl)
        {
            EnableObjectsInPanel(true, pnl);
            foreach (Control ctrl in pnl.Controls)
            {
                if (ctrl is TextBox)
                {
                    ((TextBox)ctrl).Text = string.Empty;
                }
                if (ctrl is ComboBox)
                {
                    ((ComboBox)ctrl).SelectedIndex = 0;
                }
                if (ctrl is CheckBox)
                {
                    ((CheckBox)ctrl).Checked = false;
                }
            }
        }

        #endregion

        #region Check for Saving

        public virtual bool CheckValuesBeforeInserting(System.Windows.Forms.Panel pnl)
        {
            bool ret = true;
            if (CheckFieldsIfEmpty(pnl))
            {
                ret = false;
            }
           
            if(CheckForPrimaryKeyDuplication(pnl))
            {
                ret = false;
            }
            return ret;
        }

        public virtual bool CheckForPrimaryKeyDuplication(System.Windows.Forms.Panel pnl)
        {
            bool ret = false;
            string strSearch = "Start";
            string Errmsg = "Start";
            foreach (Control ctrl in pnl.Controls)
            {
                if (Convert.ToString(ctrl.Tag) != string.Empty)
                {
                    if (CheckIfColumnNameIsPrimaryKey(Convert.ToString(ctrl.Tag)))
                    {
                        string[] values = GetValuesFromObjectUsingColumnName(pnl, Convert.ToString(ctrl.Tag));
                        if (values[1].ToString().Trim() != string.Empty)
                        {
                            strSearch += "And " + values[0] + " = " + values[1] + @"
                                    ";
                            Errmsg += "- " + values[2] + "\n";
                        }
                    }
                }
            }
            if (strSearch != "Start")
            {
                strSearch = strSearch.Replace("StartAnd", "Where ");
                using (DALHelper dal = new DALHelper())
                {
                    try
                    {
                        dal.OpenDB();
                        string query = @" select * from {0} 
                                        {1}";
                        query = string.Format(query, MasterFileTableName, strSearch);
                        DataSet ds = dal.ExecuteDataSet(query, CommandType.Text);
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            CommonProcedures.showMessageError("Cannot Insert Duplicate Primary keys!\nPlease check the following: \n" + Errmsg.Replace("Start-", "-"));
                            ret = true;
                        }
                    }
                    catch (Exception er)
                    {
                        ret = true;
                        CommonProcedures.showMessageError(er.Message);
                    }
                    finally
                    {
                        dal.CloseDB();
                    }
                }
            }
            else
            {
                ret = true;
            }
            return ret;
        }

        public virtual string[] GetValuesFromObjectUsingColumnName(System.Windows.Forms.Panel pnl, string Colname)
        {
            string[] str = new string[3];
            foreach (Control ctrl in pnl.Controls)
            {
                if (ctrl is TextBox)
                {
                    if (((TextBox)ctrl).Tag.ToString().Trim() != string.Empty
                        && ((TextBox)ctrl).Tag.ToString().Trim() == Colname)
                    {
                        str[0] = ((TextBox)ctrl).Tag.ToString().Trim();
                        str[1] = ((TextBox)ctrl).Text.ToString();
                        str[1] = str[1].Replace("'", "''");
                        MasterFile mf = GetMasterfileRecordFromMasterFileList(str[0]);
                        if (mf.iDataEntry != CommonEnum.DataEntryType.DECIMALAMOUNT
                            && mf.iDataEntry != CommonEnum.DataEntryType.NULL 
                            && str[1].ToString().Trim() != string.Empty)
                        {
                            str[1] = "'" + str[1] + "'";
                        }

                    }
                }
                else if (ctrl is ComboBox)
                {
                    if (((ComboBox)ctrl).Tag.ToString().Trim() != string.Empty
                        && ((ComboBox)ctrl).Tag.ToString().Trim() == Colname)
                    {
                        str[0] = ((ComboBox)ctrl).Tag.ToString().Trim();
                        str[1] = "'" + ((ComboBox)ctrl).SelectedValue.ToString() + "'";
                    }
                }
            }
            if (str[0] != string.Empty)
            {
                ColumnLists cl = GetDataColumnRecordFromDataColumnList(str[0]);
                str[2] = cl.iGridColumnName;
            }

            return str;
        }

        #endregion

        #region Save New Recod

        public virtual bool SaveNewRecord(Panel pnl)
        {
            bool isSuccess = false;
            isSuccess = InsertRecord(pnl);
            return isSuccess;
        }

        #endregion

        #endregion

        #region Modify Event

        #region Initialization

        public virtual void SetFormToModifyState(System.Windows.Forms.Panel pnl)
        {
            EnableObjectsInPanel(true, pnl);
            DisablePrimaryKeyField(pnl);
        }

        public virtual void DisablePrimaryKeyField(System.Windows.Forms.Panel pnl)
        {
            foreach (Control ctrl in pnl.Controls)
            {
                if (Convert.ToString(ctrl.Tag) != string.Empty)
                {
                    for (int idx = 0; idx < Primarykeys.Length; idx++)
                    {
                        if (Convert.ToString(ctrl.Tag) == Primarykeys[idx].Trim())
                        {
                            ctrl.Enabled = false;
                        }
                    }
                }
            }
        }

        public virtual void EnableObjectsInPanel(bool isEnabled, System.Windows.Forms.Panel pnlObjects)
        {
            foreach (Control ctrl in pnlObjects.Controls)
            {
                ctrl.Enabled = isEnabled;
            }
        }

        #endregion

        #region Check for Saving

        public virtual bool SetFormToSaveState(System.Windows.Forms.Panel pnl)
        {
            return CheckValuesBeforeSaving(pnl);
        }

        public virtual bool CheckValuesBeforeSaving(System.Windows.Forms.Panel pnl)
        {
            bool isValid = false;
            if (!CheckFieldsIfEmpty(pnl))
            {
                isValid = true;
            }
            return isValid;
        }

        public virtual bool CheckFieldsIfEmpty(System.Windows.Forms.Panel pnl)
        {
            string ErrMsg = string.Empty;
            bool Errflag = false;
            foreach (Control ctrl in pnl.Controls)
            {
                if (ctrl is TextBox)
                {
                    if (((TextBox)ctrl).Tag.ToString().Trim() != string.Empty)
                    {
                        MasterFile mf = GetMasterfileRecordFromMasterFileList(((TextBox)ctrl).Tag.ToString().Trim());
                        ColumnLists clist = GetDataColumnRecordFromDataColumnList(((TextBox)ctrl).Tag.ToString().Trim());
                        if (((TextBox)ctrl).Text.Trim() == string.Empty && !mf.iIsNullable)
                        {
                            ErrMsg += "\n - " + clist.iGridColumnName + " should not be Empty";
                            Errflag = true;
                        }
                        
                    }
                }
            }
            if (ErrMsg != string.Empty)
            {
                CommonProcedures.showMessageError("Pleas correct the following errors: " + ErrMsg);                
            }
            return Errflag;
        }

        #endregion

        #region Save record

        public virtual bool SaveModifiedRecord(Panel pnl)
        {
            bool isSuccess = false;
            isSuccess = UpdateRecord(pnl);
            return isSuccess;
        }

        #endregion

        #endregion

        #region Delete Record

        public virtual bool SetupDeletionProcess(System.Windows.Forms.Panel pnl)
        {
            bool isSuccess = false;
            isSuccess = DeleteRecord(pnl);
            return isSuccess;
        }

        public virtual bool SetupCancellationProcess(System.Windows.Forms.Panel pnl)
        {
            bool isSuccess = false;
            isSuccess = CancelStatusRecord(pnl);
            return isSuccess;
        }

        #endregion

        #region Print Event
        
        public DataSet GetCompanyData()
        {
            DataSet ds = ExecuteDataSetQuery(
                @"
                    select 
	Ccd_CompanyName
	,Ccd_CompanyAddress1 + isnull(Adt_AccountDesc, '')
	,Ccd_TelephoneNo
from T_CompanyMaster
left join T_AccountDetail
on Adt_AccountCode = Ccd_CompanyAddress3
and Adt_AccountType = 'ZIPCODE'

                "
                );
            return ds;
        }

        #endregion

        #region Built in Query Executor

        public virtual DataSet GetResultFromMasterFileQuery()
        {
            DataSet ds = null;
            if (MasterFileQuery != string.Empty)
            {
                ds = ExecuteDataSetQuery(MasterFileQuery);
            }
            else
            {
                CommonProcedures.showMessageError("Master File Query not yet set!");
            }
            return ds;
        }

        public virtual DataSet ExecuteDataSetQuery(string query)
        {
            DataSet dsret = null;
            using (DALHelper dal = new DALHelper())
            {
                try
                {
                    dal.OpenDB();
                    dsret = dal.ExecuteDataSet(query, CommandType.Text);
                }
                catch(Exception er)
                {
                    CommonProcedures.showMessageError(er.Message);
                }
                finally
                {
                    dal.CloseDB();
                }
            }
            return dsret;
        }

        public virtual bool ExecuteNonQuery(string query)
        {
            bool ret = false;
            using (DALHelper dal = new DALHelper())
            {
                try
                {
                    dal.OpenDB();
                    dal.BeginTransaction();
                    dal.ExecuteNonQuery(query, CommandType.Text);
                    dal.CommitTransaction();
                    ret = true;
                }
                catch (Exception er)
                {
                    ret = false;
                    dal.RollBackTransaction();
                    CommonProcedures.showMessageError(er.Message);
                }
                finally
                {
                    dal.CloseDB();
                }
            }
            return ret;
        }

        public string FormulateInsertQuery(Panel pnl, string tableName, string user, string userLogName)
        {
            string FinalQuery = string.Empty;
            string strInsertheader = "Start";
            string strInsertBody = "Start";
            foreach (Control ctrl in pnl.Controls)
            {
                if (Convert.ToString(ctrl.Tag) != string.Empty)
                {
                    string[] values = GetValuesOfControl(ctrl);
                    strInsertheader += "," + values[1].Trim() + "\n";
                    strInsertBody += "," + values[0].Trim() + "\n";
                }
            }
            if (strInsertheader != "Start" && strInsertBody != "Start")
            {
                strInsertheader = strInsertheader.Replace("Start,", "");
                strInsertBody = strInsertBody.Replace("Start,", "");
                strInsertheader += "\n ," + userLogName;
                strInsertheader += "\n ,Ludatetime";
                strInsertBody += "\n ,'" + user + "'";
                strInsertBody += "\n , GETDATE()";

                FinalQuery = string.Format( @"Insert into {0} 
                                   (
                                    {1}
                                    )
                                Values(
                                    {2}
                                    )",tableName, strInsertheader, strInsertBody);
            }
            return FinalQuery;
        }

        #endregion

        public string encryptPassword(string password)
        {
            System.Security.Cryptography.MD5CryptoServiceProvider md5Pass = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] arrB;
            StringBuilder sb = new StringBuilder(String.Empty);

            arrB = md5Pass.ComputeHash(Encoding.ASCII.GetBytes(password));

            foreach (byte b in arrB)
            {
                sb.Append(b.ToString("x").PadLeft(2, '0'));
            }

            md5Pass.Clear();

            return sb.ToString().Substring(0, CommonConstants.Misc.PasswordLength);
        }
    }
}
