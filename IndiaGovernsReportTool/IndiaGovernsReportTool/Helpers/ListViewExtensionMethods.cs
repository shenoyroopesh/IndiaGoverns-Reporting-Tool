using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace IndiaGovernsReportTool.Helpers
{
    public static class ListViewExtensionMethods
    {
        public static void CheckItemsWithText(this ListView listView, IEnumerable<string> values)
        {
            if (values == null) return;
            foreach (var item in values)
            {
                var listViewItem = listView.FindItemWithText(item);
                if (listViewItem != null)
                {
                    listViewItem.Checked = true;
                }
            }
        }
    }
}
