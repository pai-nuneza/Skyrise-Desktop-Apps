using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using UploadDownloadSchedulerConsole;
using Posting.BLogic;

namespace UploadDownloadSchedulerApp
{
    public partial class frmManualTrigger : Form
    {
        private delegate void Function();// a simple delegate for marshalling calls from event handlers to the GUI thread
        string[] arg = new string[3];
        bool IsOnprogress = false;

        public frmManualTrigger()
        {
            InitializeComponent();
            dtpFrom.Value = DateTime.Now;
            dtpTo.Value = DateTime.Now;
            cboServiceCode.Text = "LOGUPLOADING";
            this.Size = new Size(334, 160);

        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            arg[0] = "LOGUPLOADING";
            arg[1] = dtpFrom.Value.ToShortDateString();
            arg[2] = dtpTo.Value.ToShortDateString();
            try
            {
                bgwPosting.RunWorkerAsync();
            }
            catch (Exception err)
            {
                if (err.Message.Equals("This BackgroundWorker is currently busy and cannot run multiple tasks concurrently."))
                    MessageBox.Show("Time log posting is on progress.","Warning",MessageBoxButtons.OK,MessageBoxIcon.Exclamation);
                else
                    MessageBox.Show(err.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void bgwPosting_DoWork(object sender, DoWorkEventArgs e)
        {
            if (!IsOnprogress)
            {
                BeginInvoke(new Function(delegate()
                {
                    tmrPosting.Enabled = true;
                    tmrPosting.Start();
                    pgbPosting.Maximum = 100;
                    pgbPosting.Visible = true;
                    pgbPosting.Value = 0;
                    pgbPosting.Step = 1;
                    this.Size = new Size(334, 208);
                    //btnStart.Enabled = false;
                }));

                try
                {
                    SchedulerConsole.Main(arg);
                }
                catch { }

                BeginInvoke(new Function(delegate()
                {
                    try
                    {
                        //btnStart.Enabled = true;
                        //this.Size = new Size(334, 160);
                        //tmrPosting.Stop();
                        //pgbPosting.Visible = false;
                        //pgbPosting.Value = 0;
                        //Globals.Progress = 0; 
                    }
                    catch { }
                }));

                IsOnprogress = false;
            }
        }

        private void tmrPosting_Tick(object sender, EventArgs e)
        {
            pgbPosting.Value = Convert.ToInt16(Globals.Progress);
            
            if (Globals.Progress > 0)
            {
                this.Size = new Size(334, 208);
                pgbPosting.Visible = true;
                lblProcess.Text = Globals.ProgressProcess;
            }
            else
            {
                this.Size = new Size(334, 160);
                pgbPosting.Visible = false;
            }
        }

    }
}
