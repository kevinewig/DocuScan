using DocuScan.Comparer;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;

namespace DocuScan
{
    public partial class MainWindow : Window
    {
        private DirectoryCompare _comparer = new DirectoryCompare();
        private CancellationTokenSource _cancellationTokenSource;
        private ObservableCollection<CompareResult> _liveResults = new();

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

        private async void Compare_Click(object sender, RoutedEventArgs e)
        {
            var dir1 = Dir1TextBox.Text;
            var dir2 = Dir2TextBox.Text;

            if (!Directory.Exists(dir1) || !Directory.Exists(dir2))
            {
                System.Windows.MessageBox.Show("Both directories must be valid.");
                return;
            }

            EnableUI(false); // Disable Compare button
            _cancellationTokenSource = new CancellationTokenSource();
            CompareProgressBar.Value = 0;
            ProgressText.Text = "0.0%";
            _liveResults.Clear();
            ResultsGrid.ItemsSource = _liveResults;

            try
            {
                await Task.Run(() =>
                {
                    _comparer.Compare(dir1, dir2, ReportProgress, UpdateResult, _cancellationTokenSource.Token);
                });
            }
            catch (OperationCanceledException)
            {
                System.Windows.MessageBox.Show("Comparison stopped by user.");
            }
            finally
            {
                EnableUI(true); // Re-enable Compare button
                _cancellationTokenSource = null;
            }
        }

        private void UpdateResult(CompareResult result)
        {
            Dispatcher.Invoke(() => _liveResults.Add(result));
        }

        private void ReportProgress(int processed, int total)
        {
            Dispatcher.Invoke(() =>
            {
                double percent = (double)processed / total * 100;
                CompareProgressBar.Value = percent;
                ProgressText.Text = $"{percent:F2}%";
            });
        }

        private void StopCompare_Click(object sender, RoutedEventArgs e)
        {
            _cancellationTokenSource?.Cancel();
            EnableUI(true);
        }

        private void EnableUI(bool enable)
        {
            Dir1Button.IsEnabled = enable;
            Dir1TextBox.IsEnabled = enable; 
            Dir2Button.IsEnabled = enable;
            Dir2TextBox.IsEnabled = enable;
            CompareButton.IsEnabled = enable;
        }

    }
}