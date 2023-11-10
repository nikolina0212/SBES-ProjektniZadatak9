using Contracts;
using Encrypting;
using SecurityManager;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;

namespace Subscriber
{
    class Program
    {
        private static List<Alarm> AllAlarmsForThisSub = new List<Alarm>();
        static void Main(string[] args)
        {

            string serverCertCN = "pubsubserver";

            X509Certificate2 srvCert = CertManager.GetCertificateFromStorage(StoreName.TrustedPeople,
               StoreLocation.LocalMachine, serverCertCN);

            NetTcpBinding binding = new NetTcpBinding();
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;

            EndpointAddress address = new EndpointAddress(new Uri("net.tcp://localhost:8000/SubService"), new X509CertificateEndpointIdentity(srvCert));

            using (ClientProxy proxy = new ClientProxy(binding, address))
            {
                Console.WriteLine("Konekcija uspostavljena\n(exit za izlaz iz programa)");
                Console.WriteLine("Opseg rizika alarm za koji želite da prijavite (min-max) : ");

                string rizik = Console.ReadLine();
                    
                string rizikMin = rizik.Split('-')[0];
                string rizikMax = rizik.Split('-')[1];
                int minRizik = Int32.Parse(rizikMin);
                int maxRizik = Int32.Parse(rizikMax);
                string key = SecretKey.LoadKey("keyFile.txt");

                

                while (true)
                {

                    Dictionary<byte[],byte[]> alarms = proxy.ForwardAlarm(minRizik, maxRizik);
                    if (alarms != null && alarms.Count > 0)
                    {
                        List<Alarm> NewAlarms = new List<Alarm>();
                        Dictionary<byte[], Alarm> keyValuePairs = new Dictionary<byte[], Alarm>();
                        foreach (byte[] a in alarms.Values)
                        {
                            if (AllAlarmsForThisSub.Count == alarms.Count)
                            {
                                NewAlarms.Clear();
                                keyValuePairs.Clear();
                                break;
                            }
                            else if ((AllAlarmsForThisSub.Count == 0) && (alarms.Count > 0))
                            {
                                foreach (KeyValuePair<byte[], byte[]> al in alarms)
                                {
                                    Alarm alarmForSub = AESInECB.DecryptAlarm(al.Value, key);
                                    AllAlarmsForThisSub.Add(alarmForSub);
                                    NewAlarms.Add(alarmForSub);
                                    keyValuePairs.Add(al.Key, alarmForSub);
                                }
                            }
                            else
                            {

                                List<byte[]> alarmsInList = alarms.Values.ToList();
                                List<byte[]> signList = alarms.Keys.ToList();
                                for (int i = AllAlarmsForThisSub.Count - 1; i < alarms.Count; i++)
                                {
                                    Alarm alarmForSub = AESInECB.DecryptAlarm(alarmsInList[i], key);
                                    AllAlarmsForThisSub.Add(alarmForSub);
                                    NewAlarms.Add(alarmForSub);
                                    keyValuePairs.Add(signList[i], alarmForSub);
                                }
                                break;
                            }

                        }

                        if (keyValuePairs.Count > 0)
                        {
                            foreach (KeyValuePair<byte[],Alarm> a in keyValuePairs)
                            {
                                Console.WriteLine("Alarm : ");
                                Console.WriteLine("Vreme generisanja : " + a.Value.GeneratingTime);
                                Console.WriteLine("Poruka o alarmu : " + a.Value.MessegAlarm);
                                Console.WriteLine("Rizik : " + a.Value.Risk);

                                using (StreamWriter writer = new StreamWriter("alarm.txt", true))
                                {
                                    string text = "";
                                    text += "Vreme generisanja: " + a.Value.GeneratingTime + " Poruka o alarmu: " + a.Value.MessegAlarm + " Rizik: " + a.Value.Risk;
                                    writer.WriteLine(text);

                                }
                                try
                                {
                                    string id= Guid.NewGuid().ToString();
                                    string sign="";
                                    foreach (byte b in a.Key)
                                        sign += b.ToString();
                                    Audit.WriteAlarm(a.Value.GeneratingTime, "alarm.txt", id,sign,CertManager.GetSignatureCertificate());
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine(e.Message);
                                }
                            }


                            NewAlarms.Clear();
                        }

                    }
                }
            }

        }
    }
}
