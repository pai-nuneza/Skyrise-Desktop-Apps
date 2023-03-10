using System;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using C1.Win.C1FlexGrid;

namespace Payroll.CFC
{
	/// <summary>
	/// The print preview form.
	/// </summary>
	public class PreviewForm : System.Windows.Forms.Form
	{
        private C1FlexGrid _grid;

		/// <summary>
		/// Creates a new instance of the PrintFrom class.
		/// </summary>
        public PreviewForm(C1FlexGrid grid)
            : base()
		{
            _grid = grid;
            Init();
        }

		/// <summary>
		/// Additional initialization of the form.
		/// </summary>
        protected virtual void Init()
        {
            Rectangle screen = Screen.GetWorkingArea(this);
            Size = new Size(
                (int)Math.Round(screen.Height * 0.75, 0),
                (int)Math.Round(screen.Height * 0.75, 0));
            Location = new Point((screen.Width - Width) / 2, (screen.Height - Height) / 2);
            //FormBorderStyle = FormBorderStyle.FixedToolWindow;
            ResumeLayout(false);
        }

		/// <summary>
		/// Overloaded. Overridden. Releases all resources used by the Control.
		/// </summary>
		/// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
		}
        #region Internal properties
        internal C1FlexGrid Grid
        {
            get { return _grid; }
            set { _grid = value; }
        }
        #endregion

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // PreviewForm
            // 
            this.ClientSize = new System.Drawing.Size(292, 266);
            this.Name = "PreviewForm";
            this.Text = "C1FlexGrid preview";
            this.ResumeLayout(false);

        }
    }
}
