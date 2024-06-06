using TaskSe.Model.Models.Par;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskSe.Data.Repositories.Interfaces.Par
{
    public interface IParRepository
    {
        public Task<IEnumerable<TipoDocumento>> GetTipoDocumentos();
        public Task<IEnumerable<Ciiu>> GetCodigosCiiu();
        public Task<IEnumerable<Ciudad>> GetCiudades();
        public Task<IEnumerable<Ciudad>> GetCiudadesByDepartamento(string id_departamento);
        public Task<IEnumerable<Departamento>> GetDepartamentos();
        public Task<IEnumerable<ClaseVinculacion>> GetClasesVinculacion();
        public Task<IEnumerable<OrigenRecursos>> GetOrigenesRecursos();
        public Task<IEnumerable<Pais>> GetPaises();
        public Task<IEnumerable<Profesion>> GetProfesiones();
        public Task<IEnumerable<SiNo>> GetRespuestasSiNo();
        public Task<IEnumerable<Sexo>> GetSexos();
        public Task<IEnumerable<SituacionLaboral>> GetSituacionesLaborales();
        public Task<IEnumerable<TipoPep>> GetTiposPep();
        public Task<IEnumerable<TipoSolicitud>> GetTiposSolicitud();
        public Task<IEnumerable<TipoEmpresa>> GetTiposEmpresas();
        public Task<IEnumerable<TipoRepresentante>> GetTiposRepresentantesLegales();
    }
}
