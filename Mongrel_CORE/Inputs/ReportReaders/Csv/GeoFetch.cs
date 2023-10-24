using CsvHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Mongrel.Common.Utils;
using Yipper;

namespace Mongrel.Inputs.ReportReaders.Csv
{
    public class GeoFetchHeaders
    {
        public int Id { get; set; }
        public string SOFEX { get; set; }
        public string Exhibit { get; set; }
        public string DeviceType { get; set; }
        public string Filename { get; set; }
        public string Hash { get; set; }
        public string Path { get; set; }
        public string Filetype { get; set; }
        public string Timestamp { get; set; }
        public string Longitude { get; set; }
        public string Latitude { get; set; }
        public string MGRS { get; set; }
        public string Altitude { get; set; }
        public string AltitudeMode { get; set; }
        public string GeometryType { get; set; }
        public string DeviceMake { get; set; }
        public string DeviceModel { get; set; }
    }

    public class GeoFetch
    {
        public static IEnumerable<Locations> ReadGeoFetchCsv(IReader csv, string reportFileName)
        {
            IEnumerable<GeoFetchHeaders> records;
            try
            {
                Logger.Instance.Info("Attempting to read file as GeoFetch CSV");
                records = csv.GetRecords<GeoFetchHeaders>();

                var testCsvRecordHeaders = csv.GetRecords<CsvRecord>();
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
                    Sofex = record.SOFEX ?? sofex,
                    DeviceType = record.DeviceType ?? deviceType,
                    FileName = record.Filename,
                    Hash = record.Hash,
                    Path = record.Path,
                    TimeStr = record.Timestamp,
                    Mgrs = record.MGRS ?? GetMgrs(normLat, normLon),
                    Altitude = record.Altitude,
                    AltitudeMode = record.AltitudeMode,
                    Load = "",
                    SheetName = record.Filetype,
                    ColumnName = $"Type-{record.GeometryType}",
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
