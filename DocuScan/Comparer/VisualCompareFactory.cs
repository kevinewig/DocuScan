namespace DocuScan.Comparer
{
    internal static class VisualCompareFactory
    {
        public static VisualCompare GetVisualCompare(string filePath)
        {
            var extension = System.IO.Path.GetExtension(filePath).ToLowerInvariant();
            return extension switch
            {
                ".pdf" => new VisualPdfCompare(),
                ".docx" => new VisualWordCompare(),
                _ => throw new NotSupportedException($"File extension '{extension}' is not supported for visual comparison.")
            };
        }   
    }
}
