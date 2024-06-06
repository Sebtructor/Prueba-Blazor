using Microsoft.AspNetCore.Components.Server.Circuits;
using TaskSe.Data.Connection;
using TaskSe.Services.Interfaces.ANI;
using TaskSe.Services.Interfaces.Auditoria;
using TaskSe.Services.Interfaces.Configuracion.Perfilamiento;
using TaskSe.Services.Interfaces.FirmaDigital;
using TaskSe.Services.Interfaces.Mensajes;
using TaskSe.Services.Interfaces.Par;
using TaskSe.Services.Interfaces.Utilidades;
using TaskSe.Services.Services.ANI;
using TaskSe.Services.Services.Auditoria;
using TaskSe.Services.Services.Configuracion.Perfilamiento;
using TaskSe.Services.Services.FirmaDigital;
using TaskSe.Services.Services.Mensajes;
using TaskSe.Services.Services.Par;
using TaskSe.Services.Services.Utilidades;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Serilog;
using Serilog.Events;
using TaskSe.Web.Services;
using TaskSe.Web.Authentication;
using TaskSe.Web.Workers;
using TaskSe.Web.Helpers;
using TaskSe.Web.Authorization;


//Logs
string ruta_logger = Directory.GetCurrentDirectory() +
    "\\Log\\LogEventos_" + DateTime.Now.ToString("dd_MM_yyyy_hh_mm_ss") + ".txt";

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.File(ruta_logger)
    .WriteTo.Console()
    .WriteTo.Seq("http://localhost:5341/")
    .CreateLogger();

Log.Information("Starting up the application");


try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog();

    // Add services to the container.
    builder.Services.AddAuthenticationCore();
    builder.Services.AddRazorPages();
    builder.Services.AddServerSideBlazor()
        .AddHubOptions(options =>
        {
            options.ClientTimeoutInterval = TimeSpan.FromSeconds(60);
            options.HandshakeTimeout = TimeSpan.FromSeconds(30);
        });

    builder.Services.AddSingleton<ICircuitUserService, CircuitUserService>();
    builder.Services.AddScoped<CircuitHandler>((sp) => new CircuitHandlerService(sp.GetRequiredService<ICircuitUserService>()));


    //INYECCI�N DE DEPENDENCIAS - SERVICIOS

    //FUNDAMENTALES

    //Worker
    builder.Services.AddHostedService<Worker>();

    builder.Services.AddScoped<ProtectedSessionStorage>();
    builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
    builder.Services.AddScoped<AuthorizationService>();

    builder.Services.AddScoped<IAuditoriaService, AuditoriaService>();
    builder.Services.AddTransient<GooglereCaptchaService>();
    builder.Services.AddScoped(sp => new HttpClient());

    builder.Services.AddScoped<IEncryptService, EncryptService>();
    builder.Services.AddScoped<IEmailService, EmailService>();
    builder.Services.AddScoped<IFirmaDigitalService, FirmaDigitalService>();
    builder.Services.AddScoped<IMensajesService, MensajesService>();
    builder.Services.AddScoped<IUserService, UserService>();
    builder.Services.AddScoped<IModuloService, ModuloService>();
    builder.Services.AddScoped<IRolService, RolService>();
    builder.Services.AddScoped<IParService, ParService>();
    builder.Services.AddScoped<IParametroGeneralService, ParametroGeneralService>();
    builder.Services.AddScoped<IANIService, ANIService>();

    //DEFINICI�N DE LA CONEXI�N A BASE DE DATOS
    var sqlConnectionConfiguration = new SqlConfiguration(builder.Configuration.GetConnectionString("DefaultConnection"));
    builder.Services.AddSingleton(sqlConnectionConfiguration);

    IJSRuntimeExtension.setAuditoriaService(new AuditoriaService(sqlConnectionConfiguration));

    //CommandTimeout Dapper
    Dapper.SqlMapper.Settings.CommandTimeout = 10000;

    builder.Services.AddSignalR(e => {
        e.MaximumReceiveMessageSize = 102400000;
    });

    builder.Services.AddHsts(options =>
    {
        options.Preload = true;
        options.IncludeSubDomains = true;
        options.MaxAge = TimeSpan.FromDays(181);
    });


    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Error");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
    }


    app.Use(async (context, next) =>
    {
        context.Response.Headers.Add("X-Frame-Options", "SAMEORIGIN");
        context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
        context.Response.Headers.Add("Referrer-Policy", "same-origin");
        context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
        context.Response.Headers.Add("SameSite", "Strict");
        await next();
    });

    app.UseHttpsRedirection();
    app.UseHsts();

    app.UseStaticFiles();

    app.UseRouting();

    app.UseAuthentication();

    app.MapControllers();
    app.MapBlazorHub();
    app.MapFallbackToPage("/_Host");

    app.Run();
}
catch (Exception exe)
{
    Log.Fatal(exe, "There was a problem starting the application");
    return;
}
finally
{
    Log.CloseAndFlush();
}


