using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CsvHelper;
using log4net;

namespace VE_Count_Consolidator
{
    public class Consolidator
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(Consolidator));

        /// <summary>
        ///     Process extractions.
        /// </summary>
        public static void Process()
        {
            try
            {
                var countGetterList = new List<ICountGetter> {new ARRL()};
                var persons = ProcessList(countGetterList);
                Output(persons);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }
        }

        /// <summary>
        ///     Extract from each of VEC
        /// </summary>
        /// <param name="list">List of class for getting counts</param>
        /// <returns>List of person</returns>
        private static IEnumerable<Person> ProcessList(IEnumerable<ICountGetter> list)
        {
            var plist = new List<Person>();
            foreach (var item in list)
            {
                Log.Info("Processing VEC: " + item.Vec);
                plist.AddRange(item.Extract());
            }

            return plist;
        }

        /// <summary>
        ///     Output person list to CSV
        /// </summary>
        /// <param name="persons">List of person</param>
        private static void Output(IEnumerable<Person> persons)
        {
            try
            {
                Log.Info("Writing output to output.csv in a CSV");
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
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }
        }

        public interface ICountGetter
        {
            string Vec { get; }
            IEnumerable<Person> Extract();
        }

        public class Person
        {
            public string Call;
            public int Count;
            public string Name;
            public State State;
            public string Vec;
        }

        public class State
        {
            public string StateCode;
            public string StateName;
        }
    }
}