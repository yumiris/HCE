using System.Windows;
using Microsoft.Win32;

namespace Atarashii.GUI.Executable
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private readonly Main _main;

        public MainWindow()
        {
            InitializeComponent();
            _main = (Main) DataContext;
        }

        private void Load(object sender, RoutedEventArgs e)
        {
            _main.Load();
        }

        private void Detect(object sender, RoutedEventArgs e)
        {
            _main.Detect();
        }

        private void Browse(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "HCE Executable|haloce.exe"
            };

            if (openFileDialog.ShowDialog() == true) _main.HcePath = openFileDialog.FileName;
        }
    }
}