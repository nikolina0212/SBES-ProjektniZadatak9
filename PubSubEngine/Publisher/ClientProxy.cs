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

namespace Publisher
{
    internal class ClientProxy : ChannelFactory<IPublish>, IPublish, IDisposable
    {
        IPublish factory;

        public int PublishingInterval { get; set; }

        public ClientProxy(NetTcpBinding binding, EndpointAddress address) : base(binding, address)
        {
            
            string cltCertCN = Formatter.ParseName(WindowsIdentity.GetCurrent().Name);

            this.Credentials.ServiceCertificate.Authentication.CertificateValidationMode = System.ServiceModel.Security.X509CertificateValidationMode.ChainTrust;
            this.Credentials.ServiceCertificate.Authentication.CustomCertificateValidator = new ClientCertValidator();
            this.Credentials.ServiceCertificate.Authentication.RevocationMode = X509RevocationMode.NoCheck;

            this.Credentials.ClientCertificate.Certificate = CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, cltCertCN);
            
            factory = this.CreateChannel();
            
        }

        public void Send(byte[] alarm, byte[] sign)
        {
            factory.Send(alarm, sign);
        }
    }
}
