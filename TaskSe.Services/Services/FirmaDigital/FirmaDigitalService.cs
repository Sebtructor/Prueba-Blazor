using TaskSe.Data.Connection;
using TaskSe.Data.Repositories.Repositories.FirmaDigital;
using TaskSe.Data.Repositories.Interfaces.FirmaDigital;
using TaskSe.Model.DTO;
using TaskSe.Model.Models.Configuracion.Perfilamiento;
using TaskSe.Model.Models.FirmaDigital;
using TaskSe.Services.Interfaces.FirmaDigital;
using TaskSe.Services.Interfaces.Utilidades;
using TaskSe.Services.Services.Utilidades;
using iTextSharp.text.log;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskSe.Services.Services.FirmaDigital
{
    public class FirmaDigitalService : IFirmaDigitalService
    {
        private readonly IFirmaDigitalRepository _firmaDigitalRepository;
        private readonly ILogger<FirmaDigitalService> _logger;

        public FirmaDigitalService(SqlConfiguration sqlConfiguration,
            ILogger<FirmaDigitalService> logger)
        {
            _firmaDigitalRepository = new FirmaDigitalRepository(sqlConfiguration.ConnectionString);
            _logger = logger;
        }

        public async Task<string> firmaDocumentoDigitalmente(IReadOnlyList<IBrowserFile> files,  string ipAddress, string usuario, int tipoFirma)
        {
            string ruta_archivo = "";
            string ruta_base = Directory.GetCurrentDirectory() + "\\DocumentosFirmadosDigital";
            if (!Directory.Exists(ruta_base)) Directory.CreateDirectory(ruta_base);

            string ruta_archivos_base = ruta_base + "\\ArchivosBase";
            if (!Directory.Exists(ruta_archivos_base)) Directory.CreateDirectory(ruta_archivos_base);
            string ruta_archivos_firmados = ruta_base + "\\ArchivosFirmados";
            if (!Directory.Exists(ruta_archivos_firmados)) Directory.CreateDirectory(ruta_archivos_firmados);

            foreach (var file in files)
            {
                Stream stream = file.OpenReadStream(999999999);
                string ext = Path.GetExtension(file.Name);
                string nombreArchivo = Path.GetFileNameWithoutExtension(file.Name);
                string nombre_final = nombreArchivo + "_" + DateTime.Now.ToString("ddMMyyyyhhmmss") + ext;
                ruta_archivo = ruta_base + $"\\ArchivosBase\\{nombre_final}";
                FileStream fs = File.Create(ruta_archivo);
                try
                {
                    await stream.CopyToAsync(fs);
                }
                catch (Exception exe)
                {
                    _logger.LogError(exe, $"Error al guardar archivo para firma");
                }
                finally
                {
                    stream.Close();
                    fs.Close();
                }
            }

            if (tipoFirma != 1)
                return "";

            if (!File.Exists(ruta_archivo))
                return "";

            string ruta_final = ruta_base + $"\\ArchivosFirmados\\DocumentoFirmado_{DateTime.Now.ToString("ddMMyyyyhhmmss")}.pdf";
            string ruta_certificado = Directory.GetCurrentDirectory() + "\\wwwroot\\Certificados\\DesarrolloExcellentiam.pfx";
            string contraseñaCertificado = "Des.2020*";
            string nombreFirmante = "EXCELLENTIAM SOLUCIONES EMPRESARIALES";
            string nombreCliente = "Cliente de prueba";
            string numeroDocumentoCliente = "123456789";
            int pagina = FirmaDocumento.getCantidadPaginasPDF(ruta_archivo);
            int x = 120;
            int y = 77;

            if (await firmarDocumentoLocal(ruta_certificado,contraseñaCertificado,ruta_archivo,ruta_final,nombreFirmante,nombreCliente,numeroDocumentoCliente,x,y,pagina,usuario,ipAddress, "Firma digital de documento local"))
            {
                return ruta_final;
            } 
            else
                return "";
        }

        public async Task<bool> firmarDocumentoLocal(string ruta_certificado, string contraseña_certificado, string ruta_documento_original, string ruta_final, 
            string nombreFirmante, string nombreCliente, string documentoCliente, int posicionX, int posicionY, int pagina, string usuario, string ipAddress, string descripcion)
        {
            FirmaDocumento firmaDocumento = new FirmaDocumento();

            if (firmaDocumento.FirmarDocumento(ruta_documento_original, ruta_final, ruta_certificado, contraseña_certificado, nombreFirmante, nombreCliente, documentoCliente, posicionX, posicionY, pagina))
            {
                await _firmaDigitalRepository.insertarFirmaDigital(new Firma()
                {
                    descripcion = descripcion,
                    direccion_ip = ipAddress,
                    identificacion_cliente = documentoCliente,
                    nombre_archivo = ruta_final,
                    usuario = usuario
                });
                return true;
            }
            else
            {
                await _firmaDigitalRepository.insertarFirmaDigital(new Firma()
                {
                    descripcion = "ERROR EN FIRMA PARA: " +descripcion,
                    direccion_ip = ipAddress,
                    identificacion_cliente = documentoCliente,
                    nombre_archivo = ruta_final,
                    usuario = usuario
                });
                return false;
            }
                
        }

        public Task<bool> firmarDocumentoOlimpia(string ruta_documento_original, string ruta, string nombreFirmante, string nombreCliente, string documentoCliente, int pagina)
        {
            throw new NotImplementedException();
        }

        public async Task<Firma> getFirma(string id_firma)
        {
            return await _firmaDigitalRepository.getFirma(id_firma);
        }

        public async Task<IEnumerable<Firma>> getFirmas()
        {
            return await _firmaDigitalRepository.getFirmas();
        }

        public async Task<bool> guardarBase64FirmaElectronica(string base64, string ruta, FirmaElectronicaDTO firma)
        {
            Byte[] bytes = Convert.FromBase64String(base64);
            File.WriteAllBytes(ruta, bytes);

            if (File.Exists(ruta)) {

                await _firmaDigitalRepository.insertarFirmaDigital(new Firma()
                {
                    descripcion = firma.descripcion,
                    direccion_ip = firma.ip_accion,
                    identificacion_cliente = firma.numero_documento_proceso,
                    nombre_archivo = firma.ruta_final,
                    usuario = firma.usuario
                });

                return true;
            }

            return false;
        }
    }
}
