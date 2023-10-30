using CsvHelper;
using CsvHelper.Configuration;
using Yipper;
using static Mongrel.Common.Utils;

namespace Mongrel.Inputs.ReportReaders.Csv
{
    public class MongrelSelfRead
    {
        private static readonly string[] _MongrelHeaders = 
            {"id", "sofex", "exhibit", "devicetype", "origin", "filename", "hash", "path", "timestr", "originallat", 
            "originallon", "convertedlat", "convertedlon", "mgrs", "altitude", "altitudemode", "load", "sheetname",
            "columnname", "reporttype", "deleted", "bssid", "ssid", "notes"};
        
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
            var columns = row.Split(',');

            return new Locations(
                sofex: columns[0],
                deviceType: columns[1],
                fileName: $"Mongrel Injest: {reportFileName} |> {columns[2]}",
                hash: columns[3],
                path: columns[4],
                timeStr: columns[5],
                mgrs: columns[6],
                altitude: columns[7],
                altitudeMode: columns[8],
                load: columns[9],
                sheetName: columns[10],
                columnName: columns[11],
                reportType: columns[12],
                deleted: columns[13],
                bssid: columns[14],
                ssid: columns[15],
                notes: columns[16],
                exhibit: columns[17],
                origin: columns[18],
                originalLon: columns[19],
                originalLat: columns[20],
                timestamp: columns[21],
                convertedLon: TryConvertToDouble(columns[22]) ?? 999999,
                convertedLat: TryConvertToDouble(columns[23]) ?? 999999);
        }

        public static IEnumerable<Locations> ReadMongrel(string reportFilePath)
        {
            Logger.Instance.Info("Attempting to read file as Mongrel CSV");

            if (!IsMongrel(reportFilePath))
            {
                Logger.Instance.Info("File not detected as a Mongrel CSV file | Header mismach");
                return Enumerable.Empty<Locations>();
            };

            var reportFileName = Path.GetFileName(reportFilePath);
            var locations = ParseMongrelRows(File.ReadLines(reportFilePath).Skip(1), reportFileName);

            if (!locations.Any())
            {
                Logger.Instance.Info("File not detected as a Mongrel CSV file | No rows");
                return Enumerable.Empty<Locations>();
            }

            return locations;
        }
    }
}
