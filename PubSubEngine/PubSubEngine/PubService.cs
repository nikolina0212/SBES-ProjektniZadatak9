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
    public class PubService : IPublish
    {

        public void Send(byte[] alarm,byte[] sign)
        {
            string clienName = Formatter.ParseName(ServiceSecurityContext.Current.PrimaryIdentity.Name);
            string clientNameSign = clienName + "_sign";
            X509Certificate2 certificate = CertManager.GetCertificateFromStorage(StoreName.TrustedPeople,
                StoreLocation.LocalMachine, clientNameSign);

            string key = SecretKey.LoadKey("keyFile.txt");
            Alarm alarm_dekriptovan = AESInECB.DecryptAlarm(alarm,key);
            AlarmStorage.alarms.Add(alarm_dekriptovan);
            AlarmStorage.alarmsEncripted.Add(sign, alarm);
            AlarmStorage.alarmsSigned.Add(sign, alarm_dekriptovan);
        }
    }
}
