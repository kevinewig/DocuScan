using DocuScan.Comparer;

namespace DocuScan.Tests
{
    [TestClass]
    public sealed class VisualWordCompareTests
    {
        [TestMethod]
        public void Can_Detect_If_Two_WordDocs_Are_Different()
        {
            var compare = new VisualWordCompare();
            var result = compare.AreVisuallyEqual(".\\WordDocuments\\SlightlyDifferent\\1022.docx", ".\\WordDocuments\\SlightlyDifferent\\1022_different.docx");
            Assert.IsFalse(result.Same);
        }

        /// <summary>
        /// Spaces were added to the second _different document, but they are visually identical.
        /// </summary>
        [TestMethod]
        public void Can_Detect_If_Two_WordDocs_Have_Different_But_Are_Visually_The_Same()
        {
            var compare = new VisualWordCompare();
            var result = compare.AreVisuallyEqual(".\\WordDocuments\\SlightlyDifferent\\1015-S AMH YBAR ALLI 417.docx", ".\\WordDocuments\\SlightlyDifferent\\1015-S AMH YBAR ALLI 417_different.docx");
            Assert.IsTrue(result.Same);
        }

        /// <summary>
        /// Metadata (comments, tags) were added to the properties of the second _different document, but they are visually identical.
        /// </summary>
        [TestMethod]
        public void Can_Detect_If_Two_WordDocs_Have_Different_Metadata_But_Are_Visually_The_Same()
        {
            var compare = new VisualWordCompare();
            var result = compare.AreVisuallyEqual(".\\WordDocuments\\SlightlyDifferent\\1160-S AMH WALE WRIK 399.docx", ".\\WordDocuments\\SlightlyDifferent\\1160-S AMH WALE WRIK 399_different.docx");
            Assert.IsTrue(result.Same);
        }

        [TestMethod]
        public void Can_Detect_If_Two_WordDocs_Are_Same()
        {
            var compare = new VisualWordCompare();
            var result = compare.AreVisuallyEqual(".\\WordDocuments\\Identical\\LawFiles 1021.docx", ".\\WordDocuments\\Identical\\LawFilesTest 1021.docx");
            Assert.IsTrue(result.Same);
        }
    }
}
