using CsvHelper.Configuration.Attributes;

namespace Mongrel;

public readonly struct CsvRecord
{
    public CsvRecord(Locations rowData, int id = -1)
    {
        Id = id;
        Sofex = rowData.Sofex;
        Exhibit = rowData.Exhibit;
        DeviceType = rowData.DeviceType;
        Origin = rowData.Origin;
        FileName = rowData.FileName;
        Hash = rowData.Hash;
        Path = rowData.Path;
        TimeStr = rowData.TimeStr;
        Timestamp = rowData.Timestamp;
        OriginalLon = rowData.OriginalLon;
        OriginalLat = rowData.OriginalLat;
        ConvertedLon = rowData.ConvertedLon;
        ConvertedLat = rowData.ConvertedLat;
        Mgrs = rowData.Mgrs;
        Altitude = rowData.Altitude;
        AltitudeMode = rowData.AltitudeMode;
        Load = rowData.Load;
        SheetName = rowData.SheetName;
        ReportType = rowData.ReportType;
        Deleted = rowData.Deleted;
        Bssid = rowData.Bssid;
        Ssid = rowData.Ssid;
        Notes = rowData.Notes;
    }
    [Index(0)] public int Id { get; }
    [Index(1)] public string Sofex { get; }
    [Index(2)] public string Exhibit { get; }
    [Index(3)] public string DeviceType { get; }
    [Index(4)] public string Origin { get; }
    [Index(5)] public string FileName { get; }
    [Index(6)] public string Hash { get; }
    [Index(7)] public string Path { get; }
    [Index(8)] public string? TimeStr { get; }
    [Index(9)] public string Timestamp { get; }
    [Index(10)] public string OriginalLat { get; }
    [Index(11)] public string OriginalLon { get; }
    [Index(12)] public double ConvertedLat { get; }
    [Index(13)] public double ConvertedLon { get; }
    [Index(14)] public string Mgrs { get; }
    [Index(15)] public string Altitude { get; }
    [Index(16)] public string AltitudeMode { get; }
    [Index(17)] public string Load { get; }
    [Index(18)] public string SheetName { get; }
    [Index(19)] public string ReportType { get; }
    [Index(20)] public string Deleted { get; }
    [Index(21)] public string Bssid { get; }
    [Index(22)] public string Ssid { get; }
    [Index(23)] public string Notes { get; }
}