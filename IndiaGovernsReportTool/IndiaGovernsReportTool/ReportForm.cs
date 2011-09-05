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

namespace IndiaGovernsReportTool
{
    public partial class ReportForm : Form
    {
        private String reportName;
        private bool first = true;
        private String fileName = String.Empty;
        Bitmap bitmap;
        Graphics graphic;

        public ReportForm(Report report)
        {
            

            InitializeComponent();
            dataGridView1.DataSource = report.GeneralData;
            dataGridView2.DataSource = report.Group1Data;
            dataGridView3.DataSource = report.Group2Data;
            lblIntro.Text = report.Intro;
            lblNorms.Text = report.Comment;

            lblGroup1.Text = report.Group1Name;
            lblGroup2.Text = report.Group2Name;

            this.reportName = report.ReportName;

            //chart data
            var chart1Data = report.Group1Data.Rows.Cast<DataRow>().Where(p => p["Data"].ToString() == report.Chart1Column).First();
            var chart2Data = report.Group2Data.Rows.Cast<DataRow>().Where(p => p["Data"].ToString() == report.Chart2Column).First();


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
            chart2.Titles.Add(report.Chart2Column);
        }


        private void ReportForm_Load(object sender, EventArgs e)
        {

        }

        private void ReportForm_Shown(object sender, EventArgs e)
        {
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork +=
                new DoWorkEventHandler(bw_DoWork);
            worker.RunWorkerCompleted +=
                new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted); 
            //for second scrshot
            if (!first) this.ScrollControlIntoView(lblEndBlank);
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
                Rectangle panel = panel1.Bounds;
                bitmap = new Bitmap(panel.Width, panel.Height);
                graphic = Graphics.FromImage(bitmap);
                graphic.CopyFromScreen(form.Location, Point.Empty, form.Size);        
            }  else {
                //print to a bmp and save file
                Rectangle form = this.Bounds;
                graphic.CopyFromScreen(form.Location, new Point(0, this.VerticalScroll.Value), form.Size);
                bitmap.Save("D:\\IndiaGovernsReports\\" + this.reportName + ".png", ImageFormat.Png);
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
            else { this.Close(); }
        }   
    }
}
