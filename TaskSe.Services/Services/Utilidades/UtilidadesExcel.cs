using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskSe.Services.Services.Utilidades
{
    public class UtilidadesExcel
    {
        public static string totalRegistros(string ruta_archivo, int hoja)
        {
            string REGISTROS = "0";
            try
            {
                XSSFWorkbook LibroExcel = new XSSFWorkbook(ruta_archivo);
                XSSFSheet HojaExcel = LibroExcel.GetSheetAt(hoja) as XSSFSheet;
                try
                {
                    REGISTROS = HojaExcel.LastRowNum.ToString();
                }
                catch (Exception exe)
                {

                }
                finally
                {
                    LibroExcel.Close();
                }
            }
            catch (Exception exe)
            {

            }

            return REGISTROS;
        }

        public static Boolean ARCHIVO_VACIO(string RUTA, int hoja)
        {
            Boolean resultado = true;
            try
            {
                XSSFWorkbook LibroExcel = new XSSFWorkbook(RUTA);
                XSSFSheet HojaExcel = LibroExcel.GetSheetAt(hoja) as XSSFSheet;
                try
                {

                    if (HojaExcel.LastRowNum >= 1)
                    {
                        resultado = false;
                    }
                }
                catch (Exception exe)
                {

                }
                finally
                {
                    LibroExcel.Close();
                }
            }
            catch (Exception exe)
            {

            }

            return resultado;
        }
    }
}
