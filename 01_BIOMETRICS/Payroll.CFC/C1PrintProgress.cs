using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Threading;

namespace Payroll.CFC
{
	/// <summary>
	/// Print progress window.
	/// </summary>
	public class C1PrintProgress : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Label lblProgress;
		private bool _cancelClicked = false;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		/// <summary>
		/// Initializes a new instance of the C1PrintProgress class.
		/// </summary>
		public C1PrintProgress()
		{
			InitializeComponent();
		}
		/// <summary>
		/// Overloaded. Releases the resources used by the component.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.btnCancel = new System.Windows.Forms.Button();
            this.lblProgress = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(102, 60);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 0;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // lblProgress
            // 
            this.lblProgress.Location = new System.Drawing.Point(32, 20);
            this.lblProgress.Name = "lblProgress";
            this.lblProgress.Size = new System.Drawing.Size(219, 23);
            this.lblProgress.TabIndex = 1;
            this.lblProgress.Text = "Printing is started...";
            this.lblProgress.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // C1PrintProgress
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(277, 99);
            this.ControlBox = false;
            this.Controls.Add(this.lblProgress);
            this.Controls.Add(this.btnCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "C1PrintProgress";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Printing Report";
            this.TopMost = true;
            this.ResumeLayout(false);

		}
		#endregion

		private void btnCancel_Click(object sender, System.EventArgs e)
		{
			this.btnCancel.Enabled = false;
			_cancelClicked = true;
		}

		/// <summary>
		/// Gets or sets the text used to display the current progress when the grid is printed/previewed.
		/// </summary>
		public string TxtPrinting
		{
			get { return this.lblProgress.Text; }
			set { this.lblProgress.Text = value; }
		}

		/// <summary>
		/// Gets or sets the text for the Cancel button.
		/// </summary>
		public string TxtCancel
		{
			get { return this.btnCancel.Text; }
			set { this.btnCancel.Text = value; }
		}

		/// <summary>
		/// Gets or sets the text for the Windows caption.
		/// </summary>
		public string TxtTitle
		{
			get { return this.Text; }
			set { this.Text = value; }
		}

		/// <summary>
		/// Gets a value indicating if the print/preview was cancelled.
		/// </summary>
		public bool CancelClicked
		{
			get { return _cancelClicked; }
		}
	}
}
