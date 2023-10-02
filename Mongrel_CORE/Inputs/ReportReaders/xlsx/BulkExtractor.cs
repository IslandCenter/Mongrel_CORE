using static Mongrel.Common.Utils;

namespace Mongrel.Inputs.ReportReaders.xlsx;

public class BulkExtractor : ReportReader
{
    public BulkExtractor(ExcelDocument document, IEnumerable<(string name, int index)> validSheetAndIndex) : base(document, validSheetAndIndex) { }

    public override IEnumerable<Locations> GetLocations()
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

            if (!dirtyLocationDict.TryGetValue("Modified", out var timeStamp))
                timeStamp = "";
            if (!dirtyLocationDict.TryGetValue("Location Name", out var locationName))
                locationName = "";
            if (!dirtyLocationDict.TryGetValue("Unique ID", out var uniqueId))
                uniqueId = "";
            if (!dirtyLocationDict.TryGetValue("Search String", out var searchString))
                searchString = "";
            if (!dirtyLocationDict.TryGetValue("Related Application", out var relatedApplication))
                relatedApplication = "";

            var type = "";
            if (string.IsNullOrEmpty(searchString) && !dirtyLocationDict.TryGetValue("Type", out type))
                type = "";

            var normLon = NormalizeCordStr(lon);
            var normLat = NormalizeCordStr(lat);
            var (sofex, exhibit, deviceType) = ParseReportFileName(Document.FileName);

            yield return new Locations
            {
                Sofex = sofex,
                DeviceType = deviceType,
                FileName = locationName,
                Hash = uniqueId,
                Path = "",
                TimeStr = timeStamp,
                Mgrs = GetMgrs(normLat, normLon),
                Altitude = "",
                AltitudeMode = "",
                Load = "",
                Category = relatedApplication,
                ReportType = "Bulk Extractor",
                Deleted = "",
                Bssid = "",
                Ssid = "",
                Notes = string.IsNullOrEmpty(searchString) ? type : searchString,
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