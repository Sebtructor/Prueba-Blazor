using TaskSe.Model.Models.Configuracion.Perfilamiento;
using TaskSe.Services.Interfaces.Auditoria;
using Microsoft.JSInterop;
using Serilog;

namespace TaskSe.Web.Helpers
{
    public static class IJSRuntimeExtension
    {
        public static IAuditoriaService _auditoriaService { get; private set; }

        public static void setAuditoriaService(IAuditoriaService auditoriaService)
        {
            _auditoriaService = auditoriaService;
        }

        public static async Task SweetAlertUsual(this IJSRuntime jsRuntime, string titulo, string mensaje, TipoMensajeSweetAlert tipoMensaje)
        {
            try
            {
                await jsRuntime.InvokeVoidAsync("Swal.fire", titulo, mensaje, tipoMensaje.ToString());
            }
            catch (Exception exe)
            {
                Log.Error(exe, "Error al ejecutar javascript");
            }
        }

        public static async Task SweetAlertHtml(this IJSRuntime jsRuntime, string titulo, string mensaje, TipoMensajeSweetAlert tipoMensaje)
        {
            try
            {
                await jsRuntime.InvokeVoidAsync("swalWithHtml", titulo, mensaje, tipoMensaje.ToString());
            }
            catch (Exception exe)
            {
                Log.Error(exe, "Error al ejecutar javascript");
            }
        }

        public static async Task SweetAlertUsualWithFooter(this IJSRuntime jsRuntime, string titulo, string mensaje, string footer, TipoMensajeSweetAlert tipoMensaje)
        {
            try
            {
                await jsRuntime.InvokeVoidAsync("swalWithFooter", titulo, mensaje, footer, tipoMensaje.ToString());
            }
            catch (Exception exe)
            {
                Log.Error(exe, "Error al ejecutar javascript");
            }
        }

        public static async Task SweetAlertBasico(this IJSRuntime jsRuntime, string message)
        {
            try
            {
                await jsRuntime.InvokeVoidAsync("Swal.fire", message);
            }
            catch (Exception exe)
            {
                Log.Error(exe, "Error al ejecutar javascript");
            }
        }

        public static async Task<bool> SweetAlertConfirm(this IJSRuntime jsRuntime, string titulo, string mensaje, TipoMensajeSweetAlert tipoMensaje)
        {
            try
            {
                return await jsRuntime.InvokeAsync<bool>("CustomConfirmSwal", titulo, mensaje, tipoMensaje.ToString());
            }
            catch (Exception exe)
            {
                Log.Error(exe, "Error al ejecutar javascript");
            }

            return false;
        }

        public static async Task<bool> SweetAlertConfirmSuccess(this IJSRuntime jsRuntime, string titulo, string mensaje, TipoMensajeSweetAlert tipoMensaje)
        {
            try
            {
                return await jsRuntime.InvokeAsync<bool>("CustomConfirmSuccessSwal", titulo, mensaje, tipoMensaje.ToString());
            }
            catch (Exception exe)
            {
                Log.Error(exe, "Error al ejecutar javascript");
            }

            return false;
        }

        public static async Task SweetAlertLoading(this IJSRuntime jsRuntime, string titulo, string mensaje)
        {
            try
            {
                await jsRuntime.InvokeVoidAsync("LoadingSwal", titulo, mensaje);
            }
            catch (Exception exe)
            {
                Log.Error(exe, "Error al ejecutar javascript");
            }
        }

        public static async Task SweetAlertClose(this IJSRuntime jsRuntime)
        {
            try
            {
                await jsRuntime.InvokeVoidAsync("Swal.close");
            }
            catch (Exception exe)
            {
                Log.Error(exe, "Error al ejecutar javascript");
            }
        }

        public static async Task DescargarArchivo(this IJSRuntime jsRuntime, string ruta, string nombre)
        {
            if (!File.Exists(ruta)) throw new Exception($"La ruta especificada para el archivo para descargar {ruta} no existe");

            try
            {
                FileInfo fi = new FileInfo(ruta);
                string ruta_destino = Directory.GetCurrentDirectory() + "\\wwwroot\\archivosTemporales";
                if (!Directory.Exists(ruta_destino)) Directory.CreateDirectory(ruta_destino);

                string ruta_final = ruta_destino + "\\" + fi.Name;

                if (File.Exists(ruta_final)) File.Delete(ruta_final);

                File.Copy(ruta, ruta_final);

                string ruta_cliente = ruta_final.Replace(Directory.GetCurrentDirectory() + "\\wwwroot\\", "").Replace("\\", "/");

                await jsRuntime.InvokeVoidAsync("downloadURI", ruta_cliente, nombre + fi.Extension);

                AuditoriaDescargaArchivo auditoria = new AuditoriaDescargaArchivo()
                {
                    extension_archivo = fi.Extension,
                    ip_address = await jsRuntime.InvokeAsync<string>("getIpAddress")
                    .ConfigureAwait(true),
                    nombre_archivo = nombre + fi.Extension,
                    ruta_descargada = ruta_cliente,
                    ruta_original = ruta,
                    usuario = await jsRuntime.InvokeAsync<string>("getActualUser"),
                    peso_archivo = new FileInfo(ruta).Length.ToString()
                };

                await _auditoriaService.registrarAuditoriaDescargaArchivos(auditoria);

                if (File.Exists(ruta_final)) File.Delete(ruta_final);
            }
            catch (Exception exe)
            {
                Log.Error(exe, "Error al ejecutar javascript");

            }
        }

        public static async Task<string> GetIpAddress(this IJSRuntime jsRuntime)
        {
            try
            {
                var ipAddress = await jsRuntime.InvokeAsync<string>("getIpAddress")
                    .ConfigureAwait(true);
                return ipAddress;
            }
            catch (Exception exe)
            {
                //If your request was blocked by CORS or some extension like uBlock Origin then you will get an exception.
                await jsRuntime.InvokeVoidAsync("console.log", $"Error al obtener IP: {exe.Message} - {exe.InnerException}");
                return string.Empty;
            }
        }

        public static async Task InitializeInactivityTimer<T>(this IJSRuntime jsRuntime,
            DotNetObjectReference<T> dotNetObjectReference) where T : class
        {
            try
            {
                await jsRuntime.InvokeVoidAsync("initializeInactivityTimer", dotNetObjectReference);
            }
            catch (Exception exe)
            {
                Log.Error(exe, "Error al ejecutar javascript");
            }
        }

        public enum TipoMensajeSweetAlert
        {
            question, warning, error, success, info
        }


    }
}
