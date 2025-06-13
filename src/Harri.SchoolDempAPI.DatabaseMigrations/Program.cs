using DbUp;
using DbUp.Engine;
using DbUp.Support;
using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace Harri.SchoolDempAPI.DatabaseMigrations
{
    public class Program
    {
        static int Main(string[] args)
        {
            var projectName = "Harri.SchoolDempAPI.DatabaseMigrations";
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            var sqlConnectionString = config["SQLConnectionString"];
            // Use appsettings.json for default connection string

            var connectionString =
                args.FirstOrDefault() ?? sqlConnectionString;
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentException("SQL connection string is empty, use appsettings.json, or console argument");
            }

            var upgradeEngineBuilder = DeployChanges.To
                .SqlDatabase(connectionString, null) //null or "" for default schema for user
                // Script name for filtering looks like: "Harri.SchoolDempAPI.DatabaseMigrations.TestScripts.TestScript0001 - Create schools.sql"
                .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly(), script => script.StartsWith($"{projectName}.Scripts."), new SqlScriptOptions { ScriptType = ScriptType.RunOnce })
                .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly(), script => script.StartsWith($"{projectName}.TestScripts."), new SqlScriptOptions { ScriptType = ScriptType.RunOnce })
                .LogToConsole(); //TODO integrate with API logging services

            var upgrader = upgradeEngineBuilder.Build();
            var result = upgrader.PerformUpgrade();

            if (!result.Successful)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(result.Error);
                Console.ResetColor();
#if DEBUG
                Console.ReadLine();
#endif
                return -1;
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Success!");
            Console.ResetColor();
            return 0;
        }

    }
}