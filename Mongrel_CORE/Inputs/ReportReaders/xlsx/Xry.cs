using System.Text.RegularExpressions;
using Yipper;
using static Mongrel.Common.Utils;

namespace Mongrel.Inputs.ReportReaders.xlsx;

public class Xry : ReportReader
{
    public Xry(ExcelDocument document, IEnumerable<(string name, int index)> validSheetAndIndex) : base(document, validSheetAndIndex) { }

    public Locations GetLocationFromLatitudeLongitudeKeys(Dictionary<string, string> dirtyDict, string sofex, string exhibit, string deviceType)
    {
        if (!dirtyDict.TryGetValue("Latitude", out var originalLat))
            originalLat = "";
        if (!dirtyDict.TryGetValue("Longitude", out var originalLon))
            originalLon = "";
        if (string.IsNullOrEmpty(originalLat) && string.IsNullOrEmpty(originalLon))
        {
            return new Locations();
        }

        if (!dirtyDict.TryGetValue("Altitude", out var altitude))
            altitude = "";
        if (!dirtyDict.TryGetValue("GPS Time", out var gpsTime))
            gpsTime = "";
        if (!dirtyDict.TryGetValue("Time", out var time))
            time = "";
        if (!dirtyDict.TryGetValue("Type", out var sheetName))
            sheetName = "";
        if (!dirtyDict.TryGetValue("Related Application", out var notes))
            notes = "";

        var timestamp = string.IsNullOrEmpty(gpsTime) ? time : gpsTime;
        var normLon = NormalizeCordStr(originalLon);
        var normLat = NormalizeCordStr(originalLat);

        return new Locations
        {
            Sofex = sofex,
            DeviceType = deviceType,
            FileName = "",
            Hash = "",
            Path = "",
            TimeStr = timestamp,
            Mgrs = GetMgrs(normLat, normLon),
            Altitude = altitude,
            AltitudeMode = "",
            Load = "",
            SheetName = sheetName,
            ReportType = "XRY",
            Deleted = "",
            Bssid = "",
            Ssid = "",
            Notes = notes,
            Exhibit = exhibit,
            Origin = Document.FileName,
            OriginalLon = originalLon,
            OriginalLat = originalLat,
            Timestamp = NormalizeTimeStr(timestamp),
            ConvertedLon = normLon,
            ConvertedLat = normLat
        };
    }

    public Locations GetLocationFromRawString(Dictionary<string, string> dirtyDict, string sofex, string exhibit, string deviceType)
    {
        if (!dirtyDict.TryGetValue("Raw", out var gpsString))
            gpsString = "";
        if(string.IsNullOrEmpty(gpsString))
        {
            Logger.Instance.Error($"Raw string detected but is Empty or Null in File: {Document.FileName}");
            return new Locations();
        }
        
        var dateGeoSplit = gpsString.Split(',');

        if (dateGeoSplit.Length < 3)
        {
            Logger.Instance.Error($"Improper gpsString split, skipping {gpsString} from file {Document.FileName}");
            return new Locations();
        }

        var lon = dateGeoSplit[2];
        var lat = dateGeoSplit[1];

        if (string.IsNullOrEmpty(lon) && string.IsNullOrEmpty(lat))
        {
            return new Locations();
        }

        var timestamp = dateGeoSplit.FirstOrDefault()?.Split('T').FirstOrDefault();
        var normLon = NormalizeCordStr(lon);
        var normLat = NormalizeCordStr(lat);

        return new Locations
        {
            Sofex = sofex,
            DeviceType = deviceType,
            FileName = "",
            Hash = "",
            Path = "",
            TimeStr = timestamp,
            Mgrs = GetMgrs(normLat, normLon),
            Altitude = "",
            AltitudeMode = "",
            Load = "",
            SheetName = "RAW",
            ReportType = "XRY",
            Deleted = "",
            Bssid = "",
            Ssid = "",
            Notes = "",
            Exhibit = exhibit,
            Origin = Document.FileName,
            OriginalLon = lon,
            OriginalLat = lat,
            Timestamp = NormalizeTimeStr(timestamp),
            ConvertedLon = normLon,
            ConvertedLat = normLat
        };
    }

    public Locations GetLocationFromGeographicLink(Dictionary<string, string> dirtyDict, string sofex, string exhibit, string deviceType)
    {
        if (!dirtyDict.TryGetValue("Geographic Link", out var gpsString))
            gpsString = "";

        if (string.IsNullOrEmpty(gpsString)) return new Locations();

        var (lat, lon) = ParseCoordinates(gpsString);

        var normLon = NormalizeCordStr(lon);
        var normLat = NormalizeCordStr(lat);

        return new Locations
        {
            Sofex = sofex,
            DeviceType = deviceType,
            FileName = "",
            Hash = "",
            Path = "",
            TimeStr = null,
            Mgrs = GetMgrs(normLat, normLon),
            Altitude = "",
            AltitudeMode = "",
            Load = "",
            SheetName = "Geographic Link",
            ReportType = "XRY",
            Deleted = "",
            Bssid = "",
            Ssid = "",
            Notes = "",
            Exhibit = exhibit,
            Origin = Document.FileName,
            OriginalLon = lon,
            OriginalLat = lat,
            Timestamp = "",
            ConvertedLon = normLon,
            ConvertedLat = normLat
        };
    }

    public static (string Latitude, string Longitude) ParseCoordinates(string input)
    {
        string pattern = @"(-?\d+\.\d+),\s*(-?\d+\.\d+)";
        var match = Regex.Match(input, pattern);

        if (match.Success)
            return (match.Groups[1].Value, match.Groups[2].Value);

        return ("99999.0", "99999.0");
    }

    public override IEnumerable<Locations>? GetLocations()
    {
        var sheets = Document.GetRowsFromValidSheets(ValidSheetAndIndex);

        foreach (var dirtyLocationDict in GetDirtyLocationDict(sheets))
        {
            var (sofex, exhibit, deviceType) = ParseReportFileName(Document.FileName);

            if (dirtyLocationDict.ContainsKey("Raw")) yield return GetLocationFromRawString(dirtyLocationDict, sofex, exhibit, deviceType);
            if (dirtyLocationDict.ContainsKey("Latitude") && dirtyLocationDict.ContainsKey("Longitude")) yield return GetLocationFromLatitudeLongitudeKeys(dirtyLocationDict, sofex, exhibit, deviceType);
            if (dirtyLocationDict.ContainsKey("Geographic Link")) yield return GetLocationFromGeographicLink(dirtyLocationDict, sofex, exhibit, deviceType);

            else
            {
                Logger.Instance.Error($"Unable to parse Coordinates from XRY read in File: {Document.FileName}");
                yield return new Locations();
            }
        }
    }
}