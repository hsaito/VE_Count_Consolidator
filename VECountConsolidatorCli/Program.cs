using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using CommandLine;
using CsvHelper;
using VECountConsolidator;

namespace VECountConsolidatorCli
{
    internal class Program
    {
        public static int Main(string[] args)
        {
            try
            {
                Parser.Default.ParseArguments<Options>(args).WithParsed(
                    Generate);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
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

                    var csvWriter = new CsvWriter(writer, CultureInfo.InvariantCulture);
                    csvWriter.Configuration.RegisterClassMap<VeCountMapping>();
                    csvWriter.WriteRecords(entryList);
                    writer.Close();
                }
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.Message);
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