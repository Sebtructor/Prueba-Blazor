using TaskSe.Data.Connection;
using TaskSe.Data.Repositories.Repositories.Mensajes;
using TaskSe.Data.Repositories.Repositories.Par;
using TaskSe.Data.Repositories.Interfaces.Mensajes;
using TaskSe.Data.Repositories.Interfaces.Par;
using TaskSe.Model.Models.Par;
using TaskSe.Services.Interfaces.Par;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskSe.Services.Services.Par
{
    public class ParService : IParService
    {
        private readonly IParRepository _parRepository;

        public ParService(SqlConfiguration sqlConfiguration)
        {
            _parRepository = new ParRepository(sqlConfiguration.ConnectionString);
        }

        public async Task<IEnumerable<Ciudad>> GetCiudades()
        {
            return await _parRepository.GetCiudades();
        }

        public async Task<IEnumerable<Ciudad>> GetCiudadesByDepartamento(string id_departamento)
        {
            return await _parRepository.GetCiudadesByDepartamento(id_departamento);
        }

        public async Task<IEnumerable<ClaseVinculacion>> GetClasesVinculacion()
        {
            return await _parRepository.GetClasesVinculacion();
        }

        public async Task<IEnumerable<Ciiu>> GetCodigosCiiu()
        {
            return await _parRepository.GetCodigosCiiu();
        }

        public async Task<IEnumerable<Departamento>> GetDepartamentos()
        {
            return await _parRepository.GetDepartamentos();
        }

        public async Task<IEnumerable<OrigenRecursos>> GetOrigenesRecursos()
        {
            return await _parRepository.GetOrigenesRecursos();
        }

        public async Task<IEnumerable<Pais>> GetPaises()
        {
            return await _parRepository.GetPaises();
        }

        public async Task<IEnumerable<Profesion>> GetProfesiones()
        {
            return await _parRepository.GetProfesiones();
        }

        public async Task<IEnumerable<SiNo>> GetRespuestasSiNo()
        {
            return await _parRepository.GetRespuestasSiNo();
        }

        public async Task<IEnumerable<Sexo>> GetSexos()
        {
            return await _parRepository.GetSexos();
        }

        public async Task<IEnumerable<SituacionLaboral>> GetSituacionesLaborales()
        {
            return await _parRepository.GetSituacionesLaborales();
        }

        public async Task<IEnumerable<TipoDocumento>> GetTipoDocumentos()
        {
            return await _parRepository.GetTipoDocumentos();
        }

        public async Task<IEnumerable<TipoEmpresa>> GetTiposEmpresas()
        {
            return await _parRepository.GetTiposEmpresas();
        }

        public async Task<IEnumerable<TipoPep>> GetTiposPep()
        {
            return await _parRepository.GetTiposPep();
        }

        public async Task<IEnumerable<TipoRepresentante>> GetTiposRepresentantesLegales()
        {
            return await _parRepository.GetTiposRepresentantesLegales();
        }

        public async Task<IEnumerable<TipoSolicitud>> GetTiposSolicitud()
        {
            return await _parRepository.GetTiposSolicitud();
        }
    }
}
