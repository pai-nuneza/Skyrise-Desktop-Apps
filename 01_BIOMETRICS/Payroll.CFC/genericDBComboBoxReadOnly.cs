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
    public partial class genericDBComboBoxReadOnly : UserControl
    {
      
        private bool _isReadOnly;

        #region Properties

        [Browsable(true)]
        [DefaultValue(false)]
        [Category("Behavior")]
        [Description("Controls whether the value in the combobox control can be changed or not")]
        public bool ReadOnly
        {
            get 
            {
                return _isReadOnly; 
            }
            set
            {
                _isReadOnly = value;

                if ((!this.DesignMode) && (this.comboBox.Items.Count == 0))
                    this.comboBox.BindCBOData();

                setReadOnly();
            }
        }

        [Browsable(true)]
        [DefaultValue(CommonEnum.GenericCBType.CURRENCY)]
        [Category("Behavior")]
        [Description("Type of data to be displayed by the combo box.")]
        public CommonEnum.GenericCBType DBType
        {
            get { return this.comboBox.DBType; }
            set { this.comboBox.DBType = value; }
        }

        //combo box properties

        public ComboBox.ObjectCollection Items
        {
            get { return this.comboBox.Items;}
        }

        public int SelectedIndex
        {
            get { return this.comboBox.SelectedIndex; }
            set { this.comboBox.SelectedIndex = value; }
        }

        public object SelectedItem
        {
            get { return this.comboBox.SelectedItem; }
            set { this.comboBox.SelectedItem = value; }
        }

        public string SelectedText
        {
            get { return this.comboBox.SelectedText; }
            set { this.comboBox.SelectedText = value; }
        }

        public object SelectedValue
        {
            get { return this.comboBox.SelectedValue; }
            set { this.comboBox.SelectedValue = value; }
        }

        public override string Text
        {
            get { return this.comboBox.Text; }
            set { this.comboBox.Text = value; }
        }

        public event EventHandler SelectedIndexChanged
        {
            add { this.comboBox.SelectedIndexChanged += value; }
            remove { this.comboBox.SelectedIndexChanged -= value; }
        }

        #endregion

        public genericDBComboBoxReadOnly()
        {
            
            InitializeComponent();

            //this.comboBox.SelectedIndexChanged;
        }

        private void setReadOnly()
        {
            this.textBox.Visible = _isReadOnly;
            this.comboBox.Visible = !_isReadOnly;

            if (_isReadOnly)
                this.textBox.Text = this.comboBox.Text;
        }

        protected override void OnCreateControl()
        {
            base.CreateControl();

            //setReadOnly();
        }
    }
}
