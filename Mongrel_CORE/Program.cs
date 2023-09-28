using Mongrel.Inputs;
using Mongrel.Outputs;
using Yipper;
using static Mongrel.Common.InputFileFormats;

namespace Mongrel
{
    public class Mongrel
    {
        private static readonly List<string> ReadFileFormats = new() { ".csv", ".xlsx", ".xls" };
        private const int DefaultMinFileLength = 100;

        public static void RunMongrel(string inputDir, string outputDir, string outputName, Lazy<Logger> logger, int minFileLength = DefaultMinFileLength)
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            Logger.SetLogger(logger);

            Logger.Instance.Info("Searching for input files");

            if (!Directory.Exists(inputDir))
            {
                Logger.Instance.Error("Input Directory not found exiting");
                throw new Exception("Input Directory not found");
            }

            if (!Directory.Exists(outputDir))
            {
                Logger.Instance.Error("Output Directory not found exiting");
                throw new Exception("Output Directory not found");
            }

            var inputs = GetInputPaths(inputDir, minFileLength);
            using var output = new GenerateOutputs(outputDir, outputName);

            var filesRead = 0;
            foreach (var file in inputs)
            {
                Logger.Instance.Info($"Reading file: {file}");
                filesRead++;

                IEnumerable<Locations> locations = null;

                try
                {
                    Reader reader = null;

                    switch (GetFileFormatsFromName(new FileInfo(file).Extension))
                    {
                        case FileFormats.Csv:
                            Logger.Instance.Info($"Reading csv file: {file}");
                            reader = new CsvRead();
                            locations = reader.GetLocations(file);
                            break;
                        case FileFormats.Excel:
                            Logger.Instance.Info($"Reading excel file: {file}");
                            reader = new ExcelRead();
                            locations = reader.GetLocations(file);
                            break;
                        case FileFormats.Unknown:
                            Logger.Instance.Warning($"Unknown file skipping : {file}");
                            break;
                        default:
                            Logger.Instance.Error($"Unhandled file extension found skipping file: {file}");
                            continue;
                    }
                    Logger.Instance.Info($"Writing location data to output for file: {file}");
                    if (locations != null) output.WriteLocations(locations.Where(i => !i.Equals(new Locations())));
                    reader?.Dispose();
                }
                catch (Exception e)
                {
                    Logger.Instance.Error($"Critical error found while reading file: {file}    File has been skipped");
                    Logger.Instance.Error($"Exception: {e}");
                }
            }

            Logger.Instance.Info($"Finished reading {filesRead} files");
            Logger.Instance.Info("Finished writing outputs");
            Logger.Instance.Flush();
        }

        private static IEnumerable<string> GetInputPaths(string inputDirPath, int minFileLength)
        {
            var inputPaths = HandleLongInputPaths(Directory.EnumerateFiles(inputDirPath, "*", SearchOption.AllDirectories));

            return inputPaths.Where(path => ReadFileFormats.Contains(new FileInfo(path).Extension) && new FileInfo(path).Length > minFileLength && !Path.GetFileName(path).StartsWith("~"));
        }

        //TODO setup actual long input handling 
        private static IEnumerable<string> HandleLongInputPaths(IEnumerable<string> inputPaths)
        {
            foreach (var path in inputPaths)
            {
                if (path.Length > 255)
                {
                    Logger.Instance.Warning($"file path greater then windows max file length, skipping file: {path}");
                }
                else yield return path;
            }
        }
    }
}
