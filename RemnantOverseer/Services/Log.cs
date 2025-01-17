using Serilog;
using Serilog.Templates;
using System;
using System.IO;
using System.Reflection;

namespace RemnantOverseer.Services;
internal class Log
{
    public const string LogFileName = "log.txt";
    public static ILogger? Instance { get; private set; }

    public static void Initialize()
    {
        ExpressionTemplate template = new("{@t:dd MMM yyyy HH:mm:ss} {@l:u3} [{Substring(SourceContext, LastIndexOf(SourceContext, '.') + 1)}] {@m}\n");
        string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, LogFileName);
        if (File.Exists(path))
        {
            File.Delete(path);
        }
        LoggerConfiguration config = new LoggerConfiguration()
            .WriteTo.File(template, path);

        Instance = config.CreateLogger();
        lib.remnant2.analyzer.Log.Logger = Instance;
        lib.remnant2.saves.Log.Logger = Instance;
        Instance.ForContext(typeof(Serilog.Log)).Information($"Version {Assembly.GetExecutingAssembly().GetName().Version}");
    }
}
