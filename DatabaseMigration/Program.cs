using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using DbUp;
using DbUp.Builder;

namespace DatabaseMigration
{
    class Program
    {
        private static class MagicInfixes
        {
            public const string DbCreate = ".dbcreate.";
            public const string TestData = ".testdata.";
        }

        const string ArgTestData = "TestData";

        private static bool _applyTestData;

        static int Main(string[] args)
        {
            var argslist = args.ToList();
            _applyTestData = CheckIfArgumentPresentAndRemove(argslist, ArgTestData);

            if (argslist.Any())
            {
                PrintUsage();
                return 0;
            }

            var retcode = MainRun();

            if (Debugger.IsAttached)
            {
                Console.WriteLine("Press ENTER key to exit.");
                Console.ReadLine();
            }

            return retcode;
        }

        private static int MainRun()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["TypeFineContext"].ConnectionString;
            var connectionStringBuilder = new SqlConnectionStringBuilder(connectionString);
            var databaseName = connectionStringBuilder.InitialCatalog;

            var databaseOk = CreateDatabaseIfNotExists(connectionString);
            if (!databaseOk)
                return 2;

            var upgraderBuilder =
                DeployChanges.To
                    .SqlDatabase(connectionString)
                    .WithVariablesEnabled()
                    .WithVariable("DatabaseName", databaseName)
                    .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly(), ScriptFilter)
                    .LogToConsole()
                    .WithTransactionPerScript()
                    .LogScriptOutput()
                ;
            upgraderBuilder.Configure(
                c =>
                    c.ScriptExecutor.ExecutionTimeoutSeconds =
                        int.Parse(ConfigurationManager.AppSettings["CommandTimeoutSeconds"]));
            var result = PerformUpgrade(upgraderBuilder);

            if (!result)
                return 1;

            return 0;
        }

        private static bool PerformUpgrade(UpgradeEngineBuilder upgraderBuilder)
        {
            var upgrader = upgraderBuilder.Build();

            var result = upgrader.PerformUpgrade();

            if (!result.Successful)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(result.Error);
                Console.ResetColor();

                return false;
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Success!");
            Console.ResetColor();

            return true;
        }

        private static bool CreateDatabaseIfNotExists(string connectionString)
        {
            if (DatabaseExists(connectionString))
                return true;

            var csBuilder = new SqlConnectionStringBuilder(connectionString);
            var applicationDatabaseName = csBuilder.InitialCatalog;

            csBuilder.InitialCatalog = "master";

            var upgraderBuilder =
                DeployChanges.To
                    .SqlDatabase(csBuilder.ConnectionString)
                    .JournalTo(new EmptyJournal())
                    .WithVariablesEnabled()
                    .WithVariable("DatabaseName", applicationDatabaseName)
                    .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly(), ScriptFilterDbCreate)
                    .LogToConsole()
                    .WithoutTransaction()
                    .LogScriptOutput()
                ;

            return PerformUpgrade(upgraderBuilder);
        }

        private static bool DatabaseExists(string connectionString)
        {
            var csBuilder = new SqlConnectionStringBuilder(connectionString);
            var databaseName = csBuilder.InitialCatalog;
            csBuilder.InitialCatalog = "master";

            var connection = new SqlConnection(csBuilder.ConnectionString);
            try
            {
                const string query = "SELECT database_id FROM sys.databases WHERE Name = @DatabaseName";
                object objectOut;

                connection.Open();
                using (var cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("DatabaseName", databaseName);
                    objectOut = cmd.ExecuteScalar();
                }

                return objectOut != null;
            }
            finally
            {
                connection.Dispose();
            }
        }

        private static bool ScriptFilter(string scriptName)
        {
            var scriptNameLowercase = scriptName.ToLowerInvariant();

            if (scriptNameLowercase.Contains(MagicInfixes.DbCreate))
                return false;

            return _applyTestData || scriptNameLowercase.Contains(MagicInfixes.TestData) == false;
        }

        private static bool ScriptFilterDbCreate(string scriptName)
        {
            var scriptNameLowercase = scriptName.ToLowerInvariant();
            return scriptNameLowercase.Contains(MagicInfixes.DbCreate);
        }

        private static bool CheckIfArgumentPresentAndRemove(ICollection<string> argslist, string argumentText)
        {
            var argument = argslist.SingleOrDefault(x => string.Equals(x, argumentText, StringComparison.OrdinalIgnoreCase));
            if (argument == null)
                return false;

            argslist.Remove(argument);
            return true;
        }

        private static void PrintUsage()
        {
            Console.Error.WriteLine("Usage: DbMigration.exe [reinitialize] [testdata]");
            Console.Error.WriteLine("       reinitialize option will clear database and create it from scratch");
            Console.Error.WriteLine("       testdata option will include test scripts");
        }
    }
}
