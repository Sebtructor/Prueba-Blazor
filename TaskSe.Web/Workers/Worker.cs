using TaskSe.Data.Connection;
using TaskSe.Model.Models.FirmaDigital;
using Dapper;
using System.Data.SqlClient;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace TaskSe.Web.Workers
{
    public class Worker : BackgroundService
    {
        private readonly SqlConfiguration _sqlConfiguration;
        private readonly ILogger<Worker> _logger;

        public Worker(
            ILogger<Worker> logger,
            SqlConfiguration sqlConfiguration)
        {
            _logger = logger;
            _sqlConfiguration = sqlConfiguration;
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("The service has been started...");

            try
            {

            }
            catch (Exception exe)
            {
                _logger.LogError(exe, "Error al obtener configuraciones");
            }

            await base.StartAsync(cancellationToken);
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("The service has been stopped...");

            await base.StopAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    int proxima_ejecucion = Convert.ToInt32(calcularProximaEjecucion());
                    DateTime fecha_proxima_ejecucion = DateTime.Now.AddMilliseconds(proxima_ejecucion);
                    _logger.LogInformation($"La próxima ejecución del worker se realizará dentro de {proxima_ejecucion} milisegundos en la fecha {fecha_proxima_ejecucion.ToString()}");

                    await Task.Delay(proxima_ejecucion, stoppingToken);

                    await iniciarTareas();
                }

            }
            catch (Exception exe)
            {
                _logger.LogError(exe, "Error al ejecutar tarea de workers");
            }
        }

        private double calcularProximaEjecucion()
        {
            double intervalo = 0;

            int intervaloEjecucion = 1;
            int diasEjecucion = 1;
            int hora_actual = DateTime.Now.Hour;

            //Se ejecuta en el mismo día
            if (hora_actual < intervaloEjecucion)
            {
                intervalo = (DateTime.Now.Date.AddHours(intervaloEjecucion) - DateTime.Now).TotalMilliseconds;

                if (intervalo > 0) return intervalo;
            }

            //Se ejecuta al día siguiente
            intervalo = (DateTime.Now.AddDays(diasEjecucion).Date.AddHours(intervaloEjecucion) - DateTime.Now).TotalMilliseconds;

            return intervalo;

        }

        private async Task iniciarTareas()
        {
            _logger.LogInformation("Inicio de ejecución de tareas worker");

            try
            {
                await inactivarUsuarios();
            }
            catch (Exception exe)
            {
                _logger.LogError(exe, "Error al ejecutar tareas worker");
            }

            _logger.LogInformation("Fin de ejecución de tareas worker");
        }

        private SqlConnection dbConnection()
        {
            return new SqlConnection(_sqlConfiguration.ConnectionString);
        }

        private async Task inactivarUsuarios()
        {
            var result = 0;
            var db = dbConnection();

            var sql = @"UPDATE [SEG].[SEG_USUARIO]
                            SET
                            ESTADO = '2'
                            WHERE ESTADO = '1' 
                            AND
                            USUARIO IN
                            (
	                            SELECT 
	                            Q.USUARIO
	                            FROM
	                            (
		                            SELECT 
		                            [USUARIO]
		                            ,FECHA_ULTIMO_INGRESO =
		                            CASE
		                            WHEN (
			                            SELECT TOP (1) FECHA FROM SEG.AUDITORIA_LOGIN_USUARIO A 
			                            WHERE DESCRIPCION IN
			                            (
			                            'Exitoso',
			                            'Exitoso, ingreso por RememberMe entrando a pantalla de login'
			                            )
			                            AND A.USUARIO = U.USUARIO
			                            ORDER BY FECHA DESC
		                            ) IS NOT NULL THEN (
			                            SELECT TOP (1) FECHA FROM SEG.AUDITORIA_LOGIN_USUARIO A 
			                            WHERE DESCRIPCION IN
			                            (
			                            'Exitoso',
			                            'Exitoso, ingreso por RememberMe entrando a pantalla de login'
			                            )
			                            AND A.USUARIO = U.USUARIO
			                            ORDER BY FECHA DESC
		                            )
		                            ELSE U.FECHA_ADICION
		                            END
		                            FROM [SEG].[SEG_USUARIO] U
	                            ) AS Q
	                            WHERE DATEDIFF(day,FECHA_ULTIMO_INGRESO,getdate()) >= 90
                            )";

            result = await db.ExecuteAsync(sql.ToString());

        }
    }
}
