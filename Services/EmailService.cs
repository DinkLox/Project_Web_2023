using Microsoft.Extensions.Options;
using WEB_2023.Config;
using RestSharp;
using RestSharp.Authenticators;

namespace WEB_2023.Services
{
    public interface IEmailService
    {
        Task<IRestResponse> SendEmailAsync(string email, string subject, string templateName, string mailGunVariables);
    }
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;
        public EmailService(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }
        public Task<IRestResponse> SendEmailAsync(string email, string subject, string templateName, string mailGunVariables)
        {
            RestClient client = new RestClient
            {
                BaseUrl = new Uri(_emailSettings.ApiBaseUrl),
                Authenticator = new HttpBasicAuthenticator("api", _emailSettings.ApiKey)
            };
            RestRequest request = new RestRequest();
            request.AddParameter("domain", _emailSettings.RequestUrl, ParameterType.UrlSegment);
            request.Resource = "{domain}/messages";
            request.AddParameter("from", $"{_emailSettings.DisplayName} <{_emailSettings.From}>");
            request.AddParameter("to", email);
            request.AddParameter("subject", subject);
            request.AddParameter("template", templateName);
            request.AddParameter("h:X-Mailgun-Variables", mailGunVariables);
            request.Method = Method.POST;
            return Task.FromResult(client.Execute(request));
        }
    }
}