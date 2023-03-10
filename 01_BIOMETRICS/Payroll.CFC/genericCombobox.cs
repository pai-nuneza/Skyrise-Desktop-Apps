using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using CommonLibrary;

namespace Payroll.CFC
{
    public partial class genericCombobox : UserControl
    {
        private bool isReadOnly = false;


        public CommonEnum.GenericCBType DBType
        {
            get
            {
                return this.Combobox.DBType;
            }
            set
            {
                this.Combobox.DBType = value;
            }
        }

        public bool ReadOnly
        {
            get 
            {
                return isReadOnly;
            }
            set
            {
                this.isReadOnly = value;
                SwitchContols();
            }
        }

        public genericCombobox()
        {
            InitializeComponent();
            SwitchContols();
        }

        void Combobox_myEvent(object sender, EventArgs e)
        {
            string s = "asdf";
            s = "sss";
        }

        private void SwitchContols()
        {
            if (isReadOnly == true)
            {
                this.txtReadOnly.Visible = true;
                this.Combobox.Visible = false;
                //SetTxtReadOnly();
            }
            else
            {
                this.txtReadOnly.Visible = false;
                this.Combobox.Visible = true;
            }
        }

        private void SetTxtReadOnly()
        {
            if (this.Combobox.Items.Count > 0)
                this.txtReadOnly.Text = this.Combobox.GetItemText(this.Combobox.Items[this.Combobox.SelectedIndex]);
            else
                this.txtReadOnly.Text = String.Empty;
        }

      
    }
}
