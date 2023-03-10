using System;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Data;
using System.Drawing;
using C1.Win.C1FlexGrid;
using System.Windows.Forms;
using Payroll.BLogic;
using Payroll.DAL;
using CommonLibrary;
using Excel = Microsoft.Office.Interop.Excel;

namespace Payroll.CFC
{
    public partial class NpaxFlexGrid : C1FlexGridPrintable
    {
        public string[] Options = new string[20];
        #region Class Variable
        public enum SelectionModeStyle
        {
            RowSelection,
            CellSelection
        };
        public class ColumnEx
        {
            public object ColumnData = null; //Data Table or ListDictionary (first field is key)
            public string ColumnHeaderName = "";
            public bool allowEdit = false;
            public bool allowSort = false;
            public bool allowResize = false;
            public bool allowMerge = false;
            public bool visible = false;
            public int maxLength = 2000;
            public Color foreColor = Color.Black;
            public Type DataType;
            public object[] LinkedControl = null;
            public string[] CheckedValue = null;
            //public Framework.Common.CommonEnum.ControlEnabledOn ControlEnabledOn = Framework.Common.CommonEnum.ControlEnabledOn.EditNewMode;
        }

        private SelectionModeStyle _SelectionModeStyle = SelectionModeStyle.CellSelection;
        private ColumnEx[] _ColumnEx;

        //private string _Msg_LongerValue = "Inputed value is too long and automatically changed";

        #endregion
        #region Properties
        [DefaultValue(SelectionModeStyle.CellSelection)]
        public SelectionModeStyle SelectionModeEx
        {
            set
            {
                this._SelectionModeStyle = value;
            }
            get
            {
                return this._SelectionModeStyle;
            }
        }
        public ColumnEx[] ColumnExSettings
        {
            set
            {
                this._ColumnEx = value;
            }
            get
            {
                return this._ColumnEx;
            }
        }
        public void GetCollection(string[] data)
        {
            int value = data.Length;
            Array.Copy(data, Options, value);

        }
        #endregion


        public NpaxFlexGrid()
        {
            InitializeComponent();
            this.AllowEditing = false;
            this.AllowMerging = AllowMergingEnum.Nodes;
            this.AllowSorting = AllowSortingEnum.SingleColumn;
            this.AutoResize = true;
            this.SelectionMode = SelectionModeEnum.Cell;
            this.Tree.Column = 0;
            this.Tree.Style = TreeStyleFlags.Simple;
            this.Cols[0].Visible = true;
            this.Cols[0].Width = 15;


            this.AllowSorting = C1.Win.C1FlexGrid.AllowSortingEnum.SingleColumn;
            this.Cols[0].AllowDragging = false;
            this.Cols[0].AllowResizing = false;
            this.ShowThemedHeaders = ShowThemedHeadersEnum.Columns;
            this.HighLight = HighLightEnum.Always;
            this.SelectionMode = SelectionModeEnum.RowRange;
            this.Styles["Normal"].WordWrap = true;
            this.ShowCursor = true;
            this.ShowSort = true;
            this.ShowErrors = true;

            this.BeforeSort += new SortColEventHandler(FlexgridControl_BeforeSort);
            this.AfterEdit += new RowColEventHandler(FlexgridControl_AfterEdit);

            this.AutoSizeRows();
        }
        void FlexgridControl_BeforeSort(object sender, SortColEventArgs e)
        {
            this.Sort(C1.Win.C1FlexGrid.SortFlags.UseColSort, 2);
            this.ShowSort = true;
        }

        public void refreshData(AggregateEnum agg, int levelOn, int groupOn, int totalOn, string caption)
        {
            this.Subtotal(agg, levelOn, groupOn, totalOn, caption);
            this.Tree.Show(2);
        }

        private void FlexgridControl_AfterResizeRow(object sender, RowColEventArgs e)
        {

        }
        private void FlexgridControl_AfterEdit(object sender, RowColEventArgs e)
        {
            //dave temporarily removed

            //if (this.DataSource != null)
            //{
            //    int nMaxLength = this._ColumnEx[e.Col].maxLength;
            //    if (nMaxLength > 0)
            //    {
            //        DataTable dt = (DataTable)this.DataSource;
            //        DataRow dr = dt.Rows[e.Row-1];                    
            //        if (dr[e.Col].GetType() == typeof(String))
            //        {
            //            string strInputVal = this[e.Row, e.Col].ToString();
            //            if (strInputVal.Length > nMaxLength)
            //            {
            //                string setValue = strInputVal.Substring(0, nMaxLength);
            //                dr[e.Col] = setValue;
            //                //this[e.Row, e.Col] = setValue;
            //                dr.SetColumnError(e.Col, _Msg_LongerValue);
            //            }
            //            else
            //            {
            //                dr.SetColumnError(e.Col, "");
            //            }
            //        }
            //    }
            //}
            //if (this._ColumnEx[0].LinkedControl[0].GetType().ToString().Contains("TextBox"))
            //{
            //    ((TextBoxControl)this._ColumnEx[0].LinkedControl[0]).Text = this[e.Row, 0].ToString();
            //}
            //if (this._ColumnEx[0].LinkedControl[1].GetType().ToString().Contains("CheckBox"))
            //{
            //    ((CheckBoxControl)this._ColumnEx[0].LinkedControl[1]).Checked = false;
            //    if (this._ColumnEx[0].CheckedValue != null && this._ColumnEx[0].CheckedValue.Length > 0)
            //    {
            //        for (int nCheckCnt = 0; nCheckCnt < this._ColumnEx[0].CheckedValue.Length; nCheckCnt++)
            //        {
            //            if (this[e.Row, 0].ToString().Contains(this._ColumnEx[0].CheckedValue[nCheckCnt]))
            //            {
            //                ((CheckBoxControl)this._ColumnEx[0].LinkedControl[1]).Checked = true;
            //            }
            //        }
            //    }
            //}
        }

        public void SetCheckBox(int leftrow, int leftcol, int bottomrow, int rightcol, C1.Win.C1FlexGrid.CheckEnum chkStat)
        {
            if (bottomrow <= 0) return;
            try
            {
                CellRange cr;
                cr = this.GetCellRange(leftrow, leftcol, bottomrow, rightcol);
                cr.Checkbox = chkStat;
            }
            catch //(Exception ex) - Jule Removed
            {
                //ex = null;
            }
        }

        public void GridFilter(string FilterString)
        {
            if (((DataTable)this.DataSource).Rows.Count <= 0) return;
            ((DataTable)this.DataSource).DefaultView.RowFilter = FilterString;

            if (this.Cols[this.Cols.Count - 1].Name != "")
            {
                this.Cols.Add();
                this.Cols[this.Cols.Count - 1].AllowEditing = false;
            }

        }
        /*
        public NpaxFlexGrid(IContainer container)
        {
            container.Add(this);           
            InitializeComponent();
            
        }
         */
        
        public bool ColumnExAdd(ColumnEx columnEx)
        {
            int nArrayCnt = 0;
            if (_ColumnEx != null) nArrayCnt = _ColumnEx.Length;
            
            Array.Resize<ColumnEx>(ref this._ColumnEx, nArrayCnt+1);
            this._ColumnEx[nArrayCnt] = columnEx;

            return true;
        }

        public bool PopulateGrid(object DataSourceObject)
        {


            this.Cols.Fixed = 0;
            this.AutoClipboard = true;
            this.AllowEditing = true;
            this.AllowSorting = AllowSortingEnum.SingleColumn;
            this.AutoResize = true;
            this.Cols.Count = 0;
            this.Cols.Fixed = 0;
            this.Rows.Count = 1;
            this.HighLight = HighLightEnum.Always;
            this.Rows.Fixed = 1;
            this.ExtendLastCol = false;


            if (this._SelectionModeStyle == SelectionModeStyle.CellSelection)
            {
                this.SelectionMode = SelectionModeEnum.CellRange;
            }
            else
            {
                this.SelectionMode = SelectionModeEnum.Row;
            }

            this.Cols.Count = this._ColumnEx.Length;
            if (DataSourceObject != null) this.DataSource = DataSourceObject;
            int nColMax = this.Cols.Count;
            for (int nCol = 0; nCol < nColMax; nCol++)
            {               
                if (this._ColumnEx.Length > nCol)
                {
                    this.Cols[nCol].Caption = this._ColumnEx[nCol].ColumnHeaderName;
                    this.Cols[nCol].AllowEditing = this._ColumnEx[nCol].allowEdit;
                    this.Cols[nCol].AllowMerging = this._ColumnEx[nCol].allowMerge;
                    this.Cols[nCol].AllowResizing = this._ColumnEx[nCol].allowResize;
                    this.Cols[nCol].AllowSorting = this._ColumnEx[nCol].allowSort;
                    this.Cols[nCol].Visible = this._ColumnEx[nCol].visible;
                    this.Cols[nCol].Style.ForeColor = this._ColumnEx[nCol].foreColor;
                    if (this._ColumnEx[nCol].DataType != null) this.Cols[nCol].DataType = this._ColumnEx[nCol].DataType;

                    ListDictionary ld = null;
                    System.Collections.Hashtable ht = null;
                    if (this._ColumnEx[nCol].ColumnData != null)
                    {
                        if (this._ColumnEx[nCol].ColumnData.GetType() == typeof(DataTable))
                        {
                            DataTable dt = (DataTable)this._ColumnEx[nCol].ColumnData;
                            if (dt.Rows.Count > 0 && dt.Columns.Count >= 2)
                            {
                                ld = new ListDictionary();
                                for (int nRow = 0; nRow < dt.Rows.Count; nRow++)
                                {
                                    ld.Add(dt.Rows[nRow][0], dt.Rows[nRow][1]);
                                }
                            }
                        }
                        else if (this._ColumnEx[nCol].ColumnData.GetType() == typeof(ListDictionary))
                        {
                            ld = (ListDictionary)this._ColumnEx[nCol].ColumnData;
                        }
                        else if (this._ColumnEx[nCol].ColumnData.GetType() == typeof(System.Collections.Hashtable))
                        {
                            ht = (System.Collections.Hashtable)this._ColumnEx[nCol].ColumnData;
                        }
                    }
                    if(ld != null)this.Cols[nCol].DataMap = ld;
                    if (ht != null) this.Cols[nCol].DataMap = ht;
                }
            }
            
            this.AutoSizeCols();

            return true;
        }
        #region Variables
        private String _FieldName = string.Empty;
        //private Object _DataBinding = null;
       // private Boolean _AllowNull = true;
        private Boolean _ShowError = true;
        private Boolean _AutoValidate = true;

        private ArrayList _ErrorList = new ArrayList();
        private ErrorProvider _ErrorProvider = new ErrorProvider();
        #endregion


      

        private void NpaxFlexGrid_AfterDataRefresh(object sender, ListChangedEventArgs e)
        {
            this.AutoSizeRows();
        }

        private void NpaxFlexGrid_Resize(object sender, EventArgs e)
        {
            this.AutoSizeRows();
        }

        private void menChangeFont_Click(object sender, EventArgs e)
        {
            FontDialog fd = new FontDialog();
            fd.Font = this.Font;
            DialogResult dr = fd.ShowDialog();

            if (dr == DialogResult.OK)
            {
                this.Font = fd.Font;
            }
        }

        private void menPrintGrid_Click(object sender, EventArgs e)
        {
            //FETCH HIDDEN COLUMNS - AND COLUMNS
            ArrayList hiddenColumns = new ArrayList();
            SortedList columns = new SortedList();
            for (int i = 0; i < this.Cols.Count; i++)
            {
                string colName = this.Cols[i].Name;
                string colCaption = this.Cols[i].Caption;

                //SHOW ALL NA TANAN
                if (!this.Cols[i].Visible)
                {
                    hiddenColumns.Add(colName);
                }
                if (colCaption == "user_login")
                    this.Cols[i].Caption = "Last Updated By";
                if (colCaption == "ludatetime")
                    this.Cols[i].Caption = "Last Updated Date";

                if (colCaption != "HeaderRowNum" && colCaption != "DetailRowNum")
                    columns.Add(colName, this.Cols[i].Caption);
            }
            //END

            Form frm = this.FindForm();
            if (frm != null)
            {
                FHDRprtSelectForm report = new FHDRprtSelectForm(this, hiddenColumns, columns, frm.Icon, Options);
                report.Text = frm.Text;
                report.ShowDialog();
            }
        }

        private void menSaveToExcel_Click(object sender, EventArgs e)
        {
            SaveFileDialog sf = new SaveFileDialog();
            sf.DefaultExt = "xls";
            sf.Filter = "Excel File (.xls)|*.xls";
            sf.ShowDialog();
            if (sf.FileName != null && sf.FileName != string.Empty)
            {
                SaveExcel(sf.FileName, FileFlags.IncludeFixedCells | FileFlags.AsDisplayed | FileFlags.VisibleOnly | FileFlags.IncludeMergedRanges);

                Excel.Application xlApp;
                Excel.Workbook xlWorkBook;
                Excel.Worksheet xlWorkSheet;

                xlApp = new Excel.Application();
                xlWorkBook = xlApp.Workbooks.Open(sf.FileName, 0, false, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
                xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);
                int y = this.Rows.Count;
                string[] companyprofile = GetCompanyProfile();

            

                string xlShiftDown = "Printed on " + DateTime.Now.ToString("MMMM/dd/yyyy hh:mm tt");
                xlWorkSheet.Cells[y + 7, 2] = xlShiftDown;

                for (int i = 1; i < 11; i++)
                {
                    Excel.Range rng = (Excel.Range)xlWorkSheet.Cells[i, 1];
                    Excel.Range row = rng.EntireRow;
                    row.Insert(Excel.XlInsertShiftDirection.xlShiftDown, false);
                }

                xlWorkSheet.Cells[1, 2] = companyprofile[0];
                xlWorkSheet.Cells[2, 2] = companyprofile[1];
                xlWorkSheet.Cells[3, 2] = companyprofile[2];

                xlWorkSheet.Cells[5, 2] = Options[11] + "   " + Options[6];

                xlWorkSheet.Cells[6, 2] = Options[0] + " " + Options[1];
                xlWorkSheet.Cells[7, 2] = Options[4] + " " + Options[5];
                xlWorkSheet.Cells[6, 4] = Options[7] + " " + Options[8];
                xlWorkSheet.Cells[7, 4] = Options[9] + " " + Options[10];

                xlWorkSheet.Cells[8, 2] = Options[2] + " " + Options[3];
                

                xlWorkBook.RefreshAll();
                xlWorkBook.Save();
                //xlWorkBook.Close(null, null, null);
                xlApp.Quit();

                releaseObject(xlWorkSheet);
                releaseObject(xlWorkBook);
                releaseObject(xlApp);

                if (!sf.FileName.ToString().Substring(sf.FileName.ToString().Length - 4, 1).Equals("."))
                    sf.FileName = sf.FileName + ".xls";

                using (System.Diagnostics.Process.Start(sf.FileName))
                {

                }
            }


        }

        private string[] GetCompanyProfile()
        {
            DataTable dt;
            string query = @"
                SELECT Ccd_CompanyCode
	                ,Ccd_CompanyName  
	                ,Ccd_CompanyAddress1  
	                ,Ccd_CompanyAddress2+' '+Adt_Accountdesc 'Ccd_CompanyAddress2'
                FROM T_CompanyMaster
                INNER JOIN T_AccountDetail 
				ON Ccd_CompanyAddress3 = Adt_AccountCode and Adt_AccountType = 'zipcode'";

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dt = dal.ExecuteDataSet(query).Tables[0];
                dal.CloseDB();
            }

  
            string[] comp = new string[3];
            comp[0] = dt.Rows[0]["Ccd_CompanyName"].ToString();
            comp[1] = dt.Rows[0]["Ccd_CompanyAddress1"].ToString();
            comp[2] = dt.Rows[0]["Ccd_CompanyAddress2"].ToString();

            return comp;
        }

        private void releaseObject(object obj)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch (Exception ex)
            {
                obj = null;
                MessageBox.Show("Unable to release the Object " + ex.ToString());
            }
            finally
            {
                GC.Collect();
            }
        } 


        private void NpaxFlexGrid_KeyDown(object sender, KeyEventArgs e)
        {
            //CTRL+A functionality similar to Excel
            if (e.Control && e.KeyCode == Keys.A)
            {
                CellRange rg = new CellRange();
                rg.c1 = Cols.Fixed;
                rg.c2 = Cols.Count - 1;
                rg.r1 = Rows.Fixed;
                rg.r2 = Rows.Count - 1;
                Select(rg);
            }

            //CTRL+C copy to clipboard
            if (e.Control && e.KeyCode == Keys.C)
                Clipboard.SetText(Clip);
        }





    }
}
