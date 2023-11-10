using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography.X509Certificates;
using System.IdentityModel.Selectors;

namespace SecurityManager
{
    public class ClientCertValidator : X509CertificateValidator
    {
        // klijentska validacija: da li je sertifikat istekao, da je self signed i da li je CN odgovarajuci
        /// <param name="certificate"> certificate to be validate </param>
        public override void Validate(X509Certificate2 certificate)
        {
            if (certificate.Subject.Equals(certificate.Issuer))
            {
                throw new Exception("Certificate is self-issued.");
            }

            DateTime expirationDate = DateTime.Parse(certificate.GetExpirationDateString());
            if (expirationDate < DateTime.Now)
            {
                throw new Exception($"Certificate expired on [{expirationDate}]");
            }
        }
    }
}
