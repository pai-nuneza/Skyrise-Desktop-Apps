using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Payroll.CFC
{
    /// <summary>
    /// Created DEV0741 2010-07-15
    /// </summary>
    [DesignTimeVisible]
    public partial class CustomDateTimePicker : UserControl
    {
        #region Class Variable
        private bool _bIsLoading;
        private bool _bNullable;
        private bool _bReadOnly;
        private string _strCustomFormat=String.Empty;
        private Object _Value = null;
        #endregion

        #region Constructor
        public CustomDateTimePicker()
        {
            _bIsLoading = true;
            ValueChanged += new ValueChangedHandler(CustomDateTimePicker_ValueChanged);
            InitializeComponent();
            txtDateTime.Location = new Point(dtp.Location.X, dtp.Location.Y);
            
            dtp.Value = DateTime.Now;
            dtp.Checked = true;
            _bIsLoading = false;           
        }

        void CustomDateTimePicker_ValueChanged(object sender, EventArgs e)
        {
            
        }
        #endregion

        #region Delegates
        public delegate void ValueChangedHandler(object sender, EventArgs e);
        #endregion

        #region Events
        public event ValueChangedHandler ValueChanged;
        #endregion

        #region Properties

        [Bindable(true)]
        [Browsable(true)]  
        public bool Nullable
        {
            get { return _bNullable; }
            set { 
                _bNullable = value;
                if (!_bNullable)
                    this.Value = DateTime.Now;
                else
                    this.Value = null;
            }
        }
        [Bindable(true)]
        [Browsable(true)]  
        public DateTimePickerFormat Format
        {
            get { return dtp.Format; }
            set { dtp.Format = value; }
        }
        [Bindable(true)]
        [Browsable(true)]          
        public string CustomFormat
        {
            get { return _strCustomFormat; }
            set 
            { 
                _strCustomFormat = value;
                dtp.CustomFormat = _strCustomFormat;
            }
        }

        [Bindable(true)]
        [Browsable(true)]        
        public Object Value
        {
            get { return _Value; }
            set 
            {
                try
                {
                    _Value = value;
                    if (value == null || value == DBNull.Value||value.ToString()==String.Empty)
                    {                        
                        Tip.RemoveAll();
                        if (_bNullable)
                        {
                            dtp.CustomFormat = " ";
                            dtp.Checked = false;                           
                        }
                        else
                        {
                            string tooltip = "Please input date!";
                            Tip.SetToolTip(this, tooltip);
                            Tip.SetToolTip(txtDateTime, tooltip);
                            Tip.SetToolTip(dtp, tooltip);
                        }                        
                        txtDateTime.Text = String.Empty;                        
                    }
                    else
                    {
                        dtp.Value = Convert.ToDateTime(value);
                        dtp.CustomFormat = _strCustomFormat;
                        dtp.Checked = true;
                        txtDateTime.Text = dtp.Value.ToString(_strCustomFormat);
                        string tooltip = dtp.Value.Date.ToLongDateString();
                        Tip.SetToolTip(this, tooltip);
                        Tip.SetToolTip(txtDateTime, tooltip);
                        Tip.SetToolTip(dtp, tooltip);
                    }
                     
                    EventArgs e = new EventArgs();                    
                    ValueChanged(this, e);
                }
                catch(Exception ex) 
                { 
                    errorProvider.SetError(this, "Please enter valid date!");
                    txtDateTime.Text = String.Empty;
                    txtDateTime.Focus();
 
                    Tip.RemoveAll();
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                }
                Application.DoEvents();
            }
        }

        [Bindable(true)]
        [Browsable(true)]  
        public DateTime DateTimeValue
        {
            get { return Convert.ToDateTime(_Value); }
        }
        [Bindable(true)]
        [Browsable(true)]  
        public DateTime MinDate
        {
            get { return dtp.MinDate; }
            set { dtp.MinDate = value; }
        }
        [Bindable(true)]
        [Browsable(true)]  
        public DateTime MaxDate
        {
            get { return dtp.MaxDate; }
            set { dtp.MaxDate = value; }
        }

        [Bindable(true)]
        [Browsable(true)]  
        public bool ReadOnly
        {
            get { return _bReadOnly; }
            set {
                _bReadOnly = value;
                if (_bReadOnly)
                {
                    txtDateTime.ReadOnly = true;
                    dtp.Enabled = false;
                }
                else
                { 
                    txtDateTime.ReadOnly = false;
                    dtp.Enabled = true;
                }
            }
        }

        public override String Text
        {
            get { return txtDateTime.Text; }
          
        }
        #endregion
 

        private void txtDateTime_TextChanged(object sender, EventArgs e)
        {
            if (txtDateTime.Text.Trim().Equals(String.Empty)) 
                this.Value = null;                 
        }

        private void dtp_ValueChanged(object sender, EventArgs e)
        {
            //if(!_bIsLoading)
                this.Value = dtp.Value;
        }

        private void txtDateTime_Leave(object sender, EventArgs e)
        {
            try
            {
                if (txtDateTime.Text.Trim().Equals(String.Empty))
                {
                    if (!_bNullable)
                    {
                        errorProvider.SetError(this, "Please enter valid date!");
                        txtDateTime.Focus();
                    }
                    else
                        this.Value = null;
                }
                else
                {
                    errorProvider.Clear();
                    DateTime dtval = new DateTime();
                    if (DateTime.TryParse(txtDateTime.Text.Trim(), out dtval))
                        this.Value = dtval;
                    else
                    {
                        txtDateTime.Text = String.Empty;

                        errorProvider.SetError(this, "Please enter valid date!");
                        txtDateTime.Focus();

                        this.Value = null;
                    }
                }
            }
            catch 
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();

                errorProvider.SetError(this, "Please enter valid date!");
                txtDateTime.Focus();

                this.Value = null;
                txtDateTime.Text = String.Empty;
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }

        private void dtp_Resize(object sender, EventArgs e)
        {
            txtDateTime.Size = new Size(dtp.Size.Width - 32,dtp.Size.Height);
        }

        private void dtp_LocationChanged(object sender, EventArgs e)
        {
            txtDateTime.Location = new Point(dtp.Location.X, dtp.Location.Y);
        }

        private void ctrlDateTimePicker_Load(object sender, EventArgs e)
        {
            this.Size = new Size(dtp.Size.Width, dtp.Size.Height);           
        }

        private void txtDateTime_KeyPress(object sender, KeyPressEventArgs e)
        {
            int nKey = (int)e.KeyChar;
            if (!((nKey >= 47 && nKey <= 57)
                //|| nKey == 92
                || nKey == 45
                || nKey == 8))
                e.Handled = true;                
        }

        private void CustomDateTimePicker_Enter(object sender, EventArgs e)
        {
            txtDateTime.SelectAll();
            txtDateTime.Focus();
        }
    }
}