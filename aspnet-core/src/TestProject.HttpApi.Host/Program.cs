﻿using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.InProcess;
using Serilog;
using Serilog.Events;

namespace TestProject
{
    public class Program
    {
        public static int Main(string[] args)
        {
            CurrentDirectoryHelpers.SetCurrentDirectory();

            Log.Logger = new LoggerConfiguration()
#if DEBUG
                .MinimumLevel.Debug()
#else
                .MinimumLevel.Information()
#endif
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.Async(c => c.File("Logs/logs.txt"))
                .CreateLogger();

            try
            {
                Log.Information("Starting TestProject.HttpApi.Host.");
                BuildWebHostInternal(args).Run();
                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly!");
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IWebHost BuildWebHostInternal(string[] args) =>
            new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIIS()
                .UseIISIntegration()
                .UseStartup<Startup>()
                .UseSerilog()
                .Build();
    }
}
