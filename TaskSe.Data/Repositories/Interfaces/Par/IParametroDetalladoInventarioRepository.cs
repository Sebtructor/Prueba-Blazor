
using TaskSe.Model.Models.Par;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskSe.Data.Repositories.Interfaces.Par
{
    public interface IParametroDetalladoRepository
    {
        public Task<bool> insertarInformacionParametroDetallado(ParametroDetallado parametroDetallado, string id_parametro_general, string usuario_accion);
        public Task<IEnumerable<ParametroDetallado>> consultarParametrosDetalladosByParametroGeneral(string id_parametro_general);
        public Task<ParametroDetallado> consultarParametroDetalladoById(string id_parametro_detallado);
        public Task<bool> eliminarParametroDetallado(string id_parametro_detallado);
    }
}
