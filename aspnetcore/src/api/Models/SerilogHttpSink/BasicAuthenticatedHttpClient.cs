using Microsoft.Extensions.Configuration;
using Serilog.Sinks.Http;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Mydatalogging;

/*
 * Custom HttpClient to be used with Serilog HTTP sink.
 * This client enables use of HTTP basic authentication.
 * Source:
 * https://github.com/FantasticFiasco/serilog-sinks-http/wiki/Custom-HTTP-client
 */
public class BasicAuthenticatedHttpClient : IHttpClient
{
    private readonly HttpClient httpClient;

    public BasicAuthenticatedHttpClient()
    {
        httpClient = new HttpClient();
    }

    public void Configure(IConfiguration configuration)
    {
        var username = configuration["LogServer:username"];
        var password = configuration["LogServer:password"];

        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
          "Basic",
          Convert.ToBase64String(Encoding.UTF8.GetBytes($"{username}:{password}")));
    }

    public async Task<HttpResponseMessage> PostAsync(string requestUri, Stream contentStream)
    {
        using var content = new StreamContent(contentStream);
        content.Headers.Add("Content-Type", "application/json");

        var response = await httpClient
          .PostAsync(requestUri, content)
          .ConfigureAwait(false);

        return response;
    }

    public void Dispose() => httpClient?.Dispose();
}