using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using WebDiffy.UI.Infrastructure.Options;

namespace WebDiffy.UI.Services;

public class BaseService
{
    private const string JsonMediaType = "application/json";

    private readonly ApiOptions _options;

    public readonly string BaseUrl;

    public BaseService(IOptions<ApiOptions> options)
    {
        _options = options.Value;
        BaseUrl = _options.Url;
    }

    protected HttpRequestMessage BuildPostRequestMessage(string url, object content)
    {
        return BuildRequestMessageWithBody(HttpMethod.Post, url, content);
    }
    
    protected HttpRequestMessage BuildPutRequestMessage(string url, object content)
    {
        return BuildRequestMessageWithBody(HttpMethod.Put, url, content);
    }

    protected HttpRequestMessage BuildGetRequestMessage(string baseUrl,
        Dictionary<string, string> queryParams)
    {
        return new HttpRequestMessage(HttpMethod.Get, BuildUrl(baseUrl, queryParams));
    }

    protected HttpRequestMessage BuildDeleteRequestMessage(string baseUrl,
        Dictionary<string, string> queryParams)
    { 
        return new HttpRequestMessage(HttpMethod.Delete, BuildUrl(baseUrl, queryParams));
    }
    
    private static HttpRequestMessage BuildRequestMessageWithBody(HttpMethod method, string url, object content)
    {
        var message = new HttpRequestMessage(method, url);
        message.Content = new StringContent(JsonSerializer.Serialize(content), Encoding.UTF8);
        message.Content.Headers.ContentType = new MediaTypeHeaderValue(JsonMediaType);

        return message;
    }
    
    private static string BuildUrl(string baseUrl, Dictionary<string, string> queryParams)
    {
        var uriBuilder = new UriBuilder(baseUrl);
        uriBuilder.Query = string.Join("&", queryParams.Select(kvp => $"{kvp.Key}={kvp.Value}"));

        return uriBuilder.Uri.AbsoluteUri;
    }
}
