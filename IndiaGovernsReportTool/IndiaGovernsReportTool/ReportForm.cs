using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Drawing.Imaging;
using System.Threading;
using System.IO;
using System.Drawing.Printing;
using PdfSharp;
using PdfSharp.Pdf;
using PdfSharp.Drawing;
using System.Windows.Forms.DataVisualization.Charting;

namespace IndiaGovernsReportTool
{
    public partial class ReportForm : Form
    {
        private String reportName;
        private bool first = true;
        private bool second = false;
        private String fileName = String.Empty;
        Bitmap bitmap;
        Graphics graphic;
        int initScroll = 0;
        int _COLUMNWIDTH_ = 100;
        const string SuperscriptDigits = 
                "\u2070\u00b9\u00b2\u00b3\u2074\u2075\u2076\u2077\u2078\u2079";

        public string year;

        public ReportForm(Report report)
        {
            InitializeComponent();
            dataGridView1.DataSource = report.GeneralData;
            dataGridView2.DataSource = report.Group1Data;
            dataGridView3.DataSource = report.Group2Data;
            lblIntro.Text = report.Intro;
            lblComment.Text = report.Comment;

            DataTable dtFull = new DataTable();
            DataTable dtSub1 = new DataTable();
            DataTable dtSub2 = new DataTable();
            DataTable dtSub3 = new DataTable();

            
            foreach (var col in report.GeneralData.Columns.Cast<DataColumn>())
            {
                dtSub1.Columns.Add(col.ColumnName);
                dtFull.Columns.Add(col.ColumnName);

                //second column needs to be state avg
                if(!dtFull.Columns.Contains("StateAvg")) dtFull.Columns.Add("StateAvg");
            }

            dtFull.Rows.Add();
            dtFull.Rows[0]["StateAvg"] = "State Avg. per Constituency";

            foreach (var col in report.GeneralData.Columns.Cast<DataColumn>())
            {
                var columnNameWords = col.ColumnName.Split(' ');
                //insert spaces so that headers wrap, if necessary
                for (int i = 0; i < columnNameWords.Length; i++ )
                {
                    String word = columnNameWords[i];
                    if (word.Length > 9)
                    {
                        columnNameWords[i] = word.Insert(word.Length/2, " ");
                    }
                }
                dtFull.Rows[0][col.ColumnName] = String.Join(" ", columnNameWords);
            }
            foreach (var col in report.Group1Data.Columns.Cast<DataColumn>()) dtSub2.Columns.Add(col.ColumnName);
            foreach (var col in report.Group2Data.Columns.Cast<DataColumn>()) dtSub3.Columns.Add(col.ColumnName);
            dtSub1.Rows.Add("General", "", "", "", "");
            dtSub2.Rows.Add(report.Group1Name, "State Avg. Per Rural Constituency", "", "", "");
            dtSub3.Rows.Add(report.Group2Name, "State Avg. Per Rural Constituency", "", "", "");

            subHeader3.Visible = false; //temp
            dataGridView3.Visible = false; //temp
            this.reportName = report.ReportName;
            bindToChart(chart1, report.Group1Data, report.Chart1Column1, report.Chart1Column2);
            //using Group1Data for now since no data selected for group2
            bindToChart(chart2, report.Group1Data, report.Chart2Column1, report.Chart2Column2);

            //start temp code                    
            var dataColumn = "";
            for (int i = 0; i < report.Group1Data.Rows.Count; i++)
            {
                if (i % 3 == 0)
                {
                    dataColumn = report.Group1Data.Rows[i][0].ToString().Replace("(s1)", "");
                    report.Group1Data.Rows[i][0] = report.Group1Data.Rows[i][0].ToString().Replace("(s1)", SuperscriptDigits[1].ToString());
                }
                else
                {
                    report.Group1Data.Rows[i][0] = report.Group1Data.Rows[i][0].ToString().Replace(dataColumn, "   ");
                }
            }
            //end temp code for curating data

            fullHeader.DataSource = dtFull;
            subHeader3.DataSource = dtSub3;

            lblRank.Text = report.Rank;
            lblRank.Find(report.ReportName + " MLA Constituency Rank");
            lblRank.SelectionFont = new Font("Cambria", 16, FontStyle.Bold);
            lblRank.SelectAll();
            lblRank.SelectionAlignment = HorizontalAlignment.Center;
            lblName.Text = report.ReportName + " MLA Constituency";
            lblName.Select();
        }


        private void bindToChart(Chart chart, DataTable data, String chartColumn1, String chartColumn2 = null)
        {
            string dataColumnName = data.Columns.Cast<DataColumn>().Where(p => p.ColumnName.Contains("Data")).First().ColumnName;
            var chartData = data.Rows.Cast<DataRow>()
                                   .Where(p => p[dataColumnName].ToString() == chartColumn1)
                                   .First();

            var xvalues = data.Columns.Cast<DataColumn>()
                                .Select(p => p.ColumnName)
                                .Where(name => name != dataColumnName).ToArray();

            Double[] yValuesArray = getYValues(chartData);

            chart.Series[0].Points.DataBindXY(xvalues, yValuesArray);
            //to show the value on top of the chart
            chart.Series[0].Label = "#VALY"; 
           
            //temp
            chart.Series[0].Name = "Govt.";

            chart.Titles.Add(chartColumn1.Replace("Govt.", "")); //temp only for removing the suffix
            chart.Titles[0].Font = new Font("Cambria", 14, FontStyle.Bold);

            chart.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
            chart.ChartAreas[0].AxisY.MajorGrid.Enabled = false;

            chart.ChartAreas[0].AxisX.LabelStyle.IsEndLabelVisible = false;
            chart.ChartAreas[0].AxisY.Enabled = AxisEnabled.False;

            chart.Series[0]["PointWidth"] = "0.5";
            chart.Series[0].Points[0].Color = Color.FromArgb(37, 64, 97);
            chart.Series[0].Points[1].Color = Color.FromArgb(185, 205, 229);

            if (chartColumn2 != null)
            {
                var chartData2 = data.Rows.Cast<DataRow>()
                                   .Where(p => p[dataColumnName].ToString() == chartColumn2)
                                   .First();
                Double[] yValuesArray2 = getYValues(chartData2);
                chart.Series.Add("Series2");
                chart.Series[1].Points.DataBindXY(xvalues, yValuesArray2);
                chart.Series[1].Label = "#VALY";
                chart.Series[1].Color = Color.FromArgb(200, 90, 100);
                chart.Series[1]["PointWidth"] = "0.5";
                chart.Series[1].Points[0].Color = Color.FromArgb(135, 45, 50);
                chart.Series[1].Points[1].Color = Color.FromArgb(230, 175, 180);
                chart.Series[1].Name = "Private";
                chart.Legends.Add("Legend");
                chart.Legends[0].Docking = Docking.Top;
            }
        }

        private Double[] getYValues(DataRow chartData)
        {
            ArrayList yvalues = new ArrayList();
            for (int i = 1; i < chartData.ItemArray.Count(); i++)
                yvalues.Add(Convert.ToDouble(chartData.ItemArray[i].ToString()));

            return (Double[])yvalues.ToArray(typeof(Double));
        }

        private void onDataGridViewBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            DataGridView dg = (DataGridView)sender;
            dg.CurrentCell = null;

            dg.Height = 0;
            foreach (DataGridViewRow row in dg.Rows)
            {
                dg.Height += row.Height;
            }

            //to account for wierd datagrid height issue - 
            dg.Height = 4 * dg.Height / 5;
        }


        private void ReportForm_Load(object sender, EventArgs e)
        {
            //scaling just to get better clarity
            this.Scale(new SizeF((float)1.3, (float)1.3));
        }

        private void ReportForm_Shown(object sender, EventArgs e)
        {
            if (first)
            {
                //formatting the datagrids width
                DataGridView[] allGrids = new DataGridView[] { fullHeader, dataGridView1, 
                dataGridView2, dataGridView3, subHeader3 };

                DataGridView[] lowerGrids = new DataGridView[] { fullHeader, dataGridView2, dataGridView3, subHeader3 };

                int firstColumnWidth = dataGridView1.Columns[0].Width;
                int secondColumnWidth = _COLUMNWIDTH_;

                foreach (var grid in allGrids.Except(lowerGrids))
                    grid.Columns[0].Width = firstColumnWidth + secondColumnWidth;

                foreach (var grid in lowerGrids)
                {
                    grid.Columns[0].Width = firstColumnWidth;
                    grid.Columns[1].Width = secondColumnWidth;
                }


                // divide width remaining into equally in 4 parts
                int remainingColumnsWidth = (dataGridView1.Width - firstColumnWidth - secondColumnWidth) / 4;

                for (int i = 2; i < 6; i++)
                {
                    foreach (var grid in allGrids.Except(lowerGrids))                    
                        grid.Columns[i - 1].Width = remainingColumnsWidth;                    

                    foreach (var grid in lowerGrids)
                        grid.Columns[i].Width = remainingColumnsWidth;
                }


                foreach (var grid in allGrids)
                {
                    //for numbers
                    for (int i = 1; i < grid.Columns.Count; i++)
                        grid.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

                    grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
                    grid.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
                    grid.AutoSize = true;
                }

                fullHeader.AutoSize = false;
                fullHeader.Height = fullHeader.Rows[0].Height;

                //size up the comment 

                lblComment.Height = flowLayoutPanel2.Height -
                    flowLayoutPanel2.Controls.Cast<Control>()
                        .Where(p => p.Visible)
                        .Where(p => p != lblComment)
                        .Select(p => p.Height).Sum() - 10; //for buffer
            }

            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork +=
                new DoWorkEventHandler(bw_DoWork);
            worker.RunWorkerCompleted +=
                new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted); 
            //for second scrshot
            if (first) 
            {
                this.ScrollControlIntoView(pictureBox1);
                //this is used later to determine where the initial scroll started - use to position the second screen capture exactly.
                initScroll = this.VerticalScroll.Value;
            }
            else if (second)
            {
                this.ScrollControlIntoView(dataGridView2);
            }
            else
            {
                this.ScrollControlIntoView(lblEndBlank);
            }

            //this has to work asynchronously, so that the UI does not freeze up and all the controls complete 
            // loading before the scrshot is taken
            worker.RunWorkerAsync();
        }

        private void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            //to allow the report to load fully
            Thread.Sleep(1500);
            Rectangle form = this.Bounds;

            if (first)
            {
                //print to a bmp and save file
                Rectangle panel = flowLayoutPanel1.Bounds;
                bitmap = new Bitmap(panel.Width, panel.Height);
                graphic = Graphics.FromImage(bitmap);
                graphic.CopyFromScreen(form.Location, Point.Empty, form.Size);
            }
            else
            {
                graphic.CopyFromScreen(form.Location, new Point(0, this.VerticalScroll.Value - initScroll), form.Size);
                
                //last only
                if (!second)
                {
                    string pathPdf = "D:\\IndiaGovernsReports\\" + this.reportName + ".pdf";
                    saveAsPdf(bitmap, pathPdf);
                }
            }
        }

        /// <summary>
        /// Takes a Bitmap and a file path and saves the bitmap as a pdf
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="pathPdf"></param>
        private void saveAsPdf(Bitmap bitmap, string pathPdf)
        {
            PdfDocument doc = new PdfDocument();
            doc.Pages.Add(new PdfPage());
            XGraphics xgr = XGraphics.FromPdfPage(doc.Pages[0]);
            XImage img = XImage.FromGdiPlusImage(bitmap);
            xgr.DrawImage(img, 0, 0, doc.Pages[0].Width, doc.Pages[0].Height);
            doc.Save(pathPdf);
            doc.Close();
        }


        private void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.ScrollControlIntoView(lblEndBlank);
            if (first)
            {
                first = false;
                second = true;
                ReportForm_Shown(null, null);
            }
            else if (second)
            {
                second = false;
                ReportForm_Shown(null, null);
            }
            else
            {
                this.Close();
            }
        }

        /// <summary>
        /// This is mainly to paint a colored border around a windows forms control - no property present in the control.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BorderPaintBlue(object sender, PaintEventArgs e)
        {
            ControlPaint.DrawBorder(e.Graphics, ((Control)sender).DisplayRectangle, 
                                    Color.FromArgb(220, 230, 242), 3, ButtonBorderStyle.Solid, 
                                    Color.FromArgb(220, 230, 242), 3, ButtonBorderStyle.Solid, 
                                    Color.FromArgb(220, 230, 242), 3, ButtonBorderStyle.Solid, 
                                    Color.FromArgb(220, 230, 242), 3, ButtonBorderStyle.Solid);
        }

        /// <summary>
        /// This is mainly to paint a colored border around a windows forms control - no property present in the control.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BorderPaintBlack(object sender, PaintEventArgs e)
        {
            ControlPaint.DrawBorder(e.Graphics, ((Control)sender).DisplayRectangle, Color.Black, ButtonBorderStyle.Solid);
        }

        private void BorderPaintFullHeader(object sender, PaintEventArgs e)
        {
            ControlPaint.DrawBorder(e.Graphics, ((Control)sender).DisplayRectangle,
                        Color.White, 1, ButtonBorderStyle.Solid,
                        Color.FromArgb(31, 73, 125), 3, ButtonBorderStyle.Solid,
                        Color.White, 1, ButtonBorderStyle.Solid,
                        Color.FromArgb(31, 73, 125), 3, ButtonBorderStyle.Solid);
        }


        private void BorderPaintDataGrid(object sender, PaintEventArgs e)
        {
            ControlPaint.DrawBorder(e.Graphics, ((Control)sender).DisplayRectangle,
                        Color.White, 1, ButtonBorderStyle.Solid,
                        Color.FromArgb(220, 230, 242), 0, ButtonBorderStyle.Solid,
                        Color.White, 1, ButtonBorderStyle.Solid,
                        Color.FromArgb(31, 73, 125), 0, ButtonBorderStyle.Solid);
        }


        private void subHeaderAvg_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.Value.ToString().Contains("Avg"))
            {
                e.CellStyle.Font = new Font("Cambria", 8, FontStyle.Italic);
                e.CellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
        }
    }
}