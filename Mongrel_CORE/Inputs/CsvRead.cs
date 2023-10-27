using System.Globalization;
using CsvHelper.Configuration;
using Mongrel.Inputs.ReportReaders.Csv;
using Yipper;
using static Mongrel.Inputs.ReportReaders.Csv.GeoFetch;

namespace Mongrel.Inputs;



internal class CsvRead : Reader
{
    /*//function that uses CsvHelper to determin if it is a paticular CSV file
    public static bool IsCsv(string filePath, T header)
    {
        IEnumerable<header> records;
        var test = IReader.GetRecords<header>();
    }
*/
    public override IEnumerable<Locations>? GetLocations(string filePath)
    {
        var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            // Standardizes headers to Title Case since some CSVs have headers have had random capitalization
            // All CSV headers will need to be tile cased for the program to work
            //PrepareHeaderForMatch = args => CultureInfo.CurrentCulture.TextInfo.ToTitleCase(args.Header.ToLower()),
        };

        

        Logger.Instance.Info($"Attempting CSV read on file: {filePath}");
        try
        {
            //TODO: Refactor this to be more dynamic

            var dataRows = ReadGeoFetchCsv(filePath, csvConfig).ToList();
            if (dataRows.Count != 0) return dataRows;

            var test = MongrelSelfRead.ReadMongrel(filePath, csvConfig);
            dataRows = test.ToList();
            if (dataRows.Count != 0) return dataRows;

            Logger.Instance.Warning($"Unknown CSV type detected skipping file: {filePath}");
            return null;
        }
        catch (CsvHelper.MissingFieldException e)
        {
            Logger.Instance.Error($"Improper format in CSV: {filePath}  error --> {e.ToString().Split(':')[1].Split('.')[0]}");
            return null;
        }
    }

    public override void Dispose() { }
}