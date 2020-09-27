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
    /// Interaction logic for DataTablePreview.xaml
    /// </summary>
    public partial class DataTablePreview : Window
    {
        public DataTablePreview()
        {
            InitializeComponent();
        }
        public DataTablePreview(DataTable dt)
        {
            InitializeComponent();
            lblTableName.Content = dt.TableName;
            lblTotalRowCount.Content = "TOtal Rows Count :"+dt.Rows;
            dgLoadTable.DataContext = dt;
        }
    }
}
