using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using log4net;

public class Consolidator
{
    private static readonly ILog log = LogManager.GetLogger(typeof(Consolidator));

    class Person
    {
        public string name;
        public string call;
        public State state;
        public int count;
    }

    List<Person> persons = new List<Person>();

    class State
    {
        public string state_code;
        public string state;
    }

    List<State> states = new List<State>();

    string base_url;

    public void Process()
    {
        try
        {
            LoadConfig();
            Extract().Wait();
            Output();
        }
        catch (Exception ex)
        {
            log.Error(ex.Message);
        }
    }

    private void Output()
    {
        try
        {
            log.Info("Writing output to output.tsv in a TSV (Tab Separated Value)");
            var writer = new StreamWriter("output.tsv");
            writer.AutoFlush = true;
            writer.WriteLine("Call\tName\tState\tCount");
            foreach (var item in persons)
            {
                writer.WriteLine(item.call+"\t"+item.name+"\t"+item.state.state+"\t"+item.count.ToString());
            }
            writer.Close();
        }
        catch (Exception ex)
        {
            log.Error(ex.Message);
        }
    }

    private async Task Extract()
    {
        try
        {
            foreach (var state in states)
            {
                var target = string.Format(base_url, state.state_code);
                log.Info("Retrieving " + target);
                var web = await GetWeb(target);
                web = ExtractTable(web);

                GetNameElement(web, state);
            }
        }
        catch (Exception ex)
        {
            log.Error(ex.Message);
        }
    }

    private void GetNameElement(string pagecontent, State state)
    {
        var content = XElement.Parse(pagecontent);
        foreach (var entry in content.Elements("tr"))
        {
            var column = entry.Elements("td").ToList();
            if (column.Count == 2)
            {
                var data = new Person();
                (data.call, data.name) = SeparateCallName(column[0].Value);
                data.count = Convert.ToInt32(column[1].Value);
                data.state = state;
                persons.Add(data);
            }
        }
    }

    private string ExtractTable(string input)
    {
        var matches = Regex.Matches(input, @"(<table(.*)/table>)", RegexOptions.Singleline | RegexOptions.Multiline);
        return matches[0].Value;
    }

    void LoadConfig()
    {
        var reader = new StreamReader("config.xml");
        var xl = XElement.Load(reader);
        base_url = xl.Element("baseurl").Value;

        foreach (var state in xl.Element("states").Elements("option"))
        {
            var entry = new State();
            entry.state_code = state.Attribute("value").Value;
            entry.state = state.Value;
            states.Add(entry);
        }
    }

    (string, string) SeparateCallName(string input)
    {
        var entry = Regex.Split(input.Trim(), @"(.*)\((.*?)\)");
        return (entry[1].Trim(), entry[2].Trim());
    }

    /// <summary>
    /// Retrieve content from the web.
    /// </summary>
    /// <param name="location">URL of the website</param>
    /// <returns>String of the website</returns>
    public static async Task<string> GetWeb(string location)
    {
        // Make a request
        var request = WebRequest.Create(location);

        // Get the response
        var response = await request.GetResponseAsync();
        // Get the stream
        var stream = response.GetResponseStream();
        // Read the stream
        StreamReader reader = new StreamReader(stream);
        string data = await reader.ReadToEndAsync();
        return data;
    }

}