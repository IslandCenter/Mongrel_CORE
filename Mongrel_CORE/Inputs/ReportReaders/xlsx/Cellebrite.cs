using static Mongrel.Common.Utils;

namespace Mongrel.Inputs.ReportReaders.xlsx;

public class Cellebrite : ReportReader
{
    public Cellebrite(ExcelDocument document, IEnumerable<(string name, int index)> validSheetAndIndex) : base(document, validSheetAndIndex) { }

    public override IEnumerable<Locations>? GetLocations()
    {
        var sheets = Document.GetRowsFromValidSheets(ValidSheetAndIndex);

        foreach (var dirtyLocationDict in GetDirtyLocationDict(sheets))
        {
            if (!dirtyLocationDict.TryGetValue("Latitude", out var lat))
                lat = "";
            if (!dirtyLocationDict.TryGetValue("Longitude", out var lon))
                lon = "";
            if (string.IsNullOrEmpty(lat) && string.IsNullOrEmpty(lon))
            {
                yield return new Locations();
                continue;
            }

            if (!dirtyLocationDict.TryGetValue("Time", out var timeStamp))
                timeStamp = "";
            if (!dirtyLocationDict.TryGetValue("Category", out var category))
                category = "";
            if (string.IsNullOrEmpty(category) && !dirtyLocationDict.TryGetValue("Source", out category))
                category = "";
            if (!dirtyLocationDict.TryGetValue("Deleted", out var deleted))
                deleted = "";

            if (!dirtyLocationDict.TryGetValue("Description", out var description))
                description = "";
            var (bssid, ssid) = GetBssidSSid(description);

            var normLon = NormalizeCordStr(lon);
            var normLat = NormalizeCordStr(lat);
            var (sofex, exhibit, deviceType) = ParseReportFileName(Document.FileName);

            yield return new Locations
            {
                Sofex = sofex,
                DeviceType = deviceType,
                FileName = "",
                Hash = "",
                Path = "",
                TimeStr = timeStamp,
                Mgrs = GetMgrs(normLat, normLon),
                Altitude = "",
                AltitudeMode = "",
                Load = "",
                SheetName = category,
                ColumnName = "Latitude_Longitude",
                ReportType = "Cellebrite",
                Deleted = deleted,
                Bssid = bssid,
                Ssid = ssid,
                Notes = description,
                Exhibit = exhibit,
                Origin = Document.FileName,
                OriginalLon = lon,
                OriginalLat = lat,
                Timestamp = NormalizeTimeStr(timeStamp),
                ConvertedLon = normLon,
                ConvertedLat = normLat
            };
        }
    }
}