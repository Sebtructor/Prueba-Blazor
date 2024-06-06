using TaskSe.Data.Connection;
using TaskSe.Services.Interfaces.Utilidades;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TaskSe.Data.Repositories.Interfaces.Auditoria;
using TaskSe.Data.Repositories.Repositories.Auditoria;
using System.Net.Mime;
using TaskSe.Services.Interfaces.Auditoria;
using iTextSharp.text.log;
using Microsoft.Extensions.Logging;
using NPOI.SS.Formula.Functions;

namespace TaskSe.Services.Services.Utilidades
{
    public class EmailService : IEmailService
    {
        private readonly IAuditoriaService _auditoriaService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(
            IConfiguration configuration,
            IAuditoriaService auditoriaService,
            ILogger<EmailService> logger)
        {
            _auditoriaService = auditoriaService;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<bool> enviarCorreo(string destinatario, string cc, string asunto, string mensaje, 
            string pantalla, string concepto, string numero_identificacion, List<string> anexos)
        {
            Boolean resultado = false;

            if (!string.IsNullOrEmpty(destinatario))
            {
                try
                {
                    string email_from = _configuration.GetSection("AppSettings:EmailFrom").Value;
                    string email_bcc = _configuration.GetSection("AppSettings:EmailBCC").Value;
                    string email_host = _configuration.GetSection("AppSettings:EmailHost").Value;
                    string email_port = _configuration.GetSection("AppSettings:EmailPort").Value;
                    string email_ssl = _configuration.GetSection("AppSettings:EmailSSLEnabled").Value;
                    string email_pass = _configuration.GetSection("AppSettings:EmailPass").Value;

                    MailMessage emailsend = new MailMessage();
                    emailsend.To.Add(new MailAddress(destinatario));
                    emailsend.From = new MailAddress(email_from);
                    emailsend.Bcc.Add(new MailAddress(email_bcc));
                    if (!string.IsNullOrEmpty(cc))
                    {
                        emailsend.CC.Add(cc);
                    }
                    emailsend.Subject = asunto;
                    emailsend.IsBodyHtml = true;
                    emailsend.Body = mensaje;
                    emailsend.IsBodyHtml = true;
                    emailsend.Priority = MailPriority.Normal;


                    //ANEXOS
                    if (anexos != null)
                    {
                        if (anexos.Count > 0)
                        {
                            foreach(var anexo in anexos)
                            {
                                if (File.Exists(anexo))
                                {
                                    emailsend.Attachments.Add(new Attachment(anexo, MediaTypeNames.Application.Octet));
                                }
                            }
                        }
                    }

                    SmtpClient smtp = new SmtpClient(email_host);
                    smtp.Host = email_host;
                    smtp.Port = int.Parse(email_port);
                    smtp.EnableSsl = Convert.ToBoolean(email_ssl);
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new NetworkCredential(email_from, email_pass);

                    //INTENTOS DE ENVIO DE CORREO
                    Boolean enviado = false;
                    int intentos = 0;
                    while (!enviado && intentos < 3)
                    {
                        try
                        {
                            smtp.Send(emailsend);
                            enviado = true;
                            emailsend.Dispose();

                            await _auditoriaService.mtdAuditoriaEnvioCorreo(destinatario, email_from, mensaje, "Si", "", pantalla, concepto, numero_identificacion, "");
                            resultado = true;

                            _logger.LogInformation ($"Correo enviado correctamente; Destinatario {destinatario}, Email De: {email_from}, Pantalla: {pantalla}, Concepto: {concepto}, Numero de Identificación {numero_identificacion}");
                        }
                        catch (Exception exe)
                        {
                            _logger.LogError(exe,$"Error al enviar correo; Destinatario {destinatario}, Email De: {email_from}, Pantalla: {pantalla}, Concepto: {concepto}, Numero de Identificación {numero_identificacion}");

                            await _auditoriaService.mtdAuditoriaEnvioCorreo(destinatario, email_from, mensaje, "No", exe.Message + " " + exe.InnerException, pantalla, concepto, numero_identificacion, "");

                        }
                        Thread.Sleep(5000);
                        intentos++;
                    }
                }
                catch (Exception exe)
                {
                    _logger.LogError(exe, $"Error al enviar correo; Destinatario {destinatario}, Email De: , Pantalla: {pantalla}, Concepto: {concepto}, Numero de Identificación {numero_identificacion}");
                }
            }

            return resultado;
        }

        public async Task<bool> enviarCorreoVariosDestinatarios(string destinatarios, string cc, string asunto, string mensaje,
            string pantalla, string concepto, string numero_identificacion, List<string> anexos)
        {
            Boolean resultado = false;

            if (!string.IsNullOrEmpty(destinatarios))
            {
                try
                {
                    string email_from = _configuration.GetSection("AppSettings:EmailFrom").Value;
                    string email_bcc = _configuration.GetSection("AppSettings:EmailBCC").Value;
                    string email_host = _configuration.GetSection("AppSettings:EmailHost").Value;
                    string email_port = _configuration.GetSection("AppSettings:EmailPort").Value;
                    string email_ssl = _configuration.GetSection("AppSettings:EmailSSLEnabled").Value;
                    string email_pass = _configuration.GetSection("AppSettings:EmailPass").Value;

                    MailMessage emailsend = new MailMessage();

                    //Destinatarios
                    foreach (var destinatario in destinatarios.Split(";"))
                    {
                        emailsend.To.Add(new MailAddress(destinatario));
                    }
                    emailsend.From = new MailAddress(email_from);
                    emailsend.Bcc.Add(new MailAddress(email_bcc));
                    if (!string.IsNullOrEmpty(cc))
                    {
                        emailsend.CC.Add(cc);
                    }
                    emailsend.Subject = asunto;
                    emailsend.IsBodyHtml = true;
                    emailsend.Body = mensaje;
                    emailsend.IsBodyHtml = true;
                    emailsend.Priority = MailPriority.Normal;


                    //ANEXOS
                    if (anexos != null)
                    {
                        if (anexos.Count > 0)
                        {
                            foreach (var anexo in anexos)
                            {
                                if (File.Exists(anexo))
                                {
                                    emailsend.Attachments.Add(new Attachment(anexo, MediaTypeNames.Application.Octet));
                                }
                            }
                        }
                    }

                    SmtpClient smtp = new SmtpClient(email_host);
                    smtp.Host = email_host;
                    smtp.Port = int.Parse(email_port);
                    smtp.EnableSsl = Convert.ToBoolean(email_ssl);
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new NetworkCredential(email_from, email_pass);

                    //INTENTOS DE ENVIO DE CORREO
                    Boolean enviado = false;
                    int intentos = 0;
                    while (!enviado && intentos < 3)
                    {
                        try
                        {
                            smtp.Send(emailsend);
                            enviado = true;
                            emailsend.Dispose();

                            await _auditoriaService.mtdAuditoriaEnvioCorreo(destinatarios, email_from, mensaje, "Si", "", pantalla, concepto, numero_identificacion, "");

                            resultado = true;

                            _logger.LogInformation($"Correo enviado correctamente; Destinatario {destinatarios}, Email De: {email_from}, Pantalla: {pantalla}, Concepto: {concepto}, Numero de Identificación {numero_identificacion}");
                        }
                        catch (Exception exe)
                        {
                            _logger.LogError(exe, $"Error al enviar correo; Destinatario {destinatarios}, Email De: {email_from}, Pantalla: {pantalla}, Concepto: {concepto}, Numero de Identificación {numero_identificacion}");

                            await _auditoriaService.mtdAuditoriaEnvioCorreo(destinatarios, email_from, mensaje, "No", exe.Message + " " + exe.InnerException, pantalla, concepto, numero_identificacion, "");

                        }
                        Thread.Sleep(5000);
                        intentos++;
                    }
                }
                catch (Exception exe)
                {
                    _logger.LogError(exe, $"Error al enviar correo; Destinatario {destinatarios}, Email De: , Pantalla: {pantalla}, Concepto: {concepto}, Numero de Identificación {numero_identificacion}");
                }
            }

            return resultado;
        }
    }
}
