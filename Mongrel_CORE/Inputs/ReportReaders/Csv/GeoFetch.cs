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
        public string Sofex { get; set; }
        public string Exhibit { get; set; }
        public string Devicetype { get; set; }
        public string Filename { get; set; }
        public string Hash { get; set; }
        public string Path { get; set; }
        public string Filetype { get; set; }
        public string Timestamp { get; set; }
        public string Longitude { get; set; }
        public string Latitude { get; set; }
        public string Mgrs { get; set; }
        public string Altitude { get; set; }
        public string Altitudemode { get; set; }
        public string Geometrytype { get; set; }
        public string Devicemake { get; set; }
        public string Devicemodel { get; set; }
    }

    public class GeoFetch
    {
        public static IEnumerable<Locations> ReadGeoFetchCsv(string reportFilePath, CsvConfiguration? csvConfig)
        {
            var reportFileName = Path.GetFileName(reportFilePath);

            using var reader = new StreamReader(reportFilePath);
            using var csv = new CsvReader(reader, csvConfig);

            csv.Read();
            csv.ReadHeader();

            IEnumerable<GeoFetchHeaders> records;
            try
            {
                Logger.Instance.Info("Attempting to read file as GeoFetch CSV");
                records = csv.GetRecords<GeoFetchHeaders>();
                _ = records.Any();
            }
            catch
            {
                Logger.Instance.Info("File not detected as a GeoFetch CSV file");
                yield break;
            }

            var (sofex, exhibit, deviceType) = ParseReportFileName(reportFileName);

            foreach (var record in records)
            {
                if (string.IsNullOrEmpty(record.Longitude) && string.IsNullOrEmpty(record.Latitude))
                {
                    yield return new Locations();
                    continue;
                }

                var normLon = NormalizeCordStr(record.Longitude);
                var normLat = NormalizeCordStr(record.Latitude);

                yield return new Locations
                {
                    Sofex = record.Sofex ?? sofex,
                    DeviceType = record.Devicetype ?? deviceType,
                    FileName = record.Filename,
                    Hash = record.Hash,
                    Path = record.Path,
                    TimeStr = record.Timestamp,
                    Mgrs = record.Mgrs ?? GetMgrs(normLat, normLon),
                    Altitude = record.Altitude,
                    AltitudeMode = record.Altitudemode,
                    Load = "",
                    SheetName = record.Filetype,
                    ColumnName = $"Type-{record.Geometrytype}",
                    ReportType = "GeoFetch",
                    Deleted = "",
                    Bssid = "",
                    Ssid = "",
                    Notes = "",
                    Exhibit = record.Exhibit ?? exhibit,
                    Origin = reportFileName,
                    OriginalLon = record.Longitude,
                    OriginalLat = record.Latitude,
                    Timestamp = null,
                    ConvertedLon = normLon,
                    ConvertedLat = normLat
                };
            }
        }
    }
}
