using TaskSe.Model.Models.Configuracion.Perfilamiento;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.Security.Claims;

namespace TaskSe.Web.Authentication
{
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly ProtectedLocalStorage _localStorage;
        private ClaimsPrincipal _anonumous = new ClaimsPrincipal(new ClaimsIdentity());
        private readonly ILogger<CustomAuthenticationStateProvider> _logger;
        public CustomAuthenticationStateProvider(ProtectedLocalStorage sessionStorage, ILogger<CustomAuthenticationStateProvider> logger)
        {
            _localStorage = sessionStorage;
            _logger = logger;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                var userSessionStorageResult = await _localStorage.GetAsync<UserSession>("UserSession");
                var userSession = userSessionStorageResult.Success ? userSessionStorageResult.Value : null;

                if (userSession == null)
                    return await Task.FromResult(new AuthenticationState(_anonumous));

                var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier,userSession.UserName),

                }, "CustomAuth"));
                return await Task.FromResult(new AuthenticationState(claimsPrincipal));
            }
            catch (Exception exe)
            {
                return await Task.FromResult(new AuthenticationState(_anonumous));
            }
        }

        public async Task UpdateAuthenticationState(UserSession userSession)
        {
            ClaimsPrincipal claimsPrincipal;

            if (userSession != null)
            {
                await _localStorage.SetAsync("UserSession", userSession);

                claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier,userSession.UserName),
                }));
            }
            else
            {
                await _localStorage.DeleteAsync("UserSession");
                claimsPrincipal = _anonumous;
            }

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claimsPrincipal)));
        }

        public async Task<UserSession> GetActualUser()
        {
            var authState = await GetAuthenticationStateAsync();
            var user = authState.User;
            if (user.Identity.IsAuthenticated)
            {
                var result = await _localStorage.GetAsync<UserSession>("UserSession");
                UserSession userSession = result.Success ? result.Value : new UserSession();
                return userSession;
            }
            else
            {
                return new UserSession();
            }
        }

    }
}
