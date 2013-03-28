using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using IndiaGovernsReportTool.Helpers;

namespace IndiaGovernsReportTool
{
    public partial class Step5 : UserControl, ISaveSettings
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

        public Step5()
        {
            InitializeComponent();
        }

        public void SaveSettings(Settings settings)
        {
            settings.CommentColumns = CommentColumns;
            settings.SingleReport = SingleReport;
        }

        public void LoadSettings(Settings settings)
        {
            var possibleColumns = settings.Group1Columns.Concat(settings.Group2Columns);
            possibleColumns
                .ToList()
                .ForEach(p => this.listView1.Items.Add(p));

            listView1.Items.Cast<ListViewItem>().ToList().ForEach(p => p.Selected = true);
            listView1.CheckItemsWithText(settings.CommentColumns);
            chkSingleReport.Checked = settings.SingleReport;
        }
    }
}
