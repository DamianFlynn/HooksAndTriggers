#r "Newtonsoft.Json"
using System.Net;
using System.Net.Http.Headers;
using Newtonsoft.Json;

public static async Task<HttpStatusCode> PostObjectAsJsonAsync<T>(string uri, T value)
{
    var json = JsonConvert.SerializeObject(
        value,
        Formatting.Indented
    );
    
    return await UseHttpClient(
        client => client.PostAsync(
            uri,
            new StringContent(json, System.Text.Encoding.UTF8, "application/json")
        )
    );
}

private static async Task<HttpStatusCode> UseHttpClient(Func<HttpClient, Task<HttpResponseMessage>> action)
{
    using (var client = new HttpClient())
    {
        client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("KuduWebHook","1.0.0"));

        return (await action?.Invoke(client))?.StatusCode ?? HttpStatusCode.PreconditionFailed;
    }
}
