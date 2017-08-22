using System;
using System.Collections.Generic;
using log4net;
using System.Xml.Linq;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;

namespace VE_Count_Consolidator
{
    class ARRL : Consolidator.CountGetter
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ARRL));

        List<Consolidator.State> states = new List<Consolidator.State>();
        string base_url;
        public override string vec { get { return "ARRL"; } }

        /// <summary>
        /// Extract the list of VE from the ARRL list
        /// </summary>
        /// <returns>List of person</returns>
        public override List<Consolidator.Person> Extract()
        {
            List<Consolidator.Person> list = new List<Consolidator.Person>();
            try
            {
                LoadConfig();
                foreach (var state in states)
                {
                    var target = string.Format(base_url, state.state_code);
                    log.Info("Retrieving " + target);
                    var web = Utils.GetWeb(target).Result;
                    web = ExtractTable(web);

                    list.AddRange(GetNameElement(web, state));
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                return null;
            }
            return list;
        }

        /// <summary>
        /// Process the page contents and fill the list of person
        /// </summary>
        /// <param name="pagecontent">Content of the table segment</param>
        /// <param name="state">List of the state</param>
        /// <returns>List of person</returns>
        private List<Consolidator.Person> GetNameElement(string pagecontent, Consolidator.State state)
        {
            List<Consolidator.Person> persons = new List<Consolidator.Person>();
            var content = XElement.Parse(pagecontent);
            foreach (var entry in content.Elements("tr"))
            {
                var column = entry.Elements("td").ToList();
                if (column.Count == 2)
                {
                    var data = new Consolidator.Person();
                    (data.call, data.name) = SeparateCallName(column[0].Value);
                    data.count = Convert.ToInt32(column[1].Value);
                    data.state = state;
                    data.vec = vec;
                    persons.Add(data);
                }
            }

            return persons;
        }

        /// <summary>
        /// Load the config file
        /// </summary>
        void LoadConfig()
        {
            var reader = new StreamReader("ARRL.xml");
            var xl = XElement.Load(reader);
            base_url = xl.Element("baseurl").Value;

            foreach (var state in xl.Element("states").Elements("option"))
            {
                var entry = new Consolidator.State();
                entry.state_code = state.Attribute("value").Value;
                entry.state = state.Value;
                states.Add(entry);
            }
        }

        /// <summary>
        /// Extract the table element from ARRL page
        /// </summary>
        /// <param name="input">String of the page</param>
        /// <returns>Table segment from the page</returns>
        private string ExtractTable(string input)
        {
            var matches = Regex.Matches(input, @"(<table(.*)/table>)", RegexOptions.Singleline | RegexOptions.Multiline);
            return matches[0].Value;
        }

        /// <summary>
        /// Separate callsign and name from "Call (Name)" format.
        /// </summary>
        /// <param name="input">String of the "Call (Name)"</param>
        /// <returns>Tuple of the call and name</returns>
        (string, string) SeparateCallName(string input)
        {
            var entry = Regex.Split(input.Trim(), @"(.*)\((.*?)\)");
            return (entry[1].Trim(), entry[2].Trim());
        }
    }
}