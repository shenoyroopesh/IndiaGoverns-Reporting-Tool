using System;
using System.Data;

namespace IndiaGovernsReportTool
{
    /// <summary>
    /// This class represents an individual constituency report, and holds the data that will be published 
    /// to a PDF
    /// 
    /// Instead of directly publishing to PDF, we will be using a image, which will then be copied and 
    /// pasted into a pdf
    /// </summary>
    public class Report
    {
        public String ReportName { get; set; }
        public String MLAConstituency { get; set; }
        public DataTable GeneralData { get; set; }
        public DataTable Group1Data { get; set; }
        public DataTable Group2Data { get; set; }
        public String Group1Name { get; set; }
        public String Group2Name { get; set; }
        public String Intro { get; set; }
        public String Comment { get; set; }
        public String Chart1Column1 { get; set; }
        public String Chart1Column2 { get; set; }
        public String Chart2Column1 { get; set; }
        public String Chart2Column2 { get; set; }
        public String Rank { get; set; }
        public String DataYear { get; set; }
    }
}
