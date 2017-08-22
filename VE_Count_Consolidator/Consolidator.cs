using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using log4net;

namespace VE_Count_Consolidator
{
    public class Consolidator
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Consolidator));

        public abstract class CountGetter
        {
            public abstract List<Person> Extract();
            public abstract string vec { get; }
        }

        public class Person
        {
            public string name;
            public string call;
            public State state;
            public int count;
            public string vec;
        }

        public class State
        {
            public string state_code;
            public string state;
        }


        List<State> states = new List<State>();

        string base_url;

        /// <summary>
        /// Process extractions.
        /// </summary>
        public void Process()
        {
            try
            {
                List<CountGetter> count_getter_list = new List<CountGetter>();
                count_getter_list.Add(new ARRL());
                var persons = ProcessList(count_getter_list);
                Output(persons);
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }
        }

        /// <summary>
        /// Extract from each of VEC
        /// </summary>
        /// <param name="list">List of class for getting counts</param>
        /// <returns>List of person</returns>
        private List<Person> ProcessList(List<CountGetter> list)
        {
            List<Person> plist = new List<Person>();
            foreach(var item in list)
            {
                log.Info("Processing VEC: " + item.vec);
                plist.AddRange(item.Extract());
            }
            return plist;
        }


        /// <summary>
        /// Output person list to TSV
        /// </summary>
        /// <param name="persons">List of person</param>
        private void Output(List<Person> persons)
        {
            try
            {
                log.Info("Writing output to output.tsv in a TSV (Tab Separated Value)");
                var writer = new StreamWriter("output.tsv");
                writer.AutoFlush = true;
                writer.WriteLine("Call\tName\tState\tCount\tVEC");
                foreach (var item in persons)
                {
                    writer.WriteLine(item.call + "\t" + item.name + "\t" + item.state.state + "\t" + item.count.ToString() + "\t" + item.vec);
                }
                writer.Close();
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }
        }

    }
}