﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace SecurityManager
{
	public class CertManager
	{
		public static X509Certificate2 GetSignatureCertificate()
		{

			string clientNameSign = "publisher_sign";

			X509Certificate2 certificate = null; 
			try
			{
				certificate = CertManager.GetCertificateFromStorage(StoreName.TrustedPeople, StoreLocation.LocalMachine, clientNameSign);
				return certificate;
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				throw new Exception(e.Message);
			}
		}
		public static X509Certificate2 GetCertificateFromStorage(StoreName storeName, StoreLocation storeLocation, string subjectName)
		{
			X509Store store = new X509Store(storeName, storeLocation);
			store.Open(OpenFlags.ReadOnly);
			X509Certificate2Collection certCollection = store.Certificates.Find(X509FindType.FindBySubjectName, subjectName, true);

			foreach (X509Certificate2 c in certCollection)
			{
				if (c.SubjectName.Name.Equals(string.Format("CN={0}", subjectName)))
				{
					return c;
				}
			}

			return null;
		}

		public static X509Certificate2 GetCertificateFromFile(string fileName)
		{
			X509Certificate2 certificate = null;


			return certificate;
		}

		/// <summary>
		/// Get a certificate from file.
		/// </summary>
		/// <param name="fileName">.pfx file name</param>
		/// <param name="pwd"> password for .pfx file</param>
		/// <returns>The requested certificate. If no valid certificate is found, returns null.</returns>
		public static X509Certificate2 GetCertificateFromFile(string fileName, SecureString pwd)
		{
			X509Certificate2 certificate = null;


			return certificate;
		}
	}
}
