using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IndiaGovernsReportTool
{
    /// <summary>
    /// Application wide settings to be saved to a config file
    /// </summary>
    [Serializable]
    public class Settings
    {
        public string[] AllPossibleColumns { get; set; }

        public String Group1Name { get; set; }

        public String Group2Name { get; set; }

        public String[] Group1Columns{ get; set; }

        public String[] Group2Columns{ get; set; }

        public String Chart1Column1{ get; set; }

        public String Chart1Column2 { get; set; }

        public String Chart2Column1 { get; set; }
        public String Chart2Column2 { get; set; }

        public String Rank1Column { get; set; }
        public String Rank2Column { get; set; }
        public String Rank3Column { get; set; }

        public String[] CommentColumns { get; set; }

        public bool SingleReport { get; set; }

        public String DataYear { get; set; }
    }
}
