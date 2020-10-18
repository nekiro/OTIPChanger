using Microsoft.Win32;
using OTIPChanger.Classes;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace OTIPChanger
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IpChanger _ipChanger;

        public MainWindow()
        {
            InitializeComponent();
            _ipChanger = new IpChanger();

            if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Tibia\packages\Tibia\bin\client.exe"))
            {
                SetClientData(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Tibia\packages\Tibia\bin\client.exe");
            }
        }

        private void ExitApp(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void MinimizeApp(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.WindowState = WindowState.Minimized;
        }

        private void BrowseClient(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog() { Filter = "Tibia exe (*.exe)|*.exe", InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) };
            if (openFileDialog.ShowDialog() == true)
            {
                SetClientData(openFileDialog.FileName);
            }
        }

        private void SetClientData(string path)
        {
            (string loginUrl, string webUrl) = _ipChanger.LoadClientData(path);
            PathToClient.Text = path;
            LoginUrl.Text = loginUrl;
            ServiceUrl.Text = webUrl;
        }

        private void TryChangeIp(object sender, RoutedEventArgs e)
        {
            (bool success, string msg) = _ipChanger.TryChangeIp(PathToClient.Text, LoginUrl.Text, ServiceUrl.Text);
            if (!success)
            {
                MessageBox.Show(msg, "Error");
                return;
            }

            MessageBox.Show(msg, "Success");
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void CreditsClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("This app was brought to you by nekiro!\nhttps://github.com/nekiro\n\nThanks for using.", "Credits");
        }

        private void OnCheckChange(object sender, RoutedEventArgs e)
        {
            var checkbox = sender as CheckBox;
            if (checkbox == null)
            {
                return;
            }

            _ipChanger.SaveAsNewFile = checkbox.IsChecked == true;
        }
    }
}
