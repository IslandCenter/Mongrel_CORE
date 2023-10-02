using Mongrel.Outputs.OutputTypes;
using Yipper;

namespace Mongrel.Outputs;

public class GenerateOutputs : IDisposable
{
    public const string DefaultOutputName = "Consolidated_Geo_Output";
    public string OutputName;
    private readonly List<OutputFile> _outputs;

    public GenerateOutputs(string outputDir, string outputFileName = null)
    {
        OutputName = $"{Path.Combine(outputDir, $"{(string.IsNullOrEmpty(outputFileName) ? DefaultOutputName : outputFileName)}")}";
        _outputs = GetOutputs().ToList();
    }

    private IEnumerable<OutputFile> GetOutputs()
    {
        yield return new CsvOut($"{OutputName}.csv");
        yield return new Kml($"{OutputName}.kml");
    }

    public void WriteLocations(IEnumerable<Locations> locations)
    {
        var totalLocationsObjects = 0;
        var totalLocations = 0;

        foreach (var location in locations)
        {
            totalLocationsObjects++;
            var locationsWrittenPerFile = 0;

            foreach (var output in _outputs.Where(_ => location.ConvertedLon != 0 && location.ConvertedLat != 0))
            {
                output.WriteLocation(location);
                locationsWrittenPerFile++;
            }
            totalLocations += locationsWrittenPerFile;
            Logger.Instance.Info($"Wrote {locationsWrittenPerFile} locations from location object {totalLocationsObjects}");
        }
        Logger.Instance.Info($"Wrote {totalLocations} locations from {totalLocationsObjects} location objects");
    }

    public void Dispose()
    {
        foreach (var output in _outputs)
        {
            output.Dispose();
        }
    }
}