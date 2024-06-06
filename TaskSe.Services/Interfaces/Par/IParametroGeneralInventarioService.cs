
using TaskSe.Model.Models.Par;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskSe.Services.Interfaces.Par
{
    public interface IParametroGeneralService
    {
        public Task<bool> insertarInfoParametroGeneral(ParametroGeneral parametroGeneral, string usuario_adiciono);
        public Task<IEnumerable<ParametroGeneral>> consultarParametrosGenerales();
        public Task<ParametroGeneral> consultarParametroGeneral(string id_parametro_general);
        public Task<bool> eliminarParametroGeneral(string id_parametro_general);

        public Task<bool> eliminarParametroDetallado(string id_parametro_detallado);
        public Task<bool> validarParametroDetallado(ParametroDetallado pDetallado);

    }
}
