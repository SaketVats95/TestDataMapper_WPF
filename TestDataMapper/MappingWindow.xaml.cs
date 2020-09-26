using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Forms;
using Ookii.Dialogs.Wpf;

namespace TestDataMapper
{
    /// <summary>
    /// Interaction logic for MappingWindow.xaml
    /// </summary>
    public partial class MappingWindow : Window
    {
        public MappingWindow()
        {
            InitializeComponent();
        }
        public List<string> GetAllFiles(string dirPath)
        {
            List<string> files = new List<string>();
            foreach (string fName in Directory.GetFiles(dirPath))
            {
                FileInfo f = new FileInfo(fName);
                if (f.Extension.ToLower() == ".json")
                {
                    files.Add(f.FullName);
                }
            }
            return files;
        }

        private void btnMappingFolder_Click(object sender, RoutedEventArgs e)
        {
           
            var ookiiDialog = new VistaFolderBrowserDialog();
            if (ookiiDialog.ShowDialog() == true)
            {
                //MessageBox.Show(ookiiDialog.SelectedPath);
                txtboxMappingFolderPath.Text = ookiiDialog.SelectedPath;
            }
            if (txtboxMappingFolderPath.Text!="") {
                List<string> filesPath = GetAllFiles(txtboxMappingFolderPath.Text);
                foreach (string f in filesPath)
                {
                    FileInfo file = new FileInfo(f);
                   wdPanelMapColList.Children.Add(addLabelWithClickEvent(file.Name));
                }
            }
           
        }

        public Label addLabelWithClickEvent(string content)
        {
            Label l = new Label();
            l.Content = content;
            l.Margin = new Thickness {Top = 10, Bottom= 10, Left = 10, Right = 10 };
            l.Height = 50;
            l.Background = Brushes.LightSeaGreen;
            l.HorizontalContentAlignment = HorizontalAlignment.Center;
            l.VerticalContentAlignment = VerticalAlignment.Center;
            l.FontSize = 18;
            l.FontWeight = FontWeights.Bold;
            l.FontFamily = new FontFamily("SimSun");
            l.MouseDoubleClick += MyButton_DoubleClick; 
            return l;

        }
        public void MyButton_DoubleClick(object sender, RoutedEventArgs e)
        {
            Label l = (Label)sender;
            MessageBox.Show(l.Content.ToString());
        }
    }
}
