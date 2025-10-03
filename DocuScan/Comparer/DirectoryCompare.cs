using System.IO;

namespace DocuScan.Comparer
{
    internal class DirectoryCompare
    {
        public void Compare(string directory1, string directory2, Action<int, int> reportProgress, Action<CompareResult> reportResult, CancellationToken token)
        {
            var allPaths = GetAllRelativePaths(directory1, directory2)
                           .Where(VisualCompareFactory.IsSupportedFileType)
                           .ToList();

            int total = allPaths.Count;
            int processed = 0;

            var options = new ParallelOptions
            {
                CancellationToken = token,
                MaxDegreeOfParallelism = Environment.ProcessorCount
            };

            Parallel.ForEach(allPaths, options, (relPath, state) => {

                token.ThrowIfCancellationRequested();

                if (!VisualCompareFactory.IsSupportedFileType(relPath))
                {
                    return;
                }

                var comparer = VisualCompareFactory.GetVisualCompare(relPath);
                var path1 = Path.Combine(directory1, relPath);
                var path2 = Path.Combine(directory2, relPath);

                string status;
                bool same = false;

                if (File.Exists(path1) && File.Exists(path2))
                {
                    if (FilesAreEqual(path1, path2))
                    {
                        status = "Files are identical (binary match)";
                        same = true;
                    }
                    else
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

                // Thread-safe UI update
                reportResult(result);

                // Thread-safe progress update
                int current = Interlocked.Increment(ref processed);
                reportProgress(current, total);

            });

        }

        private List<string> GetAllRelativePaths(string dir1, string dir2)
        {
            var files1 = Directory.Exists(dir1) ? Directory.GetFiles(dir1, "*", SearchOption.AllDirectories) : Array.Empty<string>();
            var files2 = Directory.Exists(dir2) ? Directory.GetFiles(dir2, "*", SearchOption.AllDirectories) : Array.Empty<string>();

            var rel1 = files1.Select(f => Path.GetRelativePath(dir1, f));
            var rel2 = files2.Select(f => Path.GetRelativePath(dir2, f));

            return rel1.Union(rel2).Distinct().ToList();
        }

        private static bool FilesAreEqual(string filePath1, string filePath2)
        {
            string hash1 = ComputeFileHash(filePath1);
            string hash2 = ComputeFileHash(filePath2);
            return hash1 == hash2;
        }

        private static string ComputeFileHash(string filePath)
        {
            using var sha = System.Security.Cryptography.SHA256.Create();
            using var stream = File.OpenRead(filePath);
            byte[] hashBytes = sha.ComputeHash(stream);
            return Convert.ToHexString(hashBytes); // .NET 5+ (uppercase hex string)
        }

    }
}
