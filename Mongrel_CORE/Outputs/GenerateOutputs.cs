using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mongrel.Outputs.OutputTypes;

namespace Mongrel.Outputs
{
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
            if (locations == null) return;

            foreach (var location in locations)
            {
                foreach (var output in _outputs.Where(_ => location.ConvertedLon != 0 && location.ConvertedLat != 0))
                {
                    output.WriteLocation(location);
                }
            }
        }

        public void Dispose()
        {
            foreach (var output in _outputs)
            {
                output.Dispose();
            }
        }
    }
}