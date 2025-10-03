using Aspose.Words;
using Aspose.Words.Saving;
using SkiaSharp;

namespace DocuScan.Comparer
{
    public class VisualWordCompare : VisualCompare
    {
        public override CompareResult AreVisuallyEqual(string wordPath1, string wordPath2)
        {
            try
            {
                var doc1 = new Document(wordPath1);
                var doc2 = new Document(wordPath2);

                var options = new ImageSaveOptions(SaveFormat.Png);

                for (int i = 0; i < doc1.PageCount; i++)
                {
                    options.PageSet = new PageSet(i);

                    // Compare images pixel-by-pixel
                    using var bmp1 = RenderPageToBitmap(doc1, i);
                    using var bmp2 = RenderPageToBitmap(doc2, i);
                    bool areEqual = AreEqual(bmp1, bmp2);

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
            catch (Exception ex)
            {
                return new CompareResult
                {
                    Status = $"Error during comparison: {ex.Message}",
                    Same = false
                };
            }
        }

        private SKBitmap RenderPageToBitmap(Document doc, int pageIndex, float scale = 1.0f)
        {
            // Page dimensions in points (1 point = 1/72 inch)
            var pageInfo = doc.GetPageInfo(pageIndex);
            float width = (float)(pageInfo.WidthInPoints * scale);
            float height = (float)(pageInfo.HeightInPoints * scale);

            var info = new SKImageInfo((int)width, (int)height, SKColorType.Bgra8888, SKAlphaType.Premul);
            var bitmap = new SKBitmap(info);

            using (var canvas = new SKCanvas(bitmap))
            {
                canvas.Clear(SKColors.White);

                // Render the page into the SkiaSharp canvas
                // Aspose exposes RenderToScale / RenderToSize overloads that accept a Graphics-like object.
                // In .NET Standard builds, that "graphics" is backed by SkiaSharp.
                doc.RenderToScale(pageIndex, canvas, 0, 0, scale);
            }

            return bitmap;
        }

        private static bool AreEqual(SKBitmap bmp1, SKBitmap bmp2)
        {
            if (bmp1 == null || bmp2 == null)
                return false;

            if (bmp1.Width != bmp2.Width || bmp1.Height != bmp2.Height)
                return false;

            // Get raw pixel spans
            var span1 = bmp1.GetPixelSpan();
            var span2 = bmp2.GetPixelSpan();

            return span1.SequenceEqual(span2);
        }

    }
}
