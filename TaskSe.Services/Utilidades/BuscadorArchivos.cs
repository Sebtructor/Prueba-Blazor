
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskSe.Services.Utilidades
{
    public class BuscadorArchivos
    {
        public static string buscarRutaArchivo(string RUTA_DESTINO, string pattern, string final)
        {
            string ruta_archivo_final = "";

            try
            {
                var query = from p in Directory.GetFiles(RUTA_DESTINO).AsEnumerable()
                            where p.Contains(pattern) && p.EndsWith(final)
                            select p;

                string rutaArchivo = query.LastOrDefault().ToString();


                if (File.Exists(rutaArchivo))
                {
                    ruta_archivo_final = rutaArchivo;
                }
                else
                {

                }
            }
            catch (Exception exe)
            {
                Log.Error(exe, $"Error al buscar archivo en la ruta {RUTA_DESTINO} con el patrón {pattern}");
            }

            return ruta_archivo_final;
        }
    }
}
