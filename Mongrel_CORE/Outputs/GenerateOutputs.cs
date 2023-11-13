using System.Collections;
using Mongrel.Common;
using Mongrel.Outputs.OutputTypes;
using Yipper;

namespace Mongrel.Outputs;

public class GenerateOutputs : IDisposable
{
    public const string DefaultOutputName = "Consolidated_Geo_Output";
    public string OutputName;
    public Hashtable UniquePoints;
    private readonly List<OutputFile> _outputs;

    public GenerateOutputs(string outputDir, string? outputFileName = null)
    {
        OutputName = $"{Path.Combine(outputDir, $"{(string.IsNullOrEmpty(outputFileName) ? DefaultOutputName : outputFileName)}")}";
        _outputs = GetOutputs().ToList();

        UniquePoints = new Hashtable();
    }

    private IEnumerable<OutputFile> GetOutputs()
    {
        yield return new CsvOut($"{OutputName}.csv");
        yield return new Kml($"{OutputName}.kml");
    }

    public void WriteLocations(IEnumerable<Locations> locations)
    {
        var totalLocations = 0;

        var totalLocationsWritten = 0;

        foreach (var location in locations)
        {
            totalLocations++;

            if(location is { ConvertedLon: 0, ConvertedLat: 0 }) continue;
            if (!Utils.ValidateLatLon(location.ConvertedLat, location.ConvertedLon)) continue;


            var locationHash = Utils.GetLocationsHashCode(location);
            if (UniquePoints.ContainsKey(locationHash)) continue;
            UniquePoints.Add(locationHash, totalLocationsWritten);

            totalLocationsWritten++;

            foreach (var output in _outputs.Where(_ => location.ConvertedLon != 0 && location.ConvertedLat != 0))
            {
                output.WriteLocation(location);
            }

            Logger.Instance.Info($"Wrote (Lat,Lon): ({location.ConvertedLat},{location.ConvertedLon})");
        }
        Logger.Instance.Info($"Wrote {totalLocationsWritten} out of {totalLocations} to file output(s)");
    }

    public void Dispose()
    {
        foreach (var output in _outputs)
        {
            output.Dispose();
        }
    }
}