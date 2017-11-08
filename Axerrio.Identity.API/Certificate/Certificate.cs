using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace Axerrio.Identity.API.Certificate
{
    //https://www.humankode.com/asp-net-core/develop-locally-with-https-self-signed-certificates-and-asp-net-core
    public static class Certificate
    {
        public static X509Certificate2 Get()
        {
            var assembly = typeof(Certificate).GetTypeInfo().Assembly;
            var names = assembly.GetManifestResourceNames();

            /***********************************************************************************************
             *  Please note that here we are using a local certificate only for testing purposes. In a 
             *  real environment the certificate should be created and stored in a secure way, which is out
             *  of the scope of this project.
             **********************************************************************************************/

            //using (var stream = assembly.GetManifestResourceStream("Axerrio.Identity.API.Certificate.idsrv3test.pfx"))
            using (var stream = assembly.GetManifestResourceStream("Axerrio.Identity.API.Certificate.localhost.pfx"))
            {
                //return new X509Certificate2(ReadStream(stream), "idsrv3test");
                return new X509Certificate2(ReadStream(stream), "vexcherk");
            }
        }

        private static byte[] ReadStream(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }
    }
}
