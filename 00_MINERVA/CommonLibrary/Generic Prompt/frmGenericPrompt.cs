using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace CommonLibrary
{
    partial class frmGenericPrompt : Form
    {
        // to be returned
        public CommonEnum.GenieDialogResult GenieDialogResult = CommonEnum.GenieDialogResult.Cancel;

        private bool IsShowDetails = false;

        private DataTable dtMessageCodeDetails = new DataTable();
        private CommonEnum.GenieMessageBoxIcon MessageBoxIcon;
        private GenericPromptBL GenericPromptBL = new GenericPromptBL();

        private List<string> Fields = new List<string>();
        private string Field1 = "";
        private string Field2 = "";
        private string Field3 = "";
        private string MenuCode = "";

        private const int MaxLinesMessage = 5;
        private const int MaxCharsMessage = 222;
        private const int MaxLinesSuggestion = 4;
        private const int MaxCharsSuggestion = 173;
        private Size MaxSize = new Size(527, 312);
        private Size MinSize = new Size(527, 215);
        private Color OrigBtnColor = Color.White;
        private Color OnEnterBtnColor = Color.Wheat;
        private bool AllowView = true;
        public bool IsView
        {
            get { return AllowView; }
        }

        #region constructors

        public frmGenericPrompt(int MessageCode, List<string> Fields)
        {
            InitializeComponent();

            this.Fields = Fields;
        }

        #region With menu codes
        public frmGenericPrompt(int MessageCode, string MenuCode)
        {
            InitializeComponent();

            this.dtMessageCodeDetails = GenericPromptBL.GetMessageCodeDetails(MessageCode);
            if (dtMessageCodeDetails == null)
            {
                AllowView = false;
            }
            this.MenuCode = MenuCode;
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        public frmGenericPrompt(int MessageCode, List<string> Fields, string MenuCode)
            : this(MessageCode, MenuCode)
        {
            this.Fields = Fields;
        }

        public frmGenericPrompt(int MessageCode, string Field, string MenuCode)
            : this(MessageCode, MenuCode)
        {
            this.Field1 = Field;
        }

        public frmGenericPrompt(int MessageCode, string Field1, string Field2, string MenuCode)
            : this(MessageCode, MenuCode)
        {
            this.Field1 = Field1;
            this.Field2 = Field2;
        }

        public frmGenericPrompt(int MessageCode, string Field1, string Field2, string Field3, string MenuCode)
            : this(MessageCode, MenuCode)
        {
            this.Field1 = Field1;
            this.Field2 = Field2;
            this.Field3 = Field3;
        }
        #endregion
        #endregion

        // for the meantime
        public frmGenericPrompt(CommonEnum.GenieMessageBoxIcon MessageBoxIcon, string Message)
        {
            InitializeComponent();

            this.MessageBoxIcon = MessageBoxIcon;
            this.txtMessage.Text = Message.Replace("\n", "\r\n");
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void frmGenericPrompt_Load(object sender, EventArgs e)
        {
            if (dtMessageCodeDetails != null)
            {
                btnYes.TabStop = btnNo.TabStop = btnCancel.TabStop = false;

                StretchUnstretchForm();
                this.AcceptButton = btnYes;

                if (dtMessageCodeDetails.Rows.Count > 0)
                {
                    #region Set UI
                    DataRow row = this.dtMessageCodeDetails.Rows[0];

                    string MessageCode = row["Msm_MessageCode"].ToString();
                    string Message = row["Msm_MessageName"].ToString().Replace("\\n", "\n").Replace("\\t", "\t");
                    string Suggestion = row["Msm_Suggestion"].ToString().Replace("\\n", "\n").Replace("\\t", "\t");
                    char MessageType = Convert.ToChar(row["Msm_MessageType"]);

                    btnYes.Visible = btnNo.Visible = btnCancel.Visible = false;
                    // S - SQL Error			OK
                    // R - Runtime Error		OK
                    // V - Validation Error	    OK
                    // I - Information	        OK
                    // C - Confirmation		    YES NO

                    switch (MessageType)
                    {
                        #region Set buttons and icon
                        case 'S':
                            btnYes.Text = "OK";
                            btnYes.Visible = true;
                            btnYes.TabStop = true;
                            pbIcon.BackgroundImage = global::CommonLibrary.Properties.Resources.Error;
                            break;
                        case 'R':
                            btnYes.Text = "OK";
                            btnYes.Visible = true;
                            btnYes.TabStop = true;
                            pbIcon.BackgroundImage = global::CommonLibrary.Properties.Resources.Error;
                            break;
                        case 'V':
                            btnYes.Text = "OK";
                            btnYes.Visible = true;
                            btnYes.TabStop = true;
                            pbIcon.BackgroundImage = global::CommonLibrary.Properties.Resources.Error;
                            break;
                        case 'I':
                            btnYes.Text = "OK";
                            btnYes.Visible = true;
                            btnYes.TabStop = true;
                            pbIcon.BackgroundImage = global::CommonLibrary.Properties.Resources.Information;
                            break;
                        case 'C':
                            btnYes.Visible = btnNo.Visible = true;
                            btnYes.TabStop = btnNo.TabStop = true;
                            pbIcon.BackgroundImage = global::CommonLibrary.Properties.Resources.Question;
                            this.AcceptButton = btnNo;
                            break;
                        #endregion
                    }

                    #region Set Message
                    if (Fields.Count > 0)
                    {
                        string list = "";
                        for (int i = 0; i < Fields.Count; i++)
                        {
                            list += string.Format("{0}. {1}", i + 1, Fields[i]);

                            if (i < Fields.Count - 1)
                                list += "\n";
                        }
                        Message = string.Format(Message, list);
                    }

                    if (Field1 != "" && Field2 != "" && Field3 != "")
                        Message = string.Format(Message, Field1, Field2, Field3);
                    else if (Field1 != "" && Field2 != "")
                        Message = string.Format(Message, Field1, Field2);
                    else if (Field1 != "")
                        Message = string.Format(Message, Field1);
                    #endregion

                    txtMessage.Text = Message.Replace("\n", "\r\n");
                    int lineCount = txtMessage.GetLineFromCharIndex(txtMessage.Text.Length - 1);
                    if (lineCount > 4)
                    {
                        this.MaxSize = this.MaximumSize = new Size(this.MaxSize.Width, this.MaxSize.Height + ((lineCount - 4) * 15));
                        this.MinSize = this.MinimumSize = new Size(this.MinSize.Width, this.MinSize.Height + ((lineCount - 4) * 15));
                        this.Size = MinSize;
                        if (this.Size.Height <= 350)
                        {
                            this.pnlMessage.Size = new Size(pnlMessage.Width, pnlMessage.Height + ((lineCount - 4) * 15));
                            this.grpDetails.Location = new Point(grpDetails.Location.X, grpDetails.Location.Y + ((lineCount - 4) * 15));
                            this.txtMessage.Size = new Size(txtMessage.Width, txtMessage.Height + ((lineCount - 4) * 15));
                        }
                        else
                        {
                            this.pnlMessage.Size = new Size(pnlMessage.Width, 275);
                            this.grpDetails.Location = new Point(grpDetails.Location.X, (grpDetails.Location.Y + ((lineCount - 4) * 15)) - (this.Height - 350));
                            this.txtMessage.Size = new Size(txtMessage.Width, 235);
                            this.MaxSize = this.MaximumSize = new Size(this.MaxSize.Width, this.MaxSize.Height - (this.Height - 350));
                            this.MinSize = this.MinimumSize = new Size(this.MinSize.Width, 350);
                            this.Size = new Size(this.Width, 350);
                        }
                    }
                    lblDetails.Visible = false;
                    if (Suggestion != "")
                    {
                        lblDetails.Visible = true;
                        txtSuggestion.Text = Suggestion;
                    }
                    #endregion

                    GenericPromptBL.AddSystemLog(Convert.ToInt32(MessageCode), this.MenuCode);
                }
                else
                {
                    #region Set default UI
                    btnYes.Visible = btnNo.Visible = btnCancel.Visible = false;
                    bool var = btnCancel.TabStop;

                    switch (MessageBoxIcon)
                    {
                        case CommonEnum.GenieMessageBoxIcon.CONFIRMATION:
                            btnYes.Visible = btnNo.Visible = true;
                            btnYes.TabStop = btnNo.TabStop = true;
                            pbIcon.BackgroundImage = global::CommonLibrary.Properties.Resources.Question;
                            this.AcceptButton = btnNo;
                            break;
                        case CommonEnum.GenieMessageBoxIcon.ERROR:
                            btnYes.Visible = true;
                            btnYes.TabStop = true;
                            btnYes.Text = "OK";
                            pbIcon.BackgroundImage = global::CommonLibrary.Properties.Resources.Error;
                            break;
                        case CommonEnum.GenieMessageBoxIcon.INFORMATION:
                            btnYes.Visible = true;
                            btnYes.TabStop = true;
                            btnYes.Text = "OK";
                            pbIcon.BackgroundImage = global::CommonLibrary.Properties.Resources.Information;
                            break;
                        case CommonEnum.GenieMessageBoxIcon.WARNING:
                            btnYes.Visible = true;
                            btnYes.TabStop = true;
                            btnYes.Text = "OK";
                            pbIcon.BackgroundImage = global::CommonLibrary.Properties.Resources.Warning;
                            break;
                    }


                    //txtMessage.ScrollBars = ScrollBars.None;
                    int lineCount = txtMessage.GetLineFromCharIndex(txtMessage.Text.Length - 1);
                    if (lineCount > 4)
                    {
                        this.MaximumSize = new Size(this.Width, this.Height + ((lineCount - 4 + 1) * 15));
                        this.MinimumSize = new Size(this.Width, this.Height + ((lineCount - 4 + 1) * 15));
                        this.Size = new Size(this.Width, this.Height + ((lineCount - 4 + 1) * 15));
                        if (this.Size.Height <= 350)
                        {
                            this.pnlMessage.Size = new Size(pnlMessage.Width, pnlMessage.Height + ((lineCount - 4 + 1) * 15));
                            this.txtMessage.Size = new Size(txtMessage.Width, txtMessage.Height + ((lineCount - 4 + 1) * 15));
                        }
                        else
                        {
                            this.pnlMessage.Size = new Size(pnlMessage.Width, 275);
                            this.txtMessage.Size = new Size(txtMessage.Width, 235);
                            this.MaximumSize = new Size(this.Width, 350);
                            this.MinimumSize = new Size(this.Width, 350);
                            this.Size = new Size(this.Width, 350);
                        }
                    }
                    grpDetails.Visible = false;
                    lblDetails.Visible = false;


                    #endregion
                }
                this.ShowIcon = false;
            }
            else
            {
                this.Close();
            }
        }

        private void btnYes_Click(object sender, EventArgs e)
        {
            GenieDialogResult = CommonEnum.GenieDialogResult.Yes;

            this.Close();
        }

        private void btnNo_Click(object sender, EventArgs e)
        {
            GenieDialogResult = CommonEnum.GenieDialogResult.No;

            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            GenieDialogResult = CommonEnum.GenieDialogResult.Cancel;

            this.Close();
        }

        private void txt_TextChanged(object sender, EventArgs e)
        {
            //TextBox txtBox = (sender as TextBox);

            //int noLines = txtBox.Lines.Length;
            //int noChars = txtBox.Text.Length;

            //txtBox.ScrollBars = noLines > MaxLinesMessage || noChars > MaxCharsMessage ? ScrollBars.Vertical : ScrollBars.None;

            //txtBox.Location = noLines > MaxLinesMessage || noChars > MaxCharsMessage ? new Point(112, 23) : new Point(128, 37);
        }

        private void txtSuggestion_TextChanged(object sender, EventArgs e)
        {
            int noLines = txtSuggestion.Lines.Length;
            int noChars = txtSuggestion.Text.Length;

            txtSuggestion.ScrollBars = noLines > MaxLinesSuggestion || noChars > MaxCharsSuggestion ? ScrollBars.Vertical : ScrollBars.None;
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.IsShowDetails = !this.IsShowDetails;

            StretchUnstretchForm();
        }

        private void StretchUnstretchForm()
        {
            if (IsShowDetails) //stretch
            {
                this.MaximumSize = this.MaxSize;
                this.MinimumSize = this.MaxSize;
                this.Size = this.MaxSize; 
                this.lblDetails.Text = "Fewer Details <<";
            }
            else
            {
                this.MaximumSize = this.MinSize;
                this.MinimumSize = this.MinSize;
                this.Size = this.MinSize;
                this.lblDetails.Text = "More Details >>";
            }
        }

        private void btn_MouseEnter(object sender, EventArgs e)
        {
            //Button btn = sender as Button;

            //btn.BackColor = OnEnterBtnColor;
        }

        private void btn_MouseLeave(object sender, EventArgs e)
        {
            //Button btn = sender as Button;

            //btn.BackColor = OrigBtnColor;
        }
    }
}