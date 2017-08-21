using System;
using System.IO;
using System.Reflection;
using System.Xml;
using log4net;
using log4net.Config;
using log4net.Repository;

namespace VE_Count_Consolidator
{
    class Program
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Program));
        static int Main(string[] args)
        {
           if(InitializeLogging())
           {
               try
               {
                   var consolidator = new Consolidator();
                   consolidator.Process();
               }
               catch(Exception ex)
               {
                   log.Fatal(ex.Message);
                   return -1;
               }
               return 0;
           }
           return -1;
        }

        /// <summary>
        /// Initialize logging
        /// </summary>
        static bool InitializeLogging()
        {
            try
            {
                // Configuration for logging
                XmlDocument log4netConfig = new XmlDocument();

                using (StreamReader reader = new StreamReader(new FileStream("log4net.config", FileMode.Open, FileAccess.Read)))
                {
                    log4netConfig.Load(reader);
                }

                ILoggerRepository rep = log4net.LogManager.CreateRepository(Assembly.GetEntryAssembly(), typeof(log4net.Repository.Hierarchy.Hierarchy));
                XmlConfigurator.Configure(rep, log4netConfig["log4net"]);
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
