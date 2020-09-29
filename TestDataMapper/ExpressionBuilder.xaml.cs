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
using Newtonsoft.Json;

namespace TestDataMapper
{
    /// <summary>
    /// Interaction logic for ExpressionBuilder.xaml
    /// </summary>
    public partial class ExpressionBuilder : Window
    {
        Dictionary<string,string> allExpressions = new Dictionary<string, string>();
        DataTable dataTable = new DataTable();
        DataTable backupTable;

        public ExpressionBuilder()
        {
            InitializeComponent();
        }
        public ExpressionBuilder( DataTable dt)
        {
            InitializeComponent();
            dataTable = dt;
            dgLoadMappingTable.DataContext = dataTable; 
            AllColumnNameButton();
        }
        public void AllColumnNameButton()
        {
            foreach (DataColumn dc in dataTable.Columns)
            {
                wpPanelColumnNames.Children.Add(addLabelWithClickEvent(dc.ColumnName));
            }
        }
        public void AddColumnNameToExp(object sender, RoutedEventArgs e)
        {
            Label colLabel = (Label)sender;
            string colName = colLabel.Content.ToString();
            txtboxExpression.Text = String.Format(" {0} [{1}] ", txtboxExpression.Text, colName);
        }
        public Label addLabelWithClickEvent(string content)
        {
            Label l = new Label();
            l.Content = content;
            l.Margin = new Thickness { Top = 10, Bottom = 10, Left = 5, Right = 5 };
            l.Height = 30;
            l.Background = Brushes.LightSeaGreen;
            l.HorizontalContentAlignment = HorizontalAlignment.Center;
            l.VerticalContentAlignment = VerticalAlignment.Center;
            l.FontSize = 12;
            l.FontWeight = FontWeights.Bold;
            l.FontFamily = new FontFamily("SimSun");
            l.MouseDoubleClick += AddColumnNameToExp;
            //l.MouseDown
            return l;

        }

        private void BtnSaveExp_Click(object sender, RoutedEventArgs e)
        {
            if (txtboxColumnName.Text != "" && txtboxExpression.Text != "")
            {
                if(!allExpressions.Keys.Contains(txtboxExpression.Text))
                allExpressions.Add(txtboxColumnName.Text, txtboxExpression.Text);
            }
            txtboxColumnName.Text = ""; txtboxExpression.Text = "";
        }
        public DataTable DictToDT()
        {
            DataTable dt = new DataTable("ExpressionT");
            dt.Columns.Add("ColumnName", typeof(string));
            dt.Columns.Add("Expression", typeof(string));
            foreach (KeyValuePair<string,string> value in allExpressions)
            {
                DataRow dr = dt.NewRow();
                dr[0] = value.Key;
                dr[1] = value.Value;
                dt.Rows.Add(dr);
            }
            return dt;
        }

        private void btnResetAllExpressions_Click(object sender, RoutedEventArgs e)
        {
            DataTable dt = DictToDT();
            DataTable_Processing dataTable_Processing = new DataTable_Processing();
            string jsonString = dataTable_Processing.DataTableToJSONWithJSONNet(dt);
            ChildWindow childWindow = new ChildWindow();
            childWindow.saveFile(dataTable.TableName + ChildWindow.datetimeString, "ExpressionsList", "", jsonString);
            

        }
    }
}
