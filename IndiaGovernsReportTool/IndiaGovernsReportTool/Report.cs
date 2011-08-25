using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace IndiaGovernsReportTool
{
    /// <summary>
    /// This class represents an individual constituency report, and holds the data that will be published 
    /// to a PDF
    /// 
    /// Instead of directly publishing to PDF, we will be using an html report, which will then be copied and 
    /// pasted into the publisher file before saving it. This will make it easy to define the report layout compared to 
    /// directly creating it in a PDF file
    /// </summary>
    public class Report
    {
        public String Constituency { get; set; }

        public DataTable PopulationData { get; set; }

        public DataTable ReportData1 { get; set; }

        public DataTable ReportData2 { get; set; }

        public String Comment { get; set; }

        public String Rank { get; set; }

        public DataTable chart1Data { get; set; }

        public DataTable chart2Data { get; set; }

        /// <summary>
        /// This method publishes the report to an RDLC file
        /// </summary>
        public void publish()
        {
            
        }
    }
}
