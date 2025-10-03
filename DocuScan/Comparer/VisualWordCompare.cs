using Aspose.Words;
using Aspose.Words.Saving;
using System.IO;

namespace DocuScan.Comparer
{
    public class VisualWordCompare : VisualCompare
    {
        public override CompareResult AreVisuallyEqual(string wordPath1, string wordPath2)
        {
            var doc1 = new Document(wordPath1);
            var doc2 = new Document(wordPath2);

            var options = new ImageSaveOptions(SaveFormat.Png);

            for (int i = 0; i < doc1.PageCount; i++)
            {
                options.PageSet = new PageSet(i);

                string file1 = $"{Guid.NewGuid()}.png";
                string file2 = $"{Guid.NewGuid()}.png";

                doc1.Save(file1, options);
                doc2.Save(file2, options);

                // Compare images pixel-by-pixel
                Bitmap bmp1 = new Bitmap(file1);
                Bitmap bmp2 = new Bitmap(file2);

                bool areEqual = CompareBitmaps(bmp1, bmp2);

                bmp1.Dispose();
                bmp2.Dispose();

                File.Delete(file1);
                File.Delete(file2);

                if (!areEqual)
                {
                    return new CompareResult
                    {
                        Status = $"Difference found on page {i + 1}",
                        Same = false
                    };
                }
            }

            return new CompareResult
            {
                Status = "Word files are visually the same",
                Same = true
            };
        }

    }
}
