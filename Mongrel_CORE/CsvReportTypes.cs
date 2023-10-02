namespace Mongrel;

internal class CsvReportTypes
{
    public enum CsvReports
    {
        Unknown = -1,
        GeoFetch = 0,
    }

    public static string GetFileNameFromFormat(CsvReports format)
    {
        return format switch
        {
            CsvReports.Unknown => "unknown",
            CsvReports.GeoFetch => "geofetch",
            _ => throw new ArgumentOutOfRangeException(nameof(format), format, null)
        };
    }

    public static CsvReports GetFileFormatsFromName(string name)
    {
        return name switch
        {
            "unknown" => CsvReports.Unknown,
            "axiom" => CsvReports.GeoFetch,
            _ => throw new Exception("Unknown CSV extension type found")
        };
    }
}