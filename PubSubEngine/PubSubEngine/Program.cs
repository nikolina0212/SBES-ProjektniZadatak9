using Contracts;
using Publisher;
using Subscriber;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using SecurityManager;
using Encrypting;
using System.ServiceModel.Description;

namespace PubSubEngine
{
    class Program
    {
        static void Main(string[] args)
        {
            string srvCertCN = Formatter.ParseName(WindowsIdentity.GetCurrent().Name);

            string addressPub = "net.tcp://localhost:8000/PubService";
            ServiceHost pubHost = SreviceHostMethod.HostMethod(addressPub, typeof(PubService), typeof(IPublish),srvCertCN);
            string addressSub = "net.tcp://localhost:8000/SubService";
            ServiceHost subHost = SreviceHostMethod.HostMethod(addressSub, typeof(SubService), typeof(ISubscribe),srvCertCN);
            string key = SecretKey.GenerateKey();
            SecretKey.StoreKey(key, "keyFile.txt");

           
            ServiceSecurityAuditBehavior newAudit = new ServiceSecurityAuditBehavior();
            newAudit.AuditLogLocation = AuditLogLocation.Application;
            newAudit.ServiceAuthorizationAuditLevel = AuditLevel.Success;

            subHost.Description.Behaviors.Remove<ServiceSecurityAuditBehavior>();
            subHost.Description.Behaviors.Add(newAudit);

            try
            {
                pubHost.Open();
                subHost.Open();
                Console.WriteLine("Publisher host has started.");
                Console.WriteLine("Subscriber host has started.");

                Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine("[ERROR] {0}", e.Message);
                Console.ReadLine();
                Console.WriteLine("[StackTrace] {0}", e.StackTrace);
            }
            finally
            {
                pubHost.Close();
                subHost.Close();
            }
        }
    }
}
