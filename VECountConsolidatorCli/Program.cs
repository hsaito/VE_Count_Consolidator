using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using CommandLine;
using CsvHelper;
using log4net;
using log4net.Config;
using log4net.Repository.Hierarchy;
using VECountConsolidator;

namespace VECountConsolidatorCli
{
    internal class Program
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(Program));


        public static int Main(string[] args)
        {
            if (!InitializeLogging()) return -1;
            try
            {
                Parser.Default.ParseArguments<Options>(args).WithParsed(
                    Generate);
            }
            catch (Exception ex)
            {
                Log.Fatal(ex.Message);
                return -1;
            }

            return 0;
        }

        private static void Generate(Options options)
        {
            if (options.Mode != "create") return;
            RunProcess();
        }

        private static void RunProcess()
        {
            var output = Consolidator.Process(Consolidator.VEC.ARRL);
            Output(output);
        }


        /// <summary>
        ///     Output person list to CSV
        /// </summary>
        /// <param name="persons">List of person</param>
        private static void Output(IEnumerable<Consolidator.Person> persons)
        {
            try
            {
                using (var writer = new StreamWriter("output.csv"))
                {
                    writer.AutoFlush = true;

                    var entryList = persons.Select(item => new VeCountEntry
                        {
                            Call = item.Call,
                            Name = item.Name,
                            State = item.State.StateName,
                            Count = item.Count,
                            Vec = item.Vec
                        })
                        .ToList();

                    var csvWriter = new CsvWriter(writer);
                    csvWriter.Configuration.RegisterClassMap<VeCountMapping>();
                    csvWriter.WriteRecords(entryList);
                    writer.Close();
                }
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
        }

        /// <summary>
        ///     Initialize logging
        /// </summary>
        private static bool InitializeLogging()
        {
            try
            {
                // Configuration for logging
                var log4NetConfig = new XmlDocument();

                using (var reader = new StreamReader(new FileStream("log4net.config", FileMode.Open, FileAccess.Read)))
                {
                    log4NetConfig.Load(reader);
                }

                var rep = LogManager.CreateRepository(Assembly.GetEntryAssembly(), typeof(Hierarchy));
                XmlConfigurator.Configure(rep, log4NetConfig["log4net"]);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error initializing the logging.");
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        [SuppressMessage("ReSharper", "ClassNeverInstantiated.Local")]
        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
        private class Options
        {
            [Option('m', "mode", Required = true, HelpText = "Mode of operations. (Currently supported: create)")]
            public string Mode { get; set; }
        }
    }
}