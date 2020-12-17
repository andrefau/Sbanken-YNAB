using System;
using Serilog;

namespace SbankenYnab
{
    class Program
    {
        static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.File("logs.log")
                .CreateLogger();

            Log.Information("*** START ***");

            if (args.Length != 2)
            {
                Log.Error("Missing arguments.\nYou must supply two arguments when running this program.\nExample: dotnet run \"My account\" \"My budget\"");
                Log.Information("*** EXIT ***");
                Log.CloseAndFlush();
                return;
            }

            var client = new SbankenClient();

            try
            {
                client.init().Wait();
            } 
            catch (Exception ex) 
            {
                Log.Error(ex, ex.Message);
                Log.Information("*** EXIT ***");
                Log.CloseAndFlush();
                return;
            }

            Log.Information("*** EXIT ***");
            Log.CloseAndFlush();
        }
    }
}
