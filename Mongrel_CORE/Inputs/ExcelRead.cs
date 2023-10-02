using Mongrel.Inputs.ReportReaders;
using Mongrel.Inputs.ReportReaders.xlsx;
using Yipper;
using static Mongrel.Common.ExcelReportTypes;
using static Mongrel.Common.Utils;

namespace Mongrel.Inputs;

internal class ExcelRead : Reader
{
    public static readonly (string name, int index)[] CellebriteSheetAndIndex =
    {
        ("Locations", 1),
        ("Genie Locations", 1)
    };
    public static readonly (string name, int index)[] AxiomSheetAndIndex =
    {
        ("Google Maps", 0),
        ("Pictures", 0),
        ("Apple Maps Searches", 0)
    };
    public static readonly (string name, int index)[] BulkExtractorSheetAndIndex =
    {
        ("GPS", 0)
    };
    public static readonly (string name, int index)[] VikingSheetAndIndex =
    {
        ("GPS Track", 0)
    };
    public static readonly (string name, int index)[] XrySheetAndIndex =
    {
        ("1_Files & Media Pictures", 0),
        ("1_Files & Media Audio", 0),
        ("1_Files & Media Videos", 0),
        ("1_Files & Media Documents", 0),
        ("1_Files & Media Archives", 0),
        ("1_Files & Media Databases", 0),
        ("1_Files & Media Application Bin", 0),
        ("1_Files & Media Readable Files", 0),
        ("1_Files & Media Unrecognized", 0),
        ("2_Files & Media Pictures", 0),
        ("Locations Locations", 0),
        ("Locations History", 0),
        ("Locations Bookmarks", 0),
        ("Locations Searches", 0),
    };

    private ReportReader _report;
    private ExcelDocument _document;

    public override IEnumerable<Locations> GetLocations(string filePath)
    {
        _document = new ExcelDocument(filePath);
        var reportType = GetReportType(filePath, _document.SheetNames);

        Logger.Instance.Info($"File {filePath} detected as report type {GetFileNameFromFormat(reportType)}");

        _report = ReportFactory(reportType, _document);
        return _report == null ? new List<Locations>() : _report.GetLocations();
    }

    public static ReportReader ReportFactory(ExcelReport reportType, ExcelDocument document)
    {
        return reportType switch
        {
            ExcelReport.Unknown => null,
            ExcelReport.Axiom => new Axiom(document, AxiomSheetAndIndex),
            ExcelReport.Cellebrite => new Cellebrite(document, CellebriteSheetAndIndex),
            ExcelReport.Viking => new Viking(document, VikingSheetAndIndex),
            ExcelReport.Xry => new Xry(document, XrySheetAndIndex),
            ExcelReport.BulkExtractor => new BulkExtractor(document, BulkExtractorSheetAndIndex),
            _ => throw new ArgumentOutOfRangeException(nameof(reportType), reportType, null)
        };
    }

    public static ExcelReport GetReportType(string path, IEnumerable<string> sheetNames)
    {
        var filePath = Path.GetFileName(path).ToLower();

        foreach (var type in EnumUtil.GetValues<ExcelReport>())
        {
            if (filePath.ToLower().Contains(GetFileNameFromFormat(type))) return type;
        }

        if (CellebriteSheetAndIndex.Any(sheet => sheetNames.Contains(sheet.name)))
        {
            return ExcelReport.Cellebrite;
        }
        if (AxiomSheetAndIndex.Any(sheet => sheetNames.Contains(sheet.name)))
        {
            return ExcelReport.Axiom;
        }
        if (VikingSheetAndIndex.Any(sheet => sheetNames.Contains(sheet.name)))
        {
            return ExcelReport.Viking;
        }
        if (BulkExtractorSheetAndIndex.Any(sheet => sheetNames.Contains(sheet.name)))
        {
            return ExcelReport.BulkExtractor;
        }

        if (XrySheetAndIndex.Any(sheet => sheetNames.Contains(sheet.name)))
        {
            return ExcelReport.Xry;
        }

        Logger.Instance.Warning($"Not a recognized xlsx report file: {path}");
        return ExcelReport.Unknown;
    }

    public override void Dispose()
    {
        _report?.Dispose();
        _document?.Dispose();
    }
}