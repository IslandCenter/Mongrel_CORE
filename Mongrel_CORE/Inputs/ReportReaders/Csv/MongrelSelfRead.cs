using CsvHelper;
using CsvHelper.Configuration;
using Yipper;

namespace Mongrel.Inputs.ReportReaders.Csv
{
    public class MongrelSelfRead
    {
        public class MongrelHeaders
        {
            public int Id { get; set; }
            public string? Sofex { get; set; }
            public string? Exhibit { get; set; }
            public string? Devicetype { get; set; }
            public string? Origin { get; set; }
            public string? Filename { get; set; }
            public string? Hash { get; set; }
            public string? Path { get; set; }
            public string? Timestr { get; set; }
            public string? Timestamp { get; set; }
            public string? Originallat { get; set; }
            public string? Originallon { get; set; }
            public double Convertedlat { get; set; }
            public double Convertedlon { get; set; }
            public string? Mgrs { get; set; }
            public string? Altitude { get; set; }
            public string? Altitudemode { get; set; }
            public string? Load { get; set; }
            public string? Sheetname { get; set; }
            public string? Columnname { get; set; }
            public string? Reporttype { get; set; }
            public string? Deleted { get; set; }
            public string? Bssid { get; set; }
            public string? Ssid { get; set; }
            public string? Notes { get; set; }
        }

        public static IEnumerable<Locations> ReadMongrel(string reportFilePath, CsvConfiguration? csvConfig)
        {
            var reportFileName = Path.GetFileName(reportFilePath);

            using var reader = new StreamReader(reportFilePath);
            using var csv = new CsvReader(reader, csvConfig);

            csv.Read();
            csv.ReadHeader();

            IEnumerable<MongrelHeaders> records;
            try
            {
                Logger.Instance.Info("Attempting to read file as Mongrel CSV");
                records = csv.GetRecords<MongrelHeaders>();
                _ = records.Any();
            }
            catch
            {
                Logger.Instance.Info("File not detected as a Mongrel CSV file");
                yield break;
            }

            foreach (var record in records)
            {
                yield return new Locations
                {
                    Sofex = record.Sofex ?? "",
                    DeviceType = record.Devicetype ?? "",
                    FileName = record.Filename ?? "",
                    Hash = record.Hash ?? "",
                    Path = record.Path ?? "",
                    TimeStr = record.Timestr ?? "",
                    Mgrs = record.Mgrs ?? "",
                    Altitude = record.Altitude ?? "",
                    AltitudeMode = record.Altitudemode ?? "",
                    Load = record.Load ?? "",
                    SheetName = record.Sheetname ?? "",
                    ColumnName = record.Columnname ?? "",
                    ReportType = $"Mongrel Injest | Type: {record.Reporttype}",
                    Deleted = record.Deleted ?? "",
                    Bssid = record.Bssid ?? "",
                    Ssid = record.Ssid ?? "",
                    Notes = record.Notes ?? "",
                    Exhibit = record.Exhibit ?? "",
                    Origin = $"Mongrel Injest:{reportFileName} | Origin: {record.Origin}",
                    OriginalLon = record.Originallon ?? "",
                    OriginalLat = record.Originallat ?? "",
                    Timestamp = record.Timestamp ?? "",
                    ConvertedLon = record.Convertedlon,
                    ConvertedLat = record.Convertedlat
                };
            }
        }
    }
}
