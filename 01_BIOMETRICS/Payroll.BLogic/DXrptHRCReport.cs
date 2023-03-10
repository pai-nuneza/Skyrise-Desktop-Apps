using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraPivotGrid;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.UI.PivotGrid;
using System.Data;
using System.Drawing.Printing;
using DevExpress.XtraPrinting;
using System.Reflection;
using System.Collections.Generic;
using DevExpress.XtraEditors;
using System.Windows.Forms;

namespace Payroll.BLogic
{
    public partial class DXrptHRCReport : DevExpress.XtraReports.UI.XtraReport
    {

        #region variables
        //Datasources
        private DataSet dsSource;
        private DataSet dsSource2;
        private DataSet dsHeader;
        private DataSet dsSignatory;

        //Report objects
        private XRLabel[] lblDetails;
        private XRLabel[] lblList;
        private XRLabel[] lblGroupFooter;
        private XRLabel[] lblGrandtotal;
        private GroupHeaderBand pagegroup;
        private ReportFooterBand reportFooter;
        XRPanel pnlHeader = new XRPanel();
        XRPanel pnlDetails = new XRPanel();
        XRPanel pnlGroupFooter = new XRPanel();

        //Flags, indicators and variables
        private float labelsTotalWidth = 0;
        string orderBy = string.Empty;
        bool GroupByProfiles = false;
        bool GroupByCostcenter = false;
        bool SortCols = false;
        string[] ColumnsNotIncludedInSummary;
        private Payroll.BLogic.HRCReportsBL hrcReportsBL = new Payroll.BLogic.HRCReportsBL();

        //Report layouts
        private int margL = 0;
        private int margR = 0;
        private int margT = 0;
        private int margB = 0;
        private Font font;
        private string repName;

        //Report Type     true if its summary type report
        bool reportType = false;
        bool displayColTotals = true;
        bool wordWrap = false;
        #endregion


        public DXrptHRCReport()
        {
            InitializeComponent();
        }
        //Displaying only header and details
        public DXrptHRCReport(string reportname, DataSet dsheader, DataSet ds, int[] margs, Font reportFont, DataSet dsSig, bool reptype)
        {
            InitializeComponent();
            this.DataSource = ds;
            this.DataMember = ds.Tables[0].TableName;
            dsSource = ds;
            dsHeader = dsheader;
            repName = reportname;
            reportType = reptype;
            this.Name = reportname;
            lblList = new XRLabel[dsSource.Tables[0].Columns.Count];
            lblDetails = new XRLabel[dsSource.Tables[0].Columns.Count];
            margL = margs[0];
            margR = margs[1];
            margT = margs[2];
            margT = margs[3];
            dsSignatory = dsSig;
            font = reportFont;
            initializeBands();
            initializeHeader();
            InitializeheaderAndDetails();
            reinitializePage();
        }



        //Displaying header, details and totals
        public DXrptHRCReport(string reportname, DataSet dsheader, DataSet ds, int[] margs, Font reportFont, DataSet dsSig, bool groupbyprof, bool groupbycost, string orderby, string[] cols, bool sortAsc)
        {
            InitializeComponent();
            dsSource2 = ds.Copy();
            if (groupbycost)
            {
                ds.Tables[0].Columns.RemoveAt(3);
                ds.Tables[0].Columns.RemoveAt(3);
            }
            if (groupbyprof)
            {
                ds.Tables[0].Columns.RemoveAt(0);
            }
            this.DataSource = dsSource2;
            dsSource = ds;
            dsHeader = dsheader;
            orderBy = orderby;
            GroupByCostcenter = groupbycost;
            GroupByProfiles = groupbyprof;
            ColumnsNotIncludedInSummary = cols;
            repName = reportname;
            this.Name = reportname;
            SortCols = sortAsc;
            lblList = new XRLabel[dsSource.Tables[0].Columns.Count];
            lblDetails = new XRLabel[dsSource.Tables[0].Columns.Count];
            lblGroupFooter = new XRLabel[dsSource.Tables[0].Columns.Count];
            lblGrandtotal = new XRLabel[dsSource.Tables[0].Columns.Count];
            margL = margs[0];
            margR = margs[1];
            margT = margs[2];
            margT = margs[3];
            dsSignatory = dsSig;
            font = reportFont;
            initializeBands();
            initializeHeader();
            InitializeheaderAndDetails2();
            reinitializePage();
        }

        //Displaying only header and details and totals
        //Can hide column total
        public DXrptHRCReport(string reportname, DataSet dsheader, DataSet ds, int[] margs, Font reportFont, DataSet dsSig, bool reptype, bool sortType, bool displaycoltotal, bool groupCost)
        {
            InitializeComponent();
            dsSource2 = ds.Copy();
            
            //Group by profiles
            ds.Tables[0].Columns.RemoveAt(0);

            if (groupCost)
            {
                ds.Tables[0].Columns.RemoveAt(0);
                ds.Tables[0].Columns.RemoveAt(0);
            }

            GroupByCostcenter = groupCost;
            GroupByProfiles = true;

            this.DataSource = dsSource2;
            dsSource = ds;
            dsHeader = dsheader;
            repName = reportname;
            reportType = reptype;
            displayColTotals = displaycoltotal;
            this.Name = reportname;
            lblList = new XRLabel[dsSource.Tables[0].Columns.Count];
            lblDetails = new XRLabel[dsSource.Tables[0].Columns.Count];
            lblGroupFooter = new XRLabel[dsSource.Tables[0].Columns.Count];
            lblGrandtotal = new XRLabel[dsSource.Tables[0].Columns.Count];
            margL = margs[0];
            margR = margs[1];
            margT = margs[2];
            margT = margs[3];
            dsSignatory = dsSig;
            font = reportFont;
            SortCols = sortType;
            ColumnsNotIncludedInSummary = hrcReportsBL.GetColumnsNotIncludedInSummary();

            initializeHeader();
            initializeBands2();
            InitializeheaderAndDetails2();
            reinitializePage();
        }



        private void initializeHeader()
        {
            compName.Text = dsHeader.Tables[0].Rows[0][0].ToString();
            compAdd.Text = dsHeader.Tables[0].Rows[0][1].ToString();
            compTel.Text = dsHeader.Tables[0].Rows[0][2].ToString();
            compAdd.WidthF = compName.WidthF = compTel.WidthF = 750;
            
            reportLabel.Text = repName;
        }

        private void initializeBands()
        {
            this.SuspendLayout();

            this.Margins.Left = margL;
            this.Margins.Right = margR;
            this.Margins.Top = margT;
            this.Margins.Bottom = margB;

            this.Detail.Band.HeightF = 20;
            this.Detail.KeepTogether = true;

            pagegroup = new GroupHeaderBand();
            pagegroup.HeightF = 20;
            pagegroup.RepeatEveryPage = true;
            pagegroup.KeepTogether = true;
            this.Bands.Add(pagegroup);
            

            this.ResumeLayout();
        }

        private void initializeBands2()
        {
            this.SuspendLayout();

            this.Margins.Left = margL;
            this.Margins.Right = margR;
            this.Margins.Top = margT;
            this.Margins.Bottom = margB;

            this.Detail.Band.HeightF = 20;
            this.Detail.KeepTogether = true;
            if (SortCols)
                this.Detail.SortFields.Add(new GroupField(dsSource.Tables[0].Columns[0].Caption, XRColumnSortOrder.Ascending));
            else
                this.Detail.SortFields.Add(new GroupField(dsSource.Tables[0].Columns[0].Caption, XRColumnSortOrder.Descending));


            pagegroup = new GroupHeaderBand();
            pagegroup.HeightF = 20;
            pagegroup.RepeatEveryPage = true;
            pagegroup.KeepTogether = true;
            this.Bands.Add(pagegroup);


            this.ResumeLayout();
        }

        private void InitializeheaderAndDetails()
        {
            CalculatedField cl = SetCalculatedFields();
            int colCount = dsSource.Tables[0].Columns.Count;
            float colWidth = (this.PageWidth - (this.Margins.Left + this.Margins.Right)) / colCount;
            float[] colMaxWidths = getMaxWidths(font);

            float HeaderLabelHeight = 20;
            if (wordWrap)
            {
                HeaderLabelHeight = 40;
            }

            pnlHeader.WidthF = labelsTotalWidth;
            pnlHeader.HeightF = HeaderLabelHeight;
            pnlHeader.CanGrow = true;
            
            pnlDetails.WidthF = labelsTotalWidth;
            pnlDetails.HeightF = 20;
            pnlDetails.CanGrow = true;
            float trail = 0;
            float trail2 = 0;
            float trail3 = 30;

            string[] nontotallables = hrcReportsBL.GetColumnsNotIncludedInSummary();

            bool grandtotalIndicator = false;
            if (reportType)
            {
                reportFooter = new ReportFooterBand();
                reportFooter.KeepTogether = true;
                this.Bands.Add(reportFooter);
                pnlGroupFooter.WidthF = labelsTotalWidth;
                pnlGroupFooter.CanGrow = true;
                
            }

            for (int i = 0; i < dsSource.Tables[0].Columns.Count; i++)
            {
                lblList[i] = new XRLabel();
                lblList[i].CanGrow = false;
                lblList[i].Text = dsSource.Tables[0].Columns[i].Caption;
                lblList[i].WidthF = colMaxWidths[i];
                lblList[i].HeightF = HeaderLabelHeight;
                lblList[i].Font = font;
                lblList[i].Padding = new PaddingInfo(4, 4, 0, 0);
                lblList[i].TextAlignment = TextAlignment.MiddleLeft;
                lblList[i].LocationF = new PointF(trail, trail2);
                if (wordWrap)
                    lblList[i].WordWrap = true;
                
                if (i == 0)
                {
                    lblList[i].Borders = BorderSide.All;
                }
                else
                {
                    lblList[i].Borders = BorderSide.Top | BorderSide.Right | BorderSide.Bottom;
                }
                pnlHeader.Controls.Add(lblList[i]);
                //this.Bands[BandKind.PageHeader].Controls.Add(lblList[i]);

                lblDetails[i] = new XRLabel();
                lblDetails[i].DataBindings.Add("Text", null, dsSource.Tables[0].Columns[i].Caption);
                lblDetails[i].CanGrow = false;
                lblDetails[i].WidthF = colMaxWidths[i];
                lblDetails[i].HeightF = 20;
                lblDetails[i].Padding = new PaddingInfo(4, 4, 0, 0);
                lblDetails[i].TextAlignment = TextAlignment.MiddleLeft;
                //for (int j = 0; j < colAlignRight.Length; j++)
                //{
                //    if (colAlignRight[j] == dsSource.Tables[0].Columns[i].Caption)
                //    {
                //        lblDetails[i].TextAlignment = TextAlignment.MiddleRight;
                //    }
                //}
                if (dsSource.Tables[0].Columns[i].Caption != "Costcenter" &&
                     dsSource.Tables[0].Columns[i].Caption != "IDNumber" &&
                     dsSource.Tables[0].Columns[i].Caption != "Payperiod" &&
                     dsSource.Tables[0].Columns[i].Caption != "ThisPayperiod" )
                {
                    lblDetails[i].BeforePrint += new PrintEventHandler(lblDetails_BeforePrint);
                }
                lblDetails[i].LocationF = new PointF(trail, 0);
                lblDetails[i].Font = font;

                if (i == 0)
                {
                    lblDetails[i].Borders = BorderSide.Left | BorderSide.Bottom | BorderSide.Right;
                }
                else
                {
                    lblDetails[i].Borders = BorderSide.Right | BorderSide.Bottom;
                }
                pnlDetails.Controls.Add(lblDetails[i]);

                if (reportType)
                {
                    XRLabel lbl = new XRLabel();
                    lbl.HeightF = 20;
                    lbl.WidthF = colMaxWidths[i];
                    lbl.TextAlignment = TextAlignment.MiddleRight;
                    lbl.Font = font;
                    lbl.Text = "";
                    lbl.Padding = new PaddingInfo(4, 4, 0, 0);
                    int j = 0;
                    if (i == 0)
                    {
                        lbl.Text = "TOTALS";
                        lbl.LocationF = new PointF(trail, trail2);
                        pnlGroupFooter.Controls.Add(lbl);
                        lbl.Borders = BorderSide.Left | BorderSide.Bottom;
                        lbl.TextAlignment = TextAlignment.MiddleLeft;
                    }
                    else
                    {
                        for (; j < nontotallables.Length && nontotallables[j] != dsSource.Tables[0].Columns[i].Caption; j++) ;
                        if (j == nontotallables.Length)
                        {
                            lbl.DataBindings.Add(new XRBinding("Text", null, dsSource.Tables[0].Columns[i].Caption));
                            XRSummary sum = new XRSummary();
                            sum.Func = SummaryFunc.Sum;
                            sum.Running = SummaryRunning.Report;
                            sum.FormatString = "{0:#,##0.00}";
                            sum.IgnoreNullValues = true;
                            lbl.Summary = sum;
                        }
                        if (!grandtotalIndicator)
                        {
                            lbl.LocationF = new PointF(trail - 1, trail2);
                            lbl.WidthF = colMaxWidths[i] + 1;
                            grandtotalIndicator = true;
                            lbl.Borders = BorderSide.Left | BorderSide.Right | BorderSide.Bottom;
                        }
                        else
                        {
                            lbl.LocationF = new PointF(trail, trail2);
                            lbl.Borders =  BorderSide.Right | BorderSide.Bottom;
                        }
                        pnlGroupFooter.Controls.Add(lbl);
                    }
                }

                trail += colMaxWidths[i];
            }

            if (reportType && displayColTotals && cl.Expression != string.Empty)
            {
                if (cl.Expression != string.Empty)
                {
                    pnlHeader.WidthF += 100;
                    pnlDetails.WidthF += 100;
                    pnlGroupFooter.WidthF += 100;
                    XRLabel lbl = new XRLabel();
                    lbl.Text = "TOTALS";
                    lbl.Font = font;
                    lbl.Padding = new PaddingInfo(4, 4, 0, 0);
                    lbl.WidthF = 100;
                    lbl.HeightF = 20;
                    lbl.Borders = BorderSide.Top | BorderSide.Right | BorderSide.Bottom;
                    lbl.LocationF = new PointF(labelsTotalWidth, trail2);
                    lbl.TextAlignment = TextAlignment.MiddleLeft;
                    pnlHeader.Controls.Add(lbl);

                    XRLabel lbl2 = new XRLabel();
                    lbl2.DataBindings.Add("Text", null, "Order Details.myField");
                    lbl2.Font = font;
                    lbl2.Padding = new PaddingInfo(4, 4, 0, 0);
                    lbl2.WidthF = 100;
                    lbl2.BeforePrint += new PrintEventHandler(lblDetails_BeforePrint);
                    lbl2.HeightF = 20;
                    lbl2.Borders = BorderSide.Right | BorderSide.Bottom;
                    lbl2.LocationF = new PointF(labelsTotalWidth, trail2);
                    lbl2.TextAlignment = TextAlignment.MiddleRight;
                    pnlDetails.Controls.Add(lbl2);

                    XRLabel lbl3 = new XRLabel();
                    lbl3.DataBindings.Add("Text", null, "Order Details.myField");
                    lbl3.Font = font;
                    lbl3.Padding = new PaddingInfo(4, 4, 0, 0);
                    lbl3.WidthF = 100;
                    XRSummary sum = new XRSummary();
                    sum.Func = SummaryFunc.Sum;
                    sum.Running = SummaryRunning.Report;
                    sum.FormatString = "{0:#,##0.00}";
                    sum.IgnoreNullValues = true;
                    lbl3.Summary = sum;
                    lbl3.BeforePrint += new PrintEventHandler(lblDetails_BeforePrint);
                    lbl3.HeightF = 20;
                    lbl3.Borders = BorderSide.Right | BorderSide.Bottom;
                    lbl3.LocationF = new PointF(labelsTotalWidth, trail2);
                    lbl3.TextAlignment = TextAlignment.MiddleRight;
                    lbl3.BeforePrint += new PrintEventHandler(lblGroupFooter_BeforePrint);
                    pnlGroupFooter.Controls.Add(lbl3);

                    
                    
                    labelsTotalWidth += 100;
                }
            }

            

            this.Bands[BandKind.GroupHeader].Controls.Add(pnlHeader);
            this.Bands[BandKind.Detail].Controls.Add(pnlDetails);
            if (reportType)
            {
                this.Bands[BandKind.ReportFooter].Controls.Add(pnlGroupFooter);
            }
        }

        private void InitializeheaderAndDetails2()
        {

            int colCount = dsSource.Tables[0].Columns.Count;
            float colWidth = (this.PageWidth - (this.Margins.Left + this.Margins.Right)) / colCount;

            float[] colMaxWidths = getMaxWidths(font);

            pnlHeader.WidthF = labelsTotalWidth;
            pnlHeader.HeightF = 20;
            pnlHeader.CanGrow = true;

            pnlDetails.WidthF = labelsTotalWidth;
            pnlDetails.HeightF = 20;
            pnlDetails.CanGrow = true;
            float trail = 0;
            float trail2 = 0;

            GroupFooterBand groupFooter = new GroupFooterBand();
            groupFooter.HeightF = 20;
            groupFooter.KeepTogether = true;
            this.Bands.Add(groupFooter);

            pnlGroupFooter.WidthF = labelsTotalWidth;
            pnlGroupFooter.HeightF = 20;
            pnlGroupFooter.CanGrow = true;
            pnlGroupFooter.LocationF = new PointF(0, 0);
            

            float trail3 = 10;
            if (GroupByProfiles)
            {
                XRLabel lbl1 = new XRLabel();
                lbl1.Text = "PROFILE - ";
                lbl1.WidthF = 70;
                lbl1.HeightF = 20;
                lbl1.LocationF = new PointF(10, trail3);
                lbl1.TextAlignment = TextAlignment.MiddleLeft;
                lbl1.Font = new Font("Times new Roman", 9, FontStyle.Bold);
                this.pnlHeader.Controls.Add(lbl1);

                this.pagegroup.GroupFields.Add(new GroupField("Profiles", XRColumnSortOrder.Ascending));
                pagegroup.HeightF = trail3 + 20;
                pnlHeader.HeightF = trail3 + 30;
                XRLabel label = new XRLabel();
                label.HeightF = 20;
                label.Font = new Font("Times new Roman", 9, FontStyle.Bold);
                label.TextAlignment = TextAlignment.MiddleLeft;
                label.WidthF = 400;
                label.DataBindings.Add(new XRBinding("Text", null, "Profiles"));
                label.LocationF = new PointF(80, trail3);
                trail3 += 20;
                trail2 = label.LocationF.Y + label.HeightF;
                this.pnlHeader.Controls.Add(label);
            }
            if (GroupByCostcenter)
            {
                XRLabel lbl1 = new XRLabel();
                lbl1.Text = "COSTCENTER - ";
                lbl1.WidthF = 100;
                lbl1.HeightF = 20;
                lbl1.LocationF = new PointF(10, trail3);
                lbl1.TextAlignment = TextAlignment.MiddleLeft;
                lbl1.Font = new Font("Times new Roman", 9, FontStyle.Bold);
                this.pnlHeader.Controls.Add(lbl1);

                this.pagegroup.GroupFields.Add(new GroupField("Costcenter"));
                pagegroup.HeightF += trail3;
                pnlHeader.HeightF += trail3;
                XRLabel label = new XRLabel();
                label.HeightF = 20;
                label.Font = new Font("Times new Roman", 9, FontStyle.Bold);
                label.TextAlignment = TextAlignment.MiddleLeft;
                label.WidthF = 500;
                label.DataBindings.Add(new XRBinding("Text", null, "Description"));
                label.LocationF = new PointF(110, trail3);
                trail2 = label.LocationF.Y + label.HeightF;
                this.pnlHeader.Controls.Add(label);

            }
            if (orderBy != string.Empty)
            {
                if (SortCols)
                {
                    if (orderBy == "Costcenter")
                    {
                        this.Detail.SortFields.Add(new GroupField("Costcenter", XRColumnSortOrder.Ascending));
                        this.Detail.SortFields.Add(new GroupField("Employee Name", XRColumnSortOrder.Ascending));
                    }
                    else if (orderBy == "Employee ID")
                    {
                        this.Detail.SortFields.Add(new GroupField("ID Number", XRColumnSortOrder.Ascending));
                    }
                    else if (orderBy == "Employee Name")
                    {
                        this.Detail.SortFields.Add(new GroupField("Employee Name", XRColumnSortOrder.Ascending));
                    }
                }
                else
                {
                    if (orderBy == "Costcenter")
                    {
                        this.Detail.SortFields.Add(new GroupField("Costcenter", XRColumnSortOrder.Descending));
                        this.Detail.SortFields.Add(new GroupField("Employee Name", XRColumnSortOrder.Descending));
                    }
                    else if (orderBy == "Employee ID")
                    {
                        this.Detail.SortFields.Add(new GroupField("ID Number", XRColumnSortOrder.Descending));
                    }
                    else if (orderBy == "Employee Name")
                    {
                        this.Detail.SortFields.Add(new GroupField("Employee Name", XRColumnSortOrder.Descending));
                    }
                }

            }
            bool groupFooterIndicator = false;
            if (GroupByProfiles || GroupByCostcenter)
            {
                XRLabel GroupSummary = new XRLabel();
                GroupSummary.Text = @"SUB TOTAL:  ";
                GroupSummary.TextAlignment = TextAlignment.MiddleRight;
                GroupSummary.Padding = new PaddingInfo(4, 4, 0, 0);
                GroupSummary.HeightF = 20;
                GroupSummary.WidthF = 130;
                GroupSummary.Borders = BorderSide.None;
                GroupSummary.Font = new Font("Times new Roman", 9, FontStyle.Bold);
                GroupSummary.LocationF = new PointF(0, 0);
                pnlGroupFooter.Controls.Add(GroupSummary);
            }

            float trail4 = 20;
            reportFooter = new ReportFooterBand();
            reportFooter.KeepTogether = true;
            this.Bands.Add(reportFooter);

            XRLabel grandTotal = new XRLabel();
            grandTotal.Text = "GRAND TOTAL:   ";
            grandTotal.TextAlignment = TextAlignment.MiddleRight;
            grandTotal.Padding = new PaddingInfo(4, 4, 0, 0);
            grandTotal.HeightF = 20;
            grandTotal.WidthF = 130;
            grandTotal.Borders = BorderSide.None;
            grandTotal.Font = new Font("Times new Roman", 9, FontStyle.Bold);
            grandTotal.LocationF = new PointF(0, trail4);

            XRPanel pnlReportFooter = new XRPanel();
            pnlReportFooter.WidthF = labelsTotalWidth;
            pnlReportFooter.Controls.Add(grandTotal);
            bool GrandtotalIndicator = false;
            for (int i = 0; i < dsSource.Tables[0].Columns.Count; i++)
            {
                lblList[i] = new XRLabel();
                lblList[i].CanGrow = false;
                lblList[i].Text = dsSource.Tables[0].Columns[i].Caption;
                lblList[i].WidthF = colMaxWidths[i];
                lblList[i].HeightF = 20;
                lblList[i].Font = font;
                lblList[i].Padding = new PaddingInfo(4, 4, 0, 0);
                lblList[i].TextAlignment = TextAlignment.MiddleLeft;
                lblList[i].LocationF = new PointF(trail, trail2);

                if (i == 0)
                {
                    lblList[i].Borders = BorderSide.All;
                }
                else
                {
                    lblList[i].Borders = BorderSide.Top | BorderSide.Right | BorderSide.Bottom;
                }
                pnlHeader.Controls.Add(lblList[i]);
                //this.Bands[BandKind.PageHeader].Controls.Add(lblList[i]);

                lblDetails[i] = new XRLabel();
                lblDetails[i].DataBindings.Add("Text", null, dsSource.Tables[0].Columns[i].Caption);
                lblDetails[i].CanGrow = false;
                lblDetails[i].WidthF = colMaxWidths[i];
                lblDetails[i].HeightF = 20;
                lblDetails[i].Padding = new PaddingInfo(4, 4, 0, 0);
                lblDetails[i].TextAlignment = TextAlignment.MiddleLeft;
                //for (int j = 0; j < colAlignRight.Length; j++)
                //{
                //    if (colAlignRight[j] == dsSource.Tables[0].Columns[i].Caption)
                //    {
                //        lblDetails[i].TextAlignment = TextAlignment.MiddleRight;
                //    }
                //}
                if (dsSource.Tables[0].Columns[i].Caption != "Costcenter" &&
                     dsSource.Tables[0].Columns[i].Caption != "IDNumber")
                {
                    lblDetails[i].BeforePrint += new PrintEventHandler(lblDetails_Beforeprint2);
                }
                lblDetails[i].LocationF = new PointF(trail, 0);
                lblDetails[i].Font = font;

                if (i == 0)
                {
                    lblDetails[i].Borders = BorderSide.Left | BorderSide.Bottom | BorderSide.Right;
                }
                else
                {
                    lblDetails[i].Borders = BorderSide.Right | BorderSide.Bottom;
                }
                pnlDetails.Controls.Add(lblDetails[i]);


                if (GroupByProfiles || GroupByCostcenter)
                {
                    lblGroupFooter[i] = new XRLabel();
                    lblGroupFooter[i].Borders = BorderSide.None;
                    lblGroupFooter[i].Text = "";
                    lblGroupFooter[i].WidthF = colMaxWidths[i];
                    lblGroupFooter[i].TextAlignment = TextAlignment.MiddleRight;
                    lblGroupFooter[i].Font = font;
                    int k = 0;
                    for (; k < ColumnsNotIncludedInSummary.Length && ColumnsNotIncludedInSummary[k] != dsSource.Tables[0].Columns[i].Caption; k++) ;
                    if (k == ColumnsNotIncludedInSummary.Length)
                    {
                        lblGroupFooter[i].DataBindings.Add(new XRBinding("Text", null, dsSource.Tables[0].Columns[i].Caption));

                        if (!groupFooterIndicator)
                        {
                            lblGroupFooter[i].Borders = BorderSide.Left | BorderSide.Bottom | BorderSide.Right;
                            lblGroupFooter[i].LocationF = new PointF(trail - 1, 0);
                            lblGroupFooter[i].WidthF = colMaxWidths[i] + 1;
                            groupFooterIndicator = true;
                        }
                        else
                        {
                            lblGroupFooter[i].Borders = BorderSide.Bottom | BorderSide.Right;
                            lblGroupFooter[i].LocationF = new PointF(trail, 0);
                        }
                        XRSummary sum = new XRSummary();
                        sum.Func = SummaryFunc.Sum;
                        sum.Running = SummaryRunning.Group;
                        sum.FormatString = "{0:#,##0.00}";
                        sum.IgnoreNullValues = true;
                        lblGroupFooter[i].Summary = sum;
                        lblGroupFooter[i].Padding = new PaddingInfo(4, 4, 0, 0);
                        lblGroupFooter[i].BeforePrint += new PrintEventHandler(lblGroupFooter_BeforePrint);
                    }

                    pnlGroupFooter.Controls.Add(lblGroupFooter[i]);
                }

                lblGrandtotal[i] = new XRLabel();
                lblGrandtotal[i].Borders = BorderSide.None;
                lblGrandtotal[i].Text = "";
                lblGrandtotal[i].WidthF = colMaxWidths[i];
                lblGrandtotal[i].TextAlignment = TextAlignment.MiddleRight;
                lblGrandtotal[i].Font = font;
                int l = 0;
                for (; l < ColumnsNotIncludedInSummary.Length && ColumnsNotIncludedInSummary[l] != dsSource.Tables[0].Columns[i].Caption; l++) ;
                if (l == ColumnsNotIncludedInSummary.Length)
                {
                    lblGrandtotal[i].DataBindings.Add(new XRBinding("Text", null, dsSource.Tables[0].Columns[i].Caption));

                    if (!GrandtotalIndicator)
                    {
                        lblGrandtotal[i].Borders = BorderSide.Left | BorderSide.Bottom | BorderSide.Right | BorderSide.Top;
                        lblGrandtotal[i].LocationF = new PointF(trail - 1, trail4);
                        lblGrandtotal[i].WidthF = colMaxWidths[i] + 1;
                        GrandtotalIndicator = true;
                    }
                    else
                    {
                        lblGrandtotal[i].Borders = BorderSide.Bottom | BorderSide.Right | BorderSide.Top;
                        lblGrandtotal[i].LocationF = new PointF(trail, trail4);
                    }
                    XRSummary sum1 = new XRSummary();
                    sum1.Func = SummaryFunc.Sum;
                    sum1.Running = SummaryRunning.Report;
                    sum1.FormatString = "{0:#,##0.00}";
                    sum1.IgnoreNullValues = true;
                    lblGrandtotal[i].Summary = sum1;
                    lblGrandtotal[i].Padding = new PaddingInfo(4, 4, 0, 0);
                    lblGrandtotal[i].BeforePrint += new PrintEventHandler(lblGroupFooter_BeforePrint);
                }

                pnlReportFooter.Controls.Add(lblGrandtotal[i]);

                trail += colMaxWidths[i];


            }

            this.Bands[BandKind.GroupHeader].Controls.Add(pnlHeader);
            this.Bands[BandKind.Detail].Controls.Add(pnlDetails);
            if (GroupByProfiles || GroupByCostcenter)
            {
                this.Bands[BandKind.GroupFooter].Controls.Add(pnlGroupFooter);
            }
            this.Bands[BandKind.ReportFooter].Controls.Add(pnlReportFooter);
        }

        void lblGroupFooter_BeforePrint(object sender, PrintEventArgs e)
        {
            //double num;
            //string str = ((XRLabel)sender).Text.Trim();
            //bool result = double.TryParse(str, out num);
            //if (result)
            //{

            //    ((XRLabel)sender).TextAlignment = TextAlignment.MiddleRight;
            //    string s = ((XRLabel)sender).Text.Trim();
            //    if (s != string.Empty)
            //    {
            //        string.Format("{0:###,##0.00}", s);
            //        ((XRLabel)sender).Text = s;
            //    }
            if (((XRLabel)sender).Summary.GetResult().ToString() == "0.00" || ((XRLabel)sender).Summary.GetResult().ToString() == "0")
                {
                    ((XRLabel)sender).Summary.FormatString = " ";
                }
            //}
        }

        void lblDetails_Beforeprint2(object sender, PrintEventArgs e)
        {
            double num;
            string str = ((XRLabel)sender).Text.Trim();
            bool result = double.TryParse(str, out num);
            if (result)
            {

                ((XRLabel)sender).TextAlignment = TextAlignment.MiddleRight;
                string s = ((XRLabel)sender).Text.Trim();
                if (s != string.Empty)
                {
                    s = string.Format("{0:#,##0.00}", s);
                    ((XRLabel)sender).Text = s;
                }
                if (((XRLabel)sender).Text == "0.00" || ((XRLabel)sender).Text == "0")
                {
                    ((XRLabel)sender).Text = "";
                }
            }
        }

        void lblDetails_BeforePrint(object sender, PrintEventArgs e)
        {
            double num;
            string str = ((XRLabel)sender).Text.Trim();
            bool result = double.TryParse(str, out num);
            if (result)
            {
                ((XRLabel)sender).TextAlignment = TextAlignment.MiddleRight;
                string s = ((XRLabel)sender).Text.Trim();
                if (s != string.Empty)
                {
                    s = string.Format("{0:#,##0.00}", s);
                    ((XRLabel)sender).Text = s;
                }
            }
        }

        private void reinitializePage()
        {
            this.SuspendLayout();
            if (true)
            {

                this.PaperKind = PaperKind.Custom;
                this.Landscape = true;
                if(((Convert.ToInt32(labelsTotalWidth)) + margL + margR) > 950)
                    this.PageWidth = Convert.ToInt32(labelsTotalWidth) + margL + margR;
                else
                    this.PageWidth = 950;

                this.Margins.Right = margR;
                this.Margins.Left = margL;
                this.Margins.Top = margT;
                this.Margins.Bottom = margB;
            }


            #region reserve for specific pagekind
            /*
            else if (labelsTotalWidth < (827 - (margR + margL)))
                {
                    this.PaperKind = PaperKind.A4;
                    this.Landscape = false;
                }
                else if (labelsTotalWidth < (928 - (margR + margL)))
                {
                    this.PaperKind = PaperKind.A4Extra;
                    this.Landscape = false;
                }
                else if (labelsTotalWidth < (1169 - (margR + margL)))
                {
                    this.PaperKind = PaperKind.A4;
                    this.Landscape = true;
                }
                else if (labelsTotalWidth < (1268 - (margR + margL)))
                {
                    this.PaperKind = PaperKind.A4Extra;
                    this.Landscape = true;
                }
                else if (labelsTotalWidth < (1299 - (margR + margL)))
                {
                    this.PaperKind = PaperKind.A4Plus;
                    this.Landscape = true;
                }
                else if (labelsTotalWidth < (1654 - (margR + margL)))
                {
                    this.PaperKind = PaperKind.A3;
                    this.Landscape = true;
                }
                else if (labelsTotalWidth < (1752 - (margR + margL)))
                {
                    this.PaperKind = PaperKind.A3Extra;
                    this.Landscape = true;
                }
                else
                {
                    this.PaperKind = PaperKind.A2;
                    this.Landscape = true;
                }
            */
            #endregion
            
            
            float centerwidth = (this.PageWidth - (margR + margL)) - (labelsTotalWidth);
            if (centerwidth > 0)
            {
                centerwidth = (centerwidth) / 2;
                pnlHeader.LocationF = new PointF(centerwidth, pnlHeader.LocationF.Y);
                pnlDetails.LocationF = new PointF(centerwidth, pnlDetails.LocationF.Y);
                pnlGroupFooter.LocationF = new PointF(centerwidth, pnlDetails.LocationF.Y);
            }

            xrLine1.WidthF = (this.PageWidth - (margR + margL));

            float loc = ((this.PageWidth - (margR + margL))) / 2;
            loc = loc - (TextRenderer.MeasureText(reportLabel.Text, reportLabel.Font).Width / 2);
            reportLabel.LocationF = new PointF(loc, reportLabel.LocationF.Y);

            loc = (this.PageWidth - (margR + margL + 40)) - (xrLabel2.WidthF + xrPageInfo2.WidthF);
            xrLabel2.LocationF = new PointF(loc, xrLabel2.LocationF.Y);
            xrPageInfo2.LocationF = new PointF(loc + xrLabel2.WidthF, xrPageInfo2.LocationF.Y);
            
            if (this.dsSignatory != null && this.dsSignatory.Tables[0].Rows.Count > 0)
            {
                InitializeReportSigatories();
            }
            this.ResumeLayout();
        }

        private void InitializeReportSigatories()
        {
            float Y = 90;
            if (reportFooter == null)
            {
                reportFooter = new ReportFooterBand();
                reportFooter.KeepTogether = true;
                Y = 70;
            }
            float X = 0;
            
            if (this.dsSignatory.Tables[0].Rows[0]["Alignment"].ToString() == "H")
            {

                string prevSigRole = string.Empty;
                for (int i = 0; i < dsSignatory.Tables[0].Rows.Count; i++)
                {
                    XRLabel lblSigRole = new XRLabel();
                    XRLabel lblName = new XRLabel();
                    XRLabel lblPos = new XRLabel();

                    float width = ( TextRenderer.MeasureText(dsSignatory.Tables[0].Rows[i]["Signatory Name"].ToString(), font).Width) + 15;
                    if ((TextRenderer.MeasureText(dsSignatory.Tables[0].Rows[i]["Position"].ToString(), font).Width) > width)
                        width = (TextRenderer.MeasureText(dsSignatory.Tables[0].Rows[i]["Position"].ToString(), font).Width);
                    lblSigRole.SizeF = new SizeF(width, 20);
                    if (dsSignatory.Tables[0].Rows[i]["Sig Role"].ToString() == "Display")
                    {
                        if (prevSigRole != dsSignatory.Tables[0].Rows[i]["Signatory Position"].ToString())
                        {
                            lblSigRole.Text = dsSignatory.Tables[0].Rows[i]["Signatory Position"].ToString();
                            prevSigRole = dsSignatory.Tables[0].Rows[i]["Signatory Position"].ToString();
                        }  
                    }
                    lblSigRole.LocationF = new PointF(X, Y);
                    lblSigRole.Font = font;
                    lblSigRole.TextAlignment = TextAlignment.MiddleLeft;

                    lblName.SizeF = new SizeF(width, 20);
                    lblName.Text = dsSignatory.Tables[0].Rows[i]["Signatory Name"].ToString();
                    lblName.LocationF = new PointF(X, (Y + 60));
                    lblName.Font = font;
                    lblName.TextAlignment = TextAlignment.MiddleLeft;

                    lblPos.SizeF = new SizeF(width, 20);
                    if (dsSignatory.Tables[0].Rows[i]["Sig Pos"].ToString() == "Display")
                    {
                        lblPos.Text = dsSignatory.Tables[0].Rows[i]["Position"].ToString();
                    }
                    lblPos.LocationF = new PointF(X, (Y + 80));
                    lblPos.Font = font;
                    lblPos.TextAlignment = TextAlignment.MiddleLeft;

                    if (X < labelsTotalWidth)
                    {
                        X = X + width + 30;
                    }
                    else
                    {
                        X = 0;
                        Y += 140;
                    }
                    reportFooter.Controls.Add(lblSigRole);
                    reportFooter.Controls.Add(lblName);
                    reportFooter.Controls.Add(lblPos);
                }
            }
            else
            {
                X = 20;

                string prevSigRole = string.Empty;
                for (int i = 0; i < dsSignatory.Tables[0].Rows.Count; i++)
                {
                    XRLabel lblSigRole = new XRLabel();
                    XRLabel lblName = new XRLabel();
                    XRLabel lblPos = new XRLabel();

                    float width = (TextRenderer.MeasureText(dsSignatory.Tables[0].Rows[i]["Signatory Name"].ToString(), font).Width) + 15;
                    if ((TextRenderer.MeasureText(dsSignatory.Tables[0].Rows[i]["Position"].ToString(), font).Width) > width)
                        width = (TextRenderer.MeasureText(dsSignatory.Tables[0].Rows[i]["Position"].ToString(), font).Width) + 15;
                    lblSigRole.SizeF = new SizeF(200, 20);
                    if (dsSignatory.Tables[0].Rows[i]["Sig Role"].ToString() == "Display")
                    {
                        if (prevSigRole != dsSignatory.Tables[0].Rows[i]["Signatory Position"].ToString())
                        {
                            lblSigRole.Text = dsSignatory.Tables[0].Rows[i]["Signatory Position"].ToString();
                            prevSigRole = dsSignatory.Tables[0].Rows[i]["Signatory Position"].ToString();
                        }
                    }
                    lblSigRole.Font = font;
                    lblSigRole.LocationF = new PointF(X, Y);
                    lblSigRole.TextAlignment = TextAlignment.MiddleLeft;

                    lblName.SizeF = new SizeF(width, 20);
                    lblName.Font = font;
                    lblName.Text = dsSignatory.Tables[0].Rows[i]["Signatory Name"].ToString();
                    lblName.LocationF = new PointF((X + 200), Y);
                    lblName.TextAlignment = TextAlignment.MiddleLeft;

                    lblPos.SizeF = new SizeF(width, 20);
                    if (dsSignatory.Tables[0].Rows[i]["Sig Pos"].ToString() == "Display")
                    {
                        lblPos.Text = dsSignatory.Tables[0].Rows[i]["Position"].ToString();
                    }
                    lblPos.LocationF = new PointF((X + 200), (Y + 25));
                    lblPos.Font = font;
                    lblPos.TextAlignment = TextAlignment.MiddleLeft;

                    Y = Y + 80;

                    reportFooter.Controls.Add(lblSigRole);
                    reportFooter.Controls.Add(lblName);
                    reportFooter.Controls.Add(lblPos);
                }
            }
            reportFooter.HeightF = Y;

            this.Bands.Add(reportFooter);
            

        }


        private float[] getMaxWidths(Font font)
        {
            labelsTotalWidth = 0;
            float[] colwidths = new float[dsSource.Tables[0].Columns.Count];
            float tempWidth = 0;
            
            for (int i = 0; i < dsSource.Tables[0].Columns.Count; i++)
            {
                float maxWidth = 0;
                tempWidth = TextRenderer.MeasureText(dsSource.Tables[0].Columns[i].Caption, font).Width;
                maxWidth = maxWidth > tempWidth ? maxWidth : tempWidth;

                for (int j = 0; j < dsSource.Tables[0].Rows.Count; j++)
                {
                    tempWidth = TextRenderer.MeasureText(dsSource.Tables[0].Rows[j][i].ToString(), font).Width;
                    maxWidth = maxWidth > tempWidth ? maxWidth : tempWidth;
                }
                colwidths[i] = maxWidth + 10;
                labelsTotalWidth += maxWidth + 10;
            }

            return colwidths;
        }


        private CalculatedField SetCalculatedFields()
        {
            CalculatedField calcField = new CalculatedField();
            this.CalculatedFields.Add(calcField);

            // Define its properties.
            calcField.DataSource = this.DataSource;
            calcField.FieldType = FieldType.Double;
            calcField.DisplayName = "A Calculated Field";
            calcField.Name = "myField";
            
            bool flagi = true;
            bool flagj = true;
            string[] nontotallables = hrcReportsBL.GetColumnsNotIncludedInSummary();
            for (int j = 0; j < dsSource.Tables[0].Columns.Count; j++)
            {
                int k = 0;
                for (; k < nontotallables.Length && nontotallables[k] != dsSource.Tables[0].Columns[j].Caption; k++) ;
                if (k == nontotallables.Length)
                {
                    flagi = true;
                    for (int i = 0; i < dsSource.Tables[0].Rows.Count && flagi; i++)
                    {
                        double num;
                        string str = dsSource.Tables[0].Rows[i][j].ToString();
                        if (str != string.Empty)
                        {
                            bool result = double.TryParse(str, out num);
                            if (!result)
                                flagi = false;
                        }
                    }
                    if (flagi)
                    {
                        if (calcField.Expression != string.Empty)
                            calcField.Expression += " + ";
                        calcField.Expression += "(Iif( IsNullOrEmpty([" + dsSource.Tables[0].Columns[j].Caption + "]), 0 , [" + dsSource.Tables[0].Columns[j].Caption + "]))";
                    }

                }
            }
            return calcField;
        }
    }
}
