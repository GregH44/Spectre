using Microsoft.Extensions.Configuration;
using System.IO;

namespace SpectreFW.Configuration
{
    public static class ConfigurationManager
    {
        public static IConfiguration Configuration { get; private set; }

        public static IConfiguration Configure(string contentRootPath)
        {
            if (Configuration != null)
                return Configuration;

            var environmentVariables = new ConfigurationBuilder().AddEnvironmentVariables().Build();

            var builder = new ConfigurationBuilder()
                .SetBasePath(contentRootPath)
                .AddJsonFile("appsettings.json", optional: false)
                .AddEnvironmentVariables();

            var profile = environmentVariables.GetSection("COMPUTERNAME");
            var environmentProfile = environmentVariables.GetSection("ASPNETCORE_ENVIRONMENT");

            if (File.Exists($"appsettings.{profile.Value}.json"))
            {
                builder.AddJsonFile($"appsettings.{profile.Value}.json", optional: false);
            }
            else if (File.Exists($"appsettings.{environmentProfile.Value}.json"))
            {
                builder.AddJsonFile($"appsettings.{environmentProfile.Value}.json", optional: false);
            }
            else
            {
                builder.AddJsonFile("appsettings.Production.json", optional: false);
            }

            Configuration = builder.Build();

            return Configuration;
        }

        public static string GetValue(string jsonPath)
        {
            return Configuration[jsonPath];
        }
    }
}
