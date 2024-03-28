using ML_UI_App.LogService;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using static Serilog.Events.LogEventLevel;

namespace ML_UI_App.ConnectionService
{
    internal class HttpService
    {
        static readonly HttpClient httpClient = new();
        static readonly string httpHost = "http://localhost:8000/";
        public static async Task<bool> GetStatus()
        {
            try
            {
                using HttpRequestMessage request = new(HttpMethod.Get, httpHost + "health");
                using HttpResponseMessage response = await httpClient.SendAsync(request);

                string content = await response.Content.ReadAsStringAsync();
                Logger.LogByTemplate(Information, note: "Запрос [GET] серверу");

                return response.StatusCode == HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                Logger.LogByTemplate(Error, ex, "Ошибка GET запроса");
                return false;
            }
        }

        public static async Task<string> GetPictureJSONAsync(string filePath)
        {
            if (await GetStatus() == true)
            {
                try
                {
                    using var multipartFormContent = new MultipartFormDataContent();
                    byte[] fileToBytes = await File.ReadAllBytesAsync(filePath);
                    var content = new ByteArrayContent(fileToBytes);
                    content.Headers.ContentType = new MediaTypeHeaderValue("image/png");
                    multipartFormContent.Add(content, name: "image", fileName: "metamen.png");

                    using var response = await httpClient.PostAsync(httpHost + "file", multipartFormContent);
                    var responseText = await response.Content.ReadAsStringAsync();

                    return responseText;
                }
                catch (Exception ex)
                {
                    Logger.LogByTemplate(Error, ex, "Ошибка получения информации о картинке");
                    return "error";
                }
            }
            else
            {
                Logger.LogByTemplate(Warning, note:"Обращение к FastAPI без соединения");
                return "error";
            }
            
        }
    }
}
