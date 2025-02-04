using CinemaTicketingSystem.Application.ExternalServices;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace CinemaTicketingSystem.Infrastructure.Services
{
    public class GoogleReCaptchaValidator : IReCaptchaValidator
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public GoogleReCaptchaValidator(IConfiguration configuration ,HttpClient httpClient)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }
        public bool ValidateReCaptcha(string response)
        {
            var secret = _configuration["GoogleReCaptcha:SecretKey"];
            var client = new System.Net.Http.HttpClient();
            var result = client.PostAsync(
                $"https://www.google.com/recaptcha/api/siteverify?secret={secret}&response={response}",
                null).Result;

            var json = result.Content.ReadAsStringAsync().Result;
            dynamic data = Newtonsoft.Json.JsonConvert.DeserializeObject(json);

            return data.success == true;
        }
    }
}
