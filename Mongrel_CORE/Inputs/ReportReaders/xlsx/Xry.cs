using System.Collections.Generic;
using System.Linq;
using Yipper;
using static Mongrel.Common.Utils;

namespace Mongrel.Inputs.ReportReaders.xlsx
{
    public class Xry : ReportReader
    {
        public Xry(ExcelDocument document, IEnumerable<(string name, int index)> validSheetAndIndex) : base(document, validSheetAndIndex) { }

        public override IEnumerable<Locations> GetLocations()
        {
            var sheets = Document.GetRowsFromValidSheets(ValidSheetAndIndex);

            foreach (var dirtyLocationDict in GetDirtyLocationDict(sheets))
            {
                if (!dirtyLocationDict.TryGetValue("Raw", out var gpsString))
                    gpsString = "";

                var (sofex, exhibit, deviceType) = ParseReportFileName(Document.FileName);

                if (!string.IsNullOrEmpty(gpsString))
                {
                    var dateGeoSplit = gpsString.Split(',');

                    if (dateGeoSplit.Length < 3)
                    {
                        Logger.Instance.Error($"Improper gpsString split, skipping {gpsString} from file {Document.FileName}");
                        yield return new Locations();
                        continue;
                    }

                    var lon = dateGeoSplit[2];
                    var lat = dateGeoSplit[1];

                    if (string.IsNullOrEmpty(lon) && string.IsNullOrEmpty(lat))
                    {
                        yield return new Locations();
                        continue;
                    }

                    var timestamp = dateGeoSplit.FirstOrDefault()?.Split('T').FirstOrDefault();
                    var normLon = NormalizeCordStr(lon);
                    var normLat = NormalizeCordStr(lat);

                    yield return new Locations
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
                        Category = "",
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
                else
                {
                    if (!dirtyLocationDict.TryGetValue("Latitude", out var originalLat))
                        originalLat = "";
                    if (!dirtyLocationDict.TryGetValue("Longitude", out var originalLon))
                        originalLon = "";
                    if (string.IsNullOrEmpty(originalLat) && string.IsNullOrEmpty(originalLon))
                    {
                        yield return new Locations();
                        continue;
                    }

                    if (!dirtyLocationDict.TryGetValue("Altitude", out var altitude))
                        altitude = "";
                    if (!dirtyLocationDict.TryGetValue("GPS Time", out var gpsTime))
                        gpsTime = "";
                    if (!dirtyLocationDict.TryGetValue("Time", out var time))
                        time = "";
                    if (!dirtyLocationDict.TryGetValue("Type", out var category))
                        category = "";
                    if (!dirtyLocationDict.TryGetValue("Related Application", out var notes))
                        notes = "";

                    var timestamp = string.IsNullOrEmpty(gpsTime) ? time : gpsTime;
                    var normLon = NormalizeCordStr(originalLon);
                    var normLat = NormalizeCordStr(originalLat);

                    yield return new Locations
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
                        Category = category,
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
            }
        }
    }
}
