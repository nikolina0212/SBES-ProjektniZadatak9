using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace SecurityManager
{
    public class AuditEvent
    {
        private static ResourceManager resourceManager = null;
        private static object resourceLock = new object();

        private static ResourceManager ResourceMgr
        {
            get
            {
                lock (resourceLock)
                {
                    if (resourceManager == null)
                    {
                        resourceManager = new ResourceManager(typeof(FileEvent).ToString(), Assembly.GetExecutingAssembly());
                    }
                    return resourceManager;
                }
            }
        }

        public static string WriteAlarm
        {
            get
            {
                return ResourceMgr.GetString("WriteAlarm");
            }
        }
    }
}
