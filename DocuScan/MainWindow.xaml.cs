using DocuScan.Comparer;
using System.IO;
using System.Windows;

namespace DocuScan
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

        private void BrowseDir1_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                Dir1TextBox.Text = dialog.SelectedPath;
        }

        private void BrowseDir2_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                Dir2TextBox.Text = dialog.SelectedPath;
        }

        private void Compare_Click(object sender, RoutedEventArgs e)
        {
            var dir1 = Dir1TextBox.Text;
            var dir2 = Dir2TextBox.Text;

            if (!Directory.Exists(dir1) || !Directory.Exists(dir2))
            {
                System.Windows.MessageBox.Show("Both directories must be valid.");
                return;
            }

            var files1 = Directory.GetFiles(dir1).Select(System.IO.Path.GetFileName).ToHashSet();
            var files2 = Directory.GetFiles(dir2).Select(System.IO.Path.GetFileName).ToHashSet();

            var allFiles = files1.Union(files2);
            var results = new List<CompareResult>();

            foreach (var file in allFiles)
            {
                string status;
                if (files1.Contains(file) && files2.Contains(file))
                {
                    var hash1 = File.ReadAllBytes(System.IO.Path.Combine(dir1, file));
                    var hash2 = File.ReadAllBytes(System.IO.Path.Combine(dir2, file));
                    status = hash1.SequenceEqual(hash2) ? "Same" : "Different";
                }
                else if (files1.Contains(file))
                    status = "Only in Dir1";
                else
                    status = "Only in Dir2";

                results.Add(new CompareResult { FileName = file, Status = status });
            }

            ResultsGrid.ItemsSource = results;
        }
    }
}