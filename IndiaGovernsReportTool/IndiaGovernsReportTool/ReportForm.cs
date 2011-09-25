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

namespace IndiaGovernsReportTool
{
    public partial class ReportForm : Form
    {
        private String reportName;
        private bool first = true;
        private String fileName = String.Empty;
        Bitmap bitmap;
        Graphics graphic;
        int initScroll = 0;

        public ReportForm(Report report)
        {
            InitializeComponent();
            dataGridView1.DataSource = report.GeneralData;
            dataGridView2.DataSource = report.Group1Data;
            dataGridView3.DataSource = report.Group2Data;


            lblIntro.Text = report.Intro;
            lblComment.Text = report.Comment;
            lblNorms.Text = "";


            DataTable dtFull = new DataTable();
            DataTable dtSub1 = new DataTable();
            DataTable dtSub2 = new DataTable();
            DataTable dtSub3 = new DataTable();

            foreach (var col in report.GeneralData.Columns.Cast<DataColumn>())
            {
                dtSub1.Columns.Add(col.ColumnName);
                dtFull.Columns.Add(col.ColumnName);

                if (dtFull.Rows.Count == 0) dtFull.Rows.Add();

                dtFull.Rows[0][col.ColumnName] = col.ColumnName;
            }
            foreach (var col in report.Group1Data.Columns.Cast<DataColumn>()) dtSub2.Columns.Add(col.ColumnName);
            foreach (var col in report.Group2Data.Columns.Cast<DataColumn>()) dtSub3.Columns.Add(col.ColumnName);

            dtSub1.Rows.Add("General", "", "", "", "");
            dtSub2.Rows.Add(report.Group1Name, "State Avg", "", "", "");
            dtSub3.Rows.Add(report.Group2Name, "Norms", "", "", "");

            fullHeader.DataSource = dtFull;
            subHeader1.DataSource = dtSub1;
            subHeader2.DataSource = dtSub2;
            subHeader3.DataSource = dtSub3;

            this.reportName = report.ReportName;

            //chart data
            var chart1Data = report.Group1Data.Rows.Cast<DataRow>()
                                .Where(p => p["Data"].ToString() == report.Chart1Column).First();

            var chart2Data = report.Group2Data.Rows.Cast<DataRow>()
                                .Where(p => p["Data"].ToString() == report.Chart2Column).First();
            
            var xvalues1 = report.Group1Data.Columns.Cast<DataColumn>()
                                .Select(p => p.ColumnName)
                                .Where(name => name != "Data").ToArray();

            var xvalues2 = report.Group2Data.Columns.Cast<DataColumn>()
                                .Select(p => p.ColumnName)
                                .Where(name => name != "Data").ToArray();
            
            ArrayList yvalues1 = new ArrayList();

            for (int i = 1; i < chart1Data.ItemArray.Count(); i++ )
            {
                yvalues1.Add(Convert.ToDouble(chart1Data.ItemArray[i]));
            }

            Double[] yvalues1Array = (Double[])yvalues1.ToArray(typeof(Double));
            ArrayList yvalues2 = new ArrayList();

            for (int i = 1; i < chart2Data.ItemArray.Count(); i++)
            {
                yvalues2.Add(Convert.ToDouble(chart2Data.ItemArray[i]));
            }

            Double[] yvalues2Array = (Double[])yvalues1.ToArray(typeof(Double));
            chart1.Series[0].Points.DataBindXY(xvalues1, yvalues1Array);
            chart2.Series[0].Points.DataBindXY(xvalues2, yvalues2Array);

            chart1.Titles.Add(report.Chart1Column);
            chart1.Titles[0].Font = new Font("Cambria", (float)13, FontStyle.Bold);

            chart2.Titles.Add(report.Chart2Column);
            chart2.Titles[0].Font = new Font("Cambria", (float)13, FontStyle.Bold);

            chart1.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
            chart1.ChartAreas[0].AxisY.MajorGrid.Enabled = false;

            chart2.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
            chart2.ChartAreas[0].AxisY.MajorGrid.Enabled = false;

            lblRank.Text = report.Rank;
            lblRank.Find(report.ReportName + " MLA Constituency Rank");
            lblRank.SelectionFont = new Font("Verdana", 12, FontStyle.Bold);
            lblName.Text = report.ReportName + " MLA Constituency";

            if (String.IsNullOrEmpty(lblNorms.Text)) lblNorms.Height = 0;
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
            //formatting the datagrids width
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            dataGridView2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            dataGridView3.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;

            int firstColumnWidth = Math.Max(dataGridView2.Columns[0].Width, dataGridView3.Columns[0].Width);
            int secondColumnWidth = 55; //Math.Max(subHeader2.Columns[1].Width, subHeader2.Columns[1].Width);

            dataGridView1.Columns[0].Width = firstColumnWidth + secondColumnWidth;
            fullHeader.Columns[0].Width = firstColumnWidth + secondColumnWidth;

            dataGridView2.Columns[0].Width = firstColumnWidth;
            dataGridView2.Columns[1].Width = secondColumnWidth;
            dataGridView3.Columns[0].Width = firstColumnWidth;
            dataGridView3.Columns[1].Width = secondColumnWidth;            
            subHeader2.Columns[0].Width = firstColumnWidth;
            subHeader2.Columns[1].Width = secondColumnWidth;
            subHeader3.Columns[0].Width = firstColumnWidth;
            subHeader3.Columns[1].Width = secondColumnWidth;

            // divide equally in 4 parts
            int remainingColumnsWidth = (dataGridView1.Width - firstColumnWidth - secondColumnWidth) / 4;
            for (int i = 2; i < 6; i++)
            {
                dataGridView1.Columns[i - 1].Width = remainingColumnsWidth;
                fullHeader.Columns[i - 1].Width = remainingColumnsWidth;
                dataGridView2.Columns[i].Width = remainingColumnsWidth;
                subHeader2.Columns[i].Width = remainingColumnsWidth;
                dataGridView3.Columns[i].Width = remainingColumnsWidth;
                subHeader3.Columns[i].Width = remainingColumnsWidth;                
            }

            //remove borders for other constituencies
            for (int i = 3; i < 6; i++)
            {
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    row.Cells[i - 1].AdjustCellBorderStyle(new DataGridViewAdvancedBorderStyle(), 
                                                            new DataGridViewAdvancedBorderStyle(), 
                                                            false, false, false, false);
                }
            }


            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork +=
                new DoWorkEventHandler(bw_DoWork);
            worker.RunWorkerCompleted +=
                new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted); 
            //for second scrshot
            if (!first) 
                this.ScrollControlIntoView(lblEndBlank);
            else
            {
                this.ScrollControlIntoView(pictureBox1);
                initScroll = this.VerticalScroll.Value;
            }
            worker.RunWorkerAsync();
        }

        private void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            //to allow the report to load fully
            Thread.Sleep(500);
            if(first)
            {
                //print to a bmp and save file
                Rectangle form = this.Bounds;
                Rectangle panel =  flowLayoutPanel1.Bounds;
                bitmap = new Bitmap(panel.Width, panel.Height);
                graphic = Graphics.FromImage(bitmap);
                graphic.CopyFromScreen(form.Location, Point.Empty, form.Size);        
            }  else {
                //print to a pdf and save file
                Rectangle form = this.Bounds;
                graphic.CopyFromScreen(form.Location, new Point(0, this.VerticalScroll.Value - initScroll), form.Size);
                string pathPdf = "D:\\IndiaGovernsReports\\" + this.reportName + ".pdf";
                PdfDocument doc = new PdfDocument();
                doc.Pages.Add(new PdfPage());
                XGraphics xgr = XGraphics.FromPdfPage(doc.Pages[0]);
                XImage img = XImage.FromGdiPlusImage(bitmap);
                xgr.DrawImage(img, 0, 0, doc.Pages[0].Width, doc.Pages[0].Height);

                doc.Save(pathPdf);
                doc.Close();
            }
        }

        private void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.ScrollControlIntoView(lblEndBlank);
            if (first)
            {
                first = false;
                ReportForm_Shown(null, null);
            }
            //else { this.Close(); }
        }

        private void lbl_Paint(object sender, PaintEventArgs e)
        {
            ControlPaint.DrawBorder(e.Graphics, ((Label)sender).DisplayRectangle, Color.LightSkyBlue, ButtonBorderStyle.Solid);
        }
    }
}