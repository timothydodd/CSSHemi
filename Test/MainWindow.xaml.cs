using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using HTMLScrape;

namespace Test
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string _websource;
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            _websource = HTMLClient.GetPage(new CookieContainer(), txtUrl.Text);
            RefreshCSS();
        }

        private void RefreshCSS()
        {
            if (!string.IsNullOrWhiteSpace(_websource))
            {
                
                txtHTML.Document.Blocks.Clear();
                Stopwatch watch = new Stopwatch();
                watch.Start();
                foreach (var item in _websource.Query(txtCSS.Text))
                {


                    txtHTML.Document.Blocks.Add(new Paragraph(new Run(item)));
                }
                watch.Stop();
                txtTime.Text = $"{watch.ElapsedMilliseconds}ms";
            }

        }
   
        private void TxtCSS_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            RefreshCSS();
        }

        private void TxtUrl_OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {

                _websource = HTMLClient.GetPage(new CookieContainer(), txtUrl.Text);
                RefreshCSS();
            }
        }
    }
}
