using Aspose.Html;
using Aspose.Html.Drawing;
using Aspose.Html.Rendering.Image;
using System.IO;
using Size = Aspose.Html.Drawing.Size;

namespace DocuScan.Comparer
{
    public class VisualHtmlCompare : VisualCompare
    {
        public override CompareResult AreVisuallyEqual(string htmlPath1, string htmlPath2)
        {
            // Load HTML documents
            using var htmlDoc1 = new HTMLDocument(htmlPath1);
            using var htmlDoc2 = new HTMLDocument(htmlPath2);

            // Render settings
            var renderOptions = new ImageRenderingOptions(ImageFormat.Png)
            {
                PageSetup = { AnyPage = new Page(new Size(1920, 1080)) } // set viewport size
            };

            try
            {
                using var ms1 = new MemoryStream();
                using var ms2 = new MemoryStream();

                // Render HTML to PNG streams
                using (var device1 = new Aspose.Html.Rendering.Image.ImageDevice(renderOptions, ms1))
                {
                    htmlDoc1.RenderTo(device1);
                }

                using (var device2 = new Aspose.Html.Rendering.Image.ImageDevice(renderOptions, ms2))
                {
                    htmlDoc2.RenderTo(device2);
                }

                // Reset streams for reading
                ms1.Position = 0;
                ms2.Position = 0;

                using var bitmap1 = new Bitmap(ms1);
                using var bitmap2 = new Bitmap(ms2);

                var compare = CompareBitmaps(bitmap1, bitmap2);

                return new CompareResult
                {
                    Status = compare ? "HTML files are visually the same" : "HTML files differ visually",
                    Same = compare
                };
            }
            catch (Exception ex)
            {
                return new CompareResult
                {
                    Status = $"Error: {ex.Message}",
                    Same = false
                };
            }
        }
    }
}
