using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using static Mongrel.Common.Utils;

namespace Mongrel.Inputs.ReportReaders.xlsx
{
    public class Axiom : ReportReader
    {
        public Axiom(ExcelDocument document, IEnumerable<(string name, int index)> validSheetAndIndex) : base(document, validSheetAndIndex) { }

        public (string lat, string lon) GetLatLon(Dictionary<string, string> dirtyLocationDict)
        {
            var lat = "";
            var lon = "";
            var rg = new Regex(@"([0-9]+[.][0-9]+[,][0-9]+[.][0-9]+)+", RegexOptions.None, TimeSpan.FromMilliseconds(100));

            if (!dirtyLocationDict.TryGetValue("Center of Map", out var latLon))
                latLon = "";
            if (!string.IsNullOrEmpty(latLon) && rg.Match(latLon).Length > 0)
            {
                lat = latLon.Split(',').First();
                lon = latLon.Split(',').Last();
                return (lat, lon);
            }

            if (string.IsNullOrEmpty(lat) && string.IsNullOrEmpty(lon))
            {
                if (!dirtyLocationDict.TryGetValue("GPS Latitude", out lat))
                    lat = "";
                if (!dirtyLocationDict.TryGetValue("GPS Longitude", out lon))
                    lon = "";
                return (lat, lon);
            }

            if (!dirtyLocationDict.TryGetValue("Search Query", out var searchQuery)) return (lat, lon);

            var split = Regex.Replace(searchQuery, @"([^0-9.,])+", "", RegexOptions.None, TimeSpan.FromMilliseconds(100)).Split(',');

            lon = split.First();
            lat = split.Last();
            return (lat, lon);

        }

        public override IEnumerable<Locations> GetLocations()
        {
            var sheets = Document.GetRowsFromValidSheets(ValidSheetAndIndex);


            foreach (var dirtyLocationDict in GetDirtyLocationDict(sheets))
            {

                var (lat, lon) = GetLatLon(dirtyLocationDict);

                if (string.IsNullOrEmpty(lat) && string.IsNullOrEmpty(lon))
                {
                    yield return new Locations();
                    continue;
                }

                if (!dirtyLocationDict.TryGetValue("sheetName", out var sheetName))
                    sheetName = "";
                if (!dirtyLocationDict.TryGetValue("Make", out var make))
                    make = "";
                if (!dirtyLocationDict.TryGetValue("Model", out var model))
                    model = "";
                if (!dirtyLocationDict.TryGetValue("Altitude", out var altitude))
                    altitude = "";
                if (!dirtyLocationDict.TryGetValue("Created Date/Time - UTC+00:00 (M/d/yyyy)", out var timestamp))
                    timestamp = "";
                if (!dirtyLocationDict.TryGetValue("File Name", out var fileName))
                    fileName = "";
                if (!dirtyLocationDict.TryGetValue("Address", out var notes))
                    notes = "";
                if (!dirtyLocationDict.TryGetValue("Deleted source", out var deleted))
                    deleted = "";

                var normLon = NormalizeCordStr(lon);
                var normLat = NormalizeCordStr(lat);
                var (sofex, exhibit, deviceType) = ParseReportFileName(Document.FileName);

                if (!string.IsNullOrEmpty(make) && !string.IsNullOrEmpty(model)) deviceType = $"{make} | {model}";

                yield return new Locations
                {
                    Sofex = sofex,
                    DeviceType = deviceType,
                    FileName = fileName,
                    Hash = "",
                    Path = "",
                    TimeStr = timestamp,
                    Mgrs = GetMgrs(normLat, normLon),
                    Altitude = altitude,
                    AltitudeMode = "",
                    Load = "",
                    Category = sheetName,
                    ReportType = "Axiom",
                    Deleted = deleted,
                    Bssid = "",
                    Ssid = "",
                    Notes = notes,
                    Exhibit = exhibit,
                    Origin = Document.FileName,
                    OriginalLon = lon,
                    OriginalLat = lat,
                    Timestamp = NormalizeTimeStr(timestamp),
                    ConvertedLon = normLon,
                    ConvertedLat = normLat
                };
            }
        }
    }
}
