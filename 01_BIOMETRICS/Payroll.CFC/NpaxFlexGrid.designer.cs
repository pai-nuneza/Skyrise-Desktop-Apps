namespace Payroll.CFC
{
    partial class NpaxFlexGrid
    {
        /// <summary>
        /// 必要なデザイナ変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region コンポーネント デザイナで生成されたコード

        /// <summary>
        /// デザイナ サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディタで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NpaxFlexGrid));
            this.cmsGridCommands = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.menSaveToExcel = new System.Windows.Forms.ToolStripMenuItem();
            this.menPrintGrid = new System.Windows.Forms.ToolStripMenuItem();
            this.menChangeFont = new System.Windows.Forms.ToolStripMenuItem();
            this.cmsGridCommands.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // cmsGridCommands
            // 
            this.cmsGridCommands.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menSaveToExcel,
            this.menPrintGrid,
            this.menChangeFont});
            this.cmsGridCommands.Name = "cmsSaveToExcel";
            this.cmsGridCommands.Size = new System.Drawing.Size(151, 70);
            this.cmsGridCommands.Text = "Grid Commands";
            // 
            // menSaveToExcel
            // 
            this.menSaveToExcel.Name = "menSaveToExcel";
            this.menSaveToExcel.Size = new System.Drawing.Size(150, 22);
            this.menSaveToExcel.Text = "Save to Excel";
            this.menSaveToExcel.Click += new System.EventHandler(this.menSaveToExcel_Click);
            // 
            // menPrintGrid
            // 
            this.menPrintGrid.Name = "menPrintGrid";
            this.menPrintGrid.Size = new System.Drawing.Size(150, 22);
            this.menPrintGrid.Text = "Print Grid";
            this.menPrintGrid.Click += new System.EventHandler(this.menPrintGrid_Click);
            // 
            // menChangeFont
            // 
            this.menChangeFont.Name = "menChangeFont";
            this.menChangeFont.Size = new System.Drawing.Size(150, 22);
            this.menChangeFont.Text = "Change Font";
            this.menChangeFont.Click += new System.EventHandler(this.menChangeFont_Click);
            // 
            // NpaxFlexGrid
            // 
            this.AllowDragging = C1.Win.C1FlexGrid.AllowDraggingEnum.None;
            this.AllowEditing = false;
            this.AllowMerging = C1.Win.C1FlexGrid.AllowMergingEnum.Nodes;
            this.AllowResizing = C1.Win.C1FlexGrid.AllowResizingEnum.Both;
            this.AllowSorting = C1.Win.C1FlexGrid.AllowSortingEnum.None;
            this.AutoClipboard = true;
            this.BackColor = System.Drawing.Color.FloralWhite;
            this.ContextMenuStrip = this.cmsGridCommands;
            this.ExtendLastCol = true;
            this.FocusRect = C1.Win.C1FlexGrid.FocusRectEnum.None;
            this.HighLight = C1.Win.C1FlexGrid.HighLightEnum.WithFocus;
            this.Rows.DefaultSize = 16;
            this.ScrollOptions = C1.Win.C1FlexGrid.ScrollFlags.ScrollByRowColumn;
            this.SelectionMode = C1.Win.C1FlexGrid.SelectionModeEnum.CellRange;
            this.ShowButtons = C1.Win.C1FlexGrid.ShowButtonsEnum.Always;
            this.ShowCellLabels = true;
            this.ShowCursor = true;
            this.ShowSort = false;
            this.ShowThemedHeaders = C1.Win.C1FlexGrid.ShowThemedHeadersEnum.Columns;
            this.StyleInfo = resources.GetString("$this.StyleInfo");
            this.SubtotalPosition = C1.Win.C1FlexGrid.SubtotalPositionEnum.BelowData;
            this.Tree.Column = 0;
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.NpaxFlexGrid_KeyDown);
            this.Resize += new System.EventHandler(this.NpaxFlexGrid_Resize);
            this.AfterDataRefresh += new System.ComponentModel.ListChangedEventHandler(this.NpaxFlexGrid_AfterDataRefresh);
            this.cmsGridCommands.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip cmsGridCommands;
        private System.Windows.Forms.ToolStripMenuItem menSaveToExcel;
        private System.Windows.Forms.ToolStripMenuItem menPrintGrid;
        private System.Windows.Forms.ToolStripMenuItem menChangeFont;

    }
}
