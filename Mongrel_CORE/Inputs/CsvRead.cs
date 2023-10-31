using System.Globalization;
using CsvHelper.Configuration;
using Mongrel.Inputs.ReportReaders.Csv;
using Yipper;
using static Mongrel.Inputs.ReportReaders.Csv.GeoFetch;
using static Mongrel.Inputs.ReportReaders.Csv.MongrelSelfRead;

namespace Mongrel.Inputs;



internal class CsvRead : Reader
{
    public override IEnumerable<Locations>? GetLocations(string filePath)
    {   
        Logger.Instance.Info($"Attempting CSV read on file: {filePath}");

        var locations = ReadGeoFetchCsv(filePath);
        return locations.Concat(ReadMongrel(filePath));
    }

    public override void Dispose() { }
}