using CsvHelper;
using CsvHelper.Configuration;
using Yipper;
using static Mongrel.Common.Utils;

namespace Mongrel.Inputs.ReportReaders.Csv
{
    public class MongrelSelfRead
    {
        private static readonly string[] _MongrelHeaders = 
            {"id", "sofex", "exhibit", "devicetype", "origin", "filename", "hash", "path", "timestr", "timestamp", 
            "originallat", "originallon", "convertedlat", "convertedlon", "mgrs", "altitude", "altitudemode", 
            "load", "sheetname", "columnname", "reporttype", "deleted", "bssid", "ssid", "notes"};
        
        public static bool IsMongrel(string filePath)
        {
            var headerRow = File.ReadLines(filePath).First();
            var headers = headerRow.ToLower().Split(',');
            return headers.SequenceEqual(_MongrelHeaders);
        }

        private static IEnumerable<Locations> ParseMongrelRows(IEnumerable<string> rows, string reportFileName)
        {
            return rows.Select(x => ParseMongrelRow(x, reportFileName));
        }

        private static Locations ParseMongrelRow(string row, string reportFileName)
        {
            if (row.All(c => c == ',')) return new Locations();

            var columns = row.Split(',');

            var testLon = TryConvertToDouble(columns[13]);
            var testLat = TryConvertToDouble(columns[12]);

            if (testLon == null || testLat == null)
                Logger.Instance.Warning($"Invalid lat/lon skipping in Mongrel read : {testLat}, {testLon}");

            return new Locations(
                sofex: columns[1],
                deviceType: columns[3],
                fileName: $"Mongrel Injest: {reportFileName} |> {columns[5]}",
                hash: columns[6],
                path: columns[7],
                timeStr: columns[8],
                mgrs: columns[14],
                altitude: columns[15],
                altitudeMode: columns[16],
                load: columns[17],
                sheetName: columns[18],
                columnName: columns[19],
                reportType: columns[20],
                deleted: columns[21],
                bssid: columns[22],
                ssid: columns[23],
                notes: columns[24],
                exhibit: columns[2],
                origin: columns[4],
                originalLon: columns[11],
                originalLat: columns[10],
                timestamp: columns[9],
                convertedLon: TryConvertToDouble(columns[13]) ?? 999999,
                convertedLat: TryConvertToDouble(columns[12]) ?? 999999);
        }

        public static IEnumerable<Locations> ReadMongrel(string reportFilePath)
        {
            Logger.Instance.Info("Attempting to read file as Mongrel CSV");

            if (!IsMongrel(reportFilePath))
            {
                Logger.Instance.Info($"File not detected as a Mongrel CSV file | Header mismach, file {reportFilePath}");
                return Enumerable.Empty<Locations>();
            };

            Logger.Instance.Info($"Reading as Mongrel CSV, file: {reportFilePath}");

            var reportFileName = Path.GetFileName(reportFilePath);
            var locations = ParseMongrelRows(File.ReadLines(reportFilePath).Skip(1), reportFileName);

            if (!locations.Any())
            {
                Logger.Instance.Info($"No rows in file {reportFileName}");
                return Enumerable.Empty<Locations>();
            }

            return locations;
        }
    }
}
