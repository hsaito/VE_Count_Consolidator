using System;
using System.IO;
using System.Reflection;
using System.Xml;
using log4net;
using log4net.Config;

namespace VE_Count_Consolidator
{
    internal class Program
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(Program));
        // ReSharper disable once UnusedParameter.Global
        public static int Main(string[] args)
        {
            if (!InitializeLogging()) return -1;
            try
            {
                Consolidator.Process();
            }
            catch(Exception ex)
            {
                Log.Fatal(ex.Message);
                return -1;
            }
            return 0;
        }

        /// <summary>
        /// Initialize logging
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

                var rep = LogManager.CreateRepository(Assembly.GetEntryAssembly(), typeof(log4net.Repository.Hierarchy.Hierarchy));
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
    }
}
