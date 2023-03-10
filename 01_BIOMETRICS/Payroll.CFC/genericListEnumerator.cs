using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using Payroll.DAL;

namespace Payroll.CFC
{
    public partial class genericListEnumerator : UserControl
    {
        public genericListEnumerator()
        {
            InitializeComponent();
        }

        #region CONTROL FIELDS
        private String searchQuery;
        private DataTable dtSourceTable;
        private DataView dvSourceView;
        private ParameterInfo[] paramInfo;
        private int index = 0;
        #endregion

        #region CONTROL PROPERTIES

        public ParameterInfo[] ParamInfo
        {
            get
            {
                return paramInfo;
            }
            set
            {
                paramInfo = value;
            }
        }

        public String SearchQuery
        {
            get
            {
                return searchQuery;
            }
            set
            {
                searchQuery = value;
                if (searchQuery != null && searchQuery.Trim() != String.Empty)
                    RefreshData();
            }
        }

        public DataRow PrimaryKeySelected
        {
            get
            {
                if (index >= dvSourceView.ToTable().Rows.Count)
                    index = 0;
                if (index < dvSourceView.ToTable().Rows.Count)
                    return dvSourceView.ToTable().Rows[index];
                else
                    return null;
            }
        }
        #endregion

        #region CONTROL EVENT HANDLERS

        private void txtLookupCode_KeyUp(object sender, KeyEventArgs e)
        {
            SetFilter();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            RefreshData();
        }

        private void btnPrevious_Click(object sender, EventArgs e)
        {
            index = (-1 + index + dvSourceView.ToTable().Rows.Count) % dvSourceView.ToTable().Rows.Count;
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            index = (1 + index) % dvSourceView.ToTable().Rows.Count;
        }

        #endregion

        #region CONTROL EVENTS

        public event KeyEventHandler LookupCodeKeyUp
        {
            add
            {
                this.txtLookupCode.KeyUp += value;
            }
            remove
            {
                this.txtLookupCode.KeyUp -= value;
            }
        }

        public event EventHandler PreviousClick
        {
            add
            {
                this.btnPrevious.Click += value;
            }
            remove
            {
                this.btnPrevious.Click -= value;
            }
        }

        public event EventHandler NextClick
        {
            add
            {
                this.btnNext.Click += value;
            }
            remove
            {
                this.btnNext.Click -= value;
            }
        }

        public event EventHandler SearchClick
        {
            add
            {
                this.btnSearch.Click += value;
            }
            remove
            {
                this.btnSearch.Click -= value;
            }
        }

        #endregion

        #region CONTROL PRIVATE METHODS
        private void RefreshData()
        {
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                if (paramInfo == null || paramInfo.Length == 0)
                    dtSourceTable = dal.ExecuteDataSet(searchQuery, CommandType.Text).Tables[0];
                else
                    dtSourceTable = dal.ExecuteDataSet(searchQuery, CommandType.Text, paramInfo).Tables[0];
                dvSourceView = dtSourceTable.DefaultView;
                SetFilter();
                dal.CloseDB();
            }
        }

        private void SetFilter()
        {
            dvSourceView.RowFilter = getSearchQuery();
        }

        private string getSearchQuery()
        {
            StringBuilder sbSearch = new StringBuilder();
            int i = 0;
            while (i < dtSourceTable.Columns.Count
                && dtSourceTable.Columns[i].DataType != Type.GetType("System.String"))
                ++i;
            sbSearch.Append("[");
            sbSearch.Append(dtSourceTable.Columns[i++].ColumnName);
            sbSearch.Append("]");
            sbSearch.Append(" like \'");
            sbSearch.Append(txtLookupCode.Text.Replace("\'", "").Trim());
            sbSearch.Append("%\' ");
            while (i < dtSourceTable.Columns.Count)
            {
                if (dtSourceTable.Columns[i].DataType == Type.GetType("System.String"))
                {
                    sbSearch.Append(" or ");
                    sbSearch.Append("[");
                    sbSearch.Append(dtSourceTable.Columns[i].ColumnName);
                    sbSearch.Append("]");
                    sbSearch.Append(" like \'");
                    sbSearch.Append(txtLookupCode.Text.Replace("\'", "").Trim());
                    sbSearch.Append("%\' ");
                }
                ++i;
            }
            return sbSearch.ToString();
        }
        #endregion
    }
}
