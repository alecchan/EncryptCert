using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EncryptCert
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void EncyptClick(object sender, RoutedEventArgs e)
        {
            ProtectString ps = new ProtectString();
            results.Items.Add(ps.Encrypt(text.Text));
            System.Diagnostics.Debug.WriteLine(ps.Encrypt(text.Text));
        }

        private void DecryptClick(object sender, RoutedEventArgs e)
        {
            ProtectString ps = new ProtectString();
            results.Items.Add(ps.Decrypt(results.Items[results.Items.Count-1].ToString()));
        }

        private void ConnectionString(object sender, RoutedEventArgs e)
        {
            var cs = AppSettings.ConnectionString;
            results.Items.Add(cs);
        }
    }
}
