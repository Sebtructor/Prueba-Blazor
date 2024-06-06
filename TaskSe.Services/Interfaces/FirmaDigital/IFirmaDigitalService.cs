using TaskSe.Model.DTO;
using TaskSe.Model.Models.FirmaDigital;
using Microsoft.AspNetCore.Components.Forms;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace TaskSe.Services.Interfaces.FirmaDigital
{
    public interface IFirmaDigitalService
    {
        public Task<bool> firmarDocumentoLocal(string ruta_certificado, string contraseña_certificado, string ruta_documento_original, string ruta_final, string nombreFirmante, string nombreCliente, string documentoCliente, int posicionX, int posicionY, int pagina, string usuario,string ipAddress, string descripcion);
        public Task<bool> firmarDocumentoOlimpia(string ruta_documento_original, string ruta, string nombreFirmante, string nombreCliente, string documentoCliente, int pagina);
        public Task<string> firmaDocumentoDigitalmente(IReadOnlyList<IBrowserFile> files, string ipAddress, string usuario, int tipoFirma);
        public Task<IEnumerable<Firma>> getFirmas();
        public Task<Firma> getFirma(string id_firma);
        public Task<bool> guardarBase64FirmaElectronica(string base64, string ruta, FirmaElectronicaDTO firma);
    }
}
