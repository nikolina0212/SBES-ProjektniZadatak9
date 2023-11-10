using Contracts;
using SecurityManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Subscriber
{
    internal class ClientProxy : ChannelFactory<ISubscribe>, ISubscribe, IDisposable
    {
        ISubscribe factory;

        public ClientProxy(NetTcpBinding binding, EndpointAddress address) : base(binding, address)
        {
			string cltCertCN = Formatter.ParseName(WindowsIdentity.GetCurrent().Name);

            this.Credentials.ServiceCertificate.Authentication.CertificateValidationMode = System.ServiceModel.Security.X509CertificateValidationMode.ChainTrust;
            this.Credentials.ServiceCertificate.Authentication.CustomCertificateValidator = new ClientCertValidator();
            this.Credentials.ServiceCertificate.Authentication.RevocationMode = X509RevocationMode.NoCheck;

            this.Credentials.ClientCertificate.Certificate = CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, cltCertCN);
            factory = this.CreateChannel();
        }

        


        public Dictionary<byte[], byte[]> ForwardAlarm(int min, int max)
        {
            string key = SecretKey.LoadKey("keyFile.txt");
            string clienName = "publisher";
            string clientNameSign = clienName + "_sign";
            X509Certificate2 certificate = CertManager.GetCertificateFromStorage(StoreName.TrustedPeople,
                StoreLocation.LocalMachine, clientNameSign);
            Dictionary<byte[], byte[]> alarms = new Dictionary<byte[], byte[]>();
            Dictionary<byte[], byte[]> result = factory.ForwardAlarm(min, max);
            
            foreach (KeyValuePair<byte[], byte[]> keyValuePair in result)
            {
                if (DigitalSignature.Verify(keyValuePair.Value, HashAlgorithm.SHA1, keyValuePair.Key, certificate))
                {
                    alarms.Add(keyValuePair.Key, keyValuePair.Value);
                }
            }
            return alarms;
        }
        
    }
}
