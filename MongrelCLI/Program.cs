using System.Diagnostics;
using System.Reflection;
using Yipper;
using static Mongrel.Mongrel;

namespace MongrelCLI;

internal class Program
{
    private static void Main(string[] args)
    {
        var arguments = ArgumentParsing.GetArguments(args);
        if (arguments == null)
            return;

        var log = Logger.GetLogger();
        Logger.SetLogger(log);
        Logger.InitializeLogger(Logger.CreateAndReturnLogDir("Mongrel", arguments.LogDir), Logger.LogLevelOf(arguments.LevelOfLog));

        Logger.Instance.Info("Logger is initialized and all arguments have been validated");

        var assembly = Assembly.GetExecutingAssembly();
        var fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);

        Logger.Instance.Info($"Mongrel Version Number: {fileVersionInfo.ProductVersion ?? "Not Found"}");
        Logger.Instance.Info($"Input Directory: {arguments.InputDir ?? "Not Found"}");
        Logger.Instance.Info($"Output Directory: {arguments.OutputDir ?? "Not Found"}");

        if (!Directory.Exists(arguments.InputDir))
        {
            Logger.Instance.Error($"Input directory not found: {arguments.InputDir}");
            Logger.Instance.Error("Program Exiting");
            return;
        }

        if (!Directory.Exists(arguments.OutputDir))
        {
            Logger.Instance.Error($"Output directory not found: {arguments.OutputDir}");
            Logger.Instance.Error("Program Exiting");
            return;
        }

        RunMongrel(arguments.InputDir, arguments.OutputDir, arguments.OutName, log, TryConvertToInt(arguments.MinFileLength));
    }

    //TODO change the ScriptConfig file to accept only int's will validation for only numbers and remove this function
    private static int TryConvertToInt(string toParse)
    {
        try
        {
            return int.Parse(toParse);
        }
        catch
        {
            Logger.Instance.Warning($"Could not parse minimum file byte length defaulting to {100} bytes: {toParse}");
            return 100;
        }
    }
}