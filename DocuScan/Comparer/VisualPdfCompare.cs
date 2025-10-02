using Aspose.Pdf;
using Aspose.Pdf.Devices;

namespace DocuScan.Comparer
{
    public class VisualPdfCompare : VisualCompare
    {
        public override CompareResult AreVisuallyEqual(string pdfPath1, string pdfPath2)
        {
            var pdfDoc1 = new Document(pdfPath1);
            var pdfDoc2 = new Document(pdfPath2);

            if (pdfDoc1.Pages.Count != pdfDoc2.Pages.Count)
            {
                return new CompareResult
                {
                    Status = "Page counts differ",
                    Same = false
                };
            }

            Resolution resolution = new Resolution(300);
            PngDevice pngDevice = new PngDevice(resolution);

            for (int i = 1; i <= pdfDoc1.Pages.Count; i++)
            {
                using var memoryStream1 = pdfDoc1.Pages[i].ConvertToPNGMemoryStream();
                using var memoryStream2 = pdfDoc2.Pages[i].ConvertToPNGMemoryStream();

                pngDevice.Process(pdfDoc1.Pages[i], memoryStream1);
                pngDevice.Process(pdfDoc2.Pages[i], memoryStream2);
                                                             
                using var bitmap1 = new Bitmap(memoryStream1);                               
                using var bitmap2 = new Bitmap(memoryStream2);

                var compare = CompareBitmaps(bitmap1, bitmap2);

                if (!compare)
                {
                    return new CompareResult
                    {
                        Status = "PDF files are not the same",
                        Same = false
                    };
                }
            }

            return new CompareResult
            {
                Status = "PDF files are the same",
                Same = true
            };
        }

    }

}
