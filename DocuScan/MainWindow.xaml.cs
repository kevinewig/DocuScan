using DocuScan.Comparer;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading;
using System.Windows;

namespace DocuScan
{
    public partial class MainWindow : Window
    {
        private DirectoryCompare _comparer = new DirectoryCompare();
        private CancellationTokenSource _cancellationTokenSource;
        private ObservableCollection<CompareResult> _liveResults = new();
        private DateTime _startTime;
        private Task _compareTask;

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

            // Disable Compare button
            EnableUI(false);

            // Set progress bar and text
            CompareProgressBar.Value = 0;
            ProgressText.Text = "Starting...";

            // Set the time the comparison started
            _startTime = DateTime.Now;

            // Clear previous results on the data grid.
            _liveResults.Clear();
            ResultsGrid.ItemsSource = _liveResults;

            try
            {
                // Store the task so you can monitor or cancel it later
                _cancellationTokenSource = new CancellationTokenSource();
                _compareTask = Task.Run(() =>
                {
                    _comparer.Compare(dir1, dir2, ReportProgress, UpdateResult, _cancellationTokenSource.Token);
                }, _cancellationTokenSource.Token);

                await _compareTask;
            }
            catch (OperationCanceledException)
            {
                System.Windows.MessageBox.Show("Comparison stopped by user.");
            }
            finally
            {
                _cancellationTokenSource = null;
                _compareTask = null;
            }
        }

        private void UpdateResult(CompareResult result)
        {
            try
            {
                Dispatcher.Invoke(() =>
                {
                    _liveResults.Add(result);
                });
            }
            catch (ThreadInterruptedException)
            {
                return;
            }
        }

        private void ReportProgress(int processed, int total)
        {
            try
            {
                Dispatcher.Invoke(() =>
                {
                    double percent = (double)processed / total * 100;
                    CompareProgressBar.Value = percent;

                    if (processed > 0)
                    {
                        var elapsed = DateTime.Now - _startTime;
                        double estimatedTotalSeconds = elapsed.TotalSeconds / processed * total;
                        var remaining = TimeSpan.FromSeconds(estimatedTotalSeconds - elapsed.TotalSeconds);

                        ProgressText.Text = $"complete: {percent:F2}% time left: {remaining.Hours}h {remaining.Minutes}m";
                    }
                    else
                    {
                        ProgressText.Text = $"complete: {percent:F2}%";
                    }
                });
            }
            catch(ThreadInterruptedException)
            {
                return;
            }
        }

        /// <summary>
        /// Disabled because stopping is not working properly yet.
        /// </summary>
        private void StopCompare_Click(object sender, RoutedEventArgs e)
        {
            if (_cancellationTokenSource != null && !_cancellationTokenSource.IsCancellationRequested)
            {
                _cancellationTokenSource.Cancel();
            }
            ProgressText.Text = "";
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