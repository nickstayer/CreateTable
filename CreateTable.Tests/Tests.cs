using System.Text;
using Microsoft.VisualBasic;

namespace CreateTable.Tests;

public class Tests
{
    private string _testDataDir = Path.GetFullPath("..\\..\\..\\testdata");

    [SetUp]
    public void Setup()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
    }

    [Test]
    public void GetHeadersWithQuotationMarksTest()
    {
        var file = Path.Combine(_testDataDir, "test_quotation_marks_in_headers.csv");
        var encoding = Encoding.GetEncoding("utf-8");
        var actual = Statement.GetHeaders(file, ',' , encoding);
        var expected = new List<string> { "ФИО ИГ", "Дата рождения", "Тип дела, тест", "Дата создания дела" };
        Assert.That(actual, Is.EqualTo(expected));
    }

    [TestCaseSource(nameof(GetHeadersTestCases))]
    public void GetHeadersTest(string fileName, char delimiter, string encodingName, List<string> expected)
    {
        var file = Path.Combine(_testDataDir, fileName);
        var encoding = Encoding.GetEncoding(encodingName);
        var actual = Statement.GetHeaders(file, delimiter, encoding);
        Assert.That(actual, Is.EqualTo(expected));
    }

    private static IEnumerable<TestCaseData> GetHeadersTestCases()
    {
        var expectedHeaders = new List<string> { "ФИО ИГ", "Дата рождения", "Тип дела", "Дата создания дела" };

        yield return new TestCaseData(
            "test_CSV_ANSI_comma.csv",
            ',',
            "windows-1251",
            expectedHeaders
        ).SetName("TestCaseAnsiComma");

        yield return new TestCaseData(
            "test_CSV_ANSI_semicolon.csv",
            ';',
            "windows-1251",
            expectedHeaders
        ).SetName("TestCaseAnsiSemicolon");

        yield return new TestCaseData(
            "test_CSV_UTF8_comma.csv",
            ',',
            "utf-8",
            expectedHeaders
        ).SetName("TestCaseUtf8Comma");

        yield return new TestCaseData(
            "test_CSV_UTF8_delimiter_semicolon.csv",
            ';',
            "utf-8",
            expectedHeaders
        ).SetName("TestCaseUtf8Semicolon");
    }

    [TestCase("Сотрудник, создавший дело", "Сотрудник_создавший_дело")]
    [TestCase("Сотрудник, \"создавший дело\"", "Сотрудник_создавший_дело")]
    [TestCase("\"Сотрудник, \"создавший дело\"", "Сотрудник_создавший_дело")]
    public void ReplaceInvalidCharsTest(string header, string expected)
    {
        var actual = Statement.ReplaceInvalidChars(header);
        Assert.That(actual, Is.EqualTo(expected));
    }

    [TestCaseSource(nameof(GetColumnSizesTestCases))]
    public void GetColumnSizesTest(string fileName, char delimiter, string encodingName, int[] expected)
    {
        var file = Path.Combine(_testDataDir, fileName);
        var encoding = Encoding.GetEncoding(encodingName);
        var actual = Statement.GetColumnSizes(file, delimiter, encoding, expected.Length);
        Assert.That(actual, Is.EqualTo(expected));
    }

    private static IEnumerable<TestCaseData> GetColumnSizesTestCases()
    {
        int[] ecxpectedSizes = [60, 10, 55, 10];

        yield return new TestCaseData(
            "test_CSV_ANSI_semicolon.csv",
            ';',
            "windows-1251",
            ecxpectedSizes
        ).SetName("TestCaseSizesAnsiSemicolon");
    }

    [TestCase("narushen_srok_vneseniya_protokola_ob_adm_pravonarushenii_v_sistemu", "narushen_srok_vneseniya_protokola_ob_adm_pravonarushenii_v_si")]
    [TestCase("narushen_srok_vneseniya_protokola_ob_adm_pravonarushenii", "narushen_srok_vneseniya_protokola_ob_adm_pravonarushenii")]
    public void CutHeaderTest(string header, string expected)
    {
        var actual = Statement.CutHeader(header);
        Assert.That(actual, Is.EqualTo(expected));
    }

    [TestCase("narushen_srok_vneseniya_protokola_ob_adm_pravonarushenii_v_si", "another_narushen_srok_vneseniya_protokola_ob_adm_pravonarushe")]
    [TestCase("narushen_srok", "another_narushen_srok")]
    public void MakeHeaderUniqTest(string header, string expected)
    {
        var actual = Statement.MakeHeaderUniq(header);
        Assert.That(actual, Is.EqualTo(expected));
    }
}