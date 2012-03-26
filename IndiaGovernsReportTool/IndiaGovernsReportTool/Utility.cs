using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.OleDb;
using System.Collections;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;

namespace IndiaGovernsReportTool
{
    public class Utility
    {
        /// <summary>
        /// Utility method for getting a list of values selected given a listview
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static String[] getListSelectedValues(ListView list)
        {
            ArrayList selectedValues = new ArrayList();
            foreach (ListViewItem item in list.CheckedItems)
            {
                selectedValues.Add(item.Text);
            }
            return (String[])selectedValues.ToArray(typeof(String));
        }

        /// <summary>
        /// This method converts data in a xlsx Excel file into a DataSet, with each sheet data in a separate datatable
        /// </summary>
        /// <param name="fileName">Full path of the file to be read</param>
        /// <param name="hasHeaders">Whether the data has headers or not - if yes, then the first row is taken as the header names of the columns</param>
        /// <returns>Dataset with the data inside it</returns>
        public static DataSet ExcelToDataSet(string fileName, bool hasHeaders)
        {
            DataSet ds = new DataSet();
            string HDR = hasHeaders ? "Yes" : "No";
            String strConn = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileName + ";Extended Properties=\"Excel 12.0;HDR=" + HDR + ";IMEX=1\"";
            
            using (OleDbConnection conn = new OleDbConnection(strConn))
            {
                conn.Open();
                System.Data.DataTable dt = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });

                foreach (DataRow row in dt.Rows)
                {
                    string sheet = row["TABLE_NAME"].ToString();
                    OleDbCommand cmd = new OleDbCommand("SELECT * FROM [" + sheet + "]", conn);
                    cmd.CommandType = CommandType.Text;
                    var outputTable = new System.Data.DataTable(sheet);
                    ds.Tables.Add(outputTable);
                    new OleDbDataAdapter(cmd).Fill(outputTable);

                    //this is done to revert the conversion of . to # by oldedb
                    foreach (var column in outputTable.Columns.Cast<DataColumn>())
                    {
                        //fixing some wierd problem where " " is replaced by _
                        column.ColumnName = column.ColumnName.Replace("#", ".").Replace("_", " ");
                        
                    }
                }
            }
            return ds;
        }
    }
}
