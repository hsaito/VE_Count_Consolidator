using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml;
using log4net;
using log4net.Config;
using log4net.Repository;

namespace VE_Count_Consolidator
{
    public static class Utils
    {
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
}