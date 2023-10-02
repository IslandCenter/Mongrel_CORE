using Fclp;

namespace MongrelCLI;

internal class ArgumentParsing
{
    public class Arguments
    {
        public string InputDir { get; set; }
        public string OutputDir { get; set; }
        public string OutName { get; set; }
        public string LogDir { get; set; }
        public string LevelOfLog { get; set; }
        public string MinFileLength { get; set; }
        public List<string> Blacklist { get; set; }
    }

    public static Arguments? GetArguments(string[] args)
    {
        var parser = new FluentCommandLineParser<Arguments?>();

        #region ArgumentParsing

        parser.Setup(arg => arg.InputDir)
            .As('i', "input")
            .Required()
            .WithDescription("Path to the input directory (required)");

        parser.Setup(arg => arg.OutputDir)
            .As('o', "outputDir")
            .Required()
            .WithDescription("Name of the output directory (required)");

        parser.Setup(arg => arg.OutName)
            .As('n', "outName")
            .SetDefault(null)
            .WithDescription("Output file name");

        parser.Setup(arg => arg.LogDir)
            .As('l', "logDir")
            .SetDefault(null)
            .WithDescription("Path to the logging directory");

        parser.Setup(arg => arg.LevelOfLog)
            .As('v', "logLevel")
            .SetDefault("CONSOLE")
            .WithDescription("Set the log level: Debug, CONSOLE <-(default), INFO, WARNING, ERROR, or NONE");

        parser.Setup(arg => arg.Blacklist)
            .As('b', "blacklist")
            .SetDefault(new List<string>())
            .WithDescription("Space-separated list of input folders to ignore.  Default blacklist any topmost Reports folder in the given input directory  [InputDir]/Reports/");

        parser.Setup(arg => arg.MinFileLength)
            .As('m', "minFileLength")
            .WithDescription("Set the minimum file length in bytes to search for");

        parser.SetupHelp("h", "help")
            .Callback(text => Console.WriteLine(text));

        #endregion

        var results = parser.Parse(args);

        if (results.HasErrors)
            Console.WriteLine(results.ErrorText);
        else if (!results.HelpCalled)
        {
            return parser.Object;
        }
        return null;
    }
}