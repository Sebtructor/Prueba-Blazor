
using RestSharp;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskSe.Services.Utilidades
{
    public class UtilidadesOTP
    {
        public static string enviarWPP(string destinatario, string link, string preview)
        {
            string respuesta = "";

            if (!string.IsNullOrEmpty(destinatario))
            {
                try
                {
                    var client = new RestClient("https://valid.excellentiam.co:9078/send-link-message");
                    client.Timeout = 20000;
                    var request = new RestRequest(Method.POST);
                    request.AddHeader("authKey", "8289B9EB-D2A1-4049-B7F5-7A7335FB3CD5");
                    request.AddHeader("Content-Type", "application/json");

                    var body = @"{
                        " + "\n" +
                                        @"    ""number"":""+57" + destinatario + @""",
                        " + "\n" +
                                        @"    ""link"":""" + link + @""",
                        " + "\n" +
                                        @"    ""preview"":""""
                        " + "\n" +
                        @"}";


                    request.AddParameter("application/json", body, ParameterType.RequestBody);
                    IRestResponse response = client.Execute(request);

                    respuesta = response.StatusCode.ToString();

                    Log.Information($"Envío de Mensaje wpp: Estado {respuesta}, Destinatario {destinatario}");
                }
                catch (Exception ex)
                {
                    Log.Error(ex, $"Error al enviar mensaje wpp a {destinatario}");

                }
            }

            return respuesta;
        }

        public static string enviarWPP(string destinatario, string mensaje)
        {
            string respuesta = "";

            if (!string.IsNullOrEmpty(destinatario))
            {
                try
                {
                    var client = new HttpClient();
                    var request = new HttpRequestMessage(HttpMethod.Post, "https://graph.facebook.com/v18.0/111109975281154/messages");
                    request.Headers.Add("Authorization", "Bearer EAACjovydOgcBAKKTRhh7kuS9GIwZB9VZBWCuhzH4MTIbZCHFZCMGCz2ev7lmTRSSMRJqZC8LZAZCUMJ6vWMRFscWai9LZCnTyWTNIz6TrV5YIS80PFPTsMZAd3sZAyHgecT20HAwzcAsDaWkr27S2YG3U0mUBQtsGigLwNpZCn7wudHxDZCTBSQsA4c4");
                    string body_content = @"{
                        ""messaging_product"": ""whatsapp"",
                        ""recipient_type"": ""individual"",
                        ""to"": ""57" + destinatario + @""",
                        ""type"": ""template"",
                        ""template"": {
                            ""name"": ""otp"",
                            ""language"": {
                                ""code"": ""es""
                            },
                            ""components"": [
                                {
                                    ""type"": ""body"",
                                    ""parameters"": [
                                        {
                                            ""type"": ""text"",
                                            ""text"": """ + mensaje + @"""
                                        }
                                    ]
                                }
                            ]
                        }
                    }";
                    var content = new StringContent(body_content, null, "application/json");
                    request.Content = content;
                    var response = client.Send(request);
                    response.EnsureSuccessStatusCode();
                    respuesta = response.StatusCode.ToString();

                    Log.Information($"Envío de Mensaje wpp: Estado {respuesta}, Destinatario {destinatario}, Mensaje: {mensaje}");
                }
                catch (Exception ex)
                {
                    Log.Error(ex, $"Error al enviar mensaje wpp a {destinatario}");

                }
            }

            return respuesta;
        }

        public static string enviarSMS(string destinatario, string mensaje)
        {
            string respuesta = "";

            if (!string.IsNullOrEmpty(destinatario))
            {
                try
                {
                    var clientSMS = new RestClient("https://gtw.nrsgateway.com/rest/message");
                    clientSMS.Timeout = 20000;
                    var requestSMS = new RestRequest(Method.POST);
                    requestSMS.AddHeader("Content-Type", "application/json");
                    requestSMS.AddHeader("Accept", "application/json");
                    requestSMS.AddHeader("Authorization", "Basic RGVzYXJyb2xsb0V4YzpFeGNlbGxlbnRpYW0yMDIxKg==");
                    requestSMS.AddParameter("application/json", "{\"to\":[\"57" + destinatario + "\"],\"text\":\"" + mensaje + "\",\"from\":\"ESE\",\"parts\":\"1\",\"trsec\":\"1\"}", ParameterType.RequestBody);

                    IRestResponse responseSMS = clientSMS.Execute(requestSMS);
                    respuesta = responseSMS.StatusCode.ToString();

                    Log.Information($"Envío de Mensaje sms: Estado {respuesta}, Destinatario {destinatario}, Mensaje: {mensaje}");
                }
                catch (Exception exe)
                {
                    Log.Error(exe, $"Error al enviar mensaje sms a {destinatario}");
                }
            }

            return respuesta;
        }


    }
}
