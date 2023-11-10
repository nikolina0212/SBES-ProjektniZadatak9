using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace SecurityManager
{
	public class Audit : IDisposable
	{

        private static EventLog customLog = null;
        const string SourceName = "MySource";
        const string LogName = "MyLog";

        static Audit()
        {
            try
            {

                string logName;
                if (EventLog.SourceExists("MySource"))
                {
                    logName = EventLog.LogNameFromSourceName("MySource", Environment.MachineName);
                  
                }
                else
                {
                    EventLog.CreateEventSource("MySource", "MyLog");
                    Console.WriteLine("napravljeno");
                }
                customLog = new EventLog(LogName,
                    Environment.MachineName, SourceName);
            }
            catch (Exception e)
            {
                customLog = null;
                Console.WriteLine("Error while trying to create log handle. Error = {0}", e.Message);
            }

        }

        public static void WriteAlarm(DateTime Timestamp,string database,string id,string signature, X509Certificate2 publicKey)
		{
            if (customLog != null)
            {
				string str = AuditEvent.WriteAlarm;
				string[] args = { Timestamp.ToString(), database, id, signature, publicKey.GetPublicKeyString() };
				string message = String.Format(str, args);

				customLog.WriteEntry(message);
			}
		}
		public void Dispose()
        {
			if (customLog != null)
			{
				customLog.Dispose();
				customLog = null;
			}
		}
    }
}
