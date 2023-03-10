using System;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using C1.Win.C1Ribbon;
using Payroll.DAL;

using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.IO;
using CommonLibrary;
using Payroll.BLogic;
using System.Configuration;
using C1.Win.C1Report;

namespace Payroll.CFC
{
    public partial class FHDRprtSelectForm : C1RibbonForm
    {

        #region Global Variables

        public static DataTable dt1 = new DataTable();
        public static DataTable dt2 = new DataTable();

        public static string ReportName = string.Empty;
        //to be assigned a value on btnOk_Click Event
        public static string qStringSelect = string.Empty;
        // The tables needed, the Join condition and the where clause
        // are to be defined by the developer.
        public static string qStringFrom = string.Empty;
        public static string qStringWhere = string.Empty;
        //For group by option
        public static string GroupBy = string.Empty;

        public static NpaxFlexGrid tempHeader = null;
        public static NpaxFlexGrid tempDetail = null;

        //added by kevin
        private NpaxFlexGrid headergrid = null;
        private ArrayList HiddenColumns;
        private SortedList SelectedColumns;
        private NpaxFlexGrid NewGrid = null;
        private NpaxFlexGrid OrigGrid = null;
        private Hashtable OrigColOrder = new Hashtable();
        //end

        public string[] Options = new string[20];

        private bool FromDgvRight = false;
        #endregion

        #region Constructor

        public FHDRprtSelectForm()
        {
            InitializeComponent();
        }

        public FHDRprtSelectForm(NpaxFlexGrid HeaderGrid, ArrayList hiddenColumns, SortedList selectColumns, Icon icon, string[] Options)
            : this()
        {
            this.HiddenColumns = hiddenColumns;
            headergrid = HeaderGrid;
            OrigGrid = HeaderGrid;
            NewGrid = HeaderGrid;
            this.SelectedColumns = selectColumns;
            this.Icon = icon;
            this.Options = Options;
        }

        #endregion

        #region Events Handled

        private void FHDRprtSelectForm_Load(object sender, EventArgs e)
        {
            this.InitializeDataControls(tempHeader);//virtual function defined
            this.InitializeSettingsControls();
            this.BindDataToGrid1();
            this.btnOk.Enabled = false;
            OriginalColOrder();
            UpdateDisplay();
        }

        //Clear All Data
        private void FHDRprtSelectForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            dt1.Clear();
            dt2.Clear();
            qStringSelect = string.Empty;
            qStringFrom = string.Empty;
            qStringWhere = string.Empty;
        }

        //Generate report
        private void btnOk_Click(object sender, EventArgs e)
        {
            this.RenderReport();
        }

        #region// Events defined for row number updates when the grid's value is changed.


        private void dgvRight_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            this.label1.Text = "{" + this.dgvRight.Rows.Count.ToString() + " Rows}";
            if (this.dgvRight.Rows.Count > 0)
            {
                this.btnOk.Enabled = true;
                this.btnPrint.Enabled = true;
                this.gbSettings.Enabled = true;
            }
            else
            {
                this.btnOk.Enabled = false;
                this.btnPrint.Enabled = false;
                this.gbSettings.Enabled = false;
            }
        }

        private void dgvLeft_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            this.label2.Text = "{" + this.dgvLeft.Rows.Count.ToString() + " Rows}";
        }

        #endregion

        #region // Events defined for the grid's Drag Drop functionality.

        private void dgvRight_MouseUp(object sender, MouseEventArgs e)
        {
           
        }

        private void dgvRight_MouseDown(object sender, MouseEventArgs e)
        {
            //PreviousPoint = new Point(e.X, e.Y);
            if (e.Clicks == 1)
            {
                DataGridView.HitTestInfo hit = dgvRight.HitTest(e.X, e.Y);
                if (hit.RowIndex != -1)
                {
                    dgvRight.CurrentCell = dgvRight.Rows[hit.RowIndex].Cells[0];
                    dgvRight.Rows[hit.RowIndex].Selected = true;
                    if (this.dgvRight.Rows.Count > 0)
                    {
                        if (e.Button == MouseButtons.Left)
                        {
                            FromDgvRight = true;
                            dgvRight.DoDragDrop(dgvRight.CurrentRow, DragDropEffects.All);
                        }
                    }
                }
            }
             if (e.Clicks == 2)
            {
                DataGridView.HitTestInfo hit = dgvRight.HitTest(e.X, e.Y);
                if (hit.RowIndex != -1)
                {
                    dgvRight.CurrentCell = dgvRight.Rows[hit.RowIndex].Cells[0];
                    dgvRight.Rows[hit.RowIndex].Selected = true;
                    btnApplyLeft.PerformClick();
                }
            }
        }

        private void dgvRight_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(DataGridViewRow)))
            {
                e.Effect = DragDropEffects.Copy;
            }
        }

        private void dgvRight_DragDrop(object sender, DragEventArgs e)
        {
            DataGridViewRow row = e.Data.GetData(typeof(DataGridViewRow)) as DataGridViewRow;

            if (row != null)
            {
                DataGridViewRow newrow = row.Clone() as DataGridViewRow;
                Object[] obj = new object[2];

                obj[0] = row.Cells[0].Value;
                obj[1] = row.Cells[1].Value;
               
                    if(!FromDgvRight)
                        dt2 = this.AddSpecificDataToTable(dt2, obj, 2);
                    this.BindDataToGrid2();
                    this.deleteDataInOriginalGrid(obj, 1);
                    this.UpdateDisplay();

                //CHECKING FOR DRAGDROP ON THE SAME GRID
                    if (FromDgvRight)
                        ReOrderRows(sender, e, row);
                    else
                    {
                        ReOrderRows(e);
                    }
                //END
            }
        }

        private void ReOrderRows(DragEventArgs e)
        {
            //convert to client coordinates
            Point clientPoint = this.dgvRight.PointToClient(new Point(e.X, e.Y));

            Point gridPoint = dgvRight.Location;
            //if the drag item is on the same grid
            //if (clientPoint.X > gridPoint.X)
            //{

            //get the row index of the item the mouse is below
            int rowIndexUnderMouseDrop = dgvRight.HitTest(clientPoint.X, clientPoint.Y).RowIndex;
            if (rowIndexUnderMouseDrop == -1)
                rowIndexUnderMouseDrop = dgvRight.Rows.Count;

            try
            {
                DataRow dtrow = dt2.Rows[dt2.Rows.Count - 1];
                DataRowState rowstate = dtrow.RowState;
                object[] itemArray = dtrow.ItemArray;
                dt2.Rows.RemoveAt(dt2.Rows.Count - 1);
                dtrow.ItemArray = itemArray;
                dt2.Rows.InsertAt(dtrow, rowIndexUnderMouseDrop);
                if (rowstate == DataRowState.Added)
                {
                    dtrow.SetAdded();
                }
                else if (rowstate == DataRowState.Modified)
                {
                    dtrow.SetModified();
                }
                else if (rowstate == DataRowState.Unchanged)
                {
                    dtrow.AcceptChanges();
                }

            }
            catch (Exception ex)
            { throw new Exception(ex.Message); }
            //  }
        }

        private void ReOrderRows(object sender, DragEventArgs e, DataGridViewRow row)
        {
            //convert to client coordinates
            Point clientPoint = this.dgvRight.PointToClient(new Point(e.X, e.Y));

            Point gridPoint = dgvRight.Location;
            //if the drag item is on the same grid
            //if (clientPoint.X > gridPoint.X)
            //{

                //get the row index of the item the mouse is below
                int rowIndexUnderMouseDrop = dgvRight.HitTest(clientPoint.X, clientPoint.Y).RowIndex;
                if (rowIndexUnderMouseDrop == -1)
                    rowIndexUnderMouseDrop = dgvRight.Rows.Count;
               
                try
                {
                    DataRow dtrow = dt2.Rows[row.Index];
                    DataRowState rowstate = dtrow.RowState;
                    object[] itemArray = dtrow.ItemArray;
                    dt2.Rows.RemoveAt(row.Index);
                    dtrow.ItemArray = itemArray;
                    dt2.Rows.InsertAt(dtrow, rowIndexUnderMouseDrop);
                    if (rowstate == DataRowState.Added)
                    {
                        dtrow.SetAdded();
                    }
                    else if (rowstate == DataRowState.Modified)
                    {
                        dtrow.SetModified();
                    }
                    else if (rowstate == DataRowState.Unchanged)
                    {
                        dtrow.AcceptChanges();
                    }
                    
                }
                catch(Exception ex)
                { throw new Exception(ex.Message); }
          //  }
        }
        private void dgvLeft_MouseUp(object sender, MouseEventArgs e)
        {

        }
        private void dgvLeft_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Clicks == 2)
            {
                DataGridView.HitTestInfo hit = dgvLeft.HitTest(e.X, e.Y);
                if (hit.RowIndex != -1)
                {
                    dgvLeft.CurrentCell = dgvLeft.Rows[hit.RowIndex].Cells[0];
                    dgvLeft.Rows[hit.RowIndex].Selected = true;
                    btnApplyRow.PerformClick();
                }
            }
            else
            {
                DataGridView.HitTestInfo hit = dgvLeft.HitTest(e.X, e.Y);
                if (hit.RowIndex != -1)
                {
                    dgvLeft.CurrentCell = dgvLeft.Rows[hit.RowIndex].Cells[0];
                    dgvLeft.Rows[hit.RowIndex].Selected = true;
                    if (this.dgvLeft.Rows.Count > 0)
                    {
                        if (e.Button == MouseButtons.Left)
                        {
                            FromDgvRight = false;
                            dgvLeft.DoDragDrop(dgvLeft.CurrentRow, DragDropEffects.All);
                        }
                    }
                }
            }
        }

        private void dgvLeft_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(DataGridViewRow)))
            {
                e.Effect = DragDropEffects.Copy;
            }
        }

        private void dgvLeft_DragDrop(object sender, DragEventArgs e)
        {
            DataGridViewRow row = e.Data.GetData(typeof(DataGridViewRow)) as DataGridViewRow;

            if (row != null)
            {
                DataGridViewRow newrow = row.Clone() as DataGridViewRow;
                Object[] obj = new object[2];

                obj[0] = row.Cells[0].Value;
                obj[1] = row.Cells[1].Value;
                if (!this.CheckIfItemAlreadyExists(obj, 1))
                {
                    dt1 = this.AddSpecificDataToTable(dt1, obj, 1);
                    this.BindDataToGrid1();

                    this.deleteDataInOriginalGrid(obj, 2);
                    this.UpdateDisplay();
                }
            }
        }

        #endregion

        //Apply all items to Grid
        private void btnApplyAll_Click(object sender, EventArgs e)
        {
            dt1 = (DataTable)this.dgvLeft.DataSource;
            dt2 = (DataTable)this.dgvRight.DataSource;

            if (dt1.Rows.Count > 0)
            {
                dt2 = this.AddAllDataToOtherTable(dt1, dt2);
                dt1 = DeleteAllDataInTable(dt1);
                this.BindDataToGrid1();
                this.BindDataToGrid2();

            }
            this.UpdateDisplay();
        }

        //Apply selected item to grid
        private void btnApplyRow_Click(object sender, EventArgs e)
        {
            object[] obj = new object[2];

            if ((dt1.Rows.Count > 0))
            {
                obj[0] = this.dgvLeft.CurrentRow.Cells[0].Value;
                obj[1] = this.dgvLeft.CurrentRow.Cells[1].Value;
                dt2 = this.AddSpecificDataToTable(dt2, obj, 2);
                this.BindDataToGrid2();
                this.deleteDataInOriginalGrid(obj, 1);
            }
            this.UpdateDisplay();
        }


        private void btnApplyLeft_Click(object sender, EventArgs e)
        {
            object[] obj = new object[2];
            if ((dt2.Rows.Count > 0))
            {
                obj[0] = this.dgvRight.CurrentRow.Cells[0].Value;
                obj[1] = this.dgvRight.CurrentRow.Cells[1].Value;
                dt1 = this.AddSpecificDataToTable(dt1, obj, 1);
                this.BindDataToGrid1();
                this.deleteDataInOriginalGrid(obj, 2);
                this.UpdateDisplay();
            }
            
        }

        private void btnApplyLeftAll_Click(object sender, EventArgs e)
        {
            dt1 = (DataTable)this.dgvLeft.DataSource;
            dt2 = (DataTable)this.dgvRight.DataSource;
            if (dt2.Rows.Count > 0)
            {
                dt1 = this.AddAllDataToOtherTable(dt2, dt1);
                dt2 = DeleteAllDataInTable(dt2);
                this.BindDataToGrid1();
                this.BindDataToGrid2();
                this.UpdateDisplay();
            }
        }

        private void rbAscending_CheckedChanged(object sender, EventArgs e)
        {
            if (rbAscending.Checked)
            {
                rbDescending.Checked = false;
                rbNoSort.Checked = false;
            }
            else if (rbDescending.Checked)
            {
                rbDescending.Checked = true;
                rbNoSort.Checked = false;
                rbAscending.Checked = false;
            }
            else
            {
                rbDescending.Checked = false;
                rbNoSort.Checked = true;
                rbAscending.Checked = false;
            }
        }

        private void rbDescending_CheckedChanged(object sender, EventArgs e)
        {
            if (rbDescending.Checked)
            {
                rbAscending.Checked = false;
                rbNoSort.Checked = false;
            }
            else if (rbAscending.Checked)
            {
                rbAscending.Checked = true;
                rbNoSort.Checked = false;
                rbDescending.Checked = false;
            }
            else
            {
                rbDescending.Checked = false;
                rbNoSort.Checked = true;
                rbAscending.Checked = false;
            }
        }

        private void rbNoSort_CheckedChanged(object sender, EventArgs e)
        {
            if (rbNoSort.Checked)
            {
                rbAscending.Checked = false;
                rbDescending.Checked = false;
            }
            else if (rbAscending.Checked)
            {
                rbAscending.Checked = true;
                rbNoSort.Checked = false;
                rbDescending.Checked = false;
            }
            else
            {
                rbDescending.Checked = true;
                rbNoSort.Checked = false;
                rbAscending.Checked = false;
            }
        }

        private void txtGroupby_DragDrop(object sender, DragEventArgs e)
        {
            txtGroupby.Text = e.Data.GetData(DataFormats.Text).ToString();
        }

        private void dgvRight_SelectionChanged(object sender, EventArgs e)
        {
            if (this.dgvRight.Rows.Count > 0)
            {
                if (this.dgvRight.SelectedCells.Count > 0)
                    this.txtGroupby.Text = this.FormatCaptionName(this.dgvRight.SelectedCells[0].Value.ToString());
                GroupBy = this.txtGroupby.Text.Trim();
            }
            else
            {
                this.txtGroupby.Clear();
                GroupBy = this.txtGroupby.Text.Trim();
            }
        }
        private void dgvLeft_SelectionChanged(object sender, EventArgs e)
        {
            if (this.dgvLeft.Rows.Count > 0)
            {
                
                GroupBy = this.txtGroupby.Text.Trim();
            }
            else
            {
                this.txtGroupby.Clear();
                GroupBy = this.txtGroupby.Text.Trim();
            }
        }
        private void cbImplementGroupby_CheckedChanged(object sender, EventArgs e)
        {
                this.rbAscending.Enabled = true;
                this.rbDescending.Enabled = true;
                this.rbNoSort.Enabled = true;
        }

        #endregion

        #region Function Defined
        //Store all columns of all tables to a single DataTable
        private void ConsolidateAllColumnNames(DataTable tempdt, DataTable T, string tablename)
        {
            object[] tempcont = new object[2];

            for (int x = 0; x < T.Rows.Count; x++)
            {
                tempcont[0] = T.Rows[x][0].ToString().Trim();
                tempcont[1] = tablename + "." + T.Rows[x][0].ToString().Trim();
                tempdt.Rows.Add(tempcont);
            }
        }

        private void UpdateDisplay()
        {
            if (this.dgvLeft.Rows.Count == 0)
            {
                btnApplyRow.Enabled = false;
                btnApplyAll.Enabled = false;
                btnApplyLeft.Enabled = true;
                btnApplyLeftAll.Enabled = true;
            }
            else if (this.dgvLeft.Rows.Count > 0)
            {
                btnApplyRow.Enabled = true;
                btnApplyAll.Enabled = true;
            }

            if (this.dgvRight.Rows.Count == 0)
            {
                btnApplyLeft.Enabled = false;
                btnApplyLeftAll.Enabled = false;
                btnApplyRow.Enabled = true;
                btnApplyAll.Enabled = true;
            }
            else if (this.dgvRight.Rows.Count > 0)
            {
                btnApplyLeft.Enabled = true;
                btnApplyLeftAll.Enabled = true;
            }
            
        }

        private bool CheckIfItemAlreadyExists(object[] obj, int tableNo)
        {
            if (tableNo == 1)
            {
                if (this.dgvLeft.Rows.Count > 0)
                {
                    if (this.dgvLeft.CurrentRow.Cells[1].Value.ToString() == obj[1].ToString())
                    {
                        return true;
                    }
                }
            }
            else
            {
                if (this.dgvRight.Rows.Count > 0)
                {
                    if (this.dgvRight.CurrentRow.Cells[1].Value.ToString() == obj[1].ToString())
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private DataTable AddSpecificDataToTable(DataTable dtDest, object[] obj, int tableNo)
        {
            if (dtDest.Rows.Count < 1)
            {
                dtDest = new DataTable();
                dtDest.Columns.Add("Column Name");
                dtDest.Columns.Add("Complete Name");
            }
            else
            {
                if (tableNo == 1)
                    dtDest = (DataTable)this.dgvLeft.DataSource;
                else
                    dtDest = (DataTable)this.dgvRight.DataSource;
            }

            dtDest.Rows.Add(obj);
            return dtDest;
        }

        private DataTable AddAllDataToOtherTable(DataTable dtSrc, DataTable dtDest)
        {
            object[] obj = new object[2];

            if (this.dgvRight.Rows.Count < 1)
            {
                dtDest = new DataTable();
                dtDest.Columns.Add("Column Name");
                dtDest.Columns.Add("Complete Name");
            }

            for (int i = 0; i < dtSrc.Rows.Count; i++)
            {
                obj[0] = dtSrc.Rows[i][0].ToString();
                obj[1] = dtSrc.Rows[i][1].ToString();
                dtDest.Rows.Add(obj);
            }
            return dtDest;
        }

        private DataTable DeleteAllDataInTable(DataTable tempdt)
        {
            for (int i = tempdt.Rows.Count; i > 0; i--)
            {
                tempdt.Rows.RemoveAt(i - 1);
            }

            return tempdt;
        }

        private void BindDataToGrid1()
        {
            this.dgvLeft.AutoGenerateColumns = true;
            this.dgvLeft.DataSource = dt1;
            this.dgvLeft.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvLeft.Refresh();
            this.dgvLeft.Columns[0].HeaderText = "Available Columns";
            if (this.dgvLeft.Columns.Count > 0)
            {
                this.dgvLeft.Columns[1].Visible = false;
                this.dgvLeft.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }
        }

        private void BindDataToGrid2()
        {
            this.dgvRight.AutoGenerateColumns = true;
            this.dgvRight.DataSource = dt2;
            this.dgvRight.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvRight.Refresh();
            dgvRight.Columns[0].HeaderText = "Print Columns";
            if (this.dgvRight.Columns.Count > 0)
            {
                this.dgvRight.Columns[1].Visible = false;
                this.dgvRight.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }
        }

        private void deleteDataInOriginalGrid(object[] obj, int tableNo)
        {
            DataTable dtSrc;

            if (tableNo == 1)
                dtSrc = (DataTable)this.dgvLeft.DataSource;
            else
                dtSrc = (DataTable)this.dgvRight.DataSource;

            for (int i = 0; i < dtSrc.Rows.Count; i++)
            {
                if (dtSrc.Rows[i][1].ToString().Trim().Equals(obj[1].ToString().Trim()))
                {
                    dtSrc.Rows.RemoveAt(i);
                    break;
                }
            }

            if (tableNo == 1)
                this.BindDataToGrid1();
            else
                this.BindDataToGrid2(); 
        }

        //Replaces char '_' with ' ' in the column name
        private string FormatCaptionName(string CapName)
        {
            CapName = CapName.Replace('_', ' ');
            return CapName;
        }

        //Replaces char ' 'with '_' in the column name
        private string ReformatCaptionName(string CapName)
        {
            CapName = CapName.Replace(' ', '_');
            return CapName;
        }

      
        private void InitializeSettingsControls()
        {
            this.txtGroupby.Clear();
            this.txtGroupby.ReadOnly = true;
        }

        #endregion

        #region Virtual Functions

        //Called during the event Form_Load
        public void InitializeDataControls(NpaxFlexGrid headergrid)
        {
           
            dt1 = CreateColumnHeader();
        }

        #endregion

        #region This functions can be used if the .Table property is set

        public DataTable CreateColumnHeader()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Column Name");
            dt.Columns.Add("Complete Name");
            //dont include hidden columns -- include now
            for (int i = 1; i < headergrid.Cols.Count; i++)
            {

                string headername = headergrid.Cols[i].Name;
                if (SelectedColumns.Contains(headername))
                {
                    object[] obj = new object[2];
                    obj[0] = SelectedColumns[headername];
                    obj[1] = headername;
                    dt.Rows.Add(obj);
                }
            }

            return dt;
        }


        public void CreateColumnChoices()
        {
            DataTable tempContdt = new DataTable();
            DataTable Contdt = (DataTable)tempHeader.DataSource;
            object[] temp = new object[2];

            tempContdt.Columns.Add("Column Name");
            tempContdt.Columns.Add("Complete Name");

            for (int i = 0; i < tempHeader.Cols.Count; i++)
            {
                if (i > 1)
                {
                    temp[0] = tempHeader.Cols[i].Caption;
                    temp[1] = Contdt.Columns[i - 1].Table + "." + Contdt.Columns[i - 1].ColumnName.ToString();

                    if (!this.CheckIfCaptionAlreadyExists(tempContdt, temp[0]))
                        tempContdt.Rows.Add(temp);
                }
            }

            Contdt.Clear();
            if (tempDetail != null)
            {
                Contdt = (DataTable)tempDetail.DataSource;

                for (int i = 0; i < tempDetail.Cols.Count; i++)
                {
                    if (i > 1)
                    {
                        temp[0] = tempDetail.Cols[i].Caption;
                        temp[1] = Contdt.Columns[i - 1].Table + "." + Contdt.Columns[i - 1].ColumnName.ToString();
                        if (!this.CheckIfCaptionAlreadyExists(tempContdt, temp[0]))
                            tempContdt.Rows.Add(temp);
                    }
                }
            }
            dt1 = tempContdt;
        }

        private bool CheckIfCaptionAlreadyExists(DataTable temp, object capt)
        {
            if (temp != null)
            {
                for (int i = 0; i < temp.Rows.Count; i++)
                {
                    if ((capt.ToString().Trim() == temp.Rows[i][0].ToString().Trim()) || (capt.ToString().Trim() == string.Empty))
                    {
                        return true;
                    }
                }
                if (capt.ToString() == string.Empty)
                    return true;
            }
            return false;
        }

        #endregion

        private DataTable CreateDataTableReport()
        {
            DataTable dtNewHeader = ((DataTable)headergrid.DataSource).Copy();

            //setting caption
            for (int i = 1; i < headergrid.Cols.Count; i++)
            {
                dtNewHeader.Columns[i - 1].Caption = headergrid.Cols[i - 1].Caption;
            }
            for (int i = 1; i < headergrid.Cols.Count; i++)
            {
                string header = headergrid.Cols[i].Name;
                if (HiddenColumns.Contains(header) && header != "HeaderRowNum" && header != "DetailRowNum")
                {
                    dtNewHeader.Columns.Remove(header);
                }
                for (int j = 0; j < dgvLeft.Rows.Count; j++)
                {
                    string field = dgvLeft.Rows[j].Cells[0].Value.ToString().Trim();
                    if (field == headergrid.Cols[i].Caption)
                    {
                        string name = headergrid.Cols[i].Name;
                        dtNewHeader.Columns.Remove(name);
                    }
                }
            }
            return dtNewHeader;
        }

        private void OriginalColOrder()
        {
            for (int i = 1; i < NewGrid.Cols.Count; i++)
            {
                string colName = NewGrid.Cols[i].Name;
                OrigColOrder.Add(colName, i);
            }
        }

        private void HideColumns()
        {
            for (int j = 0; j < NewGrid.Cols.Count; j++)
            {
                NewGrid.Cols[j].Visible = false;
            }

            for (int i = 0; i < dt2.Rows.Count; i++)
            {
                string columnName = dt2.Rows[i][1].ToString();
                NewGrid.Cols[columnName].Visible = true;
            }
        }

        private void ShowColumns()
        {
            for (int i = 0; i < NewGrid.Cols.Count; i++)
            {
                NewGrid.Cols[i].Visible = true;
            }

            foreach (string col in HiddenColumns)
            {
                NewGrid.Cols[col].Visible = false;
            }
           
        }

        private void ReOrderColumns()
        {
            for (int i = 0; i < dt2.Rows.Count; i++)
            {
                string columnName = dt2.Rows[i][1].ToString();
                NewGrid.Cols[columnName].Move(i + 1);
            }
        }

        private void ResetColumns()
        {
            IDictionaryEnumerator enumerator = OrigColOrder.GetEnumerator();
            while (enumerator.MoveNext())
            {
                string colName = (String)enumerator.Key;
                int index = (Int32)enumerator.Value;

                NewGrid.Cols[colName].Move(index);
            }
        }

        private void SortColumn()
        {
            string sortColumn = txtGroupby.Text.Trim();
            if (rbAscending.Checked || rbDescending.Checked)
            {
                for (int i = 0; i < dt2.Rows.Count; i++)
                {
                    string colCap = dt2.Rows[i][0].ToString();
                    string colName = dt2.Rows[i][1].ToString();
                    if (colCap == sortColumn)
                    {
                        if (rbAscending.Checked)
                        {
                            NewGrid.Sort(C1.Win.C1FlexGrid.SortFlags.Ascending, NewGrid.Cols[colName].Index);
                        }
                        else
                        {
                            NewGrid.Sort(C1.Win.C1FlexGrid.SortFlags.Descending, NewGrid.Cols[colName].Index);
                        }
                        break;
                    }
                }
            }
        }

        private void RenderReport()
        {
            
            NewGrid.Cols[0].Visible = false;
            SortColumn();
            HideColumns();
            ReOrderColumns();
            HideOtherFixedRows(true);
            C1FlexGridPrintable print = new C1FlexGridPrintable(NewGrid);
            print.PrintInfo.IsOwnerDrawPageHeader = true;
            print.PrintInfo.PageHeaderHeight = 170;
            print.OwnerDrawPageHeader += new OwnerDrawPageEventHandler(HeaderGrid_OwnerDrawPageHeader);
            print.PrintInfo.ShowHiddenCols = false;
            print.PrintInfo.ShowHiddenRows = false;
            print.PrintInfo.UseGridColors = true;
            print.PrintInfo.PageFooter = "Printed on " + DateTime.Now.ToString("MMMM/dd/yyyy hh:mm tt") + @"\t\tPage \p of \P";
            print.PrintPreview(this.Text, this.Icon);
            NewGrid.Cols[0].Visible = true;
            ResetColumns();
            ShowColumns();
            HideOtherFixedRows(false);
        }

        void HeaderGrid_OwnerDrawPageHeader(object sender, OwnerDrawPageEventArgs e)
        {
            DataTable dt = GetCompanyProfile().Tables[0];
            string companyname = dt.Rows[0]["Ccd_CompanyName"].ToString();
            string address1 = dt.Rows[0]["Ccd_CompanyAddress1"].ToString();
            string address2 = dt.Rows[0]["Ccd_CompanyAddress2"].ToString();
            string address3 = dt.Rows[0]["Ccd_CompanyAddress3"].ToString();

            Image tempImg;
            byte[] compimage;

            compimage = ((byte[])(dt.Rows[0]["ccd_companylogo"]));
            MemoryStream ms = new MemoryStream(compimage);

            tempImg = Image.FromStream(ms);
            tempImg = this.resizeImage(tempImg, new Size(60, 50));

            e.OwnerDrawPrint.RenderDirectImage(0, 0, tempImg,
                200, 400, C1.Win.C1FlexGrid.ImageAlignEnum.TileStretch);
            e.OwnerDrawPrint.RenderDirectText(205, 0, companyname, 1000, new Font("Tahoma", 8, FontStyle.Bold),
            Color.Black, C1.Win.C1FlexGrid.TextAlignEnum.LeftTop);
            e.OwnerDrawPrint.RenderDirectText(205, 40, address1, 1000, new Font("Tahoma", 8, FontStyle.Bold),
            Color.Black, C1.Win.C1FlexGrid.TextAlignEnum.LeftTop);
            e.OwnerDrawPrint.RenderDirectText(205, 80, address2, 1000, new Font("Tahoma", 8, FontStyle.Bold),
            Color.Black, C1.Win.C1FlexGrid.TextAlignEnum.LeftTop);
            e.OwnerDrawPrint.RenderDirectText(205, 120, address3, 1000, new Font("Tahoma", 8, FontStyle.Bold),
            Color.Black, C1.Win.C1FlexGrid.TextAlignEnum.LeftTop);
            e.OwnerDrawPrint.RenderDirectText(0, 180, this.Text, 1000, new Font("Tahoma", 12, FontStyle.Bold),
                Color.Black, C1.Win.C1FlexGrid.TextAlignEnum.LeftTop);
            e.OwnerDrawPrint.RenderDirectText(450, 190, Options[6], 2000, new Font("Tahoma", 10, FontStyle.Bold),
            Color.Black, C1.Win.C1FlexGrid.TextAlignEnum.LeftTop);

            e.OwnerDrawPrint.RenderDirectText(0, 280, Options[0], 1000, new Font("Tahoma", 8, FontStyle.Bold),
            Color.Black, C1.Win.C1FlexGrid.TextAlignEnum.LeftTop);
            e.OwnerDrawPrint.RenderDirectText(300, 280, Options[1], 1000, new Font("Tahoma", 8, FontStyle.Bold),
            Color.Black, C1.Win.C1FlexGrid.TextAlignEnum.LeftTop);

            e.OwnerDrawPrint.RenderDirectText(0, 320, Options[4], 1000, new Font("Tahoma", 8, FontStyle.Bold),
            Color.Black, C1.Win.C1FlexGrid.TextAlignEnum.LeftTop);
            e.OwnerDrawPrint.RenderDirectText(300, 320, Options[5], 1000, new Font("Tahoma", 8, FontStyle.Bold),
            Color.Black, C1.Win.C1FlexGrid.TextAlignEnum.LeftTop);

            e.OwnerDrawPrint.RenderDirectText(1200, 280, Options[7], 1000, new Font("Tahoma", 8, FontStyle.Bold),
            Color.Black, C1.Win.C1FlexGrid.TextAlignEnum.LeftTop);
            e.OwnerDrawPrint.RenderDirectText(1500, 280, Options[8], 1000, new Font("Tahoma", 8, FontStyle.Bold),
            Color.Black, C1.Win.C1FlexGrid.TextAlignEnum.LeftTop);

            e.OwnerDrawPrint.RenderDirectText(1200, 320, Options[9], 1000, new Font("Tahoma", 8, FontStyle.Bold),
            Color.Black, C1.Win.C1FlexGrid.TextAlignEnum.LeftTop);
            e.OwnerDrawPrint.RenderDirectText(1500, 320, Options[10], 1000, new Font("Tahoma", 8, FontStyle.Bold),
            Color.Black, C1.Win.C1FlexGrid.TextAlignEnum.LeftTop);

            e.OwnerDrawPrint.RenderDirectText(0, 360, Options[2], 1000, new Font("Tahoma", 8, FontStyle.Bold),
            Color.Black, C1.Win.C1FlexGrid.TextAlignEnum.LeftTop);
            e.OwnerDrawPrint.RenderDirectText(300, 360, Options[3], 2400, new Font("Tahoma", 8, FontStyle.Bold),
            Color.Black, C1.Win.C1FlexGrid.TextAlignEnum.LeftTop);
    
        }

        private void btnUp_Click(object sender, EventArgs e)
        {
            //GET CURRENT SELECTED ROW
           
            DataGridViewRow currentRow = dgvRight.CurrentRow;
            if (currentRow != null)
            {
                if (currentRow.Index >= 1)
                {
                    int index = currentRow.Index;
                    DataRow dtrow = dt2.Rows[index];
                    DataRowState rowstate = dtrow.RowState;
                    object[] itemArray = dtrow.ItemArray;
                    dt2.Rows.RemoveAt(index);
                    dtrow.ItemArray = itemArray;
                    dt2.Rows.InsertAt(dtrow, index - 1);
                    dgvRight.CurrentCell = dgvRight.Rows[index - 1].Cells[0];
                    dgvRight.Rows[index - 1].Selected = true;
                }
            }
        }

        private void btnDown_Click(object sender, EventArgs e)
        {
            DataGridViewRow currentRow = dgvRight.CurrentRow;
            if (currentRow != null)
            {
                if (currentRow.Index < dgvRight.Rows.Count)
                {
                    int index = currentRow.Index;
                    DataRow dtrow = dt2.Rows[index];
                    DataRowState rowstate = dtrow.RowState;
                    object[] itemArray = dtrow.ItemArray;
                    dt2.Rows.RemoveAt(index);
                    dtrow.ItemArray = itemArray;
                    dt2.Rows.InsertAt(dtrow, index + 1);
                    if ((index + 1) == dgvRight.Rows.Count)
                        index = dgvRight.Rows.Count - 1;
                    else
                        index++;
                        dgvRight.CurrentCell = dgvRight.Rows[index].Cells[0];
                        dgvRight.Rows[index].Selected = true;
                }
            }
        }
        private void HideOtherFixedRows(bool Hide)
        {
            return;//temp
            //if (NewGrid.Rows.Fixed > 1)
            //{
            //    for (int i = 1; i < NewGrid.Rows.Fixed; i++)
            //    {
            //        NewGrid.Rows[i].Visible = !Hide;
            //    }
            //}
        }
        private void btnPrint_Click(object sender, EventArgs e)
        {
            NewGrid.Cols[0].Visible = false;
            SortColumn();
            HideColumns();
            ReOrderColumns();
            HideOtherFixedRows(true);
            C1FlexGridPrintable print = new C1FlexGridPrintable(NewGrid);
            print.PrintInfo.IsOwnerDrawPageHeader = true;
            print.PrintInfo.PageHeaderHeight = 50;
            print.OwnerDrawPageHeader += new OwnerDrawPageEventHandler(HeaderGrid_OwnerDrawPageHeader);
            print.PrintInfo.ShowHiddenCols = false;
            print.PrintInfo.ShowHiddenRows = false;
            print.PrintInfo.UseGridColors = true;
            print.PrintInfo.PageFooter = "Printed on " + DateTime.Now.ToString("MMMM/dd/yyyy hh:mm tt") + @"\t\tPage \p of \P";
            print.Print();
            NewGrid.Cols[0].Visible = true;
            ResetColumns();
            ShowColumns();
            HideOtherFixedRows(false);
        }

        private DataSet GetCompanyProfile()
        {
            #region query
            string sqlStatement = @"SELECT RTRIM(Ccd_CompanyName) AS Ccd_CompanyName,
                                         RTRIM(Ccd_CompanyAddress1) AS Ccd_CompanyAddress1,
                                         RTRIM(Ccd_CompanyAddress2) AS Ccd_CompanyAddress2,
                                         adt_accountdesc AS Ccd_CompanyAddress3,
                                         ccd_companylogo
                                 FROM T_CompanyMaster
                                 left join t_accountdetail on adt_accountcode = Ccd_CompanyAddress3 and adt_accounttype = 'ZIPCODE'";
            #endregion

            DataSet ds = new DataSet();
            DALHelper dal = new DALHelper();

            dal.OpenDB();
            ds = dal.ExecuteDataSet(sqlStatement, CommandType.Text);
            dal.CloseDB();

            return ds;
        }

        private Image resizeImage(Image imgToResize, Size size)
        {
            int destWidth = (int)size.Width;
            int destHeight = (int)size.Height;

            Bitmap tmpBmp = new Bitmap(destWidth, destHeight);
            Graphics tmpGraphics = Graphics.FromImage((Image)tmpBmp);
            tmpGraphics.InterpolationMode = InterpolationMode.HighQualityBicubic;

            tmpGraphics.DrawImage(imgToResize, 0, 0, destWidth, destHeight);
            tmpGraphics.Dispose();

            return (Image)tmpBmp;
        }
    }
}