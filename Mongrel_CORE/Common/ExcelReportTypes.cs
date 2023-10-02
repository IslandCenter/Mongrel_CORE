namespace Mongrel.Common;

internal class ExcelReportTypes
{
    public enum ExcelReport
    {
        Unknown,
        Axiom,
        Cellebrite,
        Viking,
        Xry,
        BulkExtractor
    }

    public static string GetFileNameFromFormat(ExcelReport format)
    {
        return format switch
        {
            ExcelReport.Unknown => "unknown",
            ExcelReport.Axiom => "axiom",
            ExcelReport.Cellebrite => "cellebrite",
            ExcelReport.Viking => "viking",
            ExcelReport.Xry => "xry",
            ExcelReport.BulkExtractor => "bulkExtractor",
            _ => throw new ArgumentOutOfRangeException(nameof(format), format, null)
        };
    }
}