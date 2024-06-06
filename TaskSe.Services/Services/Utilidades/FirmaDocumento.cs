using DocumentFormat.OpenXml.Drawing;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Signatures;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Pkcs;
using Serilog;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Rectangle = iText.Kernel.Geom.Rectangle;

namespace TaskSe.Services.Services.Utilidades
{
    public class FirmaDocumento
    {
        public bool FirmarDocumento(string archivoOrigen, string archivoFinal, string rutaCertificado, string contraseñaCertificado, 
            string nombreFirmante, string nombreCliente, string numeroIdentificacionCliente, int x, int y, int pagina, int tamaño_fuente = 8)
        {
            try
            {
                if (!File.Exists(archivoOrigen))
                {
                    Log.Error($"Error al firmar documento {archivoOrigen}: La ruta especificada para el archivo de origen {archivoOrigen} no existe");
                    return false;
                }
                    

                if (!File.Exists(rutaCertificado))
                {
                    Log.Error($"Error al firmar documento {archivoOrigen}: La ruta especificada para el certificado {rutaCertificado} no existe");
                    return false;
                }
                    

                if (pagina <= 0)
                    return false;

                char[] PASSWORD = contraseñaCertificado.ToCharArray();

                Pkcs12Store pk12 = new Pkcs12Store(new FileStream(rutaCertificado,
                FileMode.Open, FileAccess.Read), PASSWORD);
                string alias = null;
                foreach (object a in pk12.Aliases)
                {
                    alias = ((string)a);
                    if (pk12.IsKeyEntry(alias))
                    {
                        break;
                    }
                }
                ICipherParameters pk = pk12.GetKey(alias).Key;


                X509CertificateEntry[] ce = pk12.GetCertificateChain(alias);
                Org.BouncyCastle.X509.X509Certificate[] chain = new Org.BouncyCastle.X509.X509Certificate[ce.Length];
                for (int k = 0; k < ce.Length; ++k)
                {
                    chain[k] = ce[k].Certificate;
                }

                PdfReader reader = new PdfReader(archivoOrigen);
                PdfSigner signer = new PdfSigner(reader,
                new FileStream(archivoFinal, FileMode.Create),
                new StampingProperties());

                PdfSignatureAppearance appearance = signer.GetSignatureAppearance();


                string Firma = $"Digitally signed by {nombreFirmante}\nDate:" + signer.GetSignDate().ToString("yyyy.MM.dd HH:mm.ss zzz") + $"\nCliente Firmante: {nombreCliente} \nIdentificación Firmante: {numeroIdentificacionCliente}";

                appearance.SetReason("Firma Digital")
                  .SetLocation("Bogotá, CO")
                  .SetLayer2Text(Firma)
                  .SetLayer2FontSize(tamaño_fuente)
                  .SetPageRect(new Rectangle(x,y, 760, 300))
                  .SetPageNumber(pagina);

                signer.SetFieldName("FirmaDigital");

                IExternalSignature pks = new PrivateKeySignature(pk, DigestAlgorithms.SHA256);

                signer.SignDetached(pks, chain, null, null, null, 0,
                PdfSigner.CryptoStandard.CMS);

                if (File.Exists(archivoFinal))
                    return true;
                else
                    return false;
            }
            catch (Exception exe)
            {
                return false;
            }
        }

        public static int getCantidadPaginasPDF(string ruta_archivo)
        {
            int paginas = 0;

            try
            {
                iTextSharp.text.pdf.PdfReader pdfReader = new iTextSharp.text.pdf.PdfReader(ruta_archivo);
                

                try
                {
                    paginas = pdfReader.NumberOfPages;
                }
                catch(Exception exe)
                {

                }
                finally
                {
                    pdfReader.Close();
                }
            }
            catch(Exception exe)
            {
                
            }

            return paginas;
        }
    }
}
