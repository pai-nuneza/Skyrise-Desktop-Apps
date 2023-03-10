using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace Payroll.CFC
{
	/// <summary>
	/// Provides UI for printing, exporting or previewing C1FlexGridPrintable.
	/// </summary>
	public class PrintOptionsForm : System.Windows.Forms.Form
	{
		/// <summary>
		/// Describes the desired action, such as printing or exporting.
		/// </summary>
		public class ActionItem
        {
			/// <summary>
			/// The empty action item (no action).
			/// </summary>
			static public ActionItem Empty = new ActionItem(ActionFlags.None, null);

            private ActionFlags _action = ActionFlags.None;
            private C1.C1Preview.Export.ExportProvider _provider = null;

			/// <summary>
			/// Creates a new instance of the ActionItem class. 
			/// </summary>
            public ActionItem(ActionFlags action, C1.C1Preview.Export.ExportProvider provider)
            {
                Action = action;
                Provider = provider;
            }

			/// <summary>
			/// Gets or sets the desired action.
			/// </summary>
			public ActionFlags Action
            {
                get { return _action; }
                set { _action = value; }
            }

			/// <summary>
			/// Gets or sets the export provider.
			/// </summary>
            public C1.C1Preview.Export.ExportProvider Provider
            {
                get { return _provider; }
                set { _provider = value; }
            }

            /// <summary>
            /// 
            /// </summary>
            public override string ToString()
            {
                if (Provider != null)
                    return string.Format("Export to {0}", Provider.FormatName);
                else
                    switch (Action)
                    {
                        case ActionFlags.None:
                            return "None";
                        case ActionFlags.Preview:
                            return "Print Preview";
                        case ActionFlags.Print:
                            return "Print";
                        default:
                            goto case ActionFlags.None;
                    }
            }
		}

        /// <summary>
        /// Describes the row height.
        /// </summary>
        public class RowHeightItem
        {
            /// <summary>
            /// 
            /// </summary>
            public RowHeightEnum RowHeight = RowHeightEnum.StretchToFit;

            /// <summary>
            /// 
            /// </summary>
            public RowHeightItem(RowHeightEnum rh)
            {
                RowHeight = rh;
            }

            /// <summary>
            /// 
            /// </summary>
            public override string ToString()
            {
                switch (RowHeight)
                {
                    case RowHeightEnum.StretchToFit:
                        return "Stretch to fit";
                    case RowHeightEnum.LikeGrid:
                        return "Fixed";
                    case RowHeightEnum.StretchToMax:
                        return "Stretch up to value";
                    default:
                        goto case RowHeightEnum.StretchToFit;
                }
            }

            /// <summary>
            /// 
            /// </summary>
            public static RowHeightItem[] RowHeightItems = new RowHeightItem[]
			{
				new RowHeightItem(RowHeightEnum.StretchToFit),
				new RowHeightItem(RowHeightEnum.LikeGrid),
				new RowHeightItem(RowHeightEnum.StretchToMax),
			};
        }
        
        /// <summary>
		/// 
		/// </summary>
		public class PageBreakItem
		{
			/// <summary>
			/// 
			/// </summary>
			public PageBreaksEnum PageBreaks = PageBreaksEnum.FitIntoArea;
			/// <summary>
			/// 
			/// </summary>
			public PageBreakItem(PageBreaksEnum pb)
			{
				PageBreaks = pb;
			}
			/// <summary>
			/// 
			/// </summary>
			public override string ToString()
			{
				switch (PageBreaks)
				{
					case PageBreaksEnum.FitIntoArea:
						return "Fit columns on one page";
					case PageBreaksEnum.OnColumn:
						return "Break on any column";
                    default:
						goto case PageBreaksEnum.FitIntoArea;
				}
			}
			/// <summary>
			/// 
			/// </summary>
			public static PageBreakItem[] PageBreakItems = new PageBreakItem[]
			{
				new PageBreakItem(PageBreaksEnum.FitIntoArea),
				new PageBreakItem(PageBreaksEnum.OnColumn),
			};
		}

		/// <summary>
		/// 
		/// </summary>
		public class FillEmptyItem
		{
			/// <summary>
			/// 
			/// </summary>
			public FillEmptyEnum FillEmpty = FillEmptyEnum.ExtendAll;
			/// <summary>
			/// 
			/// </summary>
			public FillEmptyItem(FillEmptyEnum fe)
			{
				FillEmpty = fe;
			}
			/// <summary>
			/// 
			/// </summary>
			public override string ToString()
			{
				switch (FillEmpty)
				{
					case FillEmptyEnum.ExtendAll:
						return "Stretch all columns";
					case FillEmptyEnum.None:
						return "Do not stretch";
					case FillEmptyEnum.ExtendLastCol:
						return "Stretch last column";
					default:
						goto case FillEmptyEnum.ExtendAll;
				}
			}
			/// <summary>
			/// 
			/// </summary>
			public static FillEmptyItem[] FillEmptyItems = new FillEmptyItem[]
			{
				new FillEmptyItem(FillEmptyEnum.ExtendAll),
				new FillEmptyItem(FillEmptyEnum.None),
				new FillEmptyItem(FillEmptyEnum.ExtendLastCol),
			};
		}

		/// <summary>
		/// 
		/// </summary>
		public class WrapTextItem
		{
			/// <summary>
			/// 
			/// </summary>
			public WrapTextEnum WrapText = WrapTextEnum.Wrap;
			/// <summary>
			/// 
			/// </summary>
			public WrapTextItem(WrapTextEnum wt)
			{
				WrapText = wt;
			}
			/// <summary>
			/// 
			/// </summary>
			public override string ToString()
			{
				switch (WrapText)
				{
					case WrapTextEnum.Wrap:
						return "Wrap all text";
					case WrapTextEnum.NoWrap:
						return "Do not wrap text";
					case WrapTextEnum.LikeCell:
						return "Wrap like cells";
					default:
						goto case WrapTextEnum.Wrap;
				}
			}
			/// <summary>
			/// 
			/// </summary>
			public static WrapTextItem[] WrapTextItems = new WrapTextItem[]
			{
				new WrapTextItem(WrapTextEnum.Wrap),
				new WrapTextItem(WrapTextEnum.NoWrap),
				new WrapTextItem(WrapTextEnum.LikeCell),
			};
		}
        
        // filled in Form_Load
        private C1.C1Preview.Export.ExportProvider[] _providers = null;

		/// <summary>
		/// 
		/// </summary>
        public event EventHandler ExporterChanged;
		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
        protected virtual void OnExporterChanged(EventArgs e)
        {
            if (ExporterChanged != null)
                ExporterChanged(this, e);
        }

		/// <summary>
		/// 
		/// </summary>
        public event EventHandler ActionChanged;
		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
        protected virtual void OnActionChanged(EventArgs e)
        {
            if (ActionChanged != null)
                ActionChanged(this, e);
        }

        /// <summary>
        /// 
        /// </summary>
        protected C1.C1Preview.Export.Exporter m_exporter = null;
        /// <summary>
		/// 
		/// </summary>
        protected ActionFlags m_action = ActionFlags.None;
        /// <summary>
		/// 
		/// </summary>
        protected ActionFlags m_actionMask = ActionFlags.MaskAll;
		/// <summary>
		/// 
		/// </summary>
        protected RowHeightEnum m_rowHeight = RowHeightEnum.StretchToFit; 
		/// <summary>
		/// 
		/// </summary>
		protected PageBreaksEnum m_pageBreaks = PageBreaksEnum.FitIntoArea; 
		/// <summary>
		/// 
		/// </summary>
		protected FillEmptyEnum m_fillEmpty = FillEmptyEnum.ExtendAll; 
		/// <summary>
		/// 
		/// </summary>
		protected WrapTextEnum m_wrapText = WrapTextEnum.Wrap; 
		/// <summary>
		/// 
		/// </summary>
		public System.Windows.Forms.TextBox m_outputFileName;
		/// <summary>
		/// 
		/// </summary>
		public System.Windows.Forms.CheckBox m_chkUseGridColors;
		/// <summary>
		/// 
		/// </summary>
		public System.Windows.Forms.NumericUpDown m_numFooterHeight;
		/// <summary>
		/// 
		/// </summary>
		public System.Windows.Forms.NumericUpDown m_numHeaderHeight;
		/// <summary>
		/// 
		/// </summary>
		public System.Windows.Forms.TextBox m_txtFooter;
		/// <summary>
		/// 
		/// </summary>
		public System.Windows.Forms.TextBox m_txtHeader;
		/// <summary>
		/// 
		/// </summary>
		public System.Windows.Forms.NumericUpDown m_numMaxRowHeight;
		/// <summary>
		/// 
		/// </summary>
		public System.Windows.Forms.ComboBox m_cmbRowHeight;
		/// <summary>
		/// 
		/// </summary>
		public System.Windows.Forms.CheckBox m_chkHiddenRows;
		/// <summary>
		/// 
		/// </summary>
		public System.Windows.Forms.CheckBox m_chkHiddenCols;
		/// <summary>
		/// 
		/// </summary>
		public System.Windows.Forms.CheckBox m_chkRepeatRows;
		/// <summary>
		/// 
		/// </summary>
        public System.Windows.Forms.CheckBox m_chkRepeatCols;
		/// <summary>
		/// 
		/// </summary>
		public System.Windows.Forms.ComboBox m_cmbStretchToWidth;
		/// <summary>
		/// 
		/// </summary>
		public System.Windows.Forms.ComboBox m_cmbWrapText;
		/// <summary>
		/// 
		/// </summary>
		public System.Windows.Forms.ComboBox m_cmbHorzPageBreak;
		/// <summary>
		/// 
		/// </summary>
		public System.Windows.Forms.CheckBox m_chkShowProgress;
		/// <summary>
		/// 
		/// </summary>
		protected System.Windows.Forms.GroupBox m_grpPageHeaders;
		/// <summary>
		/// 
		/// </summary>
		protected System.Windows.Forms.Label m_lblFooterHeight;
		/// <summary>
		/// 
		/// </summary>
		protected System.Windows.Forms.Label m_lblHeaderHeight;
		/// <summary>
		/// 
		/// </summary>
		protected System.Windows.Forms.Label m_lblFooter;
		/// <summary>
		/// 
		/// </summary>
		protected System.Windows.Forms.Label m_lblHeader;
		/// <summary>
		/// 
		/// </summary>
		protected System.Windows.Forms.GroupBox m_grpRenderOptions;
		/// <summary>
		/// 
		/// </summary>
		protected System.Windows.Forms.Label m_lblRowHeight;
		/// <summary>
		/// 
		/// </summary>
		protected System.Windows.Forms.Label m_lblStretchToWidth;
		/// <summary>
		/// 
		/// </summary>
		protected System.Windows.Forms.Label m_lblWrapText;
		/// <summary>
		/// 
		/// </summary>
		protected System.Windows.Forms.Label m_lblHorzPageBreak;
		/// <summary>
		/// 
		/// </summary>
		protected System.Windows.Forms.Button m_btnOK;
		/// <summary>
		/// 
		/// </summary>
		protected System.Windows.Forms.Button m_btnCancel;
		/// <summary>
		/// 
		/// </summary>
		protected System.Windows.Forms.Label m_lblAction;
		/// <summary>
		/// 
		/// </summary>
		protected System.Windows.Forms.Button m_btnOutputFileName;
		/// <summary>
		/// 
		/// </summary>
		protected System.Windows.Forms.Button m_btnOptions;
		/// <summary>
		/// 
		/// </summary>
		protected System.Windows.Forms.ComboBox m_cmbAction;
		/// <summary>
		/// 
		/// </summary>
		protected System.Windows.Forms.Label m_lblOutputFileName;
		/// <summary>
		/// 
		/// </summary>
		protected System.Windows.Forms.Label m_lblMaxRowHeight;
        /// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		/// <summary>
		/// Creates a new instance of the PrintOptionsFrom class.
		/// </summary>
        public PrintOptionsForm()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();
		}

		/// <summary>
		/// Called when the class is being disposed.
		/// </summary>
		/// <param name="disposing">
		/// True to cleanup.
		/// </param>
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
            this.m_chkUseGridColors = new System.Windows.Forms.CheckBox();
            this.m_grpPageHeaders = new System.Windows.Forms.GroupBox();
            this.m_numFooterHeight = new System.Windows.Forms.NumericUpDown();
            this.m_lblFooterHeight = new System.Windows.Forms.Label();
            this.m_numHeaderHeight = new System.Windows.Forms.NumericUpDown();
            this.m_lblHeaderHeight = new System.Windows.Forms.Label();
            this.m_txtFooter = new System.Windows.Forms.TextBox();
            this.m_lblFooter = new System.Windows.Forms.Label();
            this.m_txtHeader = new System.Windows.Forms.TextBox();
            this.m_lblHeader = new System.Windows.Forms.Label();
            this.m_grpRenderOptions = new System.Windows.Forms.GroupBox();
            this.m_lblMaxRowHeight = new System.Windows.Forms.Label();
            this.m_cmbHorzPageBreak = new System.Windows.Forms.ComboBox();
            this.m_lblHorzPageBreak = new System.Windows.Forms.Label();
            this.m_cmbWrapText = new System.Windows.Forms.ComboBox();
            this.m_lblWrapText = new System.Windows.Forms.Label();
            this.m_cmbStretchToWidth = new System.Windows.Forms.ComboBox();
            this.m_lblStretchToWidth = new System.Windows.Forms.Label();
            this.m_chkHiddenRows = new System.Windows.Forms.CheckBox();
            this.m_chkHiddenCols = new System.Windows.Forms.CheckBox();
            this.m_chkRepeatRows = new System.Windows.Forms.CheckBox();
            this.m_chkRepeatCols = new System.Windows.Forms.CheckBox();
            this.m_numMaxRowHeight = new System.Windows.Forms.NumericUpDown();
            this.m_cmbRowHeight = new System.Windows.Forms.ComboBox();
            this.m_lblRowHeight = new System.Windows.Forms.Label();
            this.m_btnOK = new System.Windows.Forms.Button();
            this.m_btnCancel = new System.Windows.Forms.Button();
            this.m_cmbAction = new System.Windows.Forms.ComboBox();
            this.m_lblAction = new System.Windows.Forms.Label();
            this.m_outputFileName = new System.Windows.Forms.TextBox();
            this.m_lblOutputFileName = new System.Windows.Forms.Label();
            this.m_btnOutputFileName = new System.Windows.Forms.Button();
            this.m_btnOptions = new System.Windows.Forms.Button();
            this.m_chkShowProgress = new System.Windows.Forms.CheckBox();
            this.m_grpPageHeaders.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.m_numFooterHeight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.m_numHeaderHeight)).BeginInit();
            this.m_grpRenderOptions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.m_numMaxRowHeight)).BeginInit();
            this.SuspendLayout();
            // 
            // m_chkUseGridColors
            // 
            this.m_chkUseGridColors.Checked = true;
            this.m_chkUseGridColors.CheckState = System.Windows.Forms.CheckState.Checked;
            this.m_chkUseGridColors.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.m_chkUseGridColors.Location = new System.Drawing.Point(480, 136);
            this.m_chkUseGridColors.Name = "m_chkUseGridColors";
            this.m_chkUseGridColors.Size = new System.Drawing.Size(128, 24);
            this.m_chkUseGridColors.TabIndex = 32;
            this.m_chkUseGridColors.Text = "Use grid colors";
            // 
            // m_grpPageHeaders
            // 
            this.m_grpPageHeaders.Controls.Add(this.m_numFooterHeight);
            this.m_grpPageHeaders.Controls.Add(this.m_lblFooterHeight);
            this.m_grpPageHeaders.Controls.Add(this.m_numHeaderHeight);
            this.m_grpPageHeaders.Controls.Add(this.m_lblHeaderHeight);
            this.m_grpPageHeaders.Controls.Add(this.m_txtFooter);
            this.m_grpPageHeaders.Controls.Add(this.m_lblFooter);
            this.m_grpPageHeaders.Controls.Add(this.m_txtHeader);
            this.m_grpPageHeaders.Controls.Add(this.m_lblHeader);
            this.m_grpPageHeaders.Location = new System.Drawing.Point(8, 40);
            this.m_grpPageHeaders.Name = "m_grpPageHeaders";
            this.m_grpPageHeaders.Size = new System.Drawing.Size(456, 80);
            this.m_grpPageHeaders.TabIndex = 6;
            this.m_grpPageHeaders.TabStop = false;
            this.m_grpPageHeaders.Text = " Page Headers and Footers";
            // 
            // m_numFooterHeight
            // 
            this.m_numFooterHeight.Location = new System.Drawing.Point(400, 51);
            this.m_numFooterHeight.Name = "m_numFooterHeight";
            this.m_numFooterHeight.Size = new System.Drawing.Size(45, 20);
            this.m_numFooterHeight.TabIndex = 14;
            // 
            // m_lblFooterHeight
            // 
            this.m_lblFooterHeight.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.m_lblFooterHeight.Location = new System.Drawing.Point(280, 51);
            this.m_lblFooterHeight.Name = "m_lblFooterHeight";
            this.m_lblFooterHeight.Size = new System.Drawing.Size(118, 16);
            this.m_lblFooterHeight.TabIndex = 13;
            this.m_lblFooterHeight.Text = "Height (in 1/100 inch)";
            this.m_lblFooterHeight.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // m_numHeaderHeight
            // 
            this.m_numHeaderHeight.Location = new System.Drawing.Point(400, 27);
            this.m_numHeaderHeight.Name = "m_numHeaderHeight";
            this.m_numHeaderHeight.Size = new System.Drawing.Size(45, 20);
            this.m_numHeaderHeight.TabIndex = 10;
            this.m_numHeaderHeight.Value = new decimal(new int[] {
            30,
            0,
            0,
            0});
            // 
            // m_lblHeaderHeight
            // 
            this.m_lblHeaderHeight.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.m_lblHeaderHeight.Location = new System.Drawing.Point(280, 27);
            this.m_lblHeaderHeight.Name = "m_lblHeaderHeight";
            this.m_lblHeaderHeight.Size = new System.Drawing.Size(118, 16);
            this.m_lblHeaderHeight.TabIndex = 9;
            this.m_lblHeaderHeight.Text = "Height (in 1/100 inch)";
            this.m_lblHeaderHeight.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // m_txtFooter
            // 
            this.m_txtFooter.Location = new System.Drawing.Point(88, 51);
            this.m_txtFooter.Name = "m_txtFooter";
            this.m_txtFooter.Size = new System.Drawing.Size(176, 20);
            this.m_txtFooter.TabIndex = 12;
            // 
            // m_lblFooter
            // 
            this.m_lblFooter.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.m_lblFooter.Location = new System.Drawing.Point(11, 51);
            this.m_lblFooter.Name = "m_lblFooter";
            this.m_lblFooter.Size = new System.Drawing.Size(77, 16);
            this.m_lblFooter.TabIndex = 11;
            this.m_lblFooter.Text = "Footer text";
            this.m_lblFooter.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // m_txtHeader
            // 
            this.m_txtHeader.Location = new System.Drawing.Point(88, 24);
            this.m_txtHeader.Name = "m_txtHeader";
            this.m_txtHeader.Size = new System.Drawing.Size(176, 20);
            this.m_txtHeader.TabIndex = 8;
            this.m_txtHeader.Text = "Page \\p of \\P";
            // 
            // m_lblHeader
            // 
            this.m_lblHeader.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.m_lblHeader.Location = new System.Drawing.Point(11, 27);
            this.m_lblHeader.Name = "m_lblHeader";
            this.m_lblHeader.Size = new System.Drawing.Size(77, 16);
            this.m_lblHeader.TabIndex = 7;
            this.m_lblHeader.Text = "Header text";
            this.m_lblHeader.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // m_grpRenderOptions
            // 
            this.m_grpRenderOptions.Controls.Add(this.m_lblMaxRowHeight);
            this.m_grpRenderOptions.Controls.Add(this.m_cmbHorzPageBreak);
            this.m_grpRenderOptions.Controls.Add(this.m_lblHorzPageBreak);
            this.m_grpRenderOptions.Controls.Add(this.m_cmbWrapText);
            this.m_grpRenderOptions.Controls.Add(this.m_lblWrapText);
            this.m_grpRenderOptions.Controls.Add(this.m_cmbStretchToWidth);
            this.m_grpRenderOptions.Controls.Add(this.m_lblStretchToWidth);
            this.m_grpRenderOptions.Controls.Add(this.m_chkHiddenRows);
            this.m_grpRenderOptions.Controls.Add(this.m_chkHiddenCols);
            this.m_grpRenderOptions.Controls.Add(this.m_chkRepeatRows);
            this.m_grpRenderOptions.Controls.Add(this.m_chkRepeatCols);
            this.m_grpRenderOptions.Controls.Add(this.m_numMaxRowHeight);
            this.m_grpRenderOptions.Controls.Add(this.m_cmbRowHeight);
            this.m_grpRenderOptions.Controls.Add(this.m_lblRowHeight);
            this.m_grpRenderOptions.Location = new System.Drawing.Point(8, 128);
            this.m_grpRenderOptions.Name = "m_grpRenderOptions";
            this.m_grpRenderOptions.Size = new System.Drawing.Size(456, 155);
            this.m_grpRenderOptions.TabIndex = 15;
            this.m_grpRenderOptions.TabStop = false;
            this.m_grpRenderOptions.Text = "C1FlexGrid printing options";
            // 
            // m_lblMaxRowHeight
            // 
            this.m_lblMaxRowHeight.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.m_lblMaxRowHeight.Location = new System.Drawing.Point(352, 27);
            this.m_lblMaxRowHeight.Name = "m_lblMaxRowHeight";
            this.m_lblMaxRowHeight.Size = new System.Drawing.Size(81, 16);
            this.m_lblMaxRowHeight.TabIndex = 19;
            this.m_lblMaxRowHeight.Text = "(in 1/100 inch)";
            this.m_lblMaxRowHeight.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // m_cmbHorzPageBreak
            // 
            this.m_cmbHorzPageBreak.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.m_cmbHorzPageBreak.ItemHeight = 13;
            this.m_cmbHorzPageBreak.Items.AddRange(new object[] {
            "Fit columns on one page",
            "Clip columns in page",
            "Break on splits",
            "Break on any column"});
            this.m_cmbHorzPageBreak.Location = new System.Drawing.Point(128, 88);
            this.m_cmbHorzPageBreak.Name = "m_cmbHorzPageBreak";
            this.m_cmbHorzPageBreak.Size = new System.Drawing.Size(160, 21);
            this.m_cmbHorzPageBreak.TabIndex = 23;
            this.m_cmbHorzPageBreak.SelectedIndexChanged += new System.EventHandler(this.m_cmbHorzPageBreak_SelectedIndexChanged);
            // 
            // m_lblHorzPageBreak
            // 
            this.m_lblHorzPageBreak.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.m_lblHorzPageBreak.Location = new System.Drawing.Point(8, 88);
            this.m_lblHorzPageBreak.Name = "m_lblHorzPageBreak";
            this.m_lblHorzPageBreak.Size = new System.Drawing.Size(120, 23);
            this.m_lblHorzPageBreak.TabIndex = 22;
            this.m_lblHorzPageBreak.Text = "Horizontal page break";
            this.m_lblHorzPageBreak.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // m_cmbWrapText
            // 
            this.m_cmbWrapText.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.m_cmbWrapText.ItemHeight = 13;
            this.m_cmbWrapText.Items.AddRange(new object[] {
            "Wrap all text",
            "Do not wrap text",
            "Wrap like columns"});
            this.m_cmbWrapText.Location = new System.Drawing.Point(128, 56);
            this.m_cmbWrapText.Name = "m_cmbWrapText";
            this.m_cmbWrapText.Size = new System.Drawing.Size(160, 21);
            this.m_cmbWrapText.TabIndex = 21;
            this.m_cmbWrapText.SelectedIndexChanged += new System.EventHandler(this.m_cmbWrapText_SelectedIndexChanged);
            // 
            // m_lblWrapText
            // 
            this.m_lblWrapText.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.m_lblWrapText.Location = new System.Drawing.Point(8, 56);
            this.m_lblWrapText.Name = "m_lblWrapText";
            this.m_lblWrapText.Size = new System.Drawing.Size(120, 23);
            this.m_lblWrapText.TabIndex = 20;
            this.m_lblWrapText.Text = "Wrap text in columns";
            this.m_lblWrapText.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // m_cmbStretchToWidth
            // 
            this.m_cmbStretchToWidth.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.m_cmbStretchToWidth.ItemHeight = 13;
            this.m_cmbStretchToWidth.Items.AddRange(new object[] {
            "Stretch all columns",
            "Do not stretch",
            "Stretch last column"});
            this.m_cmbStretchToWidth.Location = new System.Drawing.Point(128, 120);
            this.m_cmbStretchToWidth.Name = "m_cmbStretchToWidth";
            this.m_cmbStretchToWidth.Size = new System.Drawing.Size(160, 21);
            this.m_cmbStretchToWidth.TabIndex = 25;
            this.m_cmbStretchToWidth.SelectedIndexChanged += new System.EventHandler(this.m_cmbStretchToWidth_SelectedIndexChanged);
            // 
            // m_lblStretchToWidth
            // 
            this.m_lblStretchToWidth.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.m_lblStretchToWidth.Location = new System.Drawing.Point(8, 120);
            this.m_lblStretchToWidth.Name = "m_lblStretchToWidth";
            this.m_lblStretchToWidth.Size = new System.Drawing.Size(120, 23);
            this.m_lblStretchToWidth.TabIndex = 24;
            this.m_lblStretchToWidth.Text = "Stretch to page width";
            this.m_lblStretchToWidth.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // m_chkHiddenRows
            // 
            this.m_chkHiddenRows.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.m_chkHiddenRows.Location = new System.Drawing.Point(304, 128);
            this.m_chkHiddenRows.Name = "m_chkHiddenRows";
            this.m_chkHiddenRows.Size = new System.Drawing.Size(144, 17);
            this.m_chkHiddenRows.TabIndex = 29;
            this.m_chkHiddenRows.Text = "Show hidden rows";
            // 
            // m_chkHiddenCols
            // 
            this.m_chkHiddenCols.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.m_chkHiddenCols.Location = new System.Drawing.Point(304, 104);
            this.m_chkHiddenCols.Name = "m_chkHiddenCols";
            this.m_chkHiddenCols.Size = new System.Drawing.Size(144, 17);
            this.m_chkHiddenCols.TabIndex = 28;
            this.m_chkHiddenCols.Text = "Show hidden columns";
            // 
            // m_chkRepeatRows
            // 
            this.m_chkRepeatRows.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.m_chkRepeatRows.Location = new System.Drawing.Point(304, 80);
            this.m_chkRepeatRows.Name = "m_chkRepeatRows";
            this.m_chkRepeatRows.Size = new System.Drawing.Size(144, 17);
            this.m_chkRepeatRows.TabIndex = 27;
            this.m_chkRepeatRows.Text = "Repeat fixed rows";
            // 
            // m_chkRepeatCols
            // 
            this.m_chkRepeatCols.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.m_chkRepeatCols.Location = new System.Drawing.Point(304, 56);
            this.m_chkRepeatCols.Name = "m_chkRepeatCols";
            this.m_chkRepeatCols.Size = new System.Drawing.Size(144, 17);
            this.m_chkRepeatCols.TabIndex = 26;
            this.m_chkRepeatCols.Text = "Repeat fixed columns";
            // 
            // m_numMaxRowHeight
            // 
            this.m_numMaxRowHeight.Location = new System.Drawing.Point(304, 24);
            this.m_numMaxRowHeight.Name = "m_numMaxRowHeight";
            this.m_numMaxRowHeight.Size = new System.Drawing.Size(42, 20);
            this.m_numMaxRowHeight.TabIndex = 18;
            this.m_numMaxRowHeight.Value = new decimal(new int[] {
            15,
            0,
            0,
            0});
            // 
            // m_cmbRowHeight
            // 
            this.m_cmbRowHeight.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.m_cmbRowHeight.ItemHeight = 13;
            this.m_cmbRowHeight.Items.AddRange(new object[] {
            "Stretch to fit",
            "Fixed",
            "Stretch up to value"});
            this.m_cmbRowHeight.Location = new System.Drawing.Point(128, 24);
            this.m_cmbRowHeight.Name = "m_cmbRowHeight";
            this.m_cmbRowHeight.Size = new System.Drawing.Size(160, 21);
            this.m_cmbRowHeight.TabIndex = 17;
            this.m_cmbRowHeight.SelectedIndexChanged += new System.EventHandler(this.m_cmbRowHeight_SelectedIndexChanged);
            // 
            // m_lblRowHeight
            // 
            this.m_lblRowHeight.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.m_lblRowHeight.Location = new System.Drawing.Point(8, 24);
            this.m_lblRowHeight.Name = "m_lblRowHeight";
            this.m_lblRowHeight.Size = new System.Drawing.Size(120, 23);
            this.m_lblRowHeight.TabIndex = 16;
            this.m_lblRowHeight.Text = "Row height";
            this.m_lblRowHeight.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // m_btnOK
            // 
            this.m_btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.m_btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.m_btnOK.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.m_btnOK.Location = new System.Drawing.Point(526, 226);
            this.m_btnOK.Name = "m_btnOK";
            this.m_btnOK.Size = new System.Drawing.Size(80, 23);
            this.m_btnOK.TabIndex = 34;
            this.m_btnOK.Text = "OK";
            // 
            // m_btnCancel
            // 
            this.m_btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.m_btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.m_btnCancel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.m_btnCancel.Location = new System.Drawing.Point(526, 257);
            this.m_btnCancel.Name = "m_btnCancel";
            this.m_btnCancel.Size = new System.Drawing.Size(80, 23);
            this.m_btnCancel.TabIndex = 35;
            this.m_btnCancel.Text = "Cancel";
            // 
            // m_cmbAction
            // 
            this.m_cmbAction.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.m_cmbAction.ItemHeight = 13;
            this.m_cmbAction.Items.AddRange(new object[] {
            "Print Preview",
            "Export to HTML",
            "Export to PDF"});
            this.m_cmbAction.Location = new System.Drawing.Point(48, 7);
            this.m_cmbAction.Name = "m_cmbAction";
            this.m_cmbAction.Size = new System.Drawing.Size(184, 21);
            this.m_cmbAction.TabIndex = 2;
            this.m_cmbAction.SelectedIndexChanged += new System.EventHandler(this.m_cmbAction_SelectedIndexChanged);
            // 
            // m_lblAction
            // 
            this.m_lblAction.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.m_lblAction.Location = new System.Drawing.Point(8, 5);
            this.m_lblAction.Name = "m_lblAction";
            this.m_lblAction.Size = new System.Drawing.Size(40, 23);
            this.m_lblAction.TabIndex = 1;
            this.m_lblAction.Text = "Action:";
            this.m_lblAction.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // m_outputFileName
            // 
            this.m_outputFileName.Location = new System.Drawing.Point(304, 8);
            this.m_outputFileName.Name = "m_outputFileName";
            this.m_outputFileName.Size = new System.Drawing.Size(272, 20);
            this.m_outputFileName.TabIndex = 4;
            // 
            // m_lblOutputFileName
            // 
            this.m_lblOutputFileName.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.m_lblOutputFileName.Location = new System.Drawing.Point(240, 5);
            this.m_lblOutputFileName.Name = "m_lblOutputFileName";
            this.m_lblOutputFileName.Size = new System.Drawing.Size(64, 23);
            this.m_lblOutputFileName.TabIndex = 3;
            this.m_lblOutputFileName.Text = "File name:";
            this.m_lblOutputFileName.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // m_btnOutputFileName
            // 
            this.m_btnOutputFileName.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.m_btnOutputFileName.Location = new System.Drawing.Point(584, 8);
            this.m_btnOutputFileName.Name = "m_btnOutputFileName";
            this.m_btnOutputFileName.Size = new System.Drawing.Size(22, 20);
            this.m_btnOutputFileName.TabIndex = 5;
            this.m_btnOutputFileName.Text = "...";
            this.m_btnOutputFileName.Click += new System.EventHandler(this.m_btnOutputFileName_Click);
            // 
            // m_btnOptions
            // 
            this.m_btnOptions.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.m_btnOptions.Location = new System.Drawing.Point(528, 48);
            this.m_btnOptions.Name = "m_btnOptions";
            this.m_btnOptions.Size = new System.Drawing.Size(80, 23);
            this.m_btnOptions.TabIndex = 31;
            this.m_btnOptions.Text = "Options...";
            this.m_btnOptions.Click += new System.EventHandler(this.m_btnOptions_Click);
            // 
            // m_chkShowProgress
            // 
            this.m_chkShowProgress.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.m_chkShowProgress.Location = new System.Drawing.Point(480, 160);
            this.m_chkShowProgress.Name = "m_chkShowProgress";
            this.m_chkShowProgress.Size = new System.Drawing.Size(128, 24);
            this.m_chkShowProgress.TabIndex = 33;
            this.m_chkShowProgress.Text = "Show progress";
            // 
            // PrintOptionsForm
            // 
            this.AcceptButton = this.m_btnOK;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.CancelButton = this.m_btnCancel;
            this.ClientSize = new System.Drawing.Size(616, 295);
            this.Controls.Add(this.m_chkShowProgress);
            this.Controls.Add(this.m_btnOptions);
            this.Controls.Add(this.m_btnOutputFileName);
            this.Controls.Add(this.m_lblOutputFileName);
            this.Controls.Add(this.m_outputFileName);
            this.Controls.Add(this.m_lblAction);
            this.Controls.Add(this.m_cmbAction);
            this.Controls.Add(this.m_btnCancel);
            this.Controls.Add(this.m_btnOK);
            this.Controls.Add(this.m_grpRenderOptions);
            this.Controls.Add(this.m_grpPageHeaders);
            this.Controls.Add(this.m_chkUseGridColors);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PrintOptionsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FlexGrid Print/Export Options";
            this.Closing += new System.ComponentModel.CancelEventHandler(this.PrintOptionsForm_Closing);
            this.Load += new System.EventHandler(this.PrintOptionsForm_Load);
            this.m_grpPageHeaders.ResumeLayout(false);
            this.m_grpPageHeaders.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.m_numFooterHeight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.m_numHeaderHeight)).EndInit();
            this.m_grpRenderOptions.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.m_numMaxRowHeight)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

        #region non-control public properties
		/// <summary>
		/// Gets or sets the exporter.
		/// </summary>
        public C1.C1Preview.Export.Exporter Exporter
        {
            get { return m_exporter; }
            set
            {
                m_exporter = value;
                this.OnExporterChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Gets or sets the output file name.
        /// </summary>
        public string OutputFileName
        {
            get {return m_outputFileName.Text; }
            set {m_outputFileName.Text = value;}
        }

        internal C1.C1Preview.Export.ExportProvider ExpProvider
        {
            get
            {
                return Exporter.ExportProvider;
            }
        }

        /// <summary>
		/// 
		/// </summary>
		public ActionFlags Action
        {
            get { return m_action; }
            set
            {
                this.OnActionChanged(EventArgs.Empty);
                m_action = value;
            }
        }

		/// <summary>
		/// 
		/// </summary>
		public ActionFlags ActionMask
        {
            get { return m_actionMask; }
            set { m_actionMask = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public RowHeightEnum RowHeight
        {
            get { return m_rowHeight; }
            set { m_rowHeight = value; }
        }
		/// <summary>
		/// 
		/// </summary>
		public PageBreaksEnum PageBreaks
        {
            get { return m_pageBreaks; }
            set { m_pageBreaks = value; }
        }
		/// <summary>
		/// 
		/// </summary>
		public FillEmptyEnum FillEmpty
        {
            get { return m_fillEmpty; }
            set { m_fillEmpty = value; }
        }
		/// <summary>
		/// 
		/// </summary>
		public WrapTextEnum WrapText
        {
            get { return m_wrapText; }
            set { m_wrapText = value; }
        }
        #endregion

		/// <summary>
		/// 
		/// </summary>
		protected virtual void m_btnOutputFileName_Click(object sender, System.EventArgs e)
        {
            C1.C1Preview.Export.ExportProvider ep;
            if (Exporter != null)
                ep = ExpProvider;
            else
                ep = null;
            string name = this.m_outputFileName.Text;
            bool ok = C1FlexGridPrintable.SelectExportFile(this, _providers, ref name, ref ep);
            if (!ok || ep == null)
                return;

            this.m_outputFileName.Text = name;
            if (Exporter == null || ep != ExpProvider)
            {
                // export format was changed in the save as dialog
                Exporter = C1FlexGridPrintable.CreateExporter(ep);
                foreach (ActionItem ai in m_cmbAction.Items)
                    if (ai.Provider == ep)
                    {
                        m_cmbAction.SelectedItem = ai;
                        break;
                    }
            }
        }

		/// <summary>
		/// 
		/// </summary>
		protected virtual void m_btnOptions_Click(object sender, System.EventArgs e)
        {
            if (Exporter != null)
                // note: it is responsibility of the exporter to save options if the user
                // pressed OK and drop them if she pressed Cancel.
                Exporter.ShowOptionsDialog();
        }

		/// <summary>
		/// 
		/// </summary>
		protected virtual void m_cmbAction_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            ActionItem ai = m_cmbAction.SelectedItem as ActionItem;
            if (ai == null)
                ai = ActionItem.Empty;
            Action = ai.Action;

            if (ai.Provider != null)
            {
                if (Exporter == null || ExpProvider != ai.Provider)
                    Exporter = C1FlexGridPrintable.CreateExporter(ai.Provider);
            }
            else
            {
                Exporter = null;
            }
            UpdateStatus();
        }

		/// <summary>
		/// 
		/// </summary>
		protected virtual void UpdateStatus()
        {
            this.m_outputFileName.Enabled = Exporter != null;
            this.m_lblOutputFileName.Enabled = this.m_outputFileName.Enabled;
            this.m_btnOutputFileName.Enabled = this.m_outputFileName.Enabled;
            this.m_btnOptions.Enabled = (Exporter != null && Exporter.ExportProvider.CanShowOptions);
            RowHeightItem rh = this.m_cmbRowHeight.SelectedItem as RowHeightItem;
			bool tomax = (rh != null) && (rh.RowHeight == RowHeightEnum.StretchToMax);
			this.m_numMaxRowHeight.Enabled = tomax;
			this.m_lblMaxRowHeight.Visible = tomax;
        }

		/// <summary>
		/// 
		/// </summary>
		protected virtual void PrintOptionsForm_Load(object sender, System.EventArgs e)
        {
            // build _providers
            if (Exporter == null)
            {
                if ((ActionMask & ActionFlags.Export) != 0)
                    _providers = C1FlexGridPrintable.GetExportProviders(null);
                else
                    _providers = new C1.C1Preview.Export.ExportProvider[0];
            }
            else
            {
                _providers = new C1.C1Preview.Export.ExportProvider[] { Exporter.ExportProvider };
            }

            this.m_cmbAction.Items.Clear();
            // for preview
            if ((ActionFlags.Preview & ActionMask) != 0)
                this.m_cmbAction.Items.Add(
                    new ActionItem(ActionFlags.Preview, null));
            // for print
            if ((ActionFlags.Print & ActionMask) != 0)
                this.m_cmbAction.Items.Add(
                    new ActionItem(ActionFlags.Print, null));
            // for all exporters
            foreach (C1.C1Preview.Export.ExportProvider provider in _providers)
                this.m_cmbAction.Items.Add(
                    new ActionItem(ActionFlags.Export, provider));
            // selected item
            this.m_cmbAction.SelectedIndex = 0;

			this.m_cmbRowHeight.Items.Clear();
			foreach (RowHeightItem rh in RowHeightItem.RowHeightItems)
			{
				this.m_cmbRowHeight.Items.Add(rh);
				if (rh.RowHeight == this.RowHeight)
					this.m_cmbRowHeight.SelectedItem = rh;
			}
			if (this.m_cmbRowHeight.SelectedItem == null)
				this.m_cmbRowHeight.SelectedIndex = 0;
			this.m_cmbHorzPageBreak.Items.Clear();
			foreach (PageBreakItem pb in PageBreakItem.PageBreakItems)
			{
				this.m_cmbHorzPageBreak.Items.Add(pb);
				if (pb.PageBreaks == this.PageBreaks)
					this.m_cmbHorzPageBreak.SelectedItem = pb;
			}
			if (this.m_cmbHorzPageBreak.SelectedItem == null)
				this.m_cmbHorzPageBreak.SelectedIndex = 0;
			this.m_cmbStretchToWidth.Items.Clear();
			foreach (FillEmptyItem fe in FillEmptyItem.FillEmptyItems)
			{
				this.m_cmbStretchToWidth.Items.Add(fe);
				if (fe.FillEmpty == this.FillEmpty)
					this.m_cmbStretchToWidth.SelectedItem = fe;
			}
			if (this.m_cmbStretchToWidth.SelectedItem == null)
				this.m_cmbStretchToWidth.SelectedIndex = 0;
			this.m_cmbWrapText.Items.Clear();
			foreach (WrapTextItem wt in WrapTextItem.WrapTextItems)
			{
				this.m_cmbWrapText.Items.Add(wt);
				if (wt.WrapText == this.WrapText)
					this.m_cmbWrapText.SelectedItem = wt;
			}
			if (this.m_cmbWrapText.SelectedItem == null)
				this.m_cmbWrapText.SelectedIndex = 0;
			//if (Exporter != null)
            //    this.m_outputFileName.Text = Exporter.OutputFileName;
            UpdateStatus();
        }

		/// <summary>
		/// 
		/// </summary>
		protected virtual void PrintOptionsForm_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (this.DialogResult != DialogResult.OK)
                return;
            if (!m_outputFileName.Enabled)
                return;
            if (m_outputFileName.Text.Length == 0)
            {
                MessageBox.Show(this, "Please provide a file name", "Export",
                    MessageBoxButtons.OK, MessageBoxIcon.Hand);
                m_outputFileName.Focus();
                e.Cancel = true;
            }
        }

		/// <summary>
		/// 
		/// </summary>
		protected virtual void m_cmbRowHeight_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			RowHeightItem rh = m_cmbRowHeight.SelectedItem as RowHeightItem;
			if (rh != null) RowHeight = rh.RowHeight;
			UpdateStatus();
		}

		/// <summary>
		/// 
		/// </summary>
		protected virtual void m_cmbHorzPageBreak_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			PageBreakItem pb = m_cmbHorzPageBreak.SelectedItem as PageBreakItem;
			if (pb != null) PageBreaks = pb.PageBreaks;
		}

		/// <summary>
		/// 
		/// </summary>
		protected virtual void m_cmbStretchToWidth_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			FillEmptyItem fe = m_cmbStretchToWidth.SelectedItem as FillEmptyItem;
			if (fe != null) FillEmpty = fe.FillEmpty;
		}

		/// <summary>
		/// 
		/// </summary>
		protected virtual void m_cmbWrapText_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			WrapTextItem wt = m_cmbWrapText.SelectedItem as WrapTextItem;
			if (wt != null) WrapText = wt.WrapText;
		}
	}
}
