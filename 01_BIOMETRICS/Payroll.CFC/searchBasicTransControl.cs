using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using Payroll.BLogic;
using CommonLibrary; //added by manuel 2008/05/16
namespace Payroll.CFC
{
    public partial class searchBasicTransControl : UserControl
    {
        private bool enableControls;
        private string searchQuery;
        private DataSet dsLookup;
        private int numColumns = 0;
        private int[] sizeColumns;
        //September 18, 2006
        //Genz
        //Start
        private DataView dtView;
        //End

        public searchBasicTransControl()
        {
            InitializeComponent();

            this.searchQuery = string.Empty;
            this.dsLookup = new DataSet();
        }

        #region property
        public int NumberOfColumns
        {
            get
            {
               return numColumns;
            }
            set
            {
                this.numColumns = value;

            }
           
        }
        public new bool Enabled
        {
            get
            {
                return enableControls;
            }
            set
            {
                this.enableControls = value;
                this.setControlEnabled(this.enableControls);
            }

        }

        public event DataGridViewCellMouseEventHandler GridCellDoubleClickEvent
        {
            add
            {
                this.gridLookup.CellMouseDoubleClick += value;
            }
            remove
            {
                this.gridLookup.CellMouseDoubleClick -= value;
            }
        }

        public event EventHandler GridSelectionChange
        {
            add
            {
                this.gridLookup.SelectionChanged  += value;
            }
            remove
            {
                this.gridLookup.SelectionChanged -= value;
            }
        }

        public event EventHandler PopUpSearchClickEvent
        {
            add
            {
                this.btnPopUpSearch.Click += value;
            }
            remove
            {
                this.btnPopUpSearch.Click -= value;
            }
        }

        public DataRowCollection Rows
        {
            get
            {
                if (this.dsLookup.Tables.Count > 0)
                    return this.dsLookup.Tables[0].Rows;
                else
                    return null;
            }
        }

        public int SelectedRowIndex
        {
            get
            {
                return getDataSetRowIndex();

                /*if (this.gridLookup.SelectedRows.Count > 0)
                    return this.gridLookup.SelectedRows[0].Index;
                else
                    return -1;*/
            }
        }

        public string SelectRow
        {
            set
            {
                for (int i = 0; i < this.gridLookup.Rows.Count; i++)
                {
                    if (this.gridLookup.Rows[i].Cells[0].Value.ToString().Trim().Equals(value))
                    {
                        this.gridLookup.Rows[i].Selected = true;
                        this.gridLookup.Rows[i].Cells[0].Selected = true;
                        break;
                    }
                }
            }
        }

        

        public int SelectedRowsCount
        {
            get
            {
                return this.gridLookup.SelectedRows.Count;
            }
        }

        public string SearchQuery
        {
            get
            {
                return this.searchQuery;
            }
            set
            {
                this.searchQuery = value;

                if (!value.Equals(string.Empty))
                {
                    string lookupCode = this.txtLookupCode.Text.Trim();

                    fetchLookupData(lookupCode);
                }
            }
        }

        //jeph added 02172007 12:26 PM
        public bool EnableLookUpControls
        {
            set
            {
                this.txtLookupCode.Enabled = value;
                this.gridLookup.Enabled = value;
                this.btnSearch.Enabled = value;
            }
        }
        //end

        #endregion//property

        #region private

        private void fetchLookupData(string lookupCode)
        
        {
            string selectQuery = this.searchQuery;

            GenericLookupBL lookup = new GenericLookupBL(selectQuery);

            this.dsLookup = lookup.GetDisplayData(lookupCode);
            this.gridLookup.DataSource = this.dsLookup.Tables[0];

            this.setGridAppearance();
        }

        private void setGridAppearance()
        {
            this.gridLookup.RowHeadersWidth = 25;
            this.gridLookup.AllowUserToAddRows = false;
            this.gridLookup.AllowUserToDeleteRows = false;
            this.gridLookup.AllowUserToResizeRows = false;
            this.gridLookup.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            //this.gridLookup.Columns[0].Width = 100;//edited by louie 01112007
            //this.gridLookup.Columns[0].ReadOnly = true;
            ////this.gridLookup.Columns[1].Width = 107;//Arthur Deleted 2006083101PM
            ////this.gridLookup.Columns[1].Width = 90;//Arthur Inserted 2006083101PM
            //this.gridLookup.Columns[1].Width = 100;//Seldon Inserted 20070123
            //this.gridLookup.Columns[1].ReadOnly = true;
            ////this.gridLookup.Columns[2].Width = 70;//Arthur Deleted 2006083101PM
            ////this.gridLookup.Columns[2].Width = 90;//Arthur Inserted 2006083101PM//edited seldon deleted
            //this.gridLookup.Columns[2].ReadOnly = true;
            //this.gridLookup.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill; // seldon inserted 01/23/2007
            ////September 18, 2006
            ////Genz
            ////Start
            this.gridLookup.DataSource = this.dsLookup.Tables[0].DefaultView;
            this.dtView = this.dsLookup.Tables[0].DefaultView;
            ////End
            ////sept 19, 2006
            ////carlo
            ////start - added property to fourth column  
            //if (gridLookup.ColumnCount == 4)
            //    gridLookup.Columns[3].Visible = false;
            //end
            int count = 0;
            int size = 100;

            for(int i=0;i<this.numColumns;i++)
            {
                count += 1;
                this.gridLookup.Columns[i].ReadOnly = true;

                if (this.sizeColumns != null)
                    size = Convert.ToInt16(this.sizeColumns[i]);

                this.gridLookup.Columns[i].Width = size;
               

                if (count == this.numColumns)
                {
                    //this.gridLookup.Columns[i].Width = 50;
                    this.gridLookup.Columns[i].ReadOnly = true;
                    this.gridLookup.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    
                }

                
            }
        }

        private void setControlEnabled(bool enabled)
        {
            this.gridLookup.Enabled = enabled;
            this.panel1.Enabled = enabled;
        }

        private bool isSelectedRowLastRow()
        {
            if (this.gridLookup.SelectedRows.Count > 0 && this.gridLookup.SelectedRows[0].Index == this.gridLookup.Rows.Count - 1)
                return true;
            else
                return false;
        }

        private bool isSelectedRowFirstRow()
        {
            if (this.gridLookup.SelectedRows.Count > 0 && this.gridLookup.SelectedRows[0].Index == 0) 
            {
                return true;
            }
            else
                return false;
        }

        #endregion//private

        #region public

        //**********************************************************************************************
        //JEPH INSERTED 03212007. ADD THE METHOD TO SELECT ROW HAVING 2 PRIMARY KEY.
        //**********************************************************************************************
        public void SelectRowMethod(string[] strSearch)
        {
            for (int i = 0; i < this.gridLookup.Rows.Count; i++)
            {
                if (this.gridLookup.Rows[i].Cells[0].Value.ToString().Trim().Equals(strSearch[0]) &&
                    this.gridLookup.Rows[i].Cells[1].Value.ToString().Trim().Equals(strSearch[1]))
                {
                    this.gridLookup.Rows[i].Selected = true;
                    this.gridLookup.Rows[i].Cells[0].Selected = true;
                    break;
                }
            }
        }
        //**********************************************************************************************

        public void ReQuery()
        {
            fetchLookupData(this.txtLookupCode.Text.Trim());
        }

        public void SetColumns(int numColumns, params int[] sizeColumns)
        {
            this.numColumns = numColumns;
            this.sizeColumns = sizeColumns;
        }

        

        public string getNextRowCode(int indexColumn)
        {
            if (this.gridLookup.SelectedRows.Count > 0)
            {
                //if (isSelectedRowFirstRow())
                //    return this.gridLookup.Rows[this.gridLookup.SelectedRows[0].Index + 1].Cells[indexColumn].Value.ToString();
                //else if (isSelectedRowLastRow())
                //    return this.gridLookup.Rows[this.gridLookup.SelectedRows[0].Index - 1].Cells[indexColumn].Value.ToString();
                //else
                //    return String.Empty;

                if (this.gridLookup.SelectedRows.Count == 1)
                    return String.Empty;
                else if (isSelectedRowLastRow())
                    return this.gridLookup.Rows[this.gridLookup.SelectedRows[0].Index - 1].Cells[indexColumn].Value.ToString();
                else
                    return this.gridLookup.Rows[this.gridLookup.SelectedRows[0].Index + 1].Cells[indexColumn].Value.ToString();

            }
            else
                return String.Empty;
        }

        #endregion//public

        private void btnSearch_Click(object sender, EventArgs e)
        {
            //September 19, 2006
            //Genz
            //Start
            fetchLookupData(String.Empty);
            //End
        }

        private void btnSearchClear_Click(object sender, EventArgs e)
        {
            this.txtLookupCode.Text = string.Empty;
            this.dsLookup.Tables[0].Rows.Clear();
        }

        private int getDataSetRowIndex()
        {
            DataGridViewRow rowGrid = this.gridLookup.CurrentRow;

            if (rowGrid != null)
            {
                string colValue1 = rowGrid.Cells[0].Value.ToString();
                string colName1 = this.dsLookup.Tables[0].Columns[0].ColumnName;
                string colValue2 = rowGrid.Cells[1].Value.ToString();
                string colName2 = this.dsLookup.Tables[0].Columns[1].ColumnName;
                //edited by louie 20070112 3.09 PM
                //string colValue3 = rowGrid.Cells[2].Value.ToString();
                //string colName3 = this.dsLookup.Tables[0].Columns[2].ColumnName;
                //end
                //edited by louie 20070112
                //original code string condition = string.Format("[{0}]='{1}' AND [{2}]='{3}' AND [{4}]='{5}'", colName1, colValue1.Replace("'", "''"), colName2, colValue2.Replace("'", "''"), colName3, colValue3.Replace("'", "''"));
                //end
                string condition = string.Format("[{0}]='{1}' AND [{2}]='{3}'", colName1, colValue1.Replace("'", "''"), colName2, colValue2.Replace("'", "''"));//, colName3, colValue3.Replace("'", "''"));
                //end
                //sept 19, 2006
                //carlo
                //start - added for cpo's 4th column
                //edited by louie 20070112 
                if (gridLookup.Columns.Count == 4)
                {
                    //string colValue4 = rowGrid.Cells[3].Value.ToString();
                    //string colName4 = this.dsLookup.Tables[0].Columns[3].ColumnName;
                    //condition = string.Format("[{0}]='{1}' AND [{2}]='{3}' AND [{4}]='{5}' AND [{6}]='{7}'", colName1, colValue1.Replace("'", "''"), colName2, colValue2.Replace("'", "''"), colName3, colValue3.Replace("'", "''"), colName4, colValue4.Replace("'", "''"));
                }
                //end
                //end

                DataRow[] rows = this.dsLookup.Tables[0].Select(condition);

                if (rows.Length > 0)
                    return this.dsLookup.Tables[0].Rows.IndexOf(rows[0]);
                else
                    return -1;
            }

            return -1;
        }

        //Arthur Inserted 2006091203PM Start
        private void txtLookupCode_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.btnSearch.PerformClick();
            }

        }
        //end

        //September 18, 2006
        //Genz
        //Start
        private void txtLookupCode_KeyUp_1(object sender, KeyEventArgs e)
        {
            string colName1 = this.dsLookup.Tables[0].Columns[0].ColumnName;
            string colName2 = this.dsLookup.Tables[0].Columns[1].ColumnName;
            //edited by louie 01132007
            if (this.dsLookup.Tables[0].Columns.Count > 2)
            {
                string colName3 = this.dsLookup.Tables[0].Columns[2].ColumnName;
                try //added by Manuel 2008/05/16
                {
                    dtView.RowFilter = string.Format("[{0}] like '{1}%' OR [{2}] like '{1}%' OR [{3}] like '{1}%'", colName1, this.txtLookupCode.Text.Substring(0, this.txtLookupCode.Text.Length - this.txtLookupCode.SelectionLength).Replace("'", "''"), colName2, colName3);
                }
                catch (Exception ex)
                {
                    CommonProcedures.showMessageError(CommonMessages.NoWILDCharacters("Search"));
                } //end
            }
            else
            {
                try //added by Manuel 2008/05/16
                {
                    dtView.RowFilter = string.Format("[{0}] like '{1}%' OR [{2}] like '{1}%' ", colName1, this.txtLookupCode.Text.Substring(0, this.txtLookupCode.Text.Length - this.txtLookupCode.SelectionLength).Replace("'", "''"), colName2);
                }
                catch (Exception ex)
                {
                    CommonProcedures.showMessageError(CommonMessages.NoWILDCharacters("Search"));
                } //end
            }
            //end
           
            this.gridLookup.DataSource = dtView;
            this.gridLookup.Refresh();
            
        }

        //End

        //Seldon Add 05/04/2007
        public void SelectRowColumnWithTwoIndex(string index1, string index2)
        {
            for (int i = 0; i < this.gridLookup.Rows.Count; i++)
            {
                if (this.gridLookup.Rows[i].Cells[0].Value.ToString().Trim().Equals(index1) && this.gridLookup.Rows[i].Cells[1].Value.ToString().Trim().Equals(index2))
                {
                    this.gridLookup.Rows[i].Selected = true;
                    this.gridLookup.Rows[i].Cells[0].Selected = true;
                    break;
                }
            }
        }

        public TextBox getSearchTextBox
        {
            get
            {
                return txtLookupCode;
            }
        }

        public DataGridView getdataGrid
        {
            get
            {
                return gridLookup;
            }
        }
    }
}
