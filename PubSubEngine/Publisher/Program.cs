using Contracts;
using Encrypting;
using SecurityManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Publisher
{
    class Program
    {

        static void Main(string[] args)
        {

            string serverCertCN = "pubsubserver";

            NetTcpBinding binding = new NetTcpBinding();
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;

            X509Certificate2 srvCert = CertManager.GetCertificateFromStorage(StoreName.TrustedPeople,
            StoreLocation.LocalMachine, serverCertCN);

            EndpointAddress address = new EndpointAddress(new Uri("net.tcp://localhost:8000/PubService"), new X509CertificateEndpointIdentity(srvCert));

            using (ClientProxy proxy = new ClientProxy(binding, address))
            {
                Console.WriteLine("Konekcija uspostavljena\n(exit za izlaz iz programa)");

                Console.WriteLine("Definisite interval za objavljivanje (integer, milliseconds): ");
                string ulaz = Console.ReadLine();
                int interval;
                while (!Int32.TryParse(ulaz, out interval))
                {
                    Console.WriteLine("Interval mora biti broj! Pokusajte ponovo: ");
                    ulaz = Console.ReadLine();
                }

                proxy.PublishingInterval = interval;

                while (true)
                {
                    bool vaidniBrojevi = true;
                    Console.WriteLine("Alarm za koji želite da objavite poruku : ");
                    Console.WriteLine("Unesite vreme generisanja (h:m:s) : ");
                    string time = Console.ReadLine();
                    if (time == "exit")
                        break;
                    Console.WriteLine("Unesite datum generisanja (day/month/year) : ");
                    string date = Console.ReadLine();

                    DateTime dateTime;
                    if (DateTime.TryParse(date + " " + time, out dateTime))
                    {
                        Console.WriteLine(dateTime);
                    }
                    else
                    {
                        Console.WriteLine("Datum nije dobar unesite alarm opet");
                        vaidniBrojevi = false;

                    }
                    Console.WriteLine("Poruka o alarmu : ");
                    string poruka = Console.ReadLine();
                    Console.WriteLine("Rizik : (1-100)");
                    string rizik = Console.ReadLine();
                    int rizikInt;
                    if (!(Int32.TryParse(rizik, out rizikInt)))
                    {
                        Console.WriteLine("Rizik je broj");
                        vaidniBrojevi = false;

                    }
                    if (vaidniBrojevi)
                    {
                        Alarm alarm = new Alarm(dateTime, poruka, rizikInt);
                        string key=SecretKey.LoadKey("keyFile.txt");
                        byte[] message= AESInECB.EncryptAlarm(alarm, key);
                        string signCertCN = Formatter.ParseName(WindowsIdentity.GetCurrent().Name)+"_sign";

                        X509Certificate2 certificateSign = CertManager.GetCertificateFromStorage(StoreName.My,
                            StoreLocation.LocalMachine, signCertCN);
                        byte[] signature = DigitalSignature.Create(message, HashAlgorithm.SHA1, certificateSign);
                        

                        try
                        {
                            proxy.Send(message,signature);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("program.cs : " + ex.Message);
                            Console.ReadLine();
                        }

                        Thread.Sleep(proxy.PublishingInterval);
                    }
                }
               
            }

        }
       
    }

}
