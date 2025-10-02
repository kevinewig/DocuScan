using DocuScan.Comparer;

namespace DocuScan.Tests
{
    [TestClass]
    public sealed class VisualPdfCompareTests
    {
        [TestMethod]
        public void Can_Detect_If_Two_Pdfs_Are_Different()
        {
            var compare = new VisualPdfCompare();
            var result = compare.AreVisuallyEqual(".\\PdfDocuments\\SlightlyDifferent\\hj_25_001.pdf", ".\\PdfDocuments\\SlightlyDifferent\\hj_25_001_different.pdf");
            Assert.IsFalse(result.Same);
        }

        [TestMethod]
        public void Can_Detect_If_Two_Pdfs_Are_Same()
        {
            var compare = new VisualPdfCompare();
            var result = compare.AreVisuallyEqual(".\\PdfDocuments\\Identical\\LawFiles 5583-S AMH ORCU LEWI 053.pdf", ".\\PdfDocuments\\Identical\\LawFilesTest 5583-S AMH ORCU LEWI 053.pdf");
            Assert.IsTrue(result.Same);
        }
    }
}
