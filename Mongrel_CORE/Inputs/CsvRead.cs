using System.Globalization;
using CsvHelper;
using Yipper;
using static Mongrel.Inputs.ReportReaders.Csv.GeoFetch;

namespace Mongrel.Inputs;



internal class CsvRead : Reader
{
    public override IEnumerable<Locations>? GetLocations(string filePath)
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

    

    public override void Dispose() { }
}