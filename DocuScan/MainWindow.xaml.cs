using DocuScan.Comparer;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Input;

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

        private void ResultsGrid_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.C && Keyboard.Modifiers == ModifierKeys.Control)
            {
                ApplicationCommands.Copy.Execute(null, ResultsGrid);
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

        private void CopyToClipboard_Click(object sender, RoutedEventArgs e)
        {
            if (ResultsGrid.Items.Count == 0)
                return;

            // Select everything
            ResultsGrid.SelectAllCells();
            ResultsGrid.ClipboardCopyMode = System.Windows.Controls.DataGridClipboardCopyMode.IncludeHeader;

            // Force copy to clipboard
            ApplicationCommands.Copy.Execute(null, ResultsGrid);

            // Clear selection so the grid doesn’t stay highlighted
            ResultsGrid.UnselectAllCells();
        }

    }
}