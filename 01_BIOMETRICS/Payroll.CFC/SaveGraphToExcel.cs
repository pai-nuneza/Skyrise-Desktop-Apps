using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;
using CommonLibrary;
using System.Reflection;
using Payroll.DAL;

namespace Payroll.CFC
{
    public class SaveGraphToExcel
    {

        public void creategraph(string filename, string[] chartTitle, string[] data,  DataView dView, int columnCounter, string[] columnNames, int left, int top, int width, int height, int type1, string sourceColRow, string celrange)
        {
            System.Data.DataTable dt = dView.ToTable();
            Excel.Application xlApp;
            Excel.Workbook xlWorkBook;
            Excel.Worksheet xlWorkSheet;
            object misValue = System.Reflection.Missing.Value;

            xlApp = new Excel.ApplicationClass();
            xlWorkBook = xlApp.Workbooks.Add(misValue);
            xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);

            string xlShiftDown = "Printed on " + DateTime.Now.ToString("MMMM/dd/yyyy hh:mm tt");
            string[] companyprofile = GetCompanyProfile();

            try
            {
                if (dt.Rows.Count > 0)
                {
                    #region Sheet1
                    xlWorkSheet.Cells[1, 1] = companyprofile[0];
                    xlWorkSheet.Cells[2, 1] = companyprofile[1];
                    xlWorkSheet.Cells[3, 1] = companyprofile[2];

                    xlWorkSheet.Cells[5, 1] = data[11] + "   " + data[6];

                    xlWorkSheet.Cells[6, 1] = data[0] + " " + data[1];
                    xlWorkSheet.Cells[7, 1] = data[4] + " " + data[5];
                    xlWorkSheet.Cells[6, 3] = data[7] + " " + data[8];
                    xlWorkSheet.Cells[7, 3] = data[9] + " " + data[10];

                    xlWorkSheet.Cells[8, 1] = data[2] + " " + data[3];


                    int row = 10;
                    int col2 = 0;

                    if (type1 == 3 || type1 == 4)
                    {
                        for (int col = 0; col < columnCounter; col++)
                        {
                            xlWorkSheet.Cells[row + 1, 1] = columnNames[col].ToString();
                            row++;
                        }

                        row = 10;
                        foreach (DataRow dr in dt.Rows)
                        {

                            for (col2 = 0; col2 < columnCounter; col2++)
                            {
                                xlWorkSheet.Cells[row + 1, 2] = dr[col2].ToString();
                                row++;
                            }
                        }
                    }
                    else
                    {
                        for (int col = 0; col < columnCounter; col++)
                        {
                            xlWorkSheet.Cells[row + 1, col + 1] = columnNames[col].ToString();
                        }

                        row = 10;
                        foreach (DataRow dr in dt.Rows)
                        {
                            row++;
                            for (col2 = 0; col2 < columnCounter; col2++)
                            {
                                xlWorkSheet.Cells[row + 1, col2 + 1] = dr[col2].ToString();
                            }
                        }
                    }

                    xlWorkSheet.Columns.AutoFit();
                    xlWorkSheet.Cells[row + 50, 1] = xlShiftDown;

                    Excel.Range chartRange;
                    top *= row;

                    Excel.ChartObjects xlCharts = (Excel.ChartObjects)xlWorkSheet.ChartObjects(Type.Missing);
                    Excel.ChartObject myChart = (Excel.ChartObject)xlCharts.Add(left, top, width, height);
                    Excel.Chart chartPage = myChart.Chart;


                    string lcolumn = string.Empty;

                    if (type1 == 3 || type1 == 4)
                    {
                        lcolumn = GetLastColumnName(2) + (row);
                    }
                    else
                    {
                        lcolumn = GetLastColumnName(col2) + (row + 1);
                    }


                    chartRange = xlWorkSheet.get_Range(celrange, lcolumn);
                    if (sourceColRow == "row")
                    {
                        chartPage.SetSourceData(chartRange, Excel.XlRowCol.xlRows);
                    }
                    else if (sourceColRow == "col")
                    {
                        chartPage.SetSourceData(chartRange, Excel.XlRowCol.xlColumns);
                    }
                    else
                    {
                        chartPage.SetSourceData(chartRange, misValue);
                    }
                    chartPage.ChartType = GetChartType(type1);
                    chartPage.HasTitle = true;
                    chartPage.ChartTitle.Text = chartTitle[0];


                    if (type1 == 3)
                    {
                        chartPage.ApplyDataLabels(Excel.XlDataLabelsType.xlDataLabelsShowLabelAndPercent,
                        misValue, misValue, misValue, misValue, misValue,
                        misValue, misValue, misValue, misValue);
                    }
                    else
                    {
                        chartPage.HasDataTable = true;
                        chartPage.DataTable.HasBorderOutline = true;
                        chartPage.HasLegend = false;
                    }

                    #endregion
                }
                xlWorkBook.Close(true, filename, misValue);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                xlWorkSheet = null;
                xlWorkBook = null;
                xlApp = null;
            }

        }

        public void creategraph(string filename, string[] chartTitle, string[] data, DataView dView, int columnCounter, string[] columnNames, int left, int top, int width, int height, int type1, string sourceColRow, string celrange
                               , DataView dView2, int columnCounter2, string[] columnNames2, int left2, int top2, int width2, int height2, int type2, string sourceColRow2, string celrange2)
        {
            System.Data.DataTable dt = dView.ToTable();
            System.Data.DataTable dt2 = dView2.ToTable();
            Excel.Application xlApp;
            Excel.Workbook xlWorkBook;
            Excel.Worksheet xlWorkSheet;
            Excel.Worksheet xlWorkSheet2;
            object misValue = System.Reflection.Missing.Value;

            xlApp = new Excel.ApplicationClass();
            xlWorkBook = xlApp.Workbooks.Add(misValue);
            xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);
            xlWorkSheet2 = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(2);

            string xlShiftDown = "Printed on " + DateTime.Now.ToString("MMMM/dd/yyyy hh:mm tt");
            string[] companyprofile = GetCompanyProfile();

            try
            {
                if (dt.Rows.Count > 0)
                {
                    #region Sheet1
                    xlWorkSheet.Cells[1, 1] = companyprofile[0];
                    xlWorkSheet.Cells[2, 1] = companyprofile[1];
                    xlWorkSheet.Cells[3, 1] = companyprofile[2];

                    xlWorkSheet.Cells[5, 1] = data[11] + "   " + data[6];

                    xlWorkSheet.Cells[6, 1] = data[0] + " " + data[1];
                    xlWorkSheet.Cells[7, 1] = data[4] + " " + data[5];
                    xlWorkSheet.Cells[6, 3] = data[7] + " " + data[8];
                    xlWorkSheet.Cells[7, 3] = data[9] + " " + data[10];

                    xlWorkSheet.Cells[8, 1] = data[2] + " " + data[3];


                    int row = 10;
                    int col2 = 0;

                    if (type1 == 3 || type1 == 4)
                    {
                        for (int col = 0; col < columnCounter; col++)
                        {
                            xlWorkSheet.Cells[row + 1, 1] = columnNames[col].ToString();
                            row++;
                        }

                        row = 10;
                        foreach (DataRow dr in dt.Rows)
                        {

                            for (col2 = 0; col2 < columnCounter; col2++)
                            {
                                xlWorkSheet.Cells[row + 1, 2] = dr[col2].ToString();
                                row++;
                            }
                        }
                    }
                    else
                    {
                        for (int col = 0; col < columnCounter; col++)
                        {
                            xlWorkSheet.Cells[row + 1, col + 1] = columnNames[col].ToString();
                        }

                        row = 10;
                        foreach (DataRow dr in dt.Rows)
                        {
                            row++;
                            for (col2 = 0; col2 < columnCounter; col2++)
                            {
                                xlWorkSheet.Cells[row + 1, col2 + 1] = dr[col2].ToString();
                            }
                        }
                    }

                    xlWorkSheet.Columns.AutoFit();
                    xlWorkSheet.Cells[row + 50, 1] = xlShiftDown;

                    Excel.Range chartRange;
                    top *= row;

                    Excel.ChartObjects xlCharts = (Excel.ChartObjects)xlWorkSheet.ChartObjects(Type.Missing);
                    Excel.ChartObject myChart = (Excel.ChartObject)xlCharts.Add(left, top, width, height);
                    Excel.Chart chartPage = myChart.Chart;


                    string lcolumn = string.Empty;

                    if (type1 == 3 || type1 == 4)
                    {
                        lcolumn = GetLastColumnName(2) + (row);
                    }
                    else
                    {
                        lcolumn = GetLastColumnName(col2) + (row + 1);
                    }


                    chartRange = xlWorkSheet.get_Range(celrange, lcolumn);
                    if (sourceColRow == "row")
                    {
                        chartPage.SetSourceData(chartRange, Excel.XlRowCol.xlRows);
                    }
                    else if (sourceColRow == "col")
                    {
                        chartPage.SetSourceData(chartRange, Excel.XlRowCol.xlColumns);
                    }
                    else
                    {
                        chartPage.SetSourceData(chartRange, misValue);
                    }
                    chartPage.ChartType = GetChartType(type1);
                    chartPage.HasTitle = true;
                    chartPage.ChartTitle.Text = chartTitle[0];


                    if (type1 == 3)
                    {
                        chartPage.ApplyDataLabels(Excel.XlDataLabelsType.xlDataLabelsShowLabelAndPercent,
                        misValue, misValue, misValue, misValue, misValue,
                        misValue, misValue, misValue, misValue);
                    }
                    else
                    {
                        chartPage.HasDataTable = true;
                        chartPage.DataTable.HasBorderOutline = true;
                        chartPage.HasLegend = false;
                    }

                    #endregion


                }

                if (dt2.Rows.Count > 0)
                {
                    #region Sheet2
                    xlWorkSheet2.Cells[1, 1] = companyprofile[0];
                    xlWorkSheet2.Cells[2, 1] = companyprofile[1];
                    xlWorkSheet2.Cells[3, 1] = companyprofile[2];

                    xlWorkSheet2.Cells[5, 1] = data[11] + "   " + data[6];

                    xlWorkSheet2.Cells[6, 1] = data[0] + " " + data[1];
                    xlWorkSheet2.Cells[7, 1] = data[4] + " " + data[5];
                    xlWorkSheet2.Cells[6, 3] = data[7] + " " + data[8];
                    xlWorkSheet2.Cells[7, 3] = data[9] + " " + data[10];

                    xlWorkSheet2.Cells[8, 1] = data[2] + " " + data[3];

                    int row2 = 10;
                    int col4 = 0;


                    if (type1 == 3 || type1 == 4)
                    {
                        for (int col3 = 0; col3 < columnCounter2; col3++)
                        {
                            xlWorkSheet2.Cells[row2 + 1, 1] = columnNames2[col3].ToString();
                            row2++;
                        }

                        row2 = 10;

                        foreach (DataRow dr2 in dt2.Rows)
                        {

                            for (col4 = 0; col4 < columnCounter2; col4++)
                            {
                                xlWorkSheet2.Cells[row2 + 1, 2] = dr2[col4].ToString();
                                row2++;
                            }
                        }
                    }
                    else
                    {
                        for (int col3 = 0; col3 < columnCounter2; col3++)
                        {
                            xlWorkSheet2.Cells[row2 + 1, col3 + 1] = columnNames2[col3].ToString();
                        }

                        row2 = 10;

                        foreach (DataRow dr2 in dt2.Rows)
                        {
                            row2++;
                            for (col4 = 0; col4 < columnCounter2; col4++)
                            {
                                xlWorkSheet2.Cells[row2 + 1, col4 + 1] = dr2[col4].ToString();
                            }
                        }
                    }




                    xlWorkSheet2.Columns.AutoFit();
                    xlWorkSheet2.Cells[row2 + 50, 1] = xlShiftDown;

                    Excel.Range chartRange2;
                    top2 *= row2;

                    Excel.ChartObjects xlCharts2 = (Excel.ChartObjects)xlWorkSheet2.ChartObjects(Type.Missing);
                    Excel.ChartObject myChart2 = (Excel.ChartObject)xlCharts2.Add(left2, top2, width2, height2);
                    Excel.Chart chartPage2 = myChart2.Chart;

                    string lcolumn2 = string.Empty;

                    if (type1 == 3 || type1 == 4)
                    {
                        lcolumn2 = GetLastColumnName(2) + (row2);
                    }
                    else
                    {
                        lcolumn2 = GetLastColumnName(col4) + (row2 + 1);
                    }


                    chartRange2 = xlWorkSheet2.get_Range(celrange2, lcolumn2);
                    if (sourceColRow2 == "row")
                    {
                        chartPage2.SetSourceData(chartRange2, Excel.XlRowCol.xlRows);
                    }
                    else if (sourceColRow2 == "col")
                    {
                        chartPage2.SetSourceData(chartRange2, Excel.XlRowCol.xlColumns);
                    }
                    else
                    {
                        chartPage2.SetSourceData(chartRange2, misValue);
                    }
                    chartPage2.ChartType = GetChartType(type2);
                    chartPage2.HasTitle = true;
                    chartPage2.ChartTitle.Text = chartTitle[1];

                    if (type2 == 3)
                    {
                        chartPage2.ApplyDataLabels(Excel.XlDataLabelsType.xlDataLabelsShowLabelAndPercent,
                        misValue, misValue, misValue, misValue, misValue,
                        misValue, misValue, misValue, misValue);
                    }
                    else
                    {
                        chartPage2.HasDataTable = true;
                        chartPage2.DataTable.HasBorderOutline = true;
                        chartPage2.HasLegend = false;
                    }


                    #endregion

                }

                  
                xlWorkBook.Close(true, filename, misValue);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                xlWorkSheet = null;
                xlWorkBook = null;
                xlApp = null;
            }
        }

        public void creategraph(string filename, string[] chartTitle, string[] data, DataView dView, int columnCounter, string[] columnNames, int left, int top, int width, int height, int type1, string sourceColRow, string celrange
                       , DataView dView2, int columnCounter2, string[] columnNames2, int left2, int top2, int width2, int height2, int type2, string sourceColRow2, string celrange2
                       , DataView dView3, int columnCounter3, string[] columnNames3, int left3, int top3, int width3, int height3, int type3, string sourceColRow3, string celrange3)
        {
            System.Data.DataTable dt = dView.ToTable();
            System.Data.DataTable dt2 = dView2.ToTable();
            System.Data.DataTable dt3 = dView3.ToTable();
            Excel.Application xlApp;
            Excel.Workbook xlWorkBook;
            Excel.Worksheet xlWorkSheet;
            Excel.Worksheet xlWorkSheet2;
            Excel.Worksheet xlWorkSheet3;
            object misValue = System.Reflection.Missing.Value;

            xlApp = new Excel.ApplicationClass();
            xlWorkBook = xlApp.Workbooks.Add(misValue);
            xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);
            xlWorkSheet2 = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(2);
            xlWorkSheet3 = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(3);

            string xlShiftDown = "Printed on " + DateTime.Now.ToString("MMMM/dd/yyyy hh:mm tt");
            string[] companyprofile = GetCompanyProfile();

            //Excel.Worksheet newWorksheet;
            //newWorksheet = (Excel.Worksheet)xlWorkBook.Worksheets.Add(Missing.Value, Missing.Value, Missing.Value, Missing.Value);

            try
            {
                if (dt.Rows.Count > 0)
                {
                    #region Sheet1
                    xlWorkSheet.Cells[1, 1] = companyprofile[0];
                    xlWorkSheet.Cells[2, 1] = companyprofile[1];
                    xlWorkSheet.Cells[3, 1] = companyprofile[2];

                    xlWorkSheet.Cells[5, 1] = data[11] + "   " + data[6];

                    xlWorkSheet.Cells[6, 1] = data[0] + " " + data[1];
                    xlWorkSheet.Cells[7, 1] = data[4] + " " + data[5];
                    xlWorkSheet.Cells[6, 3] = data[7] + " " + data[8];
                    xlWorkSheet.Cells[7, 3] = data[9] + " " + data[10];

                    xlWorkSheet.Cells[8, 1] = data[2] + " " + data[3];


                    int row = 10;
                    int col2 = 0;

                    if (type1 == 3 || type1 == 4)
                    {
                        for (int col = 0; col < columnCounter; col++)
                        {
                            xlWorkSheet.Cells[row + 1, 1] = columnNames[col].ToString();
                            row++;
                        }

                        row = 10;
                        foreach (DataRow dr in dt.Rows)
                        {

                            for (col2 = 0; col2 < columnCounter; col2++)
                            {
                                xlWorkSheet.Cells[row + 1, 2] = dr[col2].ToString();
                                row++;
                            }
                        }
                    }
                    else
                    {
                        for (int col = 0; col < columnCounter; col++)
                        {
                            xlWorkSheet.Cells[row + 1, col + 1] = columnNames[col].ToString();
                        }

                        row = 10;
                        foreach (DataRow dr in dt.Rows)
                        {
                            row++;
                            for (col2 = 0; col2 < columnCounter; col2++)
                            {
                                xlWorkSheet.Cells[row + 1, col2 + 1] = dr[col2].ToString();
                            }
                        }
                    }

                    xlWorkSheet.Columns.AutoFit();
                    xlWorkSheet.Cells[row + 50, 1] = xlShiftDown;

                    Excel.Range chartRange;
                    top *= row;

                    Excel.ChartObjects xlCharts = (Excel.ChartObjects)xlWorkSheet.ChartObjects(Type.Missing);
                    Excel.ChartObject myChart = (Excel.ChartObject)xlCharts.Add(left, top, width, height);
                    Excel.Chart chartPage = myChart.Chart;


                    string lcolumn = string.Empty;

                    if (type1 == 3 || type1 == 4)
                    {
                        lcolumn = GetLastColumnName(2) + (row);
                    }
                    else
                    {
                        lcolumn = GetLastColumnName(col2) + (row + 1);
                    }


                    chartRange = xlWorkSheet.get_Range(celrange, lcolumn);
                    if (sourceColRow == "row")
                    {
                        chartPage.SetSourceData(chartRange, Excel.XlRowCol.xlRows);
                    }
                    else if (sourceColRow == "col")
                    {
                        chartPage.SetSourceData(chartRange, Excel.XlRowCol.xlColumns);
                    }
                    else
                    {
                        chartPage.SetSourceData(chartRange, misValue);
                    }
                    chartPage.ChartType = GetChartType(type1);
                    chartPage.HasTitle = true;
                    chartPage.ChartTitle.Text = chartTitle[0];


                    if (type1 == 3)
                    {
                        chartPage.ApplyDataLabels(Excel.XlDataLabelsType.xlDataLabelsShowLabelAndPercent,
                        misValue, misValue, misValue, misValue, misValue,
                        misValue, misValue, misValue, misValue);
                    }
                    else
                    {
                        chartPage.HasDataTable = true;
                        chartPage.DataTable.HasBorderOutline = true;
                        chartPage.HasLegend = false;
                    }

                    #endregion
                }

                if (dt2.Rows.Count > 0)
                {
                    #region Sheet2
                    xlWorkSheet2.Cells[1, 1] = companyprofile[0];
                    xlWorkSheet2.Cells[2, 1] = companyprofile[1];
                    xlWorkSheet2.Cells[3, 1] = companyprofile[2];

                    xlWorkSheet2.Cells[5, 1] = data[11] + "   " + data[6];

                    xlWorkSheet2.Cells[6, 1] = data[0] + " " + data[1];
                    xlWorkSheet2.Cells[7, 1] = data[4] + " " + data[5];
                    xlWorkSheet2.Cells[6, 3] = data[7] + " " + data[8];
                    xlWorkSheet2.Cells[7, 3] = data[9] + " " + data[10];

                    xlWorkSheet2.Cells[8, 1] = data[2] + " " + data[3];

                    int row2 = 10;
                    int col4 = 0;


                    if (type1 == 3 || type1 == 4)
                    {
                        for (int col3 = 0; col3 < columnCounter2; col3++)
                        {
                            xlWorkSheet2.Cells[row2 + 1, 1] = columnNames2[col3].ToString();
                            row2++;
                        }

                        row2 = 10;

                        foreach (DataRow dr2 in dt2.Rows)
                        {

                            for (col4 = 0; col4 < columnCounter2; col4++)
                            {
                                xlWorkSheet2.Cells[row2 + 1, 2] = dr2[col4].ToString();
                                row2++;
                            }
                        }
                    }
                    else
                    {
                        for (int col3 = 0; col3 < columnCounter2; col3++)
                        {
                            xlWorkSheet2.Cells[row2 + 1, col3 + 1] = columnNames2[col3].ToString();
                        }

                        row2 = 10;

                        foreach (DataRow dr2 in dt2.Rows)
                        {
                            row2++;
                            for (col4 = 0; col4 < columnCounter2; col4++)
                            {
                                xlWorkSheet2.Cells[row2 + 1, col4 + 1] = dr2[col4].ToString();
                            }
                        }
                    }




                    xlWorkSheet2.Columns.AutoFit();
                    xlWorkSheet2.Cells[row2 + 50, 1] = xlShiftDown;

                    Excel.Range chartRange2;
                    top2 *= row2;

                    Excel.ChartObjects xlCharts2 = (Excel.ChartObjects)xlWorkSheet2.ChartObjects(Type.Missing);
                    Excel.ChartObject myChart2 = (Excel.ChartObject)xlCharts2.Add(left2, top2, width2, height2);
                    Excel.Chart chartPage2 = myChart2.Chart;

                    string lcolumn2 = string.Empty;

                    if (type1 == 3 || type1 == 4)
                    {
                        lcolumn2 = GetLastColumnName(2) + (row2);
                    }
                    else
                    {
                        lcolumn2 = GetLastColumnName(col4) + (row2 + 1);
                    }


                    chartRange2 = xlWorkSheet2.get_Range(celrange2, lcolumn2);
                    if (sourceColRow2 == "row")
                    {
                        chartPage2.SetSourceData(chartRange2, Excel.XlRowCol.xlRows);
                    }
                    else if (sourceColRow2 == "col")
                    {
                        chartPage2.SetSourceData(chartRange2, Excel.XlRowCol.xlColumns);
                    }
                    else
                    {
                        chartPage2.SetSourceData(chartRange2, misValue);
                    }
                    chartPage2.ChartType = GetChartType(type2);
                    chartPage2.HasTitle = true;
                    chartPage2.ChartTitle.Text = chartTitle[1];

                    if (type2 == 3)
                    {
                        chartPage2.ApplyDataLabels(Excel.XlDataLabelsType.xlDataLabelsShowLabelAndPercent,
                        misValue, misValue, misValue, misValue, misValue,
                        misValue, misValue, misValue, misValue);
                    }
                    else
                    {
                        chartPage2.HasDataTable = true;
                        chartPage2.DataTable.HasBorderOutline = true;
                        chartPage2.HasLegend = false;
                    }


                    #endregion
                }

                if (dt3.Rows.Count > 0)
                {
                    #region Sheet3
                    xlWorkSheet3.Cells[1, 1] = companyprofile[0];
                    xlWorkSheet3.Cells[2, 1] = companyprofile[1];
                    xlWorkSheet3.Cells[3, 1] = companyprofile[2];

                    xlWorkSheet3.Cells[5, 1] = data[11] + "   " + data[6];

                    xlWorkSheet3.Cells[6, 1] = data[0] + " " + data[1];
                    xlWorkSheet3.Cells[7, 1] = data[4] + " " + data[5];
                    xlWorkSheet3.Cells[6, 3] = data[7] + " " + data[8];
                    xlWorkSheet3.Cells[7, 3] = data[9] + " " + data[10];

                    xlWorkSheet3.Cells[8, 1] = data[2] + " " + data[3];

                    int row3 = 10;
                    int col6 = 0;

                    if (type1 == 3 || type1 == 4)
                    {
                        for (int col5 = 0; col5 < columnCounter3; col5++)
                        {
                            xlWorkSheet3.Cells[row3 + 1, 1] = columnNames3[col5].ToString();
                            row3++;
                        }

                        row3 = 10;
                        foreach (DataRow dr3 in dt3.Rows)
                        {

                            for (col6 = 0; col6 < columnCounter3; col6++)
                            {
                                xlWorkSheet3.Cells[row3 + 1, 2] = dr3[col6].ToString();
                                row3++;
                            }
                        }
                    }
                    else
                    {
                        for (int col5 = 0; col5 < columnCounter3; col5++)
                        {
                            xlWorkSheet3.Cells[row3 + 1, col5 + 1] = columnNames3[col5].ToString();
                        }

                        row3 = 10;
                        foreach (DataRow dr3 in dt3.Rows)
                        {
                            row3++;
                            for (col6 = 0; col6 < columnCounter3; col6++)
                            {
                                xlWorkSheet3.Cells[row3 + 1, col6 + 1] = dr3[col6].ToString();
                            }
                        }
                    }


                    xlWorkSheet3.Columns.AutoFit();
                    xlWorkSheet3.Cells[row3 + 50, 1] = xlShiftDown;

                    Excel.Range chartRange3;
                    top3 *= row3;

                    Excel.ChartObjects xlCharts3 = (Excel.ChartObjects)xlWorkSheet3.ChartObjects(Type.Missing);
                    Excel.ChartObject myChart3 = (Excel.ChartObject)xlCharts3.Add(left3, top3, width3, height3);
                    Excel.Chart chartPage3 = myChart3.Chart;

                    string lcolumn3 = string.Empty;

                    if (type1 == 3 || type1 == 4)
                    {
                        lcolumn3 = GetLastColumnName(2) + (row3);
                    }
                    else
                    {
                        lcolumn3 = GetLastColumnName(col6) + (row3 + 1);
                    }

                    chartRange3 = xlWorkSheet3.get_Range(celrange3, lcolumn3);
                    if (sourceColRow3 == "row")
                    {
                        chartPage3.SetSourceData(chartRange3, Excel.XlRowCol.xlRows);
                    }
                    else if (sourceColRow3 == "col")
                    {
                        chartPage3.SetSourceData(chartRange3, Excel.XlRowCol.xlColumns);
                    }
                    else
                    {
                        chartPage3.SetSourceData(chartRange3, misValue);
                    }
                    chartPage3.ChartType = GetChartType(type3);
                    chartPage3.HasTitle = true;
                    chartPage3.ChartTitle.Text = chartTitle[2];

                    if (type3 == 3)
                    {
                        chartPage3.ApplyDataLabels(Excel.XlDataLabelsType.xlDataLabelsShowLabelAndPercent,
                        misValue, misValue, misValue, misValue, misValue,
                        misValue, misValue, misValue, misValue);
                    }
                    else
                    {
                        chartPage3.HasDataTable = true;
                        chartPage3.DataTable.HasBorderOutline = true;
                        chartPage3.HasLegend = false;
                    }


                    #endregion
                }

                xlWorkBook.Close(true, filename, misValue);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                xlWorkSheet = null;
                xlWorkBook = null;
                xlApp = null;
            }
        }

        public void creategraph(string filename, string[] chartTitle, string[] data, DataView dView, int columnCounter, string[] columnNames, int left, int top, int width, int height, int type1, string sourceColRow, string celrange
                       , DataView dView2, int columnCounter2, string[] columnNames2, int left2, int top2, int width2, int height2, int type2, string sourceColRow2, string celrange2
                       , DataView dView3, int columnCounter3, string[] columnNames3, int left3, int top3, int width3, int height3, int type3, string sourceColRow3, string celrange3
                       , DataView dView4, int columnCounter4, string[] columnNames4, int left4, int top4, int width4, int height4, int type4, string sourceColRow4, string celrange4)
        {
            System.Data.DataTable dt = dView.ToTable();
            System.Data.DataTable dt2 = dView2.ToTable();
            System.Data.DataTable dt3 = dView3.ToTable();
            System.Data.DataTable dt4 = dView4.ToTable();
            Excel.Application xlApp;
            Excel.Workbook xlWorkBook;
            Excel.Worksheet xlWorkSheet;
            Excel.Worksheet xlWorkSheet2;
            Excel.Worksheet xlWorkSheet3;
            object misValue = System.Reflection.Missing.Value;

            xlApp = new Excel.ApplicationClass();
            xlWorkBook = xlApp.Workbooks.Add(misValue);
            xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);
            xlWorkSheet2 = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(2);
            xlWorkSheet3 = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(3);

            Excel.Worksheet xlWorkSheet4;
            xlWorkSheet4 = (Excel.Worksheet)xlWorkBook.Worksheets.Add(Missing.Value, Missing.Value, Missing.Value, Missing.Value);

            string xlShiftDown = "Printed on " + DateTime.Now.ToString("MMMM/dd/yyyy hh:mm tt");
            string[] companyprofile = GetCompanyProfile();


            try
            {
                if (dt.Rows.Count > 0)
                {
                    #region Sheet1
                    xlWorkSheet.Cells[1, 1] = companyprofile[0];
                    xlWorkSheet.Cells[2, 1] = companyprofile[1];
                    xlWorkSheet.Cells[3, 1] = companyprofile[2];

                    xlWorkSheet.Cells[5, 1] = data[11] + "   " + data[6];

                    xlWorkSheet.Cells[6, 1] = data[0] + " " + data[1];
                    xlWorkSheet.Cells[7, 1] = data[4] + " " + data[5];
                    xlWorkSheet.Cells[6, 3] = data[7] + " " + data[8];
                    xlWorkSheet.Cells[7, 3] = data[9] + " " + data[10];

                    xlWorkSheet.Cells[8, 1] = data[2] + " " + data[3];


                    int row = 10;
                    int col2 = 0;

                    if (type1 == 3 || type1 == 4)
                    {
                        for (int col = 0; col < columnCounter; col++)
                        {
                            xlWorkSheet.Cells[row + 1, 1] = columnNames[col].ToString();
                            row++;
                        }

                        row = 10;
                        foreach (DataRow dr in dt.Rows)
                        {

                            for (col2 = 0; col2 < columnCounter; col2++)
                            {
                                xlWorkSheet.Cells[row + 1, 2] = dr[col2].ToString();
                                row++;
                            }
                        }
                    }
                    else
                    {
                        for (int col = 0; col < columnCounter; col++)
                        {
                            xlWorkSheet.Cells[row + 1, col + 1] = columnNames[col].ToString();
                        }

                        row = 10;
                        foreach (DataRow dr in dt.Rows)
                        {
                            row++;
                            for (col2 = 0; col2 < columnCounter; col2++)
                            {
                                xlWorkSheet.Cells[row + 1, col2 + 1] = dr[col2].ToString();
                            }
                        }
                    }

                    xlWorkSheet.Columns.AutoFit();
                    xlWorkSheet.Cells[row + 50, 1] = xlShiftDown;

                    Excel.Range chartRange;
                    top *= row;

                    Excel.ChartObjects xlCharts = (Excel.ChartObjects)xlWorkSheet.ChartObjects(Type.Missing);
                    Excel.ChartObject myChart = (Excel.ChartObject)xlCharts.Add(left, top, width, height);
                    Excel.Chart chartPage = myChart.Chart;


                    string lcolumn = string.Empty;

                    if (type1 == 3 || type1 == 4)
                    {
                        lcolumn = GetLastColumnName(2) + (row);
                    }
                    else
                    {
                        lcolumn = GetLastColumnName(col2) + (row + 1);
                    }


                    chartRange = xlWorkSheet.get_Range(celrange, lcolumn);
                    if (sourceColRow == "row")
                    {
                        chartPage.SetSourceData(chartRange, Excel.XlRowCol.xlRows);
                    }
                    else if (sourceColRow == "col")
                    {
                        chartPage.SetSourceData(chartRange, Excel.XlRowCol.xlColumns);
                    }
                    else
                    {
                        chartPage.SetSourceData(chartRange, misValue);
                    }
                    chartPage.ChartType = GetChartType(type1);
                    chartPage.HasTitle = true;
                    chartPage.ChartTitle.Text = chartTitle[0];


                    if (type1 == 3)
                    {
                        chartPage.ApplyDataLabels(Excel.XlDataLabelsType.xlDataLabelsShowLabelAndPercent,
                        misValue, misValue, misValue, misValue, misValue,
                        misValue, misValue, misValue, misValue);
                    }
                    else
                    {
                        chartPage.HasDataTable = true;
                        chartPage.DataTable.HasBorderOutline = true;
                        chartPage.HasLegend = false;
                    }

                    if (type1 == 5)
                    {
                        Excel.Series series = (Excel.Series)chartPage.SeriesCollection(3);
                        series.ChartType = Excel.XlChartType.xlLineMarkers;
                        series.AxisGroup = Microsoft.Office.Interop.Excel.XlAxisGroup.xlSecondary;
                    }
                    #endregion
                }

                if (dt2.Rows.Count > 0)
                {
                    #region Sheet2
                    xlWorkSheet2.Cells[1, 1] = companyprofile[0];
                    xlWorkSheet2.Cells[2, 1] = companyprofile[1];
                    xlWorkSheet2.Cells[3, 1] = companyprofile[2];

                    xlWorkSheet2.Cells[5, 1] = data[11] + "   " + data[6];

                    xlWorkSheet2.Cells[6, 1] = data[0] + " " + data[1];
                    xlWorkSheet2.Cells[7, 1] = data[4] + " " + data[5];
                    xlWorkSheet2.Cells[6, 3] = data[7] + " " + data[8];
                    xlWorkSheet2.Cells[7, 3] = data[9] + " " + data[10];

                    xlWorkSheet2.Cells[8, 1] = data[2] + " " + data[3];

                    int row2 = 10;
                    int col4 = 0;


                    if (type1 == 3 || type1 == 4)
                    {
                        for (int col3 = 0; col3 < columnCounter2; col3++)
                        {
                            xlWorkSheet2.Cells[row2 + 1, 1] = columnNames2[col3].ToString();
                            row2++;
                        }

                        row2 = 10;

                        foreach (DataRow dr2 in dt2.Rows)
                        {

                            for (col4 = 0; col4 < columnCounter2; col4++)
                            {
                                xlWorkSheet2.Cells[row2 + 1, 2] = dr2[col4].ToString();
                                row2++;
                            }
                        }
                    }
                    else
                    {
                        for (int col3 = 0; col3 < columnCounter2; col3++)
                        {
                            xlWorkSheet2.Cells[row2 + 1, col3 + 1] = columnNames2[col3].ToString();
                        }

                        row2 = 10;

                        foreach (DataRow dr2 in dt2.Rows)
                        {
                            row2++;
                            for (col4 = 0; col4 < columnCounter2; col4++)
                            {
                                xlWorkSheet2.Cells[row2 + 1, col4 + 1] = dr2[col4].ToString();
                            }
                        }
                    }

                    xlWorkSheet2.Columns.AutoFit();
                    xlWorkSheet2.Cells[row2 + 50, 1] = xlShiftDown;

                    Excel.Range chartRange2;
                    top2 *= row2;

                    Excel.ChartObjects xlCharts2 = (Excel.ChartObjects)xlWorkSheet2.ChartObjects(Type.Missing);
                    Excel.ChartObject myChart2 = (Excel.ChartObject)xlCharts2.Add(left2, top2, width2, height2);
                    Excel.Chart chartPage2 = myChart2.Chart;

                    string lcolumn2 = string.Empty;

                    if (type1 == 3 || type1 == 4)
                    {
                        lcolumn2 = GetLastColumnName(2) + (row2);
                    }
                    else
                    {
                        lcolumn2 = GetLastColumnName(col4) + (row2 + 1);
                    }


                    chartRange2 = xlWorkSheet2.get_Range(celrange2, lcolumn2);
                    if (sourceColRow2 == "row")
                    {
                        chartPage2.SetSourceData(chartRange2, Excel.XlRowCol.xlRows);
                    }
                    else if (sourceColRow2 == "col")
                    {
                        chartPage2.SetSourceData(chartRange2, Excel.XlRowCol.xlColumns);
                    }
                    else
                    {
                        chartPage2.SetSourceData(chartRange2, misValue);
                    }
                    chartPage2.ChartType = GetChartType(type2);
                    chartPage2.HasTitle = true;
                    chartPage2.ChartTitle.Text = chartTitle[1];

                    if (type2 == 3)
                    {
                        chartPage2.ApplyDataLabels(Excel.XlDataLabelsType.xlDataLabelsShowLabelAndPercent,
                        misValue, misValue, misValue, misValue, misValue,
                        misValue, misValue, misValue, misValue);
                    }
                    else
                    {
                        chartPage2.HasDataTable = true;
                        chartPage2.DataTable.HasBorderOutline = true;
                        chartPage2.HasLegend = false;
                    }

                    if (type2 == 5)
                    {
                        Excel.Series series2 = (Excel.Series)chartPage2.SeriesCollection(2);
                        series2.ChartType = Excel.XlChartType.xlLineMarkers;
                        series2.AxisGroup = Microsoft.Office.Interop.Excel.XlAxisGroup.xlSecondary;
                    }

                    if (type2 == 6)
                    {
                        chartPage2.HasDataTable = false;
                        chartPage2.HasLegend = true;
                        chartPage2.Legend.Position = Microsoft.Office.Interop.Excel.XlLegendPosition.xlLegendPositionBottom;
                        Excel.Series series2 = (Excel.Series)chartPage2.SeriesCollection(dt2.Rows.Count);
                        series2.HasDataLabels = true;
                        series2.ApplyDataLabels(Microsoft.Office.Interop.Excel.XlDataLabelsType.xlDataLabelsShowValue, misValue, misValue, misValue, misValue, misValue, misValue, misValue, misValue, misValue);
                    }

                    #endregion
                }

                if (dt3.Rows.Count > 0)
                {
                    #region Sheet3
                    xlWorkSheet3.Cells[1, 1] = companyprofile[0];
                    xlWorkSheet3.Cells[2, 1] = companyprofile[1];
                    xlWorkSheet3.Cells[3, 1] = companyprofile[2];

                    xlWorkSheet3.Cells[5, 1] = data[11] + "   " + data[6];

                    xlWorkSheet3.Cells[6, 1] = data[0] + " " + data[1];
                    xlWorkSheet3.Cells[7, 1] = data[4] + " " + data[5];
                    xlWorkSheet3.Cells[6, 3] = data[7] + " " + data[8];
                    xlWorkSheet3.Cells[7, 3] = data[9] + " " + data[10];

                    xlWorkSheet3.Cells[8, 1] = data[2] + " " + data[3];

                    int row3 = 10;
                    int col6 = 0;

                    if (type1 == 3 || type1 == 4)
                    {
                        for (int col5 = 0; col5 < columnCounter3; col5++)
                        {
                            xlWorkSheet3.Cells[row3 + 1, 1] = columnNames3[col5].ToString();
                            row3++;
                        }

                        row3 = 10;
                        foreach (DataRow dr3 in dt3.Rows)
                        {

                            for (col6 = 0; col6 < columnCounter3; col6++)
                            {
                                xlWorkSheet3.Cells[row3 + 1, 2] = dr3[col6].ToString();
                                row3++;
                            }
                        }
                    }
                    else
                    {
                        for (int col5 = 0; col5 < columnCounter3; col5++)
                        {
                            xlWorkSheet3.Cells[row3 + 1, col5 + 1] = columnNames3[col5].ToString();
                        }

                        row3 = 10;
                        foreach (DataRow dr3 in dt3.Rows)
                        {
                            row3++;
                            for (col6 = 0; col6 < columnCounter3; col6++)
                            {
                                xlWorkSheet3.Cells[row3 + 1, col6 + 1] = dr3[col6].ToString();
                            }
                        }
                    }


                    xlWorkSheet3.Columns.AutoFit();
                    xlWorkSheet3.Cells[row3 + 50, 1] = xlShiftDown;

                    Excel.Range chartRange3;
                    top3 *= row3;

                    Excel.ChartObjects xlCharts3 = (Excel.ChartObjects)xlWorkSheet3.ChartObjects(Type.Missing);
                    Excel.ChartObject myChart3 = (Excel.ChartObject)xlCharts3.Add(left3, top3, width3, height3);
                    Excel.Chart chartPage3 = myChart3.Chart;

                    string lcolumn3 = string.Empty;

                    if (type1 == 3 || type1 == 4)
                    {
                        lcolumn3 = GetLastColumnName(2) + (row3);
                    }
                    else
                    {
                        lcolumn3 = GetLastColumnName(col6) + (row3 + 1);
                    }

                    chartRange3 = xlWorkSheet3.get_Range(celrange3, lcolumn3);
                    if (sourceColRow3 == "row")
                    {
                        chartPage3.SetSourceData(chartRange3, Excel.XlRowCol.xlRows);
                    }
                    else if (sourceColRow3 == "col")
                    {
                        chartPage3.SetSourceData(chartRange3, Excel.XlRowCol.xlColumns);
                    }
                    else
                    {
                        chartPage3.SetSourceData(chartRange3, misValue);
                    }
                    chartPage3.ChartType = GetChartType(type3);
                    chartPage3.HasTitle = true;
                    chartPage3.ChartTitle.Text = chartTitle[2];

                    if (type3 == 3)
                    {
                        chartPage3.ApplyDataLabels(Excel.XlDataLabelsType.xlDataLabelsShowLabelAndPercent,
                        misValue, misValue, misValue, misValue, misValue,
                        misValue, misValue, misValue, misValue);
                    }
                    else
                    {
                        chartPage3.HasDataTable = true;
                        chartPage3.DataTable.HasBorderOutline = true;
                        chartPage3.HasLegend = false;
                    }

                    if (type1 == 5)
                    {
                        Excel.Series series3 = (Excel.Series)chartPage3.SeriesCollection(2);
                        series3.ChartType = Excel.XlChartType.xlLineMarkers;
                        series3.AxisGroup = Microsoft.Office.Interop.Excel.XlAxisGroup.xlSecondary;
                    }

                    #endregion
                }

                if (dt4.Rows.Count > 0)
                {
                    #region Sheet4
                    xlWorkSheet4.Cells[1, 1] = companyprofile[0];
                    xlWorkSheet4.Cells[2, 1] = companyprofile[1];
                    xlWorkSheet4.Cells[3, 1] = companyprofile[2];

                    xlWorkSheet4.Cells[5, 1] = data[11] + "   " + data[6];

                    xlWorkSheet4.Cells[6, 1] = data[0] + " " + data[1];
                    xlWorkSheet4.Cells[7, 1] = data[4] + " " + data[5];
                    xlWorkSheet4.Cells[6, 3] = data[7] + " " + data[8];
                    xlWorkSheet4.Cells[7, 3] = data[9] + " " + data[10];

                    xlWorkSheet4.Cells[8, 1] = data[2] + " " + data[3];

                    int row4 = 10;
                    int col8 = 0;

                    if (type1 == 3 || type1 == 4)
                    {
                        for (int col7 = 0; col7 < columnCounter4; col7++)
                        {
                            xlWorkSheet4.Cells[row4 + 1, 1] = columnNames4[col7].ToString();
                            row4++;
                        }

                        row4 = 10;
                        foreach (DataRow dr4 in dt4.Rows)
                        {
                            for (col8 = 0; col8 < columnCounter4; col8++)
                            {
                                xlWorkSheet4.Cells[row4 + 1, 2] = dr4[col8].ToString();
                                row4++;
                            }
                        }
                    }
                    else
                    {
                        for (int col7 = 0; col7 < columnCounter4; col7++)
                        {
                            xlWorkSheet4.Cells[row4 + 1, col7 + 1] = columnNames4[col7].ToString();
                        }

                        row4 = 10;
                        foreach (DataRow dr4 in dt4.Rows)
                        {
                            row4++;
                            for (col8 = 0; col8 < columnCounter4; col8++)
                            {
                                xlWorkSheet4.Cells[row4 + 1, col8 + 1] = dr4[col8].ToString();
                            }
                        }
                    }


                    xlWorkSheet4.Columns.AutoFit();
                    xlWorkSheet4.Cells[row4 + 50, 1] = xlShiftDown;

                    Excel.Range chartRange4;
                    top4 *= row4;

                    Excel.ChartObjects xlCharts4 = (Excel.ChartObjects)xlWorkSheet4.ChartObjects(Type.Missing);
                    Excel.ChartObject myChart4 = (Excel.ChartObject)xlCharts4.Add(left4, top4, width4, height4);
                    Excel.Chart chartPage4 = myChart4.Chart;

                    string lcolumn4 = string.Empty;

                    if (type1 == 3 || type1 == 4)
                    {
                        lcolumn4 = GetLastColumnName(2) + (row4);
                    }
                    else
                    {
                        lcolumn4 = GetLastColumnName(col8) + (row4 + 1);
                    }



                    chartRange4 = xlWorkSheet4.get_Range(celrange4, lcolumn4);
                    if (sourceColRow4 == "row")
                    {
                        chartPage4.SetSourceData(chartRange4, Excel.XlRowCol.xlRows);
                    }
                    else if (sourceColRow4 == "col")
                    {
                        chartPage4.SetSourceData(chartRange4, Excel.XlRowCol.xlColumns);
                    }
                    else
                    {
                        chartPage4.SetSourceData(chartRange4, misValue);
                    }
                    chartPage4.ChartType = GetChartType(type4);
                    chartPage4.HasTitle = true;
                    chartPage4.ChartTitle.Text = chartTitle[3];

                    if (type4 == 3)
                    {
                        chartPage4.ApplyDataLabels(Excel.XlDataLabelsType.xlDataLabelsShowLabelAndPercent,
                        misValue, misValue, misValue, misValue, misValue,
                        misValue, misValue, misValue, misValue);
                    }
                    else
                    {
                        chartPage4.HasDataTable = true;
                        chartPage4.DataTable.HasBorderOutline = true;
                        chartPage4.HasLegend = false;
                    }

                    if (type1 == 5)
                    {
                        Excel.Series series4 = (Excel.Series)chartPage4.SeriesCollection(2);
                        series4.ChartType = Excel.XlChartType.xlLineMarkers;
                        series4.AxisGroup = Microsoft.Office.Interop.Excel.XlAxisGroup.xlSecondary;
                    }
                    #endregion
                }

                    xlWorkBook.Close(true, filename, misValue);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                xlWorkSheet = null;
                xlWorkBook = null;
                xlApp = null;
            }
        }

        public void creategraph(string filename, string[] chartTitle, string[] data, DataView dView, int columnCounter, string[] columnNames, int left, int top, int width, int height, int type1, string sourceColRow, string celrange
                       , DataView dView2, int columnCounter2, string[] columnNames2, int left2, int top2, int width2, int height2, int type2, string sourceColRow2, string celrange2
                       , DataView dView3, int columnCounter3, string[] columnNames3, int left3, int top3, int width3, int height3, int type3, string sourceColRow3, string celrange3
                       , DataView dView4, int columnCounter4, string[] columnNames4, int left4, int top4, int width4, int height4, int type4, string sourceColRow4, string celrange4
                       , DataView dView5, int columnCounter5, string[] columnNames5, int left5, int top5, int width5, int height5, int type5, string sourceColRow5, string celrange5)
        {
            System.Data.DataTable dt = dView.ToTable();
            System.Data.DataTable dt2 = dView2.ToTable();
            System.Data.DataTable dt3 = dView3.ToTable();
            System.Data.DataTable dt4 = dView4.ToTable();
            System.Data.DataTable dt5 = dView5.ToTable();
            Excel.Application xlApp;
            Excel.Workbook xlWorkBook;
            Excel.Worksheet xlWorkSheet;
            Excel.Worksheet xlWorkSheet2;
            Excel.Worksheet xlWorkSheet3;
            object misValue = System.Reflection.Missing.Value;

            xlApp = new Excel.ApplicationClass();
            xlWorkBook = xlApp.Workbooks.Add(misValue);
            xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);
            xlWorkSheet2 = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(2);
            xlWorkSheet3 = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(3);

            Excel.Worksheet xlWorkSheet4;
            xlWorkSheet4 = (Excel.Worksheet)xlWorkBook.Worksheets.Add(Missing.Value, Missing.Value, Missing.Value, Missing.Value);

            Excel.Worksheet xlWorkSheet5;
            xlWorkSheet5 = (Excel.Worksheet)xlWorkBook.Worksheets.Add(Missing.Value, Missing.Value, Missing.Value, Missing.Value);

            string xlShiftDown = "Printed on " + DateTime.Now.ToString("MMMM/dd/yyyy hh:mm tt");
            string[] companyprofile = GetCompanyProfile();


            try
            {
                if (dt.Rows.Count > 0)
                {
                    #region Sheet1
                    xlWorkSheet.Cells[1, 1] = companyprofile[0];
                    xlWorkSheet.Cells[2, 1] = companyprofile[1];
                    xlWorkSheet.Cells[3, 1] = companyprofile[2];

                    xlWorkSheet.Cells[5, 1] = data[11] + "   " + data[6];

                    xlWorkSheet.Cells[6, 1] = data[0] + " " + data[1];
                    xlWorkSheet.Cells[7, 1] = data[4] + " " + data[5];
                    xlWorkSheet.Cells[6, 3] = data[7] + " " + data[8];
                    xlWorkSheet.Cells[7, 3] = data[9] + " " + data[10];

                    xlWorkSheet.Cells[8, 1] = data[2] + " " + data[3];


                    int row = 10;
                    int col2 = 0;

                    if (type1 == 3 || type1 == 4)
                    {
                        for (int col = 0; col < columnCounter; col++)
                        {
                            xlWorkSheet.Cells[row + 1, 1] = columnNames[col].ToString();
                            row++;
                        }

                        row = 10;
                        foreach (DataRow dr in dt.Rows)
                        {

                            for (col2 = 0; col2 < columnCounter; col2++)
                            {
                                xlWorkSheet.Cells[row + 1, 2] = dr[col2].ToString();
                                row++;
                            }
                        }
                    }
                    else
                    {
                        for (int col = 0; col < columnCounter; col++)
                        {
                            xlWorkSheet.Cells[row + 1, col + 1] = columnNames[col].ToString();
                        }

                        row = 10;
                        foreach (DataRow dr in dt.Rows)
                        {
                            row++;
                            for (col2 = 0; col2 < columnCounter; col2++)
                            {
                                xlWorkSheet.Cells[row + 1, col2 + 1] = dr[col2].ToString();
                            }
                        }
                    }

                    xlWorkSheet.Columns.AutoFit();
                    xlWorkSheet.Cells[row + 50, 1] = xlShiftDown;

                    Excel.Range chartRange;
                    top *= row;

                    Excel.ChartObjects xlCharts = (Excel.ChartObjects)xlWorkSheet.ChartObjects(Type.Missing);
                    Excel.ChartObject myChart = (Excel.ChartObject)xlCharts.Add(left, top, width, height);
                    Excel.Chart chartPage = myChart.Chart;


                    string lcolumn = string.Empty;

                    if (type1 == 3 || type1 == 4)
                    {
                        lcolumn = GetLastColumnName(2) + (row);
                    }
                    else
                    {
                        lcolumn = GetLastColumnName(col2) + (row + 1);
                    }


                    chartRange = xlWorkSheet.get_Range(celrange, lcolumn);
                    if (sourceColRow == "row")
                    {
                        chartPage.SetSourceData(chartRange, Excel.XlRowCol.xlRows);
                    }
                    else if (sourceColRow == "col")
                    {
                        chartPage.SetSourceData(chartRange, Excel.XlRowCol.xlColumns);
                    }
                    else
                    {
                        chartPage.SetSourceData(chartRange, misValue);
                    }
                    chartPage.ChartType = GetChartType(type1);
                    chartPage.HasTitle = true;
                    chartPage.ChartTitle.Text = chartTitle[0];


                    if (type1 == 3)
                    {
                        chartPage.ApplyDataLabels(Excel.XlDataLabelsType.xlDataLabelsShowLabelAndPercent,
                        misValue, misValue, misValue, misValue, misValue,
                        misValue, misValue, misValue, misValue);
                    }
                    else
                    {
                        chartPage.HasDataTable = true;
                        chartPage.DataTable.HasBorderOutline = true;
                        chartPage.HasLegend = false;
                    }

                    #endregion
                }

                if (dt2.Rows.Count > 0)
                {
                    #region Sheet2
                    xlWorkSheet2.Cells[1, 1] = companyprofile[0];
                    xlWorkSheet2.Cells[2, 1] = companyprofile[1];
                    xlWorkSheet2.Cells[3, 1] = companyprofile[2];

                    xlWorkSheet2.Cells[5, 1] = data[11] + "   " + data[6];

                    xlWorkSheet2.Cells[6, 1] = data[0] + " " + data[1];
                    xlWorkSheet2.Cells[7, 1] = data[4] + " " + data[5];
                    xlWorkSheet2.Cells[6, 3] = data[7] + " " + data[8];
                    xlWorkSheet2.Cells[7, 3] = data[9] + " " + data[10];

                    xlWorkSheet2.Cells[8, 1] = data[2] + " " + data[3];

                    int row2 = 10;
                    int col4 = 0;


                    if (type1 == 3 || type1 == 4)
                    {
                        for (int col3 = 0; col3 < columnCounter2; col3++)
                        {
                            xlWorkSheet2.Cells[row2 + 1, 1] = columnNames2[col3].ToString();
                            row2++;
                        }

                        row2 = 10;

                        foreach (DataRow dr2 in dt2.Rows)
                        {

                            for (col4 = 0; col4 < columnCounter2; col4++)
                            {
                                xlWorkSheet2.Cells[row2 + 1, 2] = dr2[col4].ToString();
                                row2++;
                            }
                        }
                    }
                    else
                    {
                        for (int col3 = 0; col3 < columnCounter2; col3++)
                        {
                            xlWorkSheet2.Cells[row2 + 1, col3 + 1] = columnNames2[col3].ToString();
                        }

                        row2 = 10;

                        foreach (DataRow dr2 in dt2.Rows)
                        {
                            row2++;
                            for (col4 = 0; col4 < columnCounter2; col4++)
                            {
                                xlWorkSheet2.Cells[row2 + 1, col4 + 1] = dr2[col4].ToString();
                            }
                        }
                    }




                    xlWorkSheet2.Columns.AutoFit();
                    xlWorkSheet2.Cells[row2 + 50, 1] = xlShiftDown;

                    Excel.Range chartRange2;
                    top2 *= row2;

                    Excel.ChartObjects xlCharts2 = (Excel.ChartObjects)xlWorkSheet2.ChartObjects(Type.Missing);
                    Excel.ChartObject myChart2 = (Excel.ChartObject)xlCharts2.Add(left2, top2, width2, height2);
                    Excel.Chart chartPage2 = myChart2.Chart;

                    string lcolumn2 = string.Empty;

                    if (type1 == 3 || type1 == 4)
                    {
                        lcolumn2 = GetLastColumnName(2) + (row2);
                    }
                    else
                    {
                        lcolumn2 = GetLastColumnName(col4) + (row2 + 1);
                    }


                    chartRange2 = xlWorkSheet2.get_Range(celrange2, lcolumn2);
                    if (sourceColRow2 == "row")
                    {
                        chartPage2.SetSourceData(chartRange2, Excel.XlRowCol.xlRows);
                    }
                    else if (sourceColRow2 == "col")
                    {
                        chartPage2.SetSourceData(chartRange2, Excel.XlRowCol.xlColumns);
                    }
                    else
                    {
                        chartPage2.SetSourceData(chartRange2, misValue);
                    }
                    chartPage2.ChartType = GetChartType(type2);
                    chartPage2.HasTitle = true;
                    chartPage2.ChartTitle.Text = chartTitle[1];

                    if (type2 == 3)
                    {
                        chartPage2.ApplyDataLabels(Excel.XlDataLabelsType.xlDataLabelsShowLabelAndPercent,
                        misValue, misValue, misValue, misValue, misValue,
                        misValue, misValue, misValue, misValue);
                    }
                    else
                    {
                        chartPage2.HasDataTable = true;
                        chartPage2.DataTable.HasBorderOutline = true;
                        chartPage2.HasLegend = false;
                    }


                    #endregion
                }

                if (dt3.Rows.Count > 0)
                {
                    #region Sheet3
                    xlWorkSheet3.Cells[1, 1] = companyprofile[0];
                    xlWorkSheet3.Cells[2, 1] = companyprofile[1];
                    xlWorkSheet3.Cells[3, 1] = companyprofile[2];

                    xlWorkSheet3.Cells[5, 1] = data[11] + "   " + data[6];

                    xlWorkSheet3.Cells[6, 1] = data[0] + " " + data[1];
                    xlWorkSheet3.Cells[7, 1] = data[4] + " " + data[5];
                    xlWorkSheet3.Cells[6, 3] = data[7] + " " + data[8];
                    xlWorkSheet3.Cells[7, 3] = data[9] + " " + data[10];

                    xlWorkSheet3.Cells[8, 1] = data[2] + " " + data[3];

                    int row3 = 10;
                    int col6 = 0;

                    if (type1 == 3 || type1 == 4)
                    {
                        for (int col5 = 0; col5 < columnCounter3; col5++)
                        {
                            xlWorkSheet3.Cells[row3 + 1, 1] = columnNames3[col5].ToString();
                            row3++;
                        }

                        row3 = 10;
                        foreach (DataRow dr3 in dt3.Rows)
                        {

                            for (col6 = 0; col6 < columnCounter3; col6++)
                            {
                                xlWorkSheet3.Cells[row3 + 1, 2] = dr3[col6].ToString();
                                row3++;
                            }
                        }
                    }
                    else
                    {
                        for (int col5 = 0; col5 < columnCounter3; col5++)
                        {
                            xlWorkSheet3.Cells[row3 + 1, col5 + 1] = columnNames3[col5].ToString();
                        }

                        row3 = 10;
                        foreach (DataRow dr3 in dt3.Rows)
                        {
                            row3++;
                            for (col6 = 0; col6 < columnCounter3; col6++)
                            {
                                xlWorkSheet3.Cells[row3 + 1, col6 + 1] = dr3[col6].ToString();
                            }
                        }
                    }


                    xlWorkSheet3.Columns.AutoFit();
                    xlWorkSheet3.Cells[row3 + 50, 1] = xlShiftDown;

                    Excel.Range chartRange3;
                    top3 *= row3;

                    Excel.ChartObjects xlCharts3 = (Excel.ChartObjects)xlWorkSheet3.ChartObjects(Type.Missing);
                    Excel.ChartObject myChart3 = (Excel.ChartObject)xlCharts3.Add(left3, top3, width3, height3);
                    Excel.Chart chartPage3 = myChart3.Chart;

                    string lcolumn3 = string.Empty;

                    if (type1 == 3 || type1 == 4)
                    {
                        lcolumn3 = GetLastColumnName(2) + (row3);
                    }
                    else
                    {
                        lcolumn3 = GetLastColumnName(col6) + (row3 + 1);
                    }

                    chartRange3 = xlWorkSheet3.get_Range(celrange3, lcolumn3);
                    if (sourceColRow3 == "row")
                    {
                        chartPage3.SetSourceData(chartRange3, Excel.XlRowCol.xlRows);
                    }
                    else if (sourceColRow3 == "col")
                    {
                        chartPage3.SetSourceData(chartRange3, Excel.XlRowCol.xlColumns);
                    }
                    else
                    {
                        chartPage3.SetSourceData(chartRange3, misValue);
                    }
                    chartPage3.ChartType = GetChartType(type3);
                    chartPage3.HasTitle = true;
                    chartPage3.ChartTitle.Text = chartTitle[2];

                    if (type3 == 3)
                    {
                        chartPage3.ApplyDataLabels(Excel.XlDataLabelsType.xlDataLabelsShowLabelAndPercent,
                        misValue, misValue, misValue, misValue, misValue,
                        misValue, misValue, misValue, misValue);
                    }
                    else
                    {
                        chartPage3.HasDataTable = true;
                        chartPage3.DataTable.HasBorderOutline = true;
                        chartPage3.HasLegend = false;
                    }


                    #endregion
                }

                if (dt4.Rows.Count > 0)
                {
                    #region Sheet4
                    xlWorkSheet4.Cells[1, 1] = companyprofile[0];
                    xlWorkSheet4.Cells[2, 1] = companyprofile[1];
                    xlWorkSheet4.Cells[3, 1] = companyprofile[2];

                    xlWorkSheet4.Cells[5, 1] = data[11] + "   " + data[6];

                    xlWorkSheet4.Cells[6, 1] = data[0] + " " + data[1];
                    xlWorkSheet4.Cells[7, 1] = data[4] + " " + data[5];
                    xlWorkSheet4.Cells[6, 3] = data[7] + " " + data[8];
                    xlWorkSheet4.Cells[7, 3] = data[9] + " " + data[10];

                    xlWorkSheet4.Cells[8, 1] = data[2] + " " + data[3];

                    int row4 = 10;
                    int col8 = 0;

                    if (type1 == 3 || type1 == 4)
                    {
                        for (int col7 = 0; col7 < columnCounter4; col7++)
                        {
                            xlWorkSheet4.Cells[row4 + 1, 1] = columnNames4[col7].ToString();
                            row4++;
                        }

                        row4 = 10;
                        foreach (DataRow dr4 in dt4.Rows)
                        {
                            for (col8 = 0; col8 < columnCounter4; col8++)
                            {
                                xlWorkSheet4.Cells[row4 + 1, 2] = dr4[col8].ToString();
                                row4++;
                            }
                        }
                    }
                    else
                    {
                        for (int col7 = 0; col7 < columnCounter4; col7++)
                        {
                            xlWorkSheet4.Cells[row4 + 1, col7 + 1] = columnNames4[col7].ToString();
                        }

                        row4 = 10;
                        foreach (DataRow dr4 in dt4.Rows)
                        {
                            row4++;
                            for (col8 = 0; col8 < columnCounter4; col8++)
                            {
                                xlWorkSheet4.Cells[row4 + 1, col8 + 1] = dr4[col8].ToString();
                            }
                        }
                    }


                    xlWorkSheet4.Columns.AutoFit();
                    xlWorkSheet4.Cells[row4 + 50, 1] = xlShiftDown;

                    Excel.Range chartRange4;
                    top4 *= row4;

                    Excel.ChartObjects xlCharts4 = (Excel.ChartObjects)xlWorkSheet4.ChartObjects(Type.Missing);
                    Excel.ChartObject myChart4 = (Excel.ChartObject)xlCharts4.Add(left4, top4, width4, height4);
                    Excel.Chart chartPage4 = myChart4.Chart;

                    string lcolumn4 = string.Empty;

                    if (type1 == 3 || type1 == 4)
                    {
                        lcolumn4 = GetLastColumnName(2) + (row4);
                    }
                    else
                    {
                        lcolumn4 = GetLastColumnName(col8) + (row4 + 1);
                    }



                    chartRange4 = xlWorkSheet4.get_Range(celrange4, lcolumn4);
                    if (sourceColRow4 == "row")
                    {
                        chartPage4.SetSourceData(chartRange4, Excel.XlRowCol.xlRows);
                    }
                    else if (sourceColRow4 == "col")
                    {
                        chartPage4.SetSourceData(chartRange4, Excel.XlRowCol.xlColumns);
                    }
                    else
                    {
                        chartPage4.SetSourceData(chartRange4, misValue);
                    }
                    chartPage4.ChartType = GetChartType(type4);
                    chartPage4.HasTitle = true;
                    chartPage4.ChartTitle.Text = chartTitle[3];

                    if (type4 == 3)
                    {
                        chartPage4.ApplyDataLabels(Excel.XlDataLabelsType.xlDataLabelsShowLabelAndPercent,
                        misValue, misValue, misValue, misValue, misValue,
                        misValue, misValue, misValue, misValue);
                    }
                    else
                    {
                        chartPage4.HasDataTable = true;
                        chartPage4.DataTable.HasBorderOutline = true;
                        chartPage4.HasLegend = false;
                    }

                    #endregion
                }

                if (dt5.Rows.Count > 0)
                {
                    #region Sheet5
                    xlWorkSheet5.Cells[1, 1] = companyprofile[0];
                    xlWorkSheet5.Cells[2, 1] = companyprofile[1];
                    xlWorkSheet5.Cells[3, 1] = companyprofile[2];

                    xlWorkSheet5.Cells[5, 1] = data[11] + "   " + data[6];

                    xlWorkSheet5.Cells[6, 1] = data[0] + " " + data[1];
                    xlWorkSheet5.Cells[7, 1] = data[4] + " " + data[5];
                    xlWorkSheet5.Cells[6, 3] = data[7] + " " + data[8];
                    xlWorkSheet5.Cells[7, 3] = data[9] + " " + data[10];

                    xlWorkSheet5.Cells[8, 1] = data[2] + " " + data[3];

                    int row5 = 10;
                    int col10 = 0;

                    if (type1 == 3 || type1 == 4)
                    {
                        for (int col9 = 0; col9 < columnCounter5; col9++)
                        {
                            xlWorkSheet5.Cells[row5 + 1, 1] = columnNames5[col9].ToString();
                            row5++;
                        }

                        row5 = 10;
                        foreach (DataRow dr5 in dt5.Rows)
                        {
                            for (col10 = 0; col10 < columnCounter5; col10++)
                            {
                                xlWorkSheet5.Cells[row5 + 1, 2] = dr5[col10].ToString();
                                row5++;
                            }
                        }
                    }
                    else
                    {
                        for (int col9 = 0; col9 < columnCounter5; col9++)
                        {
                            xlWorkSheet5.Cells[row5 + 1, col9 + 1] = columnNames5[col9].ToString();
                        }

                        row5 = 10;
                        foreach (DataRow dr5 in dt5.Rows)
                        {
                            row5++;
                            for (col10 = 0; col10 < columnCounter5; col10++)
                            {
                                xlWorkSheet5.Cells[row5 + 1, col10 + 1] = dr5[col10].ToString();
                            }
                        }
                    }


                    xlWorkSheet5.Columns.AutoFit();
                    xlWorkSheet5.Cells[row5 + 50, 1] = xlShiftDown;

                    Excel.Range chartRange5;
                    top5 *= row5;

                    Excel.ChartObjects xlCharts5 = (Excel.ChartObjects)xlWorkSheet5.ChartObjects(Type.Missing);
                    Excel.ChartObject myChart5 = (Excel.ChartObject)xlCharts5.Add(left5, top5, width5, height5);
                    Excel.Chart chartPage5 = myChart5.Chart;

                    string lcolumn5 = string.Empty;

                    if (type1 == 3 || type1 == 4)
                    {
                        lcolumn5 = GetLastColumnName(2) + (row5);
                    }
                    else
                    {
                        lcolumn5 = GetLastColumnName(col10) + (row5 + 1);
                    }

                    chartRange5 = xlWorkSheet5.get_Range(celrange5, lcolumn5);
                    if (sourceColRow5 == "row")
                    {
                        chartPage5.SetSourceData(chartRange5, Excel.XlRowCol.xlRows);
                    }
                    else if (sourceColRow5 == "col")
                    {
                        chartPage5.SetSourceData(chartRange5, Excel.XlRowCol.xlColumns);
                    }
                    else
                    {
                        chartPage5.SetSourceData(chartRange5, misValue);
                    }
                    chartPage5.ChartType = GetChartType(type5);
                    chartPage5.HasTitle = true;
                    chartPage5.ChartTitle.Text = chartTitle[4];

                    if (type5 == 3)
                    {
                        chartPage5.ApplyDataLabels(Excel.XlDataLabelsType.xlDataLabelsShowLabelAndPercent,
                        misValue, misValue, misValue, misValue, misValue,
                        misValue, misValue, misValue, misValue);
                    }
                    else
                    {
                        chartPage5.HasDataTable = true;
                        chartPage5.DataTable.HasBorderOutline = true;
                        chartPage5.HasLegend = false;

                        if (chartTitle[4] == "TurnOver Distribution Per Year of Service")
                        {
                            Excel.ChartGroup grp = (Excel.ChartGroup)chartPage5.ChartGroups(1);
                            grp.VaryByCategories = true;
                        }
                    }

                    #endregion
                }

                xlWorkBook.Close(true, filename, misValue);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                xlWorkSheet = null;
                xlWorkBook = null;
                xlApp = null;
            }
        }

        public void creategraph(string filename, string[] chartTitle, string[] data, DataView dView, int columnCounter, string[] columnNames, int left, int top, int width, int height, int type1, string sourceColRow, string celrange, string[] coltitle
               , DataView dView2, int columnCounter2, string[] columnNames2, int left2, int top2, int width2, int height2, int type2, string sourceColRow2, string celrange2, string[] coltitle2
               , DataView dView3, int columnCounter3, string[] columnNames3, int left3, int top3, int width3, int height3, int type3, string sourceColRow3, string celrange3, string[] coltitle3
               , DataView dView4, int columnCounter4, string[] columnNames4, int left4, int top4, int width4, int height4, int type4, string sourceColRow4, string celrange4, string[] coltitle4
               , DataView dView5, int columnCounter5, string[] columnNames5, int left5, int top5, int width5, int height5, int type5, string sourceColRow5, string celrange5, string[] coltitle5)
        {
            System.Data.DataTable dt = dView.ToTable();
            System.Data.DataTable dt2 = dView2.ToTable();
            System.Data.DataTable dt3 = dView3.ToTable();
            System.Data.DataTable dt4 = dView4.ToTable();
            System.Data.DataTable dt5 = dView5.ToTable();
            Excel.Application xlApp;
            Excel.Workbook xlWorkBook;
            Excel.Worksheet xlWorkSheet;
            Excel.Worksheet xlWorkSheet2;
            Excel.Worksheet xlWorkSheet3;
            object misValue = System.Reflection.Missing.Value;

            xlApp = new Excel.ApplicationClass();
            xlWorkBook = xlApp.Workbooks.Add(misValue);
            xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);
            xlWorkSheet2 = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(2);
            xlWorkSheet3 = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(3);

            Excel.Worksheet xlWorkSheet4;
            xlWorkSheet4 = (Excel.Worksheet)xlWorkBook.Worksheets.Add(Missing.Value, Missing.Value, Missing.Value, Missing.Value);

            Excel.Worksheet xlWorkSheet5;
            xlWorkSheet5 = (Excel.Worksheet)xlWorkBook.Worksheets.Add(Missing.Value, Missing.Value, Missing.Value, Missing.Value);

            string xlShiftDown = "Printed on " + DateTime.Now.ToString("MMMM/dd/yyyy hh:mm tt");
            string[] companyprofile = GetCompanyProfile();


            try
            {
                if (dt.Rows.Count > 0)
                {
                    #region Sheet1
                    xlWorkSheet.Cells[1, 1] = companyprofile[0];
                    xlWorkSheet.Cells[2, 1] = companyprofile[1];
                    xlWorkSheet.Cells[3, 1] = companyprofile[2];

                    xlWorkSheet.Cells[5, 1] = data[11] + "   " + data[6];

                    xlWorkSheet.Cells[6, 1] = data[0] + " " + data[1];
                    xlWorkSheet.Cells[7, 1] = data[4] + " " + data[5];
                    xlWorkSheet.Cells[6, 3] = data[7] + " " + data[8];
                    xlWorkSheet.Cells[7, 3] = data[9] + " " + data[10];

                    xlWorkSheet.Cells[8, 1] = data[2] + " " + data[3];


                    int row = 10;
                    int col2 = 0;

                    if (type1 == 3 || type1 == 4)
                    {
                        xlWorkSheet.Cells[10, 1] = coltitle[0];
                        xlWorkSheet.Cells[10, 2] = coltitle[1];

                        for (int col = 0; col < columnCounter; col++)
                        {
                            xlWorkSheet.Cells[row + 1, 1] = columnNames[col].ToString();
                            row++;
                        }

                        row = 10;
                        foreach (DataRow dr in dt.Rows)
                        {

                            for (col2 = 0; col2 < columnCounter; col2++)
                            {
                                xlWorkSheet.Cells[row + 1, 2] = dr[col2].ToString();
                                row++;
                            }
                        }
                    }
                    else
                    {
                        for (int col = 0; col < columnCounter; col++)
                        {
                            xlWorkSheet.Cells[row + 1, col + 1] = columnNames[col].ToString();
                        }

                        row = 10;
                        foreach (DataRow dr in dt.Rows)
                        {
                            row++;
                            for (col2 = 0; col2 < columnCounter; col2++)
                            {
                                xlWorkSheet.Cells[row + 1, col2 + 1] = dr[col2].ToString();
                            }
                        }
                    }

                    xlWorkSheet.Columns.AutoFit();
                    xlWorkSheet.Cells[row + 50, 1] = xlShiftDown;

                    Excel.Range chartRange;
                    top *= row;

                    Excel.ChartObjects xlCharts = (Excel.ChartObjects)xlWorkSheet.ChartObjects(Type.Missing);
                    Excel.ChartObject myChart = (Excel.ChartObject)xlCharts.Add(left, top, width, height);
                    Excel.Chart chartPage = myChart.Chart;


                    string lcolumn = string.Empty;

                    if (type1 == 3 || type1 == 4)
                    {
                        lcolumn = GetLastColumnName(2) + (row);
                    }
                    else
                    {
                        lcolumn = GetLastColumnName(col2) + (row + 1);
                    }


                    chartRange = xlWorkSheet.get_Range(celrange, lcolumn);
                    if (sourceColRow == "row")
                    {
                        chartPage.SetSourceData(chartRange, Excel.XlRowCol.xlRows);
                    }
                    else if (sourceColRow == "col")
                    {
                        chartPage.SetSourceData(chartRange, Excel.XlRowCol.xlColumns);
                    }
                    else
                    {
                        chartPage.SetSourceData(chartRange, misValue);
                    }
                    chartPage.ChartType = GetChartType(type1);
                    chartPage.HasTitle = true;
                    chartPage.ChartTitle.Text = chartTitle[0];


                    if (type1 == 3)
                    {
                        chartPage.ApplyDataLabels(Excel.XlDataLabelsType.xlDataLabelsShowLabelAndPercent,
                        misValue, misValue, misValue, misValue, misValue,
                        misValue, misValue, misValue, misValue);
                    }
                    else
                    {
                        chartPage.HasDataTable = true;
                        chartPage.DataTable.HasBorderOutline = true;
                        chartPage.HasLegend = false;
                    }

                    #endregion
                }

                if (dt2.Rows.Count > 0)
                {
                    #region Sheet2
                    xlWorkSheet2.Cells[1, 1] = companyprofile[0];
                    xlWorkSheet2.Cells[2, 1] = companyprofile[1];
                    xlWorkSheet2.Cells[3, 1] = companyprofile[2];

                    xlWorkSheet2.Cells[5, 1] = data[11] + "   " + data[6];

                    xlWorkSheet2.Cells[6, 1] = data[0] + " " + data[1];
                    xlWorkSheet2.Cells[7, 1] = data[4] + " " + data[5];
                    xlWorkSheet2.Cells[6, 3] = data[7] + " " + data[8];
                    xlWorkSheet2.Cells[7, 3] = data[9] + " " + data[10];

                    xlWorkSheet2.Cells[8, 1] = data[2] + " " + data[3];

                    int row2 = 10;
                    int col4 = 0;


                    if (type1 == 3 || type1 == 4)
                    {
                        xlWorkSheet2.Cells[10, 1] = coltitle2[0];
                        xlWorkSheet2.Cells[10, 2] = coltitle2[1];

                        for (int col3 = 0; col3 < columnCounter2; col3++)
                        {
                            xlWorkSheet2.Cells[row2 + 1, 1] = columnNames2[col3].ToString();
                            row2++;
                        }

                        row2 = 10;

                        foreach (DataRow dr2 in dt2.Rows)
                        {

                            for (col4 = 0; col4 < columnCounter2; col4++)
                            {
                                xlWorkSheet2.Cells[row2 + 1, 2] = dr2[col4].ToString();
                                row2++;
                            }
                        }
                    }
                    else
                    {
                        for (int col3 = 0; col3 < columnCounter2; col3++)
                        {
                            xlWorkSheet2.Cells[row2 + 1, col3 + 1] = columnNames2[col3].ToString();
                        }

                        row2 = 10;

                        foreach (DataRow dr2 in dt2.Rows)
                        {
                            row2++;
                            for (col4 = 0; col4 < columnCounter2; col4++)
                            {
                                xlWorkSheet2.Cells[row2 + 1, col4 + 1] = dr2[col4].ToString();
                            }
                        }
                    }




                    xlWorkSheet2.Columns.AutoFit();
                    xlWorkSheet2.Cells[row2 + 50, 1] = xlShiftDown;

                    Excel.Range chartRange2;
                    top2 *= row2;

                    Excel.ChartObjects xlCharts2 = (Excel.ChartObjects)xlWorkSheet2.ChartObjects(Type.Missing);
                    Excel.ChartObject myChart2 = (Excel.ChartObject)xlCharts2.Add(left2, top2, width2, height2);
                    Excel.Chart chartPage2 = myChart2.Chart;

                    string lcolumn2 = string.Empty;

                    if (type1 == 3 || type1 == 4)
                    {
                        lcolumn2 = GetLastColumnName(2) + (row2);
                    }
                    else
                    {
                        lcolumn2 = GetLastColumnName(col4) + (row2 + 1);
                    }


                    chartRange2 = xlWorkSheet2.get_Range(celrange2, lcolumn2);
                    if (sourceColRow2 == "row")
                    {
                        chartPage2.SetSourceData(chartRange2, Excel.XlRowCol.xlRows);
                    }
                    else if (sourceColRow2 == "col")
                    {
                        chartPage2.SetSourceData(chartRange2, Excel.XlRowCol.xlColumns);
                    }
                    else
                    {
                        chartPage2.SetSourceData(chartRange2, misValue);
                    }
                    chartPage2.ChartType = GetChartType(type2);
                    chartPage2.HasTitle = true;
                    chartPage2.ChartTitle.Text = chartTitle[1];

                    if (type2 == 3)
                    {
                        chartPage2.ApplyDataLabels(Excel.XlDataLabelsType.xlDataLabelsShowLabelAndPercent,
                        misValue, misValue, misValue, misValue, misValue,
                        misValue, misValue, misValue, misValue);
                    }
                    else
                    {
                        chartPage2.HasDataTable = true;
                        chartPage2.DataTable.HasBorderOutline = true;
                        chartPage2.HasLegend = false;
                    }


                    #endregion
                }

                if (dt3.Rows.Count > 0)
                {
                    #region Sheet3
                    xlWorkSheet3.Cells[1, 1] = companyprofile[0];
                    xlWorkSheet3.Cells[2, 1] = companyprofile[1];
                    xlWorkSheet3.Cells[3, 1] = companyprofile[2];

                    xlWorkSheet3.Cells[5, 1] = data[11] + "   " + data[6];

                    xlWorkSheet3.Cells[6, 1] = data[0] + " " + data[1];
                    xlWorkSheet3.Cells[7, 1] = data[4] + " " + data[5];
                    xlWorkSheet3.Cells[6, 3] = data[7] + " " + data[8];
                    xlWorkSheet3.Cells[7, 3] = data[9] + " " + data[10];

                    xlWorkSheet3.Cells[8, 1] = data[2] + " " + data[3];

                    int row3 = 10;
                    int col6 = 0;

                    if (type1 == 3 || type1 == 4)
                    {
                        xlWorkSheet3.Cells[10, 1] = coltitle3[0];
                        xlWorkSheet3.Cells[10, 2] = coltitle3[1];

                        for (int col5 = 0; col5 < columnCounter3; col5++)
                        {
                            xlWorkSheet3.Cells[row3 + 1, 1] = columnNames3[col5].ToString();
                            row3++;
                        }

                        row3 = 10;
                        foreach (DataRow dr3 in dt3.Rows)
                        {

                            for (col6 = 0; col6 < columnCounter3; col6++)
                            {
                                xlWorkSheet3.Cells[row3 + 1, 2] = dr3[col6].ToString();
                                row3++;
                            }
                        }
                    }
                    else
                    {
                        for (int col5 = 0; col5 < columnCounter3; col5++)
                        {
                            xlWorkSheet3.Cells[row3 + 1, col5 + 1] = columnNames3[col5].ToString();
                        }

                        row3 = 10;
                        foreach (DataRow dr3 in dt3.Rows)
                        {
                            row3++;
                            for (col6 = 0; col6 < columnCounter3; col6++)
                            {
                                xlWorkSheet3.Cells[row3 + 1, col6 + 1] = dr3[col6].ToString();
                            }
                        }
                    }


                    xlWorkSheet3.Columns.AutoFit();
                    xlWorkSheet3.Cells[row3 + 50, 1] = xlShiftDown;

                    Excel.Range chartRange3;
                    top3 *= row3;

                    Excel.ChartObjects xlCharts3 = (Excel.ChartObjects)xlWorkSheet3.ChartObjects(Type.Missing);
                    Excel.ChartObject myChart3 = (Excel.ChartObject)xlCharts3.Add(left3, top3, width3, height3);
                    Excel.Chart chartPage3 = myChart3.Chart;

                    string lcolumn3 = string.Empty;

                    if (type1 == 3 || type1 == 4)
                    {
                        lcolumn3 = GetLastColumnName(2) + (row3);
                    }
                    else
                    {
                        lcolumn3 = GetLastColumnName(col6) + (row3 + 1);
                    }

                    chartRange3 = xlWorkSheet3.get_Range(celrange3, lcolumn3);
                    if (sourceColRow3 == "row")
                    {
                        chartPage3.SetSourceData(chartRange3, Excel.XlRowCol.xlRows);
                    }
                    else if (sourceColRow3 == "col")
                    {
                        chartPage3.SetSourceData(chartRange3, Excel.XlRowCol.xlColumns);
                    }
                    else
                    {
                        chartPage3.SetSourceData(chartRange3, misValue);
                    }
                    chartPage3.ChartType = GetChartType(type3);
                    chartPage3.HasTitle = true;
                    chartPage3.ChartTitle.Text = chartTitle[2];

                    if (type3 == 3)
                    {
                        chartPage3.ApplyDataLabels(Excel.XlDataLabelsType.xlDataLabelsShowLabelAndPercent,
                        misValue, misValue, misValue, misValue, misValue,
                        misValue, misValue, misValue, misValue);
                    }
                    else
                    {
                        chartPage3.HasDataTable = true;
                        chartPage3.DataTable.HasBorderOutline = true;
                        chartPage3.HasLegend = false;
                    }


                    #endregion
                }

                if (dt4.Rows.Count > 0)
                {
                    #region Sheet4
                    xlWorkSheet4.Cells[1, 1] = companyprofile[0];
                    xlWorkSheet4.Cells[2, 1] = companyprofile[1];
                    xlWorkSheet4.Cells[3, 1] = companyprofile[2];

                    xlWorkSheet4.Cells[5, 1] = data[11] + "   " + data[6];

                    xlWorkSheet4.Cells[6, 1] = data[0] + " " + data[1];
                    xlWorkSheet4.Cells[7, 1] = data[4] + " " + data[5];
                    xlWorkSheet4.Cells[6, 3] = data[7] + " " + data[8];
                    xlWorkSheet4.Cells[7, 3] = data[9] + " " + data[10];

                    xlWorkSheet4.Cells[8, 1] = data[2] + " " + data[3];

                    int row4 = 10;
                    int col8 = 0;

                    if (type1 == 3 || type1 == 4)
                    {
                        xlWorkSheet4.Cells[10, 1] = coltitle4[0];
                        xlWorkSheet4.Cells[10, 2] = coltitle4[1];

                        for (int col7 = 0; col7 < columnCounter4; col7++)
                        {
                            xlWorkSheet4.Cells[row4 + 1, 1] = columnNames4[col7].ToString();
                            row4++;
                        }

                        row4 = 10;
                        foreach (DataRow dr4 in dt4.Rows)
                        {
                            for (col8 = 0; col8 < columnCounter4; col8++)
                            {
                                xlWorkSheet4.Cells[row4 + 1, 2] = dr4[col8].ToString();
                                row4++;
                            }
                        }
                    }
                    else
                    {
                        for (int col7 = 0; col7 < columnCounter4; col7++)
                        {
                            xlWorkSheet4.Cells[row4 + 1, col7 + 1] = columnNames4[col7].ToString();
                        }

                        row4 = 10;
                        foreach (DataRow dr4 in dt4.Rows)
                        {
                            row4++;
                            for (col8 = 0; col8 < columnCounter4; col8++)
                            {
                                xlWorkSheet4.Cells[row4 + 1, col8 + 1] = dr4[col8].ToString();
                            }
                        }
                    }


                    xlWorkSheet4.Columns.AutoFit();
                    xlWorkSheet4.Cells[row4 + 50, 1] = xlShiftDown;

                    Excel.Range chartRange4;
                    top4 *= row4;

                    Excel.ChartObjects xlCharts4 = (Excel.ChartObjects)xlWorkSheet4.ChartObjects(Type.Missing);
                    Excel.ChartObject myChart4 = (Excel.ChartObject)xlCharts4.Add(left4, top4, width4, height4);
                    Excel.Chart chartPage4 = myChart4.Chart;

                    string lcolumn4 = string.Empty;

                    if (type1 == 3 || type1 == 4)
                    {
                        lcolumn4 = GetLastColumnName(2) + (row4);
                    }
                    else
                    {
                        lcolumn4 = GetLastColumnName(col8) + (row4 + 1);
                    }



                    chartRange4 = xlWorkSheet4.get_Range(celrange4, lcolumn4);
                    if (sourceColRow4 == "row")
                    {
                        chartPage4.SetSourceData(chartRange4, Excel.XlRowCol.xlRows);
                    }
                    else if (sourceColRow4 == "col")
                    {
                        chartPage4.SetSourceData(chartRange4, Excel.XlRowCol.xlColumns);
                    }
                    else
                    {
                        chartPage4.SetSourceData(chartRange4, misValue);
                    }
                    chartPage4.ChartType = GetChartType(type4);
                    chartPage4.HasTitle = true;
                    chartPage4.ChartTitle.Text = chartTitle[3];

                    if (type4 == 3)
                    {
                        chartPage4.ApplyDataLabels(Excel.XlDataLabelsType.xlDataLabelsShowLabelAndPercent,
                        misValue, misValue, misValue, misValue, misValue,
                        misValue, misValue, misValue, misValue);
                    }
                    else
                    {
                        chartPage4.HasDataTable = true;
                        chartPage4.DataTable.HasBorderOutline = true;
                        chartPage4.HasLegend = false;
                    }

                    #endregion
                }

                if (dt5.Rows.Count > 0)
                {
                    #region Sheet5
                    xlWorkSheet5.Cells[1, 1] = companyprofile[0];
                    xlWorkSheet5.Cells[2, 1] = companyprofile[1];
                    xlWorkSheet5.Cells[3, 1] = companyprofile[2];

                    xlWorkSheet5.Cells[5, 1] = data[11] + "   " + data[6];

                    xlWorkSheet5.Cells[6, 1] = data[0] + " " + data[1];
                    xlWorkSheet5.Cells[7, 1] = data[4] + " " + data[5];
                    xlWorkSheet5.Cells[6, 3] = data[7] + " " + data[8];
                    xlWorkSheet5.Cells[7, 3] = data[9] + " " + data[10];

                    xlWorkSheet5.Cells[8, 1] = data[2] + " " + data[3];

                    int row5 = 10;
                    int col10 = 0;

                    if (type1 == 3 || type1 == 4)
                    {
                        xlWorkSheet5.Cells[10, 1] = coltitle5[0];
                        xlWorkSheet5.Cells[10, 2] = coltitle5[1];

                        for (int col9 = 0; col9 < columnCounter5; col9++)
                        {
                            xlWorkSheet5.Cells[row5 + 1, 1] = columnNames5[col9].ToString();
                            row5++;
                        }

                        row5 = 10;
                        foreach (DataRow dr5 in dt5.Rows)
                        {
                            for (col10 = 0; col10 < columnCounter5; col10++)
                            {
                                xlWorkSheet5.Cells[row5 + 1, 2] = dr5[col10].ToString();
                                row5++;
                            }
                        }
                    }
                    else
                    {
                        for (int col9 = 0; col9 < columnCounter5; col9++)
                        {
                            xlWorkSheet5.Cells[row5 + 1, col9 + 1] = columnNames5[col9].ToString();
                        }

                        row5 = 10;
                        foreach (DataRow dr5 in dt5.Rows)
                        {
                            row5++;
                            for (col10 = 0; col10 < columnCounter5; col10++)
                            {
                                xlWorkSheet5.Cells[row5 + 1, col10 + 1] = dr5[col10].ToString();
                            }
                        }
                    }


                    xlWorkSheet5.Columns.AutoFit();
                    xlWorkSheet5.Cells[row5 + 50, 1] = xlShiftDown;

                    Excel.Range chartRange5;
                    top5 *= row5;

                    Excel.ChartObjects xlCharts5 = (Excel.ChartObjects)xlWorkSheet5.ChartObjects(Type.Missing);
                    Excel.ChartObject myChart5 = (Excel.ChartObject)xlCharts5.Add(left5, top5, width5, height5);
                    Excel.Chart chartPage5 = myChart5.Chart;

                    string lcolumn5 = string.Empty;

                    if (type1 == 3 || type1 == 4)
                    {
                        lcolumn5 = GetLastColumnName(2) + (row5);
                    }
                    else
                    {
                        lcolumn5 = GetLastColumnName(col10) + (row5 + 1);
                    }

                    chartRange5 = xlWorkSheet5.get_Range(celrange5, lcolumn5);
                    if (sourceColRow5 == "row")
                    {
                        chartPage5.SetSourceData(chartRange5, Excel.XlRowCol.xlRows);
                    }
                    else if (sourceColRow5 == "col")
                    {
                        chartPage5.SetSourceData(chartRange5, Excel.XlRowCol.xlColumns);
                    }
                    else
                    {
                        chartPage5.SetSourceData(chartRange5, misValue);
                    }
                    chartPage5.ChartType = GetChartType(type5);
                    chartPage5.HasTitle = true;
                    chartPage5.ChartTitle.Text = chartTitle[4];

                    if (type5 == 3)
                    {
                        chartPage5.ApplyDataLabels(Excel.XlDataLabelsType.xlDataLabelsShowLabelAndPercent,
                        misValue, misValue, misValue, misValue, misValue,
                        misValue, misValue, misValue, misValue);
                    }
                    else
                    {
                        chartPage5.HasDataTable = true;
                        chartPage5.DataTable.HasBorderOutline = true;
                        chartPage5.HasLegend = false;

                        if (chartTitle[4] == "TurnOver Distribution Per Year of Service")
                        {
                            Excel.ChartGroup grp = (Excel.ChartGroup)chartPage5.ChartGroups(1);
                            grp.VaryByCategories = true;
                        }
                    }

                    #endregion
                }

                xlWorkBook.Close(true, filename, misValue);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                xlWorkSheet = null;
                xlWorkBook = null;
                xlApp = null;
            }
        }

        public string GetLastColumnName(int lastColumnIndex)
        {
            string lastColumn = "";

            // check whether the column count is > 26
            if (lastColumnIndex > 26)
            {
                // If the column count is > 26, the the last column index will be something 
                // like "AA", "DE", "BC" etc

                // Get the first letter
                // ASCII index 65 represent char. 'A'. So, we use 64 in this calculation as a starting point
                char first = Convert.ToChar(64 + ((lastColumnIndex - 1) / 26));

                // Get the second letter
                char second = Convert.ToChar(64 + (lastColumnIndex % 26 == 0 ? 26 : lastColumnIndex % 26));

                // Concat. them
                lastColumn = first.ToString() + second.ToString();
            }
            else
            {
                // ASCII index 65 represent char. 'A'. So, we use 64 in this calculation as a starting point
                lastColumn = Convert.ToChar(64 + lastColumnIndex).ToString();
            }
            return lastColumn;
        }

        public Excel.XlChartType GetChartType(int type)
        {
            Excel.XlChartType chrt = Excel.XlChartType.xlBarClustered;
            if (type == 1)
            {
                chrt = Excel.XlChartType.xlColumnClustered;
            }
            else if(type == 2)
            {
                chrt = Excel.XlChartType.xlColumnStacked;
            }
            else if (type == 3)
            {
                chrt = Excel.XlChartType.xlPie;
            }
            else if (type == 4)
            {
                chrt = Excel.XlChartType.xlLineMarkers;
            }
            else if (type == 5)
            {
                //used for built-in line-col graph
                chrt = chrt = Excel.XlChartType.xlColumnClustered;
            }
            else if (type == 6)
            {
                //used for customized stacked graph
                chrt = chrt = Excel.XlChartType.xlColumnStacked;
            }

            return chrt;
        }

        private string[] GetCompanyProfile()
        {
            DataTable dt;
            string query = @"
                SELECT Ccd_CompanyCode
	                ,Ccd_CompanyName  
	                ,Ccd_CompanyAddress1  
	                ,Ccd_CompanyAddress2+' '+Adt_Accountdesc 'Ccd_CompanyAddress2'
                FROM T_CompanyMaster
                INNER JOIN T_AccountDetail 
				ON Ccd_CompanyAddress3 = Adt_AccountCode and Adt_AccountType = 'zipcode'";

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dt = dal.ExecuteDataSet(query).Tables[0];
                dal.CloseDB();
            }


            string[] comp = new string[3];
            comp[0] = dt.Rows[0]["Ccd_CompanyName"].ToString();
            comp[1] = dt.Rows[0]["Ccd_CompanyAddress1"].ToString();
            comp[2] = dt.Rows[0]["Ccd_CompanyAddress2"].ToString();

            return comp;
        }

    }
}
