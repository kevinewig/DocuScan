using System.IO;

namespace DocuScan.Comparer
{
    internal class DirectoryCompare
    {
        public void Compare(string directory1, string directory2, Action<int, int> reportProgress, Action<CompareResult> reportResult, CancellationToken token)
        {
            var allPaths = GetAllRelativePaths(directory1, directory2);
            int total = allPaths.Count;
            int processed = 0;

            foreach (var relPath in allPaths)
            {
                if (token.IsCancellationRequested)
                {
                    break;
                }

                var comparer = VisualCompareFactory.GetVisualCompare(relPath);
                if (comparer == null)
                {
                    continue;
                }

                string path1 = Path.Combine(directory1, relPath);
                string path2 = Path.Combine(directory2, relPath);

                string status;
                bool same = false;

                if (File.Exists(path1) && File.Exists(path2))
                {
                    var compareResult = comparer.AreVisuallyEqual(path1, path2);
                    if (compareResult == null)
                    {
                        status = "Error comparing files";
                    }
                    else
                    {
                        status = compareResult.Status;
                        same = compareResult.Same;
                    }
                }
                else if (File.Exists(path1))
                {
                    status = "Only in Directory 1";
                }
                else
                {
                    status = "Only in Directory 2";
                }

                var result = new CompareResult { FileName = relPath, Status = status, Same = same };
                reportResult(result);

                processed++;
                reportProgress(processed, total);
            }
        }


        private List<string> GetAllRelativePaths(string dir1, string dir2)
        {
            var files1 = Directory.Exists(dir1) ? Directory.GetFiles(dir1, "*", SearchOption.AllDirectories) : Array.Empty<string>();
            var files2 = Directory.Exists(dir2) ? Directory.GetFiles(dir2, "*", SearchOption.AllDirectories) : Array.Empty<string>();

            var rel1 = files1.Select(f => Path.GetRelativePath(dir1, f));
            var rel2 = files2.Select(f => Path.GetRelativePath(dir2, f));

            return rel1.Union(rel2).Distinct().ToList();
        }

    }
}
