namespace DocuScan.Comparer
{
    public class CompareResult
    {
        public string FileName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public bool Same { get; set; } = false;
    }
}
