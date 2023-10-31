using CsvHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Mongrel.Common.Utils;
using Yipper;
using CsvHelper.Configuration;

namespace Mongrel.Inputs.ReportReaders.Csv
{
    public class GeoFetchHeaders
    {
        public int Id { get; set; }
        public string SOFEX { get; set; }
        public string Exhibit { get; set; }
        public string DeviceType { get; set; }
        public string FileName { get; set; }
        public string Hash { get; set; }
        public string Path { get; set; }
        public string FileType { get; set; }
        public string TimeStamp { get; set; }
        public string Longitude { get; set; }
        public string Latitude { get; set; }
        public string Mgrs { get; set; }
        public string Altitude { get; set; }
        public string AltitudeMode { get; set; }
        public string GeomeTryType { get; set; }
        public string DeviceMake { get; set; }
        public string DeviceModel { get; set; }
    }

    public class GeoFetch
    {
        private static readonly string[] _GeoFetchHeaders =
            {"id", "sofex", "exhibit", "devicetype", "filename", "hash", "path", "filetype", "timestamp",
        "longitude", "latitude", "mgrs", "altitude", "altitudemode", "geometrytype", "devicemake", "devicemodel"};

        public static bool IsGeoFetch(string filePath)
        {
            var headerRow = File.ReadLines(filePath).First();
            var headers = headerRow.ToLower().Split(',');
            return headers.SequenceEqual(_GeoFetchHeaders);
        }

        private static IEnumerable<Locations> ParseGeoFetchRows(IEnumerable<string> rows, string reportFileName)
        {
            return rows.Select(x => ParseGeoFetchRow(x, reportFileName));
        }

        private static Locations ParseGeoFetchRow(string row, string reportFileName)
        {
            if (row.All(c => c == ',')) return new Locations();

            var columns = row.Split(',');

            return new Locations(
                sofex: columns[1],
                deviceType: columns[3],
                fileName: columns[4],
                hash: columns[5],
                path: columns[6],
                timeStr: columns[8],
                mgrs: columns[11],
                altitude: columns[12],
                altitudeMode: columns[13],
                load: "",
                sheetName: "GeoFetch",
                columnName: "Latitude Longitude",
                reportType: "GeoFetch",
                deleted: "",
                bssid: "",
                ssid: "",
                notes: "",
                exhibit: columns[2],
                origin: reportFileName,
                originalLon: columns[9],
                originalLat: columns[10],
                timestamp: columns[8],
                convertedLon: TryConvertToDouble(columns[9]) ?? 999999,
                convertedLat: TryConvertToDouble(columns[10]) ?? 999999);
        }

        public static IEnumerable<Locations> ReadGeoFetchCsv(string reportFilePath)
        {
            if (!IsGeoFetch(reportFilePath))
            {
                Logger.Instance.Info($"File not detected as a GeoFetch CSV file | Header mismach, file: {reportFilePath}");
                return Enumerable.Empty<Locations>();
            };

            Logger.Instance.Info($"Reading as GeoFetch CSV file: {reportFilePath}");

            var reportFileName = Path.GetFileName(reportFilePath);
            var locations = ParseGeoFetchRows(File.ReadLines(reportFilePath).Skip(1), reportFileName);

            if (!locations.Any())
            {
                Logger.Instance.Info($"No rows in file: {reportFileName}");
                return Enumerable.Empty<Locations>();
            }

            return locations;
        }
    }
}
