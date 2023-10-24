namespace Mongrel;

public struct Locations
{
    public Locations(string sofex,
        string deviceType,
        string fileName,
        string hash,
        string path,
        string? timeStr,
        string mgrs,
        string altitude,
        string altitudeMode,
        string load,
        string sheetName,
        string columnName,
        string reportType,
        string deleted,
        string bssid,
        string ssid,
        string notes,
        string exhibit,
        string origin,
        string originalLon,
        string originalLat,
        double convertedLon,
        double convertedLat,
        string timestamp)
    {
        Sofex = sofex;
        DeviceType = deviceType;
        FileName = fileName;
        Hash = hash;
        Path = path;
        TimeStr = timeStr;
        Mgrs = mgrs;
        Altitude = altitude;
        AltitudeMode = altitudeMode;
        Load = load;
        SheetName = sheetName;
        ColumnName = columnName;
        ReportType = reportType;
        Deleted = deleted;
        Bssid = bssid;
        Ssid = ssid;
        Notes = notes;
        Exhibit = exhibit;
        Origin = origin;
        OriginalLon = originalLon;
        OriginalLat = originalLat;
        ConvertedLon = convertedLon;
        ConvertedLat = convertedLat;
        Timestamp = timestamp;
    }

    public string Sofex { get; set; }
    public string DeviceType { get; set; }
    public string FileName { get; set; }
    public string Hash { get; set; }
    public string Path { get; set; }
    public string? TimeStr { get; set; }
    public string Mgrs { get; set; }
    public string Altitude { get; set; }
    public string AltitudeMode { get; set; }
    public string Load { get; set; }
    public string SheetName { get; set; }
    public string ColumnName { get; set; }
    public string ReportType { get; set; }
    public string Deleted { get; set; }
    public string Bssid { get; set; }
    public string Ssid { get; set; }
    public string Notes { get; set; }
    public string Exhibit { get; set; }
    public string Origin { get; set; }
    public string OriginalLon { get; set; }
    public string OriginalLat { get; set; }
    public string Timestamp { get; set; }
    public double ConvertedLon { get; set; }
    public double ConvertedLat { get; set; }

}