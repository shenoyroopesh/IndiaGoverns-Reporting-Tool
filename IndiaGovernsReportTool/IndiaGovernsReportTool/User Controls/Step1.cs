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
    public partial class Step1 : UserControl
    {   
        /// <summary>
        /// this contains the input data. The first datatable will contain aggregate data and the second datatable will contain 
        /// norms data
        /// </summary>
        public DataSet data;

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
        private void btnBrowse_Click(object sender, EventArgs e)
        {
            this.openFileDialog1.FileName = "*.xlsx";
            if (this.openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                lblFileName.Text = this.openFileDialog1.FileName;
                data = Utility.ExcelToDataSet(this.openFileDialog1.FileName, true);
            }
        }
    }
}
