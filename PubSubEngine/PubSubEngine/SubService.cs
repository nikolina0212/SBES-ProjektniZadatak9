using Contracts;
using Encrypting;
using SecurityManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace PubSubEngine
{
    public class SubService : ISubscribe
    {
       

        Dictionary<byte[], byte[]> ISubscribe.ForwardAlarm(int min, int max)
        {
            Dictionary<byte[], byte[]> alarms = new Dictionary<byte[], byte[]>();
            Dictionary<byte[], byte[]> alarmsFromStorage = AlarmStorage.alarmsEncripted;
            string key = SecretKey.LoadKey("keyFile.txt");
            
            foreach (KeyValuePair<byte[], byte[]> keyValuePair in alarmsFromStorage)
            {
                
                Alarm decriptedAlarm = AESInECB.DecryptAlarm(keyValuePair.Value, key);
                if(decriptedAlarm.Risk>min && decriptedAlarm.Risk<max)
                    alarms.Add(keyValuePair.Key,keyValuePair.Value);
                

            }
            return alarms;
        }

        
    }
}
