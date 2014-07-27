using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Excel = Microsoft.Office.Interop.Excel;
using System.Data.OleDb;

namespace IndiaGovernsReportTool
{
    /// <summary>
    /// Step1, allows excel file to be uploaded so that the data can be processed for reporting
    /// </summary>
    public partial class Step1 : UserControl, ISaveSettings
    {   
        /// <summary>
        /// this contains the input data. The first datatable will contain aggregate data and the second datatable will contain 
        /// norms data
        /// </summary>
        public DataSet Data;

        /// <summary>
        /// Year for which this report is to be run
        /// </summary>
        public string DataYear
        {
            get
            {
                return this.txtDataYear.Text;
            }
        }

        /// <summary>
        /// Constructor for Step1 User control
        /// </summary>
        public Step1()
        {
            InitializeComponent();
        }
        
        /// <summary>
        /// This method handles the browse button click, reads the excel file uploaded and saves the data into a dataset
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnBrowseClick(object sender, EventArgs e)
        {
            this.openFileDialog1.FileName = "*.xlsx";
            if (this.openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                lblFileName.Text = this.openFileDialog1.FileName;
                Data = Utility.ExcelToDataSet(this.openFileDialog1.FileName, true);
            }
        }

        public void SaveSettings(Settings settings)
        {
            settings.AllPossibleColumns = Data.Tables[0].Columns.Cast<DataColumn>()
                .Select(p => p.ColumnName)
                .ToArray<String>();

            settings.DataYear = DataYear;
        }

        public void LoadSettings(Settings settings)
        {
            this.txtDataYear.Text = settings.DataYear;
        }
    }
}
