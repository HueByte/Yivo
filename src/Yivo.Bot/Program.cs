using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Extensions.Logging;
using Serilog.Sinks.SystemConsole.Themes;
using Yivo.Bot.Configurators;

Serilog.Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .Enrich.FromLogContext()
    .CreateBootstrapLogger();

var logger = new SerilogLoggerProvider(Serilog.Log.Logger)
     .CreateLogger(nameof(Program));

var hostBuilder = Host.CreateDefaultBuilder(args);
var host = hostBuilder.ConfigureHostConfiguration(host =>
    {
        host.AddEnvironmentVariables()
            .AddJsonFile("appsettings.json", optional: false)
            .AddCommandLine(args);
    })
    .ConfigureServices((hostContext, services) =>
    {
        InjectionConfiguration ioc = new(hostContext.Configuration, services);

        ioc.AddYivoCore()
           .AddOptions()
           .AddServices()
           .AddDatabaseServices();
    })
    .UseSerilog((context, services, config) =>
    {
        config.WriteTo.Console(restrictedToMinimumLevel: LogEventLevel.Information, theme: AnsiConsoleTheme.Code)
            .WriteTo.File(Path.Combine(AppContext.BaseDirectory, "logs/log.log"), rollingInterval: RollingInterval.Day)
            .Enrich.FromLogContext()
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services);
    })
    .Build();

await host.RunAsync();