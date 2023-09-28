using System.Collections.Generic;
using System.Linq;
using static Mongrel.Common.Utils;

namespace Mongrel.Inputs.ReportReaders.xlsx
{
    public class Viking : ReportReader
    {
        public Viking(ExcelDocument document, IEnumerable<(string name, int index)> validSheetAndIndex) : base(document, validSheetAndIndex) { }

        public override IEnumerable<Locations> GetLocations()
        {
            var sheets = Document.GetRowsFromValidSheets(ValidSheetAndIndex);

            foreach (var dirtyLocationDict in GetDirtyLocationDict(sheets))
            {

                if (!dirtyLocationDict.TryGetValue("List of Track Points", out var trackPointsList))
                    trackPointsList = "";

                if (!string.IsNullOrEmpty(trackPointsList))
                {
                    var parsedData = GetPointListData(trackPointsList);

                    foreach (var innerRow in parsedData)
                    {
                        yield return NormalizeTskRow(innerRow);
                    }
                }
                else yield return NormalizeNormalRow(dirtyLocationDict);

            }
        }

        private Locations NormalizeNormalRow(IReadOnlyDictionary<string, string> loadedRowData)
        {
            if (!loadedRowData.TryGetValue("Latitude", out var lat))
                lat = "";
            if (!loadedRowData.TryGetValue("Longitude", out var lon))
                lon = "";
            if (!loadedRowData.TryGetValue("Altitude", out var alt))
                alt = "";

            if (string.IsNullOrEmpty(lat) && string.IsNullOrEmpty(lon)) return new Locations();

            var normLon = NormalizeCordStr(lon);
            var normLat = NormalizeCordStr(lat);
            var (sofex, exhibit, deviceType) = ParseReportFileName(Document.FileName);

            return new Locations
            {
                Sofex = sofex,
                DeviceType = deviceType,
                FileName = "",
                Hash = "",
                Path = "",
                TimeStr = "",
                Mgrs = GetMgrs(normLat, normLon),
                Altitude = alt,
                AltitudeMode = "",
                Load = "",
                Category = "",
                ReportType = "Viking",
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

        private Locations NormalizeTskRow(IReadOnlyDictionary<string, string> loadedRowData)
        {
            if (!loadedRowData.TryGetValue("TSK_DATETIME", out var dateTime))
                dateTime = "";
            if (!loadedRowData.TryGetValue("TSK_GEO_LATITUDE", out var lat))
                lat = "";
            if (!loadedRowData.TryGetValue("TSK_GEO_LONGITUDE", out var lon))
                lon = "";
            if (!loadedRowData.TryGetValue("TSK_GEO_ALTITUDE", out var alt))
                alt = "";

            if (string.IsNullOrEmpty(lat) && string.IsNullOrEmpty(lon)) return new Locations();

            var normLon = NormalizeCordStr(lon);
            var normLat = NormalizeCordStr(lat);
            var (sofex, exhibit, deviceType) = ParseReportFileName(Document.FileName);

            return new Locations
            {
                Sofex = sofex,
                DeviceType = deviceType,
                FileName = "",
                Hash = "",
                Path = "",
                TimeStr = dateTime,
                Mgrs = GetMgrs(normLat, normLon),
                Altitude = alt,
                AltitudeMode = "",
                Load = "",
                Category = "",
                ReportType = "Viking",
                Deleted = "",
                Bssid = "",
                Ssid = "",
                Notes = "",
                Exhibit = exhibit,
                Origin = Document.FileName,
                OriginalLon = lon,
                OriginalLat = lat,
                Timestamp = NormalizeTimeStr(dateTime),
                ConvertedLon = normLon,
                ConvertedLat = normLat
            };
        }

        public static List<Dictionary<string, string>> GetPointListData(string pointList)
        {
            var data = new List<Dictionary<string, string>>();

            const string startingPattern = "{\"pointList\":[{";
            if (!pointList.Contains(startingPattern)) return data;

            pointList = pointList.Remove(0, startingPattern.Length);

            var points = pointList.Split('{');

            if (points.Length > 1)
            {
                data.AddRange(points.Select(point => GetPointData(point.Split(','))));
            }
            else
            {
                data.Add(GetPointData(points[0].Split(',')));
            }
            return data;
        }

        public static Dictionary<string, string> GetPointData(string[] pointData) =>
            pointData.Select(item => item
                    .Replace("\"", string.Empty)
                    .Replace("}", string.Empty)
                    .Replace("]", string.Empty)
                    .Split(':'))
                .Where(split => split.Length == 2)
                .ToDictionary(split => split[0], split => split[1]);
    }
}
