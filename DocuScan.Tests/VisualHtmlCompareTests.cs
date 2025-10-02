using DocuScan.Comparer;

namespace DocuScan.Tests;

[TestClass]
public class VisualHtmlCompareTests
{
    [TestMethod]
    public void Can_Detect_If_Two_Htmls_Are_Same()
    {
        var compare = new VisualHtmlCompare();
        var result = compare.AreVisuallyEqual(".\\HtmlDocuments\\Identical\\LawFiles1005.html", ".\\HtmlDocuments\\Identical\\LawFilesTest1005.html");
        Assert.IsTrue(result.Same);
    }

    [TestMethod]
    public void Can_Detect_If_Two_Htmls_Are_Different()
    {
        var compare = new VisualHtmlCompare();
        var result = compare.AreVisuallyEqual(".\\HtmlDocuments\\SlightlyDifferent\\1005.html", ".\\HtmlDocuments\\SlightlyDifferent\\1005_different.html");
        Assert.IsFalse(result.Same);
    }
}
