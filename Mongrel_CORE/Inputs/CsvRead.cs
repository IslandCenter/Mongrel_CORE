using System.Globalization;
using CsvHelper;
using Yipper;
using static Mongrel.Common.Utils;

namespace Mongrel.Inputs;

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

internal class CsvRead : Reader
{
    public override IEnumerable<Locations> GetLocations(string filePath)
    {
        FixHeaders(filePath);
        using var reader = new StreamReader(filePath);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

        csv.Read();
        csv.ReadHeader();

        Logger.Instance.Info($"Attempting CSV read on file: {filePath}");
        try
        {
            var dataRows = ReadGeoFetchCsv(csv, Path.GetFileName(filePath)).ToList();

            if (dataRows.Count != 0) return dataRows;
            Logger.Instance.Warning($"Unknown CSV type detected skipping file: {filePath}");
            return null;
        }
        catch (CsvHelper.MissingFieldException e)
        {
            Logger.Instance.Error($"Improper format in CSV: {filePath}  error --> {e.ToString().Split(':')[1].Split('.')[0]}");
            return null;
        }
        finally
        {
            reader.Dispose();
            csv.Dispose();
        }
    }

    private static void FixHeaders(string CsvPath)
    {
        var lines = File.ReadAllLines(CsvPath);
        var header = lines[0];
        header = header.Replace("Sofex", "SOFEX");
        header = header.Replace("sofex", "SOFEX");
        header = header.Replace("FileName", "Filename");
        header = header.Replace("FILENAME", "Filename");
        header = header.Replace("FileType", "Filetype");
        header = header.Replace("FILETYPE", "Filetype");
        lines[0] = header;
        File.WriteAllLines(CsvPath, lines);
    }

    private static IEnumerable<Locations> ReadGeoFetchCsv(IReader csv, string reportFileName)
    {
        IEnumerable<GeoFetchHeaders> records;
        try
        {
            Logger.Instance.Info("Attempting to read file as GeoFetch CSV");
            records = csv.GetRecords<GeoFetchHeaders>();

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
                Category = record.Filetype,
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

    public override void Dispose() { }
}