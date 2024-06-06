
using TaskSe.Data.Connection;
using TaskSe.Data.Repositories.Repositories.Par;
using TaskSe.Data.Repositories.Interfaces.Par;
using TaskSe.Model.Models.Par;
using TaskSe.Services.Interfaces.Par;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskSe.Services.Services.Par
{
    public class ParametroGeneralService : IParametroGeneralService
    {
        private readonly IParametroGeneralRepository _parametroGeneralRepository;
        private readonly IParametroDetalladoRepository _parametroDetalladoRepository;
        private readonly SqlConfiguration _sqlConfiguration;

        public ParametroGeneralService(SqlConfiguration sqlConfiguration)
        {
            _sqlConfiguration = sqlConfiguration;
            _parametroGeneralRepository = new ParametroGeneralRepository(_sqlConfiguration.ConnectionString);
            _parametroDetalladoRepository = new ParametroDetalladoRepository(_sqlConfiguration.ConnectionString);
        }

        public async Task<ParametroGeneral> consultarParametroGeneral(string id_parametro_general)
        {
            ParametroGeneral parametroGeneral = await _parametroGeneralRepository.consultarParametroGeneral(id_parametro_general);

            if (parametroGeneral is not null)
            {
                IEnumerable<ParametroDetallado> parametrosDetallados = await _parametroDetalladoRepository.consultarParametrosDetalladosByParametroGeneral(id_parametro_general);
                parametroGeneral.listaParametrosDetallados = parametrosDetallados.ToList();
            }

            return parametroGeneral;
        }

        public async Task<IEnumerable<ParametroGeneral>> consultarParametrosGenerales()
        {
            IEnumerable<ParametroGeneral> parametrosGenerales = await _parametroGeneralRepository.consultarParametrosGenerales();

            foreach (var parametro in parametrosGenerales)
            {
                IEnumerable<ParametroDetallado> parametrosDetallados = await _parametroDetalladoRepository.consultarParametrosDetalladosByParametroGeneral(parametro.id_parametro_general);
                parametro.listaParametrosDetallados = parametrosDetallados.ToList();
            }

            return parametrosGenerales;
        }

        public async Task<bool> eliminarParametroDetallado(string id_parametro_detallado)
        {
            return await _parametroDetalladoRepository.eliminarParametroDetallado(id_parametro_detallado);
        }

        public async Task<bool> eliminarParametroGeneral(string id_parametro_general)
        {
            IEnumerable<ParametroDetallado> pDetallados = await _parametroDetalladoRepository.consultarParametrosDetalladosByParametroGeneral(id_parametro_general);

            foreach (var p in pDetallados)
            {
                await _parametroDetalladoRepository.eliminarParametroDetallado(p.id_parametro_detallado);
            }

            return await _parametroGeneralRepository.eliminarParametroGeneral(id_parametro_general);
        }

        public async Task<bool> insertarInfoParametroGeneral(ParametroGeneral parametroGeneral, string usuario_adiciono)
        {
            string id_parametro_general = await _parametroGeneralRepository.insertarInfoParametroGeneral(parametroGeneral, usuario_adiciono);

            if (string.IsNullOrEmpty(id_parametro_general)) return false;

            foreach (var pDetallado in parametroGeneral.listaParametrosDetallados)
            {
                await _parametroDetalladoRepository.insertarInformacionParametroDetallado(pDetallado, id_parametro_general, usuario_adiciono);
            }

            return true;
        }

        public async Task<bool> validarParametroDetallado(ParametroDetallado pDetallado)
        {
            if (string.IsNullOrEmpty(pDetallado.nombre_parametro_detallado))
            {
                return false;
            }

            return true;
        }

        
    }
}
