using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Diagnostics;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Printing;
using System.IO;

using C1.Win.C1FlexGrid;
using C1.Win.C1FlexGrid.Util;
using C1.C1Preview;

namespace Payroll.CFC
{

    /// <summary>
    /// Defines a C1FlexGrid that can be printed or inserted in a C1PrintDocument.
    /// Can be used either by itself, or to print another C1FlexGrid object.
    /// </summary>
    public class C1FlexGridPrintable : C1FlexGrid
    {
        #region Constants

        private const string c_tag_t = "\\t";
        private const string c_tag_p = "\\p";
        private const string c_tag_P = "\\P";
        private const string c_tag_s = "\\s";
        private const string c_tag_S = "\\S";
        private const string c_tag_g = "\\g";
        private const string c_tag_G = "\\G";

        #endregion

        #region Private instance variables

        private Size _imgSize;
        private C1PrintDocument _doc = null;
        private RenderTable _workTable = null;
        private double _zoom = 1.0;
        private double _perc = 0;
        private int _curRow = 0;
        private bool _terminated = false;
        private PrintInfo _printInfo = new PrintInfo();
        private int _pageHeaderHeight = 30;
        private int _pageFooterHeight = 30;
        private int _rowh = 0;
        private int _maxHeight = 0;
        private int _maxWidth = 0;
        private int _maxWidthPix = 0;
        private int _dpiScreen = 0;  //dots per inch for screen resolution
        private int _dpiPrinter = 0;  //dots per inch for printer resolution
        private Graphics _measureGraphics = null; // graphics for measuring string size
        private C1PrintProgress _progressDlg = null;
        private PreviewWrapper _previewWrapper = null;
        private StringFormat _sf = null;
        private Graphics _graphics;
        private Bitmap _img = new Bitmap(1, 1);

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new instance of C1FlexGridPrintable class.
        /// </summary>
        public C1FlexGridPrintable()
        {
        }

        /// <summary>
        /// Creates a new instance of C1FlexGridPrintable class to print/preview/export
        /// another C1FlexGrid object.
        /// </summary>
        /// <param name="flex">The object to print/preview/export.</param>
        public C1FlexGridPrintable(C1FlexGrid flex)
        {
            Debug.Assert(flex != null);
            this.DataSource = flex;
            this.AllowMerging = flex.AllowMerging;
            this.BackColor = flex.BackColor;
            this.BackgroundImage = flex.BackgroundImage;
            this.ForeColor = flex.ForeColor;
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets the current row which is displayed in the progress dialog.
        /// </summary>
        [Browsable(false)]
        public int CurRow
        {
            get { return _curRow; }
            private set
            {
                _curRow = value;
                if (_progressDlg != null)
                {
                    _progressDlg.TxtPrinting = string.Format("Processing row {0} of {1}", CurRow, this.Rows.Count);
                    Application.DoEvents();
                    if (_progressDlg.CancelClicked)
                    {
                        _terminated = true;
                        OnEndPrinting();
                    }
                }
                OnAfterRowPrinted();
            }
        }

        /// <summary>
        /// Gets the PrintInfo object.
        /// </summary>
        public PrintInfo PrintInfo
        {
            get { return _printInfo; }
            set { _printInfo = value; }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Gets the <see cref="C1.C1Preview.C1PrintDocument"/> object representing the grid.
        /// </summary>
        /// <remarks>
        /// <para>This property builds and generates the document representing the grid
        /// each time it is accessed, so each time a new document is created and returned.
        /// If you need to access the properties of the created document or call its methods,
        /// you must call this property once, cache the returned value, and work with it.</para>
        /// <para>Note that the current values of the C1FlexGrid.PrintInfo object
        /// are respected during the document generation. E.g. the generation progress
        /// will be shown by default (set grid.PrintInfo.ShowProgress to false to
        /// avoid that).
        /// </para>
        /// </remarks>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public C1.C1Preview.C1PrintDocument PrintDocument
        {
            get
            {
                if (base.DesignMode)
                    return null;
                LoadPreviewWrapper();
                // bool oldshowprogress = _printInfo.ShowProgressForm;
                // _printInfo.ShowProgressForm = false;
                C1.C1Preview.C1PrintDocument doc = RenderSourceTree();
                // _printInfo.ShowProgressForm = oldshowprogress;
                DisposeProgressDlg();
                return doc;
            }
        }

        /// <summary>
        /// Shows the print preview.
        /// </summary>
        public void PrintPreview(string title, Icon icon)
        {
            if (!HaveSomethingToPrint)
                return;
            C1.C1Preview.Export.Exporter exporter = null;
            ActionFlags action = ActionFlags.Preview;
            string fn = string.Empty;
            if (_printInfo.ShowOptionsDialog)
                action = DoShowOptionsDialog(ref exporter, ActionFlags.Preview, ref fn);

            if (action == ActionFlags.Preview)
            {
                LoadPreviewWrapper();
                C1.C1Preview.C1PrintDocument doc = RenderSourceTree();
                _previewWrapper.Preview(doc, title, icon);
                DisposeProgressDlg();
            }
        }

        /// <summary>
        /// Prints the grid using the default printer settings.
        /// </summary>
        public void Print()
        {
            Print((PrinterSettings)null);
        }

        /// <summary>
        /// Prints the grid using the specified printer settings.
        /// </summary>
        /// <param name="printerSettings">The printer settings to use.</param>
        public void Print(PrinterSettings printerSettings)
        {
            if (!HaveSomethingToPrint)
                return;
            ActionFlags action = ActionFlags.Print;
            C1.C1Preview.Export.Exporter exporter = null;
            string fn = string.Empty;
            if (_printInfo.ShowOptionsDialog)
                action = DoShowOptionsDialog(ref exporter, ActionFlags.Print, ref fn);
            if (action != ActionFlags.Print)
                return;
            LoadPreviewWrapper();
            C1.C1Preview.C1PrintDocument doc = RenderSourceTree();
            _previewWrapper.Print(doc);
            DisposeProgressDlg();
        }

        /// <summary>
        /// Outputs the grid as a C1PrintDocument into the specified stream.
        /// </summary>
        /// <param name="stream">The output stream.</param>
        public void Print(System.IO.Stream stream)
        {
            if (HaveSomethingToPrint)
            {
                LoadPreviewWrapper();
                bool oldshowprogress = _printInfo.ShowProgressForm;
                _printInfo.ShowProgressForm = false;
                C1.C1Preview.C1PrintDocument doc = RenderSourceTree();
                doc.Save(stream);
                // stream.Position = 0;
                _printInfo.ShowProgressForm = oldshowprogress;
            }
        }

        /// <summary>
        /// Outputs the grid as a C1PrintDocument into a memory stream.
        /// </summary>
        /// <returns>The memory stream.</returns>
        public System.IO.Stream PrintToStream()
        {
            System.IO.Stream mystream = new System.IO.MemoryStream();
            Print(mystream);
            return mystream;
        }

        /// <summary>
        /// Shows the export dialog and exports the grid to the file specified by the user.
        /// </summary>
        public void ExportTo()
        {
            C1.C1Preview.Export.Exporter exporter = null;
            string outputfn = string.Empty;
            ActionFlags action = DoShowOptionsDialog(ref exporter, ActionFlags.Export, ref outputfn);
            if (action == ActionFlags.None || exporter == null)
                return;

            _export(exporter, outputfn);
        }

        /// <summary>
        /// Exports the grid to the specified file. The format is determined by the file extension.
        /// </summary>
        /// <param name="outputFileName">The output file name.</param>
        public void ExportTo(string outputFileName)
        {
            string fileExt = System.IO.Path.GetExtension(outputFileName);
            if (fileExt.Length == 0 || fileExt == null)
                return;
            fileExt = fileExt.ToLower();
            // nuke the '.' as provider.DefaultExtension doesn't contain one
            if (fileExt[0] == '.')
                fileExt = fileExt.Remove(0, 1);
            // if the passed in extension is .html, make it .htm
            if (fileExt == "html")
                fileExt = "htm";

            _exportTo(fileExt, outputFileName);
        }

        /// <summary>
        /// Exports the grid to the specified file as HTML.
        /// </summary>
        /// <param name="outputFileName">The output file name.</param>
        public void ExportToHtml(string outputFileName)
        {
            _exportTo("htm", outputFileName);
        }

        /// <summary>
        /// Exports the grid to the specified file as PDF.
        /// </summary>
        /// <param name="outputFileName">The output file name.</param>
        public void ExportToPdf(string outputFileName)
        {
            _exportTo("pdf", outputFileName);
        }

        /// <summary>
        /// Exports the grid to the specified file as RTF.
        /// </summary>
        /// <param name="outputFileName">The output file name.</param>
        public void ExportToRtf(string outputFileName)
        {
            _exportTo("rtf", outputFileName);
        }

        /// <summary>
        /// Exports the grid to the specified file as Excel.
        /// </summary>
        /// <param name="outputFileName">The output file name.</param>
        public void ExportToExcel(string outputFileName)
        {
            _exportTo("xls", outputFileName);
        }

        /// <summary>
        /// Exports the grid to the specified file as Excel.
        /// </summary>
        /// <param name="outputFileName">The output file name.</param>
        /// <param name="onePagePerSheet">Determines whether each page is exported as a single work sheet.</param>
        public void ExportToExcel(string outputFileName, bool onePagePerSheet)
        {
            _exportTo("xls", outputFileName, new ExportParameters(onePagePerSheet));
        }

        /// <summary>
        /// Implements IC1Printable interface so the grid
        /// can be inserted in a C1PrintDocument via a
        /// RenderC1Printable object.
        /// </summary>
        /// <returns></returns>
        public Stream C1PrintableGetTree()
        {
            return PrintToStream();
        }

        #endregion

        #region Internal methods

        /// <summary>
        /// Calls dialog for defining export firmat and file name.
        /// </summary>
        /// <param name="parent">Parent form owned this dialog form.</param>
        /// <param name="availableProviders">List of available export providers.</param>
        /// <param name="name">File name to export to.</param>
        /// <param name="ep">Export format.</param>
        /// <returns></returns>
        static internal bool SelectExportFile(Form parent, C1.C1Preview.Export.ExportProvider[] availableProviders,
            ref string name, ref C1.C1Preview.Export.ExportProvider ep)
        {
            // build a filter
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.FileName = name;
            saveFileDialog.Title = "Export To...";
            saveFileDialog.Filter = PreviewWrapper.ExportFilter(availableProviders);

            for (int i = 0; i < availableProviders.Length; i++)
                if (availableProviders[i] == ep)
                {
                    saveFileDialog.FilterIndex = i + 1;
                    break;
                }
            if (saveFileDialog.ShowDialog(parent) != DialogResult.OK)
                return false;

            ep = availableProviders[saveFileDialog.FilterIndex - 1];
            name = saveFileDialog.FileName;
            return true;
        }

        /// <summary>
        /// Changes text in progress dialog window.
        /// </summary>
        /// <param name="text">New text.</param>
        internal void SetProgressText(string text)
        {
            if (_progressDlg != null)
            {
                _progressDlg.TxtPrinting = text;
                Application.DoEvents();
                if (string.IsNullOrEmpty(text))
                    _progressDlg.Hide();
            }
        }

        /// <summary>
        /// Returns a list of export providers corresponding given file extensions.
        /// </summary>
        /// <param name="formatExt">A list of format extensions.</param>
        /// <returns>A list of export providers.</returns>
        internal static C1.C1Preview.Export.ExportProvider[] GetExportProviders(string formatExt)
        {
            if (formatExt != null && formatExt.Length > 0)
                return PreviewWrapper.GetExportProviders(formatExt);
            return PreviewWrapper.GetExportProviders();
        }

        /// <summary>
        /// Returns a new exporter for given export provider.
        /// </summary>
        /// <param name="exportProvider">Export provider.</param>
        /// <param name="exportParameters">Custom parameters for Excel exporter</param>
        /// <returns></returns>
        internal static C1.C1Preview.Export.Exporter CreateExporter(C1.C1Preview.Export.ExportProvider exportProvider, ExportParameters exportParameters)
        {
            C1.C1Preview.Export.Exporter result = exportProvider.NewExporter();
            System.Type exporterType = result.GetType();
            if (exporterType.Name == "XlsExporter")
            {
                // turn off OnePagePerSheet option
                PropertyInfo onePagePerSheetProperty = exporterType.GetProperty("OnePagePerSheet");
                if (onePagePerSheetProperty != null &&
                    onePagePerSheetProperty.PropertyType == typeof(bool) &&
                    onePagePerSheetProperty.CanWrite)
                {
                    onePagePerSheetProperty.SetValue(
                        result,
                        exportParameters == null ? false : exportParameters.OnePagePerSheet,
                        new object[0]);
                }
            }

            return result;
        }

        /// <summary>
        /// Returns a new exporter for the specified export provider.
        /// </summary>
        /// <param name="exportProvider">The export provider.</param>
        /// <returns>A new instance of the exporter of the specified type.</returns>
        internal static C1.C1Preview.Export.Exporter CreateExporter(C1.C1Preview.Export.ExportProvider exportProvider)
        {
            return CreateExporter(exportProvider, null);
        }

        /// <summary>
        /// Closes the progress dialog if it is shown.
        /// </summary>
        internal void DisposeProgressDlg()
        {
            if (_progressDlg != null)
            {
                _progressDlg.Close();
                _progressDlg.Dispose();
                _progressDlg = null;
            }
        }

        #endregion

        #region Private UI-related methods

        /// <summary>
        /// Shows dialog for selecting print options
        /// </summary>
        /// <param name="exporter">Selected exporter.</param>
        /// <param name="actionMask">Action to perform.</param>
        /// <param name="outputfn">Output file name.</param>
        /// <returns>Selected action to perform.</returns>
        private ActionFlags DoShowOptionsDialog(ref C1.C1Preview.Export.Exporter exporter, ActionFlags actionMask, ref string outputfn)
        {
            // Options form can be changed (e.g. localized) by developer
            PrintOptionsForm frmOpt = new PrintOptionsForm();
            frmOpt.Exporter = exporter;
            frmOpt.OutputFileName = outputfn;
            frmOpt.Action = ActionFlags.Preview | ActionFlags.Print; //.None;
            frmOpt.ActionMask = actionMask;
            frmOpt.RowHeight = _printInfo.VarRowHeight;
            frmOpt.PageBreaks = _printInfo.PageBreak;
            frmOpt.FillEmpty = _printInfo.FillAreaWidth;
            frmOpt.WrapText = _printInfo.WrapText;
            frmOpt.m_chkRepeatRows.Checked = _printInfo.RepeatFixedRows;
            frmOpt.m_chkRepeatCols.Checked = _printInfo.RepeatFixedCols;
            frmOpt.m_chkUseGridColors.Checked = _printInfo.UseGridColors;
            frmOpt.m_chkHiddenRows.Checked = _printInfo.ShowHiddenRows;
            frmOpt.m_chkHiddenCols.Checked = _printInfo.ShowHiddenCols;
            frmOpt.m_chkShowProgress.Checked = _printInfo.ShowProgressForm;
            frmOpt.m_cmbHorzPageBreak.SelectedIndex = (int)(_printInfo.PageBreak);
            frmOpt.m_cmbStretchToWidth.SelectedIndex = (int)(_printInfo.FillAreaWidth);
            frmOpt.m_cmbWrapText.SelectedIndex = (int)(_printInfo.WrapText);
            frmOpt.m_txtFooter.Text = _printInfo.PageFooter;
            frmOpt.m_txtHeader.Text = _printInfo.PageHeader;
            frmOpt.m_numFooterHeight.Value = _printInfo.PageFooterHeight;
            frmOpt.m_numHeaderHeight.Value = _printInfo.PageHeaderHeight;
            frmOpt.m_numMaxRowHeight.Value = _printInfo.MaxRowHeight;
            DialogResult res = DialogResult.Cancel;
            try
            {
                res = frmOpt.ShowDialog();
            }
            catch
            {
                res = DialogResult.Abort;
            }
            if (res != DialogResult.OK)
            {
                frmOpt.Dispose();
                return ActionFlags.None;
            }

            System.Diagnostics.Debug.Assert((frmOpt.Action & actionMask) != 0);
            exporter = frmOpt.Exporter;
            outputfn = frmOpt.OutputFileName;
            _printInfo.RepeatFixedRows = frmOpt.m_chkRepeatRows.Checked;
            _printInfo.RepeatFixedCols = frmOpt.m_chkRepeatCols.Checked;
            _printInfo.UseGridColors = frmOpt.m_chkUseGridColors.Checked;
            _printInfo.ShowProgressForm = frmOpt.m_chkShowProgress.Checked;
            _printInfo.ShowHiddenRows = frmOpt.m_chkHiddenRows.Checked;
            _printInfo.ShowHiddenCols = frmOpt.m_chkHiddenCols.Checked;
            _printInfo.PageFooter = frmOpt.m_txtFooter.Text;
            _printInfo.PageHeader = frmOpt.m_txtHeader.Text;
            _printInfo.PageFooterHeight = (int)(frmOpt.m_numFooterHeight.Value);
            _printInfo.PageHeaderHeight = (int)(frmOpt.m_numHeaderHeight.Value);
            _printInfo.MaxRowHeight = (int)(frmOpt.m_numMaxRowHeight.Value);
            _printInfo.WrapText = frmOpt.WrapText;
            _printInfo.FillAreaWidth = frmOpt.FillEmpty;
            _printInfo.PageBreak = frmOpt.PageBreaks;
            _printInfo.VarRowHeight = frmOpt.RowHeight;
            ActionFlags action = frmOpt.Action;
            frmOpt.Dispose();
            return action;
        }

        /// <summary>
        /// Creates the preview wrapper if needed.
        /// </summary>
        private void LoadPreviewWrapper()
        {
            if (_previewWrapper == null)
                _previewWrapper = new PreviewWrapper(this);
        }

        private void OnParentFormActivate(object sender, EventArgs e)
        {
            if (_progressDlg != null)
                _progressDlg.Activate();
        }

        /// <summary>
        /// Returns true if the grid has at least one row and one column.
        /// </summary>
        private bool HaveSomethingToPrint
        {
            get
            {
                int cols = this.Cols.Count;
                int rows = this.Rows.Count;
                return cols != 0 && rows != 0;
            }
        }

        /// <summary>
        /// Creates a document with the grid, exports it to the specified format.
        /// </summary>
        /// <param name="formatExt">Format to export to.</param>
        /// <param name="outputFileName">Output file name.</param>
        /// <param name="exportParameters">Parameters.</param>
        private void _exportTo(string formatExt, string outputFileName, ExportParameters exportParameters)
        {
            if (!HaveSomethingToPrint)
                return;

            LoadPreviewWrapper();
            C1.C1Preview.Export.ExportProvider[] providers = PreviewWrapper.GetExportProviders(formatExt);
            if (providers == null || providers.Length != 1)
                throw new Exception("Could not create an export provider");

            C1.C1Preview.Export.ExportProvider ep = providers[0];

            // if we are not going to show options (which allow to select a file name)
            // and no file name has been specified, we must show a file save dialog
            bool showFileDlg = !_printInfo.ShowOptionsDialog &&
                (outputFileName == null || outputFileName.Length == 0 || formatExt == null);
            if (showFileDlg)
            {
                Form f = this.ParentForm;
                if (f != null && !SelectExportFile(f, providers, ref outputFileName, ref ep))
                    return;
            }
            C1.C1Preview.Export.Exporter exporter = CreateExporter(ep, exportParameters);
            string outputfn = outputFileName;
            if (_printInfo.ShowOptionsDialog)
                if (DoShowOptionsDialog(ref exporter, ActionFlags.Export, ref outputfn) != ActionFlags.Export)
                    return;
            _export(exporter, outputfn);
        }

        private void _exportTo(string formatExt, string outputFileName)
        {
            _exportTo(formatExt, outputFileName, null);
        }

        private void _export(C1.C1Preview.Export.Exporter exporter, string filename)
        {
            LoadPreviewWrapper();
            C1.C1Preview.C1PrintDocument doc = RenderSourceTree();
            _previewWrapper.Export(doc, exporter, filename);
            DisposeProgressDlg();
        }

        /// <summary>
        /// Creates an instance of C1PrintDocument class representing the grid.
        /// </summary>
        /// <returns>The created document.</returns>
        private C1.C1Preview.C1PrintDocument RenderSourceTree()
        {
            C1.C1Preview.C1PrintDocument c1doc = null;
            Form gridForm = null;
            try
            {
                // show progress dialog if required
                if (_printInfo.ShowProgressForm)
                {
                    _progressDlg = new C1PrintProgress();
                    // so we can change the title
                    if (_printInfo.ProgressCaption.Length > 0)
                        _progressDlg.TxtTitle = _printInfo.ProgressCaption;

                    _progressDlg.Show();
                    Application.DoEvents();
                    gridForm = this.ParentForm;
                    if (gridForm != null)
                        // imitate modal dialog
                        gridForm.Activated += new EventHandler(OnParentFormActivate);
                }
                // prepare the source tree
                c1doc = this.PrintTreeBuild(_progressDlg);
                // render it
                if (_progressDlg != null)
                {
                    Button cb = _progressDlg.CancelButton as Button;
                    if (cb != null)
                        cb.Enabled = false; // we cannot cancel rendering yet
                }
            }
            catch (PrintCancelException)
            {
                return null;
            }
            finally
            {
                if (_progressDlg != null)
                {
                    if (gridForm != null)
                        gridForm.Activated -= new EventHandler(OnParentFormActivate);
                }
            }
            return c1doc;
        }

        private Form ParentForm
        {
            get
            {
                Form f = this.FindForm();
                if (f == null && this.DataSource is C1FlexGridBase)
                    f = ((C1FlexGridBase)this.DataSource).FindForm();
                if (f == null && Form.ActiveForm != null)
                    f = Form.ActiveForm;
                return f;
            }
        }

        #endregion

        #region Private methods for preparing print document

        /// <summary>
        /// The main method to prepare print document with grid.
        /// </summary>
        /// <param name="progressDlg">The instance of progress dialog form.</param>
        /// <returns>Prepared document.</returns>
        private C1.C1Preview.C1PrintDocument PrintTreeBuild(C1PrintProgress progressDlg)
        {
            this.OnStartPrinting();

            Initialize();
            _progressDlg = progressDlg;

            if (_doc == null)
                _doc = new C1.C1Preview.C1PrintDocument();
            else
            {
                _workTable = null;
                _doc.Clear();
            }
            _doc.DefaultUnit = UnitTypeEnum.Document;  // 1/300 inch
            if (_printInfo.PageSettings != null)
                _doc.PageLayout.PageSettings = new C1PageSettings(_printInfo.PageSettings);

            RenderTable workArea = NewTable();

            // Calculates the zoom factor according to the page size.
            this.GetZoom();

            // Render the page header and footer
            this.RenderHeader();
            this.RenderFooter();

            this.CreateTableCols();
            this.CreateTableRows();
            this.CreateTableCells();

            // row/col groups must be declared after rows/cols have been actually added,
            // hence moved here from NewTable:
            if (_printInfo.RepeatFixedRows && this.Rows.Fixed > 0)
                _workTable.RowGroups[0, this.Rows.Fixed].PageHeader = true;
            if (_printInfo.RepeatFixedCols && this.Cols.Fixed > 0)
                _workTable.ColGroups[0, this.Cols.Fixed].PageHeader = true;

            _doc.Body.Children.Add(_workTable);

            this.OnEndPrinting();
            return _doc;
        }

        /// <summary>
        /// Initializes private variables.
        /// </summary>
        private void Initialize()
        {
            _terminated = false;
            // String format for measuring
            _sf = new StringFormat();
            _sf.Alignment = StringAlignment.Near;
            _sf.LineAlignment = StringAlignment.Near;
            switch (_printInfo.WrapText)
            {
                case WrapTextEnum.NoWrap:
                    _sf.FormatFlags |= StringFormatFlags.NoWrap;
                    break;
            }

            // Height of page header and footer in document units
            _pageHeaderHeight = InchToDoc(_printInfo.PageHeaderHeight);
            _pageFooterHeight = InchToDoc(_printInfo.PageFooterHeight);

            // Graphics for measuring
            if (_measureGraphics == null)
                _measureGraphics = Graphics.FromHwnd(IntPtr.Zero);

            // The value of screen and printer resolution
            _dpiPrinter = _printInfo.PageSettings.PrinterResolution.Y;
            _dpiScreen = (int)_measureGraphics.DpiX;

            // Calculate available paper size.
            try
            {
                //width in 3 hundreds of inch
                int w = InchToDoc(_printInfo.PageSettings.PaperSize.Width);
                //height in 3 hundreds of inch
                int h = InchToDoc(_printInfo.PageSettings.PaperSize.Height);
                if (_printInfo.PageSettings.Landscape)
                {
                    int swap = w;
                    w = h;
                    h = swap;
                }
                w -= InchToDoc(_printInfo.PageSettings.Margins.Left + _printInfo.PageSettings.Margins.Right);
                h -= InchToDoc(_printInfo.PageSettings.Margins.Top + _printInfo.PageSettings.Margins.Bottom);
                _maxWidth = w;
                _maxHeight = h;
            }
            catch
            {
                PaperSize ret = GetCurrentLocaleDefaultPaperSize();
                //width in 3 hundreds of inch
                _maxWidth = InchToDoc(ret.Width - _printInfo.PageSettings.Margins.Left - _printInfo.PageSettings.Margins.Right);
                //height in 3 hundreds of inch
                _maxHeight = InchToDoc(ret.Height - _printInfo.PageSettings.Margins.Top - _printInfo.PageSettings.Margins.Bottom);
            }

            // calculate max available width of grid column (column can't be greater when page width!)
            int borderw = (int)Math.Round(Utils.ConvertUnits(LineDef.Default.Width.Value, LineDef.Default.Width.Units, UnitTypeEnum.Document, 0, 0)) * 2;
            _maxWidthPix = DocToPx(_maxWidth - borderw);

            // get glyphs for expanded and collapsed nodes
            Image imgCollapsed = this.Glyphs[GlyphEnum.Collapsed];
            Image imgExpanded = this.Glyphs[GlyphEnum.Expanded];

            // button size:
            _imgSize.Width = Math.Max(imgCollapsed.Width, imgExpanded.Width);
            _imgSize.Height = Math.Max(imgCollapsed.Height, imgExpanded.Height);

            // max height of detail rows
            switch (_printInfo.VarRowHeight)
            {
                case RowHeightEnum.LikeGrid:
                    _rowh = -1;
                    break;
                case RowHeightEnum.StretchToMax:
                    _rowh = InchToDoc(_printInfo.MaxRowHeight);
                    break;
                case RowHeightEnum.StretchToFit:
                    _rowh = 0;
                    break;
            }
        }

        /// <summary>
        /// Creates a RenderTable for printing and sets its properties.
        /// </summary>
        /// <returns></returns>
        private RenderTable NewTable()
        {
            _workTable = new RenderTable(_doc);
            _workTable.Height = Unit.Auto;

            switch (_printInfo.PageBreak)
            {
                case PageBreaksEnum.FitIntoArea:
                    _workTable.Width = new Unit("100%parent");
                    _workTable.SplitHorzBehavior = SplitBehaviorEnum.SplitNewPage;
                    break;
                default:
                    _workTable.Width = Unit.Auto;
                    _workTable.SplitHorzBehavior = SplitBehaviorEnum.SplitIfNeeded;
                    switch (_printInfo.FillAreaWidth)
                    {
                        case FillEmptyEnum.ExtendAll:
                            _workTable.StretchColumns = StretchTableEnum.AllVectors;
                            break;
                        case FillEmptyEnum.ExtendLastCol:
                            _workTable.StretchColumns = StretchTableEnum.LastVectorOnPage;
                            break;
                        case FillEmptyEnum.None:
                            break;
                    }
                    break;
            }
            switch (_printInfo.WrapText)
            {
                case WrapTextEnum.NoWrap:
                    _workTable.Style.WordWrap = false;
                    break;
                case WrapTextEnum.Wrap:
                    _workTable.Style.WordWrap = true;
                    break;
                case WrapTextEnum.LikeCell:
                    //assigned for individual column
                    break;
            }

            // table styles
            _workTable.Style.GridLines.All = LineDef.Default;
            if (this.Styles.Normal.Border.Style == BorderStyleEnum.None)
                _workTable.Style.GridLines.All = LineDef.Empty;
            _workTable.CellStyle.Padding.All = "5doc";
            C1.Win.C1FlexGrid.CellStyle cstyle = this.Styles.Normal;
            if (cstyle.BackColor.IsEmpty || cstyle.BackColor.Equals(Color.Transparent))
                if (!IsPaperColor(this.BackColor))
                    cstyle.BackColor = this.BackColor;
            ApplyStyle(_workTable.CellStyle, cstyle, true, false);
            if (this.BackgroundImage != null && cstyle.BackgroundImage == null)
                cstyle.BackgroundImage = this.BackgroundImage;
            ApplyStyle(_workTable.Style, cstyle, true, false);

            return _workTable;
        }

        /// <summary>
        /// Creates page header row.
        /// </summary>
        private void RenderHeader()
        {
            if (_printInfo.IsOwnerDrawPageHeader)
            {
                C1OwnerDrawPrint odp = new C1OwnerDrawPrint(_doc);
                odp.SetHeightInch((float)_pageHeaderHeight / 300);
                odp.StartDrawing();
                OwnerDrawPageEventArgs e = new OwnerDrawPageEventArgs(odp);
                this.OnOwnerDrawPageHeader(e);
                odp.EndDrawing();
                _doc.PageLayout.PageHeader = odp.GetRootObject();
                return;
            }

            if (_printInfo.PageHeader.Length > 0)
            {
                RenderArea rt = new RenderArea(_doc);
                rt.Width = new Unit("100%parent");
                rt.Height = new Unit(_pageHeaderHeight, UnitTypeEnum.Document);
                if (_printInfo.PageHeaderStyle != null)
                    ApplyStyle(rt.Style, _printInfo.PageHeaderStyle, true, false);
                else
                    ApplyStyle(rt.Style, this.Styles.Normal, true, false);

                // draw left/center/right parts of the string
                string[] strParts = SplitLine(_printInfo.PageHeader);
                if (strParts.Length > 0 && strParts[0].Length > 0)
                {
                    RenderText mytext1 = CreatePageRenderText(strParts[0]);
                    mytext1.Style.TextAlignHorz = AlignHorzEnum.Left;
                    rt.Children.Add(mytext1);
                }
                if (strParts.Length > 1 && strParts[1].Length > 0)
                {
                    RenderText mytext1 = CreatePageRenderText(strParts[1]);
                    mytext1.Style.TextAlignHorz = AlignHorzEnum.Center;
                    rt.Children.Add(mytext1);
                }
                if (strParts.Length > 2 && strParts[2].Length > 0)
                {
                    RenderText mytext1 = CreatePageRenderText(strParts[2]);
                    mytext1.Style.TextAlignHorz = AlignHorzEnum.Right;
                    rt.Children.Add(mytext1);
                }

                _doc.PageLayout.PageHeader = rt;
            }
        }

        /// <summary>
        /// Creates page footer row.
        /// </summary>
        private void RenderFooter()
        {
            if (_printInfo.IsOwnerDrawPageFooter)
            {
                C1OwnerDrawPrint odp = new C1OwnerDrawPrint(_doc);
                odp.SetHeightInch((float)_pageFooterHeight / 300);
                odp.StartDrawing();
                OwnerDrawPageEventArgs e = new OwnerDrawPageEventArgs(odp);
                this.OnOwnerDrawPageFooter(e);
                odp.EndDrawing();
                _doc.PageLayout.PageFooter = odp.GetRootObject();
                return;
            }

            if (_printInfo.PageFooter.Length > 0)
            {
                RenderArea rt = new RenderArea(_doc);
                rt.Width = new Unit("100%parent");
                rt.Height = new Unit(_pageFooterHeight, UnitTypeEnum.Document);
                if (_printInfo.PageFooterStyle != null)
                    ApplyStyle(rt.Style, _printInfo.PageHeaderStyle, true, false);
                else
                    ApplyStyle(rt.Style, this.Styles.Normal, true, false);

                // draw left/center/right parts of the string
                string[] strParts = SplitLine(_printInfo.PageFooter);
                if (strParts.Length > 0 && strParts[0].Length > 0)
                {
                    RenderText mytext1 = CreatePageRenderText(strParts[0]);
                    mytext1.Style.TextAlignHorz = AlignHorzEnum.Left;
                    rt.Children.Add(mytext1);
                }
                if (strParts.Length > 1 && strParts[1].Length > 0)
                {
                    RenderText mytext1 = CreatePageRenderText(strParts[1]);
                    mytext1.Style.TextAlignHorz = AlignHorzEnum.Center;
                    rt.Children.Add(mytext1);
                }
                if (strParts.Length > 2 && strParts[2].Length > 0)
                {
                    RenderText mytext1 = CreatePageRenderText(strParts[2]);
                    mytext1.Style.TextAlignHorz = AlignHorzEnum.Right;
                    rt.Children.Add(mytext1);
                }

                _doc.PageLayout.PageFooter = rt;
            }
        }

        private bool ExtendLast
        {
            get
            {
                if (_printInfo.FillAreaWidth == FillEmptyEnum.ExtendLastCol)
                    return true;
                return this.ExtendLastCol;
            }
        }

        /// <summary>
        /// Defines whether given color is the default for paper backcolor.
        /// </summary>
        /// <param name="color">Tested color.</param>
        /// <returns>True if the color is the defaul, false otherwise.</returns>
        private bool IsPaperColor(Color color)
        {
            return color.Equals(SystemColors.Window) || color.Equals(Color.White) || color.IsEmpty || color.Equals(Color.Transparent);
        }

        /// <summary>
        /// Applies grid style to print style.
        /// </summary>
        /// <param name="pstyle">Print style.</param>
        /// <param name="cstyle">Grid style.</param>
        /// <param name="cell">Is it style for cells?</param>
        /// <param name="compareWithParent">Create child style, comparing with parent or not</param>
        private void ApplyStyle(Style pstyle, C1.Win.C1FlexGrid.CellStyle cstyle, bool cell, bool compareWithParent)
        {
            if (pstyle == null || cstyle == null)
                return;

            if (this._printInfo.UseGridColors)
            {
                if (!compareWithParent || _workTable.CellStyle.TextColor != cstyle.ForeColor)
                    if (!cstyle.ForeColor.IsEmpty && !cstyle.ForeColor.Equals(Color.Transparent))
                        pstyle.TextColor = cstyle.ForeColor;
                if (!compareWithParent || _workTable.CellStyle.BackColor != cstyle.BackColor)
                    if (!IsPaperColor(cstyle.BackColor))
                        pstyle.BackColor = cstyle.BackColor;
                if (!compareWithParent || _workTable.Style.BackgroundImage != cstyle.BackgroundImage)
                {
                    pstyle.BackgroundImage = cstyle.BackgroundImage;
                    pstyle.BackgroundImageAlign.TileHorz = true;
                    pstyle.BackgroundImageAlign.TileVert = true;
                }
            }
            //use the parent font if we can
            Font f = GetFont(cstyle.Font);
            if (!compareWithParent || CheckFont(f, _workTable.CellStyle))
                pstyle.Font = f;

            switch (cstyle.TextDirection)
            {
                case TextDirectionEnum.Down:
                    pstyle.TextAngle = 90;
                    break;
                case TextDirectionEnum.Up:
                    pstyle.TextAngle = 270;
                    break;
            }
            ImageAlign align = CreateImageAlign(cstyle.ImageAlign);
            if (!compareWithParent || !align.Equals(_workTable.CellStyle.ImageAlign))
                pstyle.ImageAlign = align;

            switch (cstyle.TextAlign)
            {
                case TextAlignEnum.CenterBottom:
                    pstyle.TextAlignHorz = AlignHorzEnum.Center;
                    pstyle.TextAlignVert = AlignVertEnum.Bottom;
                    break;
                case TextAlignEnum.CenterCenter:
                    pstyle.TextAlignHorz = AlignHorzEnum.Center;
                    pstyle.TextAlignVert = AlignVertEnum.Center;
                    break;
                case TextAlignEnum.CenterTop:
                    pstyle.TextAlignHorz = AlignHorzEnum.Center;
                    pstyle.TextAlignVert = AlignVertEnum.Top;
                    break;
                case TextAlignEnum.GeneralBottom:
                    pstyle.TextAlignHorz = AlignHorzEnum.Left;
                    pstyle.TextAlignVert = AlignVertEnum.Bottom;
                    break;
                case TextAlignEnum.GeneralCenter:
                    pstyle.TextAlignHorz = AlignHorzEnum.Left;
                    pstyle.TextAlignVert = AlignVertEnum.Center;
                    break;
                case TextAlignEnum.GeneralTop:
                    pstyle.TextAlignHorz = AlignHorzEnum.Left;
                    pstyle.TextAlignVert = AlignVertEnum.Top;
                    break;
                case TextAlignEnum.LeftBottom:
                    pstyle.TextAlignHorz = AlignHorzEnum.Left;
                    pstyle.TextAlignVert = AlignVertEnum.Bottom;
                    break;
                case TextAlignEnum.LeftCenter:
                    pstyle.TextAlignHorz = AlignHorzEnum.Left;
                    pstyle.TextAlignVert = AlignVertEnum.Center;
                    break;
                case TextAlignEnum.LeftTop:
                    pstyle.TextAlignHorz = AlignHorzEnum.Left;
                    pstyle.TextAlignVert = AlignVertEnum.Top;
                    break;
                case TextAlignEnum.RightBottom:
                    pstyle.TextAlignHorz = AlignHorzEnum.Right;
                    pstyle.TextAlignVert = AlignVertEnum.Bottom;
                    break;
                case TextAlignEnum.RightCenter:
                    pstyle.TextAlignHorz = AlignHorzEnum.Right;
                    pstyle.TextAlignVert = AlignVertEnum.Center;
                    break;
                case TextAlignEnum.RightTop:
                    pstyle.TextAlignHorz = AlignHorzEnum.Right;
                    pstyle.TextAlignVert = AlignVertEnum.Top;
                    break;
            }

            if (_printInfo.WrapText == WrapTextEnum.LikeCell)
                pstyle.WordWrap = cstyle.WordWrap;
        }

        /// <summary>
        /// Creates print Image align from given grid image align.
        /// </summary>
        /// <param name="align">Grid style image align.</param>
        /// <returns></returns>
        private ImageAlign CreateImageAlign(ImageAlignEnum align)
        {
            ImageAlign ia = ImageAlign.Default;
            switch (align)
            {
                case ImageAlignEnum.CenterCenter:
                    ia.AlignHorz = ImageAlignHorzEnum.Center;
                    ia.AlignVert = ImageAlignVertEnum.Center;
                    break;
                case ImageAlignEnum.CenterTop:
                    ia.AlignHorz = ImageAlignHorzEnum.Center;
                    ia.AlignVert = ImageAlignVertEnum.Top;
                    break;
                case ImageAlignEnum.CenterBottom:
                    ia.AlignHorz = ImageAlignHorzEnum.Center;
                    ia.AlignVert = ImageAlignVertEnum.Bottom;
                    break;
                case ImageAlignEnum.LeftBottom:
                    ia.AlignHorz = ImageAlignHorzEnum.Left;
                    ia.AlignVert = ImageAlignVertEnum.Bottom;
                    break;
                case ImageAlignEnum.LeftCenter:
                    ia.AlignHorz = ImageAlignHorzEnum.Left;
                    ia.AlignVert = ImageAlignVertEnum.Center;
                    break;
                case ImageAlignEnum.LeftTop:
                    ia.AlignHorz = ImageAlignHorzEnum.Left;
                    ia.AlignVert = ImageAlignVertEnum.Top;
                    break;
                case ImageAlignEnum.RightBottom:
                    ia.AlignHorz = ImageAlignHorzEnum.Right;
                    ia.AlignVert = ImageAlignVertEnum.Bottom;
                    break;
                case ImageAlignEnum.RightCenter:
                    ia.AlignHorz = ImageAlignHorzEnum.Right;
                    ia.AlignVert = ImageAlignVertEnum.Center;
                    break;
                case ImageAlignEnum.RightTop:
                    ia.AlignHorz = ImageAlignHorzEnum.Right;
                    ia.AlignVert = ImageAlignVertEnum.Top;
                    break;
                case ImageAlignEnum.Scale:
                    ia.KeepAspectRatio = true;
                    break;
                case ImageAlignEnum.Stretch:
                    ia.StretchHorz = true;
                    ia.StretchVert = true;
                    break;
                case ImageAlignEnum.Tile:
                    ia.TileHorz = true;
                    ia.TileVert = true;
                    break;
            }

            return ia;
        }

        /// <summary>
        /// Creates RenderTable cells for all grid rows and columns.
        /// </summary>
        private void CreateTableCells()
        {
            int r = 0;
            foreach (Row row in this.Rows)
            {
                if (!_printInfo.ShowHiddenRows && !row.Visible)
                    continue;

                int c = 0;
                foreach (Column col in this.Cols)
                {
                    if (!_printInfo.ShowHiddenCols && !col.Visible)
                        continue;

                    CreateTableCell(r, c, row.Index, col.Index);
                    c++;
                }
                r++;
                this.CurRow = r;
            }
        }

        /// <summary>
        /// Creates node image for tree node with level offset.
        /// </summary>
        /// <param name="ra">Parent RenderArea.</param>
        /// <param name="node">Grid node.</param>
        /// <param name="row">Row number.</param>
        private void CreateNode(RenderArea ra, Node node, int row)
        {
            int level = (node != null) ? node.Level : -1;
            if (IsFlagSet(TreeStyleFlags.Symbols))
            {
                Rectangle rc = new Rectangle(0, 0, 1000, node.Row.HeightDisplay);
                Rectangle rcBtn = GetSymbolRect(rc, level);

                ImageAlign imgAlign = ImageAlign.Default;

                Image imgBtn = GetButtonImage(row);
                if (imgBtn != null)
                {
                    RenderImage ri = new RenderImage(ra.Document, imgBtn);
                    ri.Y = "parent.height/2 - self.height/2";
                    ri.X = new Unit(rcBtn.X, UnitTypeEnum.Pixel);
                    ri.Width = new Unit(rcBtn.Width, UnitTypeEnum.Pixel);
                    ra.Children.Add(ri);
                }
            }
        }

        /// <summary>
        /// Sets the image as the background image of the render area, and apply the correct alignment.
        /// </summary>
        /// <param name="ra">Parent area.</param>
        /// <param name="img">Background image.</param>
        /// <param name="ai">Image align.</param>
        private void CreateCellImage(RenderArea ra, Image img, ImageAlignEnum ai)
        {
            if (ai == ImageAlignEnum.Hide)
                return;

            ra.Style.BackgroundImage = img;

            if (ai == ImageAlignEnum.Stretch)
            {
                ra.Style.BackgroundImageAlign.StretchHorz = true;
                ra.Style.BackgroundImageAlign.StretchVert = true;
            }
            else
            {
                ra.Style.BackgroundImageAlign.StretchHorz = false;
                ra.Style.BackgroundImageAlign.StretchVert = false;
            }

            if (ai == ImageAlignEnum.Tile)
            {
                ra.Style.BackgroundImageAlign.TileHorz = true;
                ra.Style.BackgroundImageAlign.TileVert = true;
            }
            else
            {
                ra.Style.BackgroundImageAlign.TileHorz = false;
                ra.Style.BackgroundImageAlign.TileVert = false;
            }

            if (ai == ImageAlignEnum.CenterBottom || ai == ImageAlignEnum.CenterCenter || ai == ImageAlignEnum.CenterTop)
                ra.Style.BackgroundImageAlign.AlignHorz = ImageAlignHorzEnum.Center;

            if (ai == ImageAlignEnum.LeftBottom || ai == ImageAlignEnum.LeftCenter || ai == ImageAlignEnum.LeftTop)
                ra.Style.BackgroundImageAlign.AlignHorz = ImageAlignHorzEnum.Left;

            if (ai == ImageAlignEnum.RightBottom || ai == ImageAlignEnum.RightCenter || ai == ImageAlignEnum.RightTop)
                ra.Style.BackgroundImageAlign.AlignHorz = ImageAlignHorzEnum.Right;

            if (ai == ImageAlignEnum.CenterBottom || ai == ImageAlignEnum.LeftBottom || ai == ImageAlignEnum.RightBottom)
                ra.Style.BackgroundImageAlign.AlignVert = ImageAlignVertEnum.Bottom;

            if (ai == ImageAlignEnum.CenterCenter || ai == ImageAlignEnum.LeftCenter || ai == ImageAlignEnum.RightCenter)
                ra.Style.BackgroundImageAlign.AlignVert = ImageAlignVertEnum.Center;

            if (ai == ImageAlignEnum.CenterTop || ai == ImageAlignEnum.LeftTop || ai == ImageAlignEnum.RightTop)
                ra.Style.BackgroundImageAlign.AlignVert = ImageAlignVertEnum.Top;
        }

        private void CreateCellImage(RenderArea ra, Image img, CellStyle style)
        {
            this.CreateCellImage(ra, img, style.ImageAlign);
        }

        /// <summary>
        /// Checks whether a cell can merge with another cell.
        /// </summary>
        /// <param name="row">Row number.</param>
        /// <param name="col">Column number.</param>
        /// <returns></returns>
        private bool MergeCell(int row, int col)
        {
            // special cases
            switch (this.AllowMerging)
            {
                case AllowMergingEnum.None: return false;
                case AllowMergingEnum.Spill: return true;
                case AllowMergingEnum.Nodes: return this.Rows[row].IsNode;
            }

            // default, defer to row/col settings
            return this.Rows[row].AllowMerging || this.Cols[col].AllowMerging;
        }

        /// <summary>
        /// Creates a simple table cell.
        /// </summary>
        /// <param name="r">Print table row number.</param>
        /// <param name="c">Print table column number.</param>
        /// <param name="row">Grid row number.</param>
        /// <param name="col">Grid column number.</param>
        /// <returns></returns>
        private TableCell CreateTableCellBase(int r, int c, int row, int col)
        {
            // get content
            Image img = null;
            CheckEnum chk = CheckEnum.None;
            string str = this.GetDataDisplay(row, col, out img, out chk);

            Image chkImage = GetCheckImage(chk);
            if (chkImage != null)
            {
                img = chkImage;
                if (this.IsCellCheckBox(row, col))
                    str = null;
            }

            // get style
            CellStyle style = this.GetCellStyleDisplay(row, col);
            CellStyle cs = this.GetCellStyle(row, col);

            RenderArea ra = new RenderArea(_doc);
            ra.Stacking = StackingRulesEnum.BlockLeftToRight;

            // get cell rectangle in view coordinates (scrolled)
            Rectangle rc = this.GetCellRectDisplay(row, col);
            if (this.DrawMode == DrawModeEnum.OwnerDraw)
            {
                CellStyle s = this.GetCellStyleDisplay(row, col);
                OwnerDrawCellEventArgs e = new OwnerDrawCellEventArgs(this, OwnerDrawGraphics, row, col, s, rc, str, img);
                this.OnOwnerDrawCell(e);
                if (!e.Handled) //&& e._dirty)
                {
                    str = e.Text;
                    img = e.Image;

                    if (e.Style != null)
                        cs = e.Style;
                }
            }

            this.RenderCell(ra, row, col, style, cs, rc, str, img, chk);

            TableCell result = _workTable.Cells[r, c];
            result.CellStyle.AmbientParent = _workTable.Rows[r].CellStyle;
            ra.Style.AmbientParent = result.CellStyle;
            result.RenderObject = ra;
          
            return result;
        }


        /// <summary>
        /// Creates a simple table cell or a cell with merging.
        /// </summary>
        /// <param name="r">Print table row number.</param>
        /// <param name="c">Print table column number.</param>
        /// <param name="row">Grid row number.</param>
        /// <param name="col">Grid column number.</param>
        /// <returns></returns>
        private TableCell CreateTableCell(int r, int c, int row, int col)
        {
            if (!MergeCell(row, col))
                return CreateTableCellBase(r, c, row, col);

            CellRange rg = this.GetMergedRange(row, col, false);

            // ignore invisible cells
            while (rg.r1 < rg.r2 && !this.Rows[rg.r1].Visible && !_printInfo.ShowHiddenRows) rg.r1++;
            while (rg.c1 < rg.c2 && !this.Cols[rg.c1].Visible && !_printInfo.ShowHiddenRows) rg.c1++;

            // use base implementation for single cells
            if (rg.IsSingleCell)
                return CreateTableCellBase(r, c, row, col);

            // skip this cell if it has been handled over
            if (row > rg.r1)
                return null;
            if (col > rg.c1)
                return null;

            TableCell tc = CreateTableCellBase(r, c, row, col);
            tc.SpanCols = Math.Abs(rg.c2 - rg.c1) + 1;
            tc.SpanRows = Math.Abs(rg.r2 - rg.r1) + 1;
            tc.VertSplitBehavior = CellSplitBehaviorEnum.Copy;

            return tc;
        }

        /// <summary>
        /// Applies styles to table cell.
        /// </summary>
        /// <param name="ra">Parent area.</param>
        /// <param name="row">Grid row number.</param>
        /// <param name="col">Grid column number.</param>
        /// <param name="style">Grid cell ctyle.</param>
        /// <param name="s">Ownerdraw grid cell style.</param>
        /// <param name="rc">Rectangle for cell display.</param>
        /// <param name="str">Cell text.</param>
        /// <param name="img">Cell image</param>
        /// <param name="chk">Check image state.</param>
        private void RenderCell(RenderArea ra, int row, int col, CellStyle style, CellStyle s, Rectangle rc, string str, Image img, CheckEnum chk)
        {
            int indent = style.Margins.Left;
            if (s != null)
                this.ApplyStyle(ra.Style, s, true, true);

            int widTree = TreeWidth(col);
            if (widTree > 0)
            {
                // get node object to draw tree
                Node nd = this.Rows[row].Node;
                if (nd != null && nd.Level >= 0)
                {
                    indent += (nd.Level + 1) * this.Tree.Indent;
                    this.CreateNode(ra, nd, row);
                }
            }

            Image glyph = null;
            bool right = false;
            if (this.GetCellGlyph(row, col, rc, ref glyph, ref right))
                this.CreateCellImage(ra, glyph, right ? ImageAlignEnum.RightCenter : ImageAlignEnum.LeftCenter);

            DisplayEnum d = (DisplayEnum)style.Display;
            switch (d)
            {
                case DisplayEnum.ImageOnly: str = null; break;
                case DisplayEnum.TextOnly: img = null; break;
                case DisplayEnum.None: return;
            }

            if (style.ImageAlign == ImageAlignEnum.Hide)
                img = null;

            bool bHasImage = (img != null);
            bool bHasText = (str != null && str.Length > 0);
            if (!bHasImage && !bHasText)
                return;


            //added by kevin - for right align, ky dli mo effect ang right marign, indent variable modified

            if (style != null)
            {
                if(style.TextAlign == TextAlignEnum.RightCenter || style.TextAlign == TextAlignEnum.RightBottom ||
                            style.TextAlign == TextAlignEnum.RightTop)
                {
                    indent = (-indent);
                }
            }

            //end added


            RenderText txt = new RenderText(_doc);
            txt.Text = str;
            txt.Height = "parent.height";
            txt.X = new Unit(this.PxToDoc(indent), UnitTypeEnum.Document); 
            
            //orignal code commented
            //if (IsNumeric(str))
            //    txt.Style.TextAlignHorz = //AlignHorzEnum.Right;

            //modified by kevin - as is ang alignment should be the same as flexgrid
            if (style != null)
            {
                if (style.TextAlign == TextAlignEnum.LeftCenter || style.TextAlign == TextAlignEnum.LeftBottom ||
                    style.TextAlign == TextAlignEnum.LeftTop)
                {
                    txt.Style.TextAlignHorz = AlignHorzEnum.Left;
                }
                else if (style.TextAlign == TextAlignEnum.CenterCenter || style.TextAlign == TextAlignEnum.CenterBottom ||
                    style.TextAlign == TextAlignEnum.CenterTop)
                {
                    txt.Style.TextAlignHorz = AlignHorzEnum.Center;
                }
                else if (style.TextAlign == TextAlignEnum.RightCenter || style.TextAlign == TextAlignEnum.RightBottom ||
                            style.TextAlign == TextAlignEnum.RightTop)
                {
                    txt.Style.TextAlignHorz = AlignHorzEnum.Right;
                }

            }
            //end modified

            if (bHasImage)
            {
                Offsets of = new Offsets();
                Margins m = style.Margins;

                if (style.ImageAlign == ImageAlignEnum.LeftBottom
                    || style.ImageAlign == ImageAlignEnum.LeftCenter
                    || style.ImageAlign == ImageAlignEnum.LeftTop
                    )
                    of.Left = new Unit(this.PxToDoc(img.Width + m.Left + 2), UnitTypeEnum.Document);

                if (style.ImageAlign == ImageAlignEnum.RightBottom
                    || style.ImageAlign == ImageAlignEnum.RightCenter
                    || style.ImageAlign == ImageAlignEnum.RightTop
                    )
                    of.Right = new Unit(this.PxToDoc(img.Width + m.Right + 2), UnitTypeEnum.Document);

                txt.Style.Padding = of;
                txt.Style.BackgroundImage = img;
                txt.Style.BackgroundImageAlign = this.CreateImageAlign(style.ImageAlign);
            }

            ra.Children.Add(txt);
        }

        private Graphics OwnerDrawGraphics
        {
            get
            {
                if (_graphics == null)
                    _graphics = Graphics.FromImage(_img);

                return _graphics;
            }
        }

        /// <summary>
        /// Creates table columns for all grid columns.
        /// </summary>
        private void CreateTableCols()
        {
            _workTable.Cols.Count = 0;
            int count = 0;
            _perc = 0;
            foreach (Column col in this.Cols)
            {
                if (!_printInfo.ShowHiddenCols && !col.Visible)
                    continue;

                this.CreateTableCol(count, col.Index);
                count++;
            }

            if (this.ExtendLast && !_workTable.Width.IsAuto)
                _workTable.Cols[_workTable.Cols.Count - 1].Width = Unit.Auto;
        }

        /// <summary>
        /// Creates one table column for given grid column.
        /// </summary>
        /// <param name="c">Table column number.</param>
        /// <param name="col">Grid column number.</param>
        /// <returns></returns>
        private TableCol CreateTableCol(int c, int col)
        {
            TableCol tc = _workTable.Cols[c];
            C1.Win.C1FlexGrid.Column flexCol = this.Cols[col];
            if (_printInfo.PageBreak == PageBreaksEnum.FitIntoArea)
            {
                double w = Math.Round(this.PxToDoc(GetColWidth(flexCol) / _zoom) * 100.0 / _maxWidth, 2);
                if (_perc + w > 99)
                    w = Math.Round(100 - _perc, 2);
                _perc += w;
                string swidth = w.ToString() + "%parent.width";
                swidth = swidth.Replace(",", ".");  //if delimiter is comma 
                tc.Width = swidth;
            }
            else
                tc.Width = new Unit(this.PxToDoc(GetColWidth(flexCol)), UnitTypeEnum.Document);

            if (col < this.Cols.Fixed)
                this.ApplyStyle(tc.CellStyle, flexCol.StyleFixedDisplay, true, true);

            return tc;
        }

        /// <summary>
        /// Creates table rows for all grid rows.
        /// </summary>
        private void CreateTableRows()
        {
            _workTable.Rows.Count = 0;
            int count = 0;
            foreach (Row row in this.Rows)
            {
                if (!_printInfo.ShowHiddenRows && !row.Visible)
                    continue;

                this.CreateTableRow(count, row.Index);
                count++;
            }
        }

        /// <summary>
        /// Creates one table row for given grid row.
        /// </summary>
        /// <param name="r">Table row number.</param>
        /// <param name="row">Grid row number.</param>
        /// <returns></returns>
        private TableRow CreateTableRow(int r, int row)
        {
            TableRow tr = _workTable.Rows[r];
            C1.Win.C1FlexGrid.Row flexRow = this.Rows[row];

            CellStyle curstyle = flexRow.StyleDisplay;
            if (row < this.Rows.Fixed)
                curstyle = flexRow.StyleFixedDisplay;

            int newh = PxToDoc(flexRow.HeightDisplay);
            int maxh = CalcRowMaxHeight(row, curstyle);
            if (_rowh >= 0)
            {
                if (newh < maxh) newh = maxh;  //max height of row
                if ((_rowh > 0) && (newh > _rowh))
                    newh = _rowh;  //not greater then max height of row
            }
            tr.Height = new Unit(newh, UnitTypeEnum.Document);
            this.ApplyStyle(tr.CellStyle, curstyle, true, true);

            return tr;
        }

        /// <summary>
        /// Calculates maximum height of grid row with given columns widths.
        /// </summary>
        /// <param name="row">Grid row number.</param>
        /// <param name="curstyle">Current grid cell style.</param>
        /// <returns>The height in document units.</returns>
        private int CalcRowMaxHeight(int row, CellStyle curstyle)
        {
            int maxHeight = 0;
            int iCol = -1;
            foreach (Column col in this.Cols)
            {
                iCol++;
                if (!_printInfo.ShowHiddenCols && !col.Visible)
                    continue;

                int measurew = (int)(GetColWidth(col) / _zoom);
                string str = this.GetDataDisplay(row, iCol);
                int curHeight = MeasureText(str, curstyle.Font, measurew) + 2;
                if (curHeight > maxHeight)
                    maxHeight = curHeight;
            }
            return PxToDoc(maxHeight);
        }

        /// <summary>
        /// Calculates zoom factor for fit all columns at one page.
        /// </summary>
        private void GetZoom()
        {
            // calculate scale factor: 
            // increase the size of the rectangle to make pages fit
            _zoom = 1.0;

            // get row/col count, bail if 0
            int cols = this.Cols.Count;
            int rows = this.Rows.Count;
            if (cols == 0 || rows == 0) return;

            // fit to page width: don't spill horizontally 
            if (_printInfo.PageBreak == PageBreaksEnum.FitIntoArea)
            {
                int gridwidth = 0;
                foreach (Column col in this.Cols)
                {
                    if (!_printInfo.ShowHiddenCols && !col.Visible)
                        continue;

                    gridwidth += GetColWidth(col);
                }
                double wid = PxToDoc(gridwidth);
                _zoom = wid / _maxWidth;
            }
        }

        #endregion

        #region Private support methods

        private PaperSize GetCurrentLocaleDefaultPaperSize()
        {
            C1.C1Preview.C1PageSettings c1ps = C1.C1Preview.C1PageSettings.CreateDefaultPageSettings();
            return c1ps.ToPageSettings().PaperSize;
        }

        /// <summary>
        /// Measures text height in pixels.
        /// </summary>
        /// <param name="s">Measured text.</param>
        /// <param name="f">Applied font.</param>
        /// <param name="w">Available width.</param>
        /// <returns></returns>
        private int MeasureText(string s, Font f, int w)
        {
            int res = 0;
            SizeF sAvail = new SizeF(w, 0f);  //(DocToPx(w), 0f);
            SizeF sNeeded = _measureGraphics.MeasureString(s, f, sAvail, _sf);
            res = (int)(sNeeded.Height) + 2;  //Padding 1 pix
            return res;
        }

        /// <summary>
        /// Converts screen pixels to Document units (1/300 of inch)
        /// </summary>
        /// <param name="lengthInPixel">Length in screen pixels</param>
        /// <returns></returns>
        private int PxToDoc(double lengthInPixel)
        {
            return (int)(Math.Round(Utils.ConvertUnits(lengthInPixel, UnitTypeEnum.Pixel, UnitTypeEnum.Document, _dpiScreen, 0)));
        }

        /// <summary>
        /// Converts hundreds of inch to Document units (1/300 of inch)
        /// </summary>
        /// <param name="lengthInInch">Length in hundreds of inch</param>
        /// <returns></returns>
        private int InchToDoc(int lengthInInch)
        {
            return (int)(Math.Round(Utils.ConvertUnits(lengthInInch, UnitTypeEnum.InHs, UnitTypeEnum.Document, 0, 0)));
        }

        /// <summary>
        /// Converts document units to pixels.
        /// </summary>
        /// <param name="lenghtInDoc">Length in document units.</param>
        /// <returns></returns>
        private int DocToPx(int lenghtInDoc)
        {
            return (int)(Math.Round(Utils.ConvertUnits(lenghtInDoc, UnitTypeEnum.Document, UnitTypeEnum.Pixel, 0, _dpiScreen)));
        }

        /// <summary>
        /// Returns given font in points.
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        private Font GetFont(Font f)
        {
            if (f != null)
            {
                if (f.Unit == GraphicsUnit.Point)
                    return f;
                return new Font(f.FontFamily.Name, f.SizeInPoints, GraphicsUnit.Point);
            }
            return null;
        }

        /// <summary>
        /// Compares font with parent style font.
        /// </summary>
        /// <param name="f">Tested font.</param>
        /// <param name="parentStyle">Parent preview style</param>
        /// <returns></returns>
        private bool CheckFont(Font f, C1.C1Preview.Style parentStyle)
        {
            if (f == null)
                return false;
            Font df = parentStyle.Font;
            if (df != null)
                return !df.Equals(f);
            return true;
        }

        /// <summary>
        /// Returns column width in pixels not greater then page width.
        /// </summary>
        /// <param name="col">Tested column.</param>
        /// <returns></returns>
        private int GetColWidth(Column col)
        {
            return (col.WidthDisplay <= _maxWidthPix) ? col.WidthDisplay : _maxWidthPix;
        }

        /// <summary>
        /// Creates a RenderText object for printing page header-footer.
        /// </summary>
        /// <param name="txt">Printing text.</param>
        /// <returns></returns>
        private RenderText CreatePageRenderText(string txt)
        {
            RenderText mytext1 = new RenderText(_doc);
            mytext1.Text = ReplacePageText(txt);
            mytext1.Width = new Unit("100%parent");
            mytext1.Height = new Unit("100%parent");
            mytext1.Style.TextAlignVert = AlignVertEnum.Center;
            mytext1.X = 0;
            mytext1.Y = 0;
            return mytext1;
        }

        /// <summary>
        /// Replaces tags for page number with preview style tags.
        /// </summary>
        /// <param name="text">Tested text.</param>
        /// <returns></returns>
        internal static string ReplacePageText(string text)
        {
            if (text.Length > 0)
            {
                text = text.Replace(c_tag_p, "[PageNo]");
                text = text.Replace(c_tag_P, "[PageCount]");
                text = text.Replace(c_tag_s, "[PageX]");
                text = text.Replace(c_tag_S, "[PageXCount]");
                text = text.Replace(c_tag_g, "[PageY]");
                text = text.Replace(c_tag_G, "[PageYCount]");
            }
            return text;
        }

        /// <summary>
        /// Separates text to parts using delimiter.
        /// </summary>
        /// <param name="s">Separated text.</param>
        /// <returns></returns>
        private static string[] SplitLine(string s) // splits header or footer text
        {
            string[] headerText = new string[3] { string.Empty, string.Empty, string.Empty };
            int i = s.IndexOf(c_tag_t);
            if (i >= 0)
            {
                if (i > 0)
                    headerText[0] = s.Substring(0, i);
                s = s.Substring(i + c_tag_t.Length);
                i = s.IndexOf(c_tag_t);
                if (i >= 0)
                {
                    if (i > 0)
                        headerText[1] = s.Substring(0, i);
                    headerText[2] = s.Substring(i + c_tag_t.Length);
                }
                else
                    headerText[1] = s;
            }
            else
                headerText[0] = s;
            return headerText;
        }

        private bool IsFlagSet(TreeStyleFlags f)
        {
            return (this.Tree.Style & f) != 0;
        }

        /// <summary>
        /// Returns rectangle to draw collapse/expand symbol with indent.
        /// </summary>
        /// <param name="rc">Base rectangle.</param>
        /// <param name="level">Tree level.</param>
        /// <returns></returns>
        private Rectangle GetSymbolRect(Rectangle rc, int level)
        {
            Rectangle rcSymbol = new Rectangle(0, 0, 0, 0);
            // get node level, no button if < 0
            if (level < 0) return rcSymbol;

            int indent = this.Tree.Indent;
            rcSymbol.X = rc.X + level * indent + (indent - _imgSize.Width) / 2;
            rcSymbol.Y = rc.Y + (rc.Height - _imgSize.Height) / 2;
            rcSymbol.Width = _imgSize.Width;
            rcSymbol.Height = _imgSize.Height;

            rcSymbol.Intersect(rc);
            return rcSymbol;
        }

        /// <summary>
        /// Gets service image for row.
        /// </summary>
        /// <param name="row">Grid row number.</param>
        /// <returns></returns>
        private Image GetButtonImage(int row)
        {
            // check this row
            Row thisRow = this.Rows[row];
            if (!thisRow.IsNode) return null;

            // check next row
            Row nextRow = null;
            bool above = (this.SubtotalPosition == SubtotalPositionEnum.AboveData);
            if (above && row < this.Rows.Count - 1) nextRow = this.Rows[row + 1];
            if (!above && row > this.Rows.Fixed) nextRow = this.Rows[row - 1];

            // no data/child nodes? no image
            if (nextRow == null) return null;
            if (nextRow.IsNode && nextRow.Node.Level <= thisRow.Node.Level) return null;

            // return collapse/expanded image
            bool collapsed = this.Rows[row].Node.Collapsed;
            return this.Glyphs[collapsed ? GlyphEnum.Collapsed : GlyphEnum.Expanded];
        }

        /// <summary>
        /// Gets the width of grid tree.
        /// </summary>
        /// <param name="col"></param>
        /// <returns></returns>
        private int TreeWidth(int col)
        {
            if (this.Tree.Column != col) return 0;
            if (this.Tree.Style == TreeStyleFlags.None) return 0;
            int maxLevel = this.Tree.MaximumLevel;
            if (maxLevel < 0) return 0;
            return this.Tree.Indent * (maxLevel + 1);
        }

        /// <summary>
        /// Returns check image depended of given flag.
        /// </summary>
        /// <param name="chk"></param>
        /// <returns></returns>
        private Image GetCheckImage(CheckEnum chk)
        {
            switch (chk)
            {
                case CheckEnum.Checked:
                case CheckEnum.TSChecked:
                    return this.Glyphs[GlyphEnum.Checked];
                case CheckEnum.Unchecked:
                case CheckEnum.TSUnchecked:
                    return this.Glyphs[GlyphEnum.Unchecked];
                case CheckEnum.TSGrayed:
                    return this.Glyphs[GlyphEnum.Grayed];
            }
            return null;
        }

        private bool IsNumeric(string s)
        {
            // handle empty strings
            if (s == null || s.Length == 0) return false;

            // handle percentages
            if (s.EndsWith("%")) s = s.Substring(0, s.Length - 1);

            // use TryParse to check whether this is a number
            double value;
            return double.TryParse(s, System.Globalization.NumberStyles.Any, null, out value);
        }

        #endregion

        #region Events

        /// <summary>
        /// Occurs prior to printing.
        /// </summary>
        public event CancelEventHandler StartPrinting;
        /// <summary>
        /// Raises the StartPrint event.
        /// </summary>
        protected void OnStartPrinting()
        {
            if (StartPrinting != null)
            {
                CancelEventArgs e = new CancelEventArgs();
                StartPrinting(this, e);
                if (e.Cancel)
                {
                    _terminated = true;
                    OnEndPrinting();
                }
            }
        }

        /// <summary>
        /// Occurs after a row has been printed.
        /// </summary>
        public event CancelEventHandler AfterRowPrinted;
        /// <summary>
        /// Raises the AfterRowPrinted event.
        /// </summary>
        protected void OnAfterRowPrinted()
        {
            if (AfterRowPrinted != null)
            {
                CancelEventArgs e = new CancelEventArgs();
                AfterRowPrinted(this, e);
                if (e.Cancel)
                {
                    _terminated = true;
                    OnEndPrinting();
                }
            }
        }

        /// <summary>
        /// Occurs after the grid has been printed.
        /// </summary>
        public event CancelEventHandler EndPrinting;
        /// <summary>
        /// Raises the EndPrint event.
        /// </summary>
        protected void OnEndPrinting()
        {
            if (EndPrinting != null)
            {
                CancelEventArgs e = new CancelEventArgs();
                e.Cancel = _terminated;
                EndPrinting(this, e);
            }
            if (_terminated)
                throw new PrintCancelException();
        }

        /// <summary>
        /// Occurs before the page header is to be printed.
        /// </summary>
        public event OwnerDrawPageEventHandler OwnerDrawPageHeader;

        /// <summary>
        /// Raises the <see cref="OwnerDrawPageHeader"/> event.
        /// </summary>
        /// <param name="e">A <see cref="OwnerDrawPageEventArgs"/> that contains the event data.</param>
        protected void OnOwnerDrawPageHeader(OwnerDrawPageEventArgs e)
        {
            if (OwnerDrawPageHeader != null)
                OwnerDrawPageHeader(this, e);
        }

        /// <summary>
        /// Occurs before the page footer is to be printed.
        /// </summary>
        public event OwnerDrawPageEventHandler OwnerDrawPageFooter;

        /// <summary>
        /// Raises the <see cref="OwnerDrawPageFooter"/> event.
        /// </summary>
        /// <param name="e">A <see cref="OwnerDrawPageEventArgs"/> that contains the event data.</param>
        protected void OnOwnerDrawPageFooter(OwnerDrawPageEventArgs e)
        {
            if (OwnerDrawPageFooter != null)
                OwnerDrawPageFooter(this, e);
        }

        #endregion
    }

    /// <summary>
    /// The exception that is thrown when a print is cancelled.
    /// </summary>
    public class PrintCancelException : Exception
    {
        /// <summary>
        /// Creates a new instance of the PrintCancelException class.
        /// </summary>
        public PrintCancelException()
            : base("Cancelled by user")        //base(C1Localizer.GetString("Export.Cancelled"))
        {
        }
    }

    #region Public enums

    /// <summary>
    /// Specifies how empty space is printed.
    /// </summary>
    public enum FillEmptyEnum
    {
        /// <summary>
        /// All columns are extended proportionally.
        /// </summary>
        ExtendAll,      // all columns are extended proportionally (default)
        /// <summary>
        /// Empty space on the right.
        /// </summary>
        None,           // empty space on the right
        /// <summary>
        /// Last column is extended to fill the empty space.
        /// </summary>
        ExtendLastCol,     // last column is extended to fill space
    }

    /// <summary>
    /// Specifies when page breaks are applied.
    /// </summary>
    public enum PageBreaksEnum
    {
        /// <summary>
        /// Fit all columns in one page.
        /// </summary>
        FitIntoArea,    //no any horizontally page breaks, fit all columns in one page (default)
        /// <summary>
        /// Breaks on any column that doesn't fit.
        /// </summary>
        OnColumn,		//page breaks on any non fit column
    }

    /// <summary>
    /// Specifies how cell text is wrapped.
    /// </summary>
    public enum WrapTextEnum
    {
        /// <summary>
        /// Always wrap text in a cell.
        /// </summary>
        Wrap,
        /// <summary>
        /// Never wrap.
        /// </summary>
        NoWrap,
        /// <summary>
        /// Wrap or not depending on the value of the grid column's WrapText property.
        /// </summary>
        LikeCell,
    }

    /// <summary>
    /// Specifies the height of printed rows.
    /// </summary>
    public enum RowHeightEnum
    {
        /// <summary>
        /// Stretch the row height to fit all the data.
        /// </summary>
        StretchToFit,
        /// <summary>
        /// Use row height specified in the grid.
        /// </summary>
        LikeGrid,
        /// <summary>
        /// Stretch the row height no greater than <see cref="PrintInfo.MaxRowHeight"/>.
        /// </summary>
        StretchToMax,   //stretch row as needed but not greater when max height
    }
    
    /// <summary>
    /// Specifies allowable options for printing.
    /// </summary>
    [Flags]
    public enum ActionFlags
    {
        /// <summary>
        /// No options.
        /// </summary>
        None = 0x00,
        /// <summary>
        /// Print
        /// </summary>
        Print = 0x01,
        /// <summary>
        /// Preview
        /// </summary>
        Preview = 0x02,
        /// <summary>
        /// Export
        /// </summary>
        Export = 0x04,
        /// <summary>
        /// All
        /// </summary>
        MaskAll = (-1),
    }
    #endregion

    /// <summary>
    /// The object used to specify the characteristics of a grid when it is printed.
    /// </summary>
    public class PrintInfo
    {
        #region Private instance members
        private string _pageHeader = "";
        private string _pageFooter = "";
        private C1.Win.C1FlexGrid.CellStyle _pageHeaderStyle = null;
        private C1.Win.C1FlexGrid.CellStyle _pageFooterStyle = null;
        private WrapTextEnum _wrapText = WrapTextEnum.Wrap;
        private bool _useGridColors = true;
        private int _pageHeaderHeight = 30;
        private int _pageFooterHeight = 30;
        private bool _useOwnerDrawPageHeader = false;
        private bool _useOwnerDrawPageFooter = false;
        private PageSettings _pageSettings = null;
        private bool _showOptionsDialog = false; //true;
        private bool _showProgressForm = true;
        private FillEmptyEnum _fillWidth = FillEmptyEnum.ExtendAll;
        private PageBreaksEnum _pageBreaks = PageBreaksEnum.FitIntoArea;
        private string _progressCaption = string.Empty;
        private bool _repeatFixedCols = true;
        private bool _repeatFixedRows = true;
        private bool _showHiddenRows = false;
        private bool _showHiddenCols = false;
        private RowHeightEnum _varRowHeight = RowHeightEnum.StretchToFit;
        private int _maxRowHeight = 30;
        #endregion

        internal PrintInfo()
        {
            _pageSettings = new PageSettings();
        }

        #region Public properties

        /// <summary>
        /// Gets or sets the value indicating whether to print hidden rows.
        /// </summary>
        [DefaultValue(false)]
        [Description("Gets or sets the value indicating whether to print hidden rows.")]
        public bool ShowHiddenRows
        {
            get { return _showHiddenRows; }
            set { _showHiddenRows = value; }
        }

        /// <summary>
        /// Gets or sets the value indicating whether to print hidden columns.
        /// </summary>
        [Description("Gets or sets the value indicating whether to print hidden columns.")]
        [DefaultValue(false)]
        public bool ShowHiddenCols
        {
            get { return _showHiddenCols; }
            set { _showHiddenCols = value; }
        }

        /// <summary>
        /// Gets or sets the value indicating whether to repeat fixed rows on each page.
        /// </summary>
        [Description("Gets or sets the value indicating whether to repeat fixed rows on each page.")]
        [DefaultValue(true)]
        public bool RepeatFixedRows
        {
            get { return _repeatFixedRows; }
            set { _repeatFixedRows = value; }
        }

        /// <summary>
        /// Gets or sets the value indicating whether to repeat fixed columns on each page.
        /// </summary>
        [Description("Gets or sets the value indicating whether to repeat fixed columns on each page.")]
        [DefaultValue(true)]
        public bool RepeatFixedCols
        {
            get { return _repeatFixedCols; }
            set { _repeatFixedCols = value; }
        }

        /// <summary>
        /// Gets or sets the value indicating whether the grid color scheme is used to print the grid.
        /// </summary>
        [DefaultValue(true)]
        [Localizable(true)]
        [Description("Gets or sets the value indicating whether the grid color scheme is used to print the grid.")]
        public bool UseGridColors
        {
            get { return this._useGridColors; }
            set { this._useGridColors = value; }
        }
        
        /// <summary>
        /// Gets or sets the value indicating whether word wrap is on for text in the grid cells.
        /// </summary>
        [DefaultValue(WrapTextEnum.Wrap)]
        [Description("Gets or sets the value indicating whether word wrap is on for text in the grid cells.")]
        public WrapTextEnum WrapText
        {
            get { return this._wrapText; }
            set { this._wrapText = value; }
        }

        /// <summary>
        /// Gets or sets the mode of stretching the printout to the page width.
        /// </summary>
        [DefaultValue(FillEmptyEnum.ExtendAll)]
        [Description("Gets or sets the mode of stretching the printout to the page width.")]
        public FillEmptyEnum FillAreaWidth
        {
            get { return this._fillWidth; }
            set { this._fillWidth = value; }
        }

        /// <summary>
        /// Gets or sets the horizontal page break mode.
        /// </summary>
        [DefaultValue(PageBreaksEnum.FitIntoArea)]
        [Description("Gets or sets the horizontal page break mode.")]
        public PageBreaksEnum PageBreak
        {
            get { return this._pageBreaks; }
            set { this._pageBreaks = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the page header is owner drawn.
        /// </summary>
        [DefaultValue(false)]
        [Localizable(true)]
        [Description("Gets or sets a value indicating whether the page header is owner drawn.")]
        public bool IsOwnerDrawPageHeader
        {
            get { return this._useOwnerDrawPageHeader; }
            set { this._useOwnerDrawPageHeader = value; }
        }

        /// <summary>
        /// Gets or sets the header to be printed at the top of each page.
        /// </summary>
        [DefaultValue("")]
        [Localizable(true)]
        [Description("Gets or sets the header to be printed at the top of each page.")]
        public string PageHeader
        {
            get { return this._pageHeader; }
            set { this._pageHeader = value; }
        }

        /// <summary>
        /// Gets or sets the style used to render the page header.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Description("Gets or sets the style used to render the page header.")]
        public C1.Win.C1FlexGrid.CellStyle PageHeaderStyle
        {
            get { return this._pageHeaderStyle; }
            set { this._pageHeaderStyle = value; }
        }

        /// <summary>
        /// Gets or sets the height of the page header, in hundreds of an inch.
        /// </summary>
        [DefaultValue(30)]
        [Localizable(true)]
        [Description("Gets or sets the height of the page header, in hundreds of an inch.")]
        public int PageHeaderHeight
        {
            get { return this._pageHeaderHeight; }
            set { this._pageHeaderHeight = value; }
        }

        /// <summary>
        /// Gets or sets the header to be printed at the bottom of each page.
        /// </summary>
        [DefaultValue("")]
        [Localizable(true)]
        [Description("Gets or sets the header to be printed at the bottom of each page.")]
        public string PageFooter
        {
            get { return this._pageFooter; }
            set { this._pageFooter = value; }
        }

        /// <summary>
        /// Gets or sets the style used to render the page footer.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Description("Gets or sets the style used to render the page footer.")]
        public C1.Win.C1FlexGrid.CellStyle PageFooterStyle
        {
            get { return this._pageFooterStyle; }
            set { this._pageFooterStyle = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the page footer is owner drawn.
        /// </summary>
        [DefaultValue(false)]
        [Localizable(true)]
        [Description("Gets or sets a value indicating whether the page footer is owner drawn.")]
        public bool IsOwnerDrawPageFooter
        {
            get { return this._useOwnerDrawPageFooter; }
            set { this._useOwnerDrawPageFooter = value; }
        }

        /// <summary>
        /// Gets or sets the height of the page footer, in hundreds of an inch.
        /// </summary>
        [DefaultValue(30)]
        [Localizable(true)]
        [Description("Gets or sets the height of the page footer, in hundreds of an inch.")]
        public int PageFooterHeight
        {
            get { return this._pageFooterHeight; }
            set { this._pageFooterHeight = value; }
        }

        /// <summary>
        /// Gets or sets the maximum row height, in hundredths of an inch.
        /// </summary>
        [DefaultValue(30)]
        [Localizable(true)]
        [Description("Gets or sets the maximum row height, in hundredths of an inch.")]
        public int MaxRowHeight
        {
            get { return this._maxRowHeight; }
            set { this._maxRowHeight = value; }
        }

        /// <summary>
        /// Gets or sets the value controlling the heights of rows in the printout.
        /// </summary>
        [DefaultValue(RowHeightEnum.StretchToFit)]
        [Description("Gets or sets the value controlling the heights of rows in the printout.")]
        public RowHeightEnum VarRowHeight
        {
            get { return this._varRowHeight; }
            set { this._varRowHeight = value; }
        }
        
        /// <summary>
        /// Gets or sets the value indicating whether the options dialog is
        /// displayed when the grid is about to be printed or previewed.
        /// </summary>
        [DefaultValue(false)]
        [Localizable(true)]
        [Description("Gets or sets the value indicating whether the options dialog is displayed when the grid is about to be printed or previewed.")]
        public bool ShowOptionsDialog
        {
            get { return this._showOptionsDialog; }
            set { this._showOptionsDialog = value; }
        }

        /// <summary>
        /// Gets or sets the value indicating whether the progress form is
        /// shown while the grid is being prepared for printing.
        /// </summary>
        [DefaultValue(true)]
        [Localizable(true)]
        [Description("Gets or sets the value indicating whether the progress form is shown while the grid is being prepared for printing.")]
        public bool ShowProgressForm
        {
            get { return this._showProgressForm; }
            set { this._showProgressForm = value; }
        }

        /// <summary>
        /// Gets or sets the page settings to be used for printing.
        /// </summary>
        [Description("Gets or sets the page settings to be used for printing.")]
        public PageSettings PageSettings
        {
            get { return this._pageSettings; }
            set { _pageSettings = value; }
        }

        /// <summary>
        /// Determines whether the <see cref="PageSettings"/> property should be persisted.
        /// </summary>
        /// <returns>True if the PageSettings property should be persisted, false otherwise.</returns>
        public bool ShouldSerializePageSettings()
        {
            return _pageSettings != null;
        }

        /// <summary>
        /// Gets or sets the caption of the progress form.
        /// </summary>
        [DefaultValue("")]
        [Localizable(true)]
        [Description("Gets or sets the caption of the progress form.")]
        public string ProgressCaption
        {
            get { return this._progressCaption; }
            set { this._progressCaption = value; }
        }
        #endregion
    }

    #region PreviewWrapper & ExportParameters

    internal class ExportParameters
    {
        private bool _onePagePerSheet;

        #region Constructors
        public ExportParameters(bool onePagePerSheet)
        {
            OnePagePerSheet = onePagePerSheet;
        }
        #endregion

        #region Public properties
        public bool OnePagePerSheet
        {
            get { return _onePagePerSheet; }
            set { _onePagePerSheet = value; }
        }
        #endregion
    }
    
    /// <summary>
    /// The object used to call PrintPreview methods.
    /// </summary>
    internal class PreviewWrapper
    {
        private static C1.C1Preview.Export.ExportProvider[] _availableProviders = null;
        private static string _exportFilter = string.Empty;
        private C1FlexGridPrintable _info = null;

        public PreviewWrapper(C1FlexGridPrintable info)
        {
            _info = info;
        }

        /// <summary>
        /// Shows Preview window with given documen.
        /// </summary>
        /// <param name="doc"></param>
        public void Preview(C1.C1Preview.C1PrintDocument doc, string title, Icon icon)
        {
            _info.SetProgressText("Rendering document");
            doc.Generate();
            _info.SetProgressText("");

            C1.Win.C1Preview.C1PrintPreviewControl preview = new C1.Win.C1Preview.C1PrintPreviewControl();
            using (PreviewForm f = new PreviewForm(_info))
            {
                preview.Dock = DockStyle.Fill;
                preview.Parent = f;
                preview.Document = doc;
                _info.SetProgressText("");
                f.Text = title;
                f.Icon = icon;
                // show the preview dialog
                f.ShowDialog();
            }
        }

        /// <summary>
        /// Prints given document.
        /// </summary>
        /// <param name="doc"></param>
        public void Print(C1.C1Preview.C1PrintDocument doc)
        {
            _info.SetProgressText("Rendering document");
            doc.Generate();
            _info.SetProgressText("");
            C1.C1Preview.C1PrintManager manager = new C1.C1Preview.C1PrintManager();
            manager.Document = doc;
            manager.Print(null);
        }

        /// <summary>
        /// Exports given document using Exporter.
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="exporter"></param>
        /// <param name="filename"></param>
        public void Export(C1.C1Preview.C1PrintDocument doc, C1.C1Preview.Export.Exporter exporter, string filename)
        {
            _info.SetProgressText("Rendering document");
            doc.Generate();
            _info.SetProgressText("");

            exporter.Document = doc;
            exporter.ShowOptions = false;
            exporter.Export(filename);
        }

        /// <summary>
        /// Returns a string with all export extensions for save dialog.
        /// </summary>
        /// <returns></returns>
        public static string ExportFilter()
        {
            if (_exportFilter == string.Empty)
            {
                if (_availableProviders == null)
                    GetExportProviders();
                for (int i = 0; i < _availableProviders.Length; i++)
                {
                    _exportFilter += _availableProviders[i].FormatName
                    + " (*." + _availableProviders[i].DefaultExtension
                    + ")|*." + _availableProviders[i].DefaultExtension;
                    if (i < _availableProviders.Length - 1)
                        _exportFilter += "|";
                }
            }
            return _exportFilter;
        }

        /// <summary>
        /// Returns a string with all export extensions for save dialog using given export providers.
        /// </summary>
        /// <param name="availableProviders"></param>
        /// <returns></returns>
        public static string ExportFilter(C1.C1Preview.Export.ExportProvider[] availableProviders)
        {
            string filter = string.Empty;
            for (int i = 0; i < availableProviders.Length; i++)
            {
                filter += _availableProviders[i].FormatName
                + " (*." + _availableProviders[i].DefaultExtension
                + ")|*." + _availableProviders[i].DefaultExtension;
                if (i < availableProviders.Length - 1)
                    filter += "|";
            }
            return filter;
        }

        /// <summary>
        /// Returns all available export providers.
        /// </summary>
        /// <returns></returns>
        public static C1.C1Preview.Export.ExportProvider[] GetExportProviders()
        {
            if (_availableProviders == null)
            {
                C1.C1Preview.Export.ExportProviders listproviders = C1.C1Preview.Export.ExportProviders.RegisteredProviders;
                if (listproviders != null)
                {
                    _availableProviders = new C1.C1Preview.Export.ExportProvider[listproviders.Count];
                    for (int i = 0; i < listproviders.Count; i++)
                    {
                        _availableProviders[i] = listproviders[i];
                    }
                }
            }
            return _availableProviders;
        }

        /// <summary>
        /// Returns only selected export providers.
        /// </summary>
        /// <param name="formatExt"></param>
        /// <returns></returns>
        public static C1.C1Preview.Export.ExportProvider[] GetExportProviders(string formatExt)
        {
            if (_availableProviders == null)
                GetExportProviders();
            int j = 0;
            for (int i = 0; i < _availableProviders.Length; i++)
            {
                string provext = _availableProviders[i].DefaultExtension;
                if (formatExt == null || provext.ToLower() == formatExt.ToLower())
                    j++;
            }
            C1.C1Preview.Export.ExportProvider[] res = new C1.C1Preview.Export.ExportProvider[j];
            j = 0;
            for (int i = 0; i < _availableProviders.Length; i++)
            {
                string provext = _availableProviders[i].DefaultExtension;
                if (formatExt == null || provext.ToLower() == formatExt.ToLower())
                    res[j++] = _availableProviders[i];
            }
            return res;
        }
    }

    #endregion
    
    //
    // OwnerDrawPageEventArgs use for the following events:
    //
    //	OwnerDrawPageHeader
    //	OwnerDrawPageFooter
    //

    /// <summary>
    /// Raised when custom page headers and footers need to be rendered.
    /// </summary>
    public delegate void OwnerDrawPageEventHandler(object sender, OwnerDrawPageEventArgs e);

    /// <summary>
    /// Provides data for the <see cref="C1FlexGridPrintable.OwnerDrawPageHeader"/>
    /// and <see cref="C1FlexGridPrintable.OwnerDrawPageHeader"/> events.
    /// </summary>
    public class OwnerDrawPageEventArgs : EventArgs
    {
        private C1OwnerDrawPrint _ownerDrawPrint;
        internal OwnerDrawPageEventArgs(C1OwnerDrawPrint odp)
        {
            _ownerDrawPrint = odp;
        }

        /// <summary>
        /// The <see cref="C1OwnerDrawPrint"/> object used to render the custom header or footer.
        /// </summary>
        public C1OwnerDrawPrint OwnerDrawPrint
        {
            get { return _ownerDrawPrint; }
        }
    }
}

