using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace IndiaGovernsReportTool
{
    public partial class Step5 : UserControl
    {
        public String[] CommentColumns 
        {
            get {
                return Utility.GetListSelectedValues(listView1);
            }
        }

        public bool SingleReport
        {
            get
            {
                return chkSingleReport.Checked;
            }
        }

        public Step5(IEnumerable<string> possibleColumns)
        {
            InitializeComponent();
            possibleColumns
                .ToList()
                .ForEach(p => this.listView1.Items.Add(p));

            listView1.Items.Cast<ListViewItem>().ToList().ForEach(p => p.Selected = true);
        }
    }
}
