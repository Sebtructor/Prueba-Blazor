using TaskSe.Model.Models.Configuracion.Perfilamiento;
using TaskSe.Services.Interfaces.Configuracion.Perfilamiento;
using TaskSe.Web.Authentication;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.JSInterop;

namespace TaskSe.Web.Authorization
{
    public class AuthorizationService : IDisposable
    {
        public readonly IEnumerable<Modulo> modulos;
        public readonly IEnumerable<Rol> roles;
        private readonly NavigationManager _navigationManager;
        private readonly IJSRuntime _jsRuntime;


        public AuthorizationService(
            IModuloService moduloService,
            IRolService rolService,
            NavigationManager navigationManager,
            IJSRuntime jsRuntime)
        {
            modulos = moduloService.getModulosSync();
            if (modulos is null) modulos = new List<Modulo>();

            roles = rolService.getRolesSync();
            if (roles is null) roles = new List<Rol>();

            _navigationManager = navigationManager;
            _jsRuntime = jsRuntime;


            _navigationManager.LocationChanged += InfoCliente;
        }

        private async void InfoCliente(object sender, LocationChangedEventArgs e)
        {
            await _jsRuntime.InvokeVoidAsync("getClientInfo");
        }

        void IDisposable.Dispose()
        {
            // Unsubscribe from the event when our component is disposed
            _navigationManager.LocationChanged -= InfoCliente;
        }
    }


}
