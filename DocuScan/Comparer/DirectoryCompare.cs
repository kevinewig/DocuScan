using System.IO;

namespace DocuScan.Comparer
{
    internal class DirectoryCompare
    {
        public void Compare(string directory1, string directory2)
        {
            var files1 = Directory.GetFiles(directory1).Select(System.IO.Path.GetFileName).ToHashSet();
            var files2 = Directory.GetFiles(directory2).Select(System.IO.Path.GetFileName).ToHashSet();

            var allFiles = files1.Union(files2);
            var results = new List<CompareResult>();

            foreach (var file in allFiles)
            {
                string status;
                bool same = false;
                if (files1.Contains(file) && files2.Contains(file))
                {
                    var hash1 = File.ReadAllBytes(System.IO.Path.Combine(directory1, file));
                    var hash2 = File.ReadAllBytes(System.IO.Path.Combine(directory2, file));
                    same = hash1.SequenceEqual(hash2);
                    status = same ? "Same" : "Different";
                }
                else if (files1.Contains(file))
                {
                    status = "Only in Directory 1";                    
                }
                else
                {
                    status = "Only in Directory 2";
                }
                    
                results.Add(new CompareResult { FileName = file, Status = status, Same = same });
            }
        }
    }
}
