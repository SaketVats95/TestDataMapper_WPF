using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace TestDataMapper
{
    /// <summary>
    /// Interaction logic for ExpressionManager.xaml
    /// </summary>
    public partial class ExpressionManager : Window
    {
        DataTable dt_expsInfoTable = new DataTable();
        DataTable dt_OriginalDataTable;
        DataTable dt_backUpDataTable;

        public ExpressionManager()
        {
            InitializeComponent();
        }

        public ExpressionManager(DataTable dt, string jsonExpsData)
        {
            InitializeComponent();
            dt_OriginalDataTable = dt;
            DataTable_Processing dataTable_Processing = new DataTable_Processing();
            dt_expsInfoTable = dataTable_Processing.ConvertJsonStrToDT(jsonExpsData);
            if (dt_expsInfoTable.Rows.Count > 0)
            { 
                DataColumn newColumn = new System.Data.DataColumn("Status", typeof(System.Boolean));
                newColumn.DefaultValue = true;
                dt_expsInfoTable.Columns.Add(newColumn);
                dgLoadExpressionTable.DataContext = dt_expsInfoTable;
            }
        }

        private void btnApplyExpression_Click(object sender, RoutedEventArgs e)
        {
            DataTable_Processing dataTable_Processing = new DataTable_Processing();
            dt_backUpDataTable = dt_OriginalDataTable.Copy();
            
            foreach (DataRow dr in dt_expsInfoTable.Rows)
            {
            if ((bool)dr["Status"])
            {
                    dataTable_Processing.AddNewColumn(ref dt_OriginalDataTable, dr[0].ToString(), dr[1].ToString());
            }
            }
        }

        private void btnBackToPreviousState_Click(object sender, RoutedEventArgs e)
        {
            dt_OriginalDataTable = dt_backUpDataTable.Copy();
        }
        private void btnSavetoExcelSheet_Click(object sender, RoutedEventArgs e)
        {
            if (dt_OriginalDataTable != null)
            {
                Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
                dlg.FileName = "Book1"; // Default file name
                dlg.DefaultExt = ".xlsx"; // Default file extension
                dlg.Filter = "Excel documents (.xlsx)|*.xlsx"; // Filter files by extension

                // Show save file dialog box
                Nullable<bool> result = dlg.ShowDialog();

                // Process save file dialog box results

                if (result == true)
                {
                    string filename = dlg.FileName;
                    ReadWriteExcelSheet readWriteExcelSheet = new ReadWriteExcelSheet();
                    readWriteExcelSheet.WriteToExcel(dt_OriginalDataTable, filename);
                }
            }
        }

        private void BtnPreviewDT_Click(object sender, RoutedEventArgs e)
        {
            DataTablePreview dataTablePreview = new DataTablePreview(dt_OriginalDataTable);
            dataTablePreview.Show();
        }
    }
   
}

