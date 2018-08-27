using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using log4net;

namespace VE_Count_Consolidator
{
    // ReSharper disable once InconsistentNaming
    internal class ARRL : Consolidator.ICountGetter
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ARRL));

        private readonly List<Consolidator.State> _states = new List<Consolidator.State>();
        private string _baseUrl;
        public string Vec => "ARRL";

        /// <summary>
        ///     Extract the list of VE from the ARRL list
        /// </summary>
        /// <returns>List of person</returns>
        public IEnumerable<Consolidator.Person> Extract()
        {
            var list = new List<Consolidator.Person>();
            try
            {
                LoadConfig();
                foreach (var state in _states)
                {
                    var target = string.Format(_baseUrl, state.StateCode);
                    Log.Info("Retrieving " + target);
                    var web = Utils.GetWeb(target).Result;
                    web = ExtractTable(web);

                    list.AddRange(GetNameElement(web, state));
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return null;
            }

            return list;
        }

        /// <summary>
        ///     Process the page contents and fill the list of person
        /// </summary>
        /// <param name="pagecontent">Content of the table segment</param>
        /// <param name="state">List of the state</param>
        /// <returns>List of person</returns>
        private IEnumerable<Consolidator.Person> GetNameElement(string pagecontent, Consolidator.State state)
        {
            var persons = new List<Consolidator.Person>();
            var content = XElement.Parse(pagecontent);
            foreach (var entry in content.Elements("tr"))
            {
                var column = entry.Elements("td").ToList();
                if (column.Count != 2) continue;
                var data = new Consolidator.Person();
                (data.Call, data.Name) = SeparateCallName(column[0].Value);
                data.Count = Convert.ToInt32(column[1].Value);
                data.State = state;
                data.Vec = Vec;
                persons.Add(data);
            }

            return persons;
        }

        /// <summary>
        ///     Load the config file
        /// </summary>
        private void LoadConfig()
        {
            _baseUrl = "http://www.arrl.org/ve-session-counts?state={0}";
            var web = Utils.GetWeb("http://www.arrl.org/ve-session-counts").Result;

            var matches = Regex.Matches(web, @"<option value='(.*?)'>(.*?)</option>");
            
            foreach (var match in matches.Select(x => x))
            {
                if (match.Groups[1].Value == "") continue;
                var entry = new Consolidator.State
                {
                    StateCode = match.Groups[1].Value,
                    StateName = match.Groups[2].Value
                };
                _states.Add(entry);
            }
        }

        /// <summary>
        ///     Extract the table element from ARRL page
        /// </summary>
        /// <param name="input">String of the page</param>
        /// <returns>Table segment from the page</returns>
        private static string ExtractTable(string input)
        {
            var matches = Regex.Matches(input, @"(<table(.*)/table>)",
                RegexOptions.Singleline | RegexOptions.Multiline);
            return matches[0].Value;
        }

        /// <summary>
        ///     Separate callsign and name from "Call (Name)" format.
        /// </summary>
        /// <param name="input">String of the "Call (Name)"</param>
        /// <returns>Tuple of the call and name</returns>
        private static (string, string) SeparateCallName(string input)
        {
            var entry = Regex.Split(input.Trim(), @"(.*)\((.*?)\)");
            return (entry[1].Trim(), entry[2].Trim());
        }
    }
}