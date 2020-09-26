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
    /// Interaction logic for ChildWindow.xaml
    /// </summary>
    public partial class ChildWindow : Window
    {
        public ChildWindow()
        {
            InitializeComponent();
        }
        public ChildWindow(DataTable dt)
        {
            InitializeComponent();
            lblColumnName.Content = dt.TableName;
            dgLoadMappingTable.DataContext = dt;

            //DataTable d = (DataTable)dgLoadMappingTable.DataContext;
        }
        public string Getmessage()
        {
            return "Hi Saket";
        }

        private void btnSaveChanges_Click(object sender, RoutedEventArgs e)
        {
            DataTable_Processing dataTable_Processing = new DataTable_Processing();
            DataTable dt = (DataTable)dgLoadMappingTable.DataContext;
            string mappingDataJsonString = dataTable_Processing.DataTableToJSONWithJSONNet(dt);
            
        }

        private void btnResetChanges_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
