#r "Newtonsoft.Json"

#load "../Shared/Models/KuduWebHook.csx"
#load "../Shared/Helpers/HttpClientHelper.csx"

using System;
using System.Net;
using Newtonsoft.Json;

public static async Task<object> Run(HttpRequestMessage req, TraceWriter log)
{
    log.Info($"Webhook was triggered!");

    string teamsHookUri = req?.RequestUri?.ParseQueryString()?["teamshookuri"];
    if (string.IsNullOrWhiteSpace(teamsHookUri))
    {
        return req.CreateResponse(HttpStatusCode.BadRequest, new
        {
            error = "Invalid team hook uri passed"
        });
    }

    teamsHookUri = $"https://{teamsHookUri}";

    string jsonContent = await req.Content.ReadAsStringAsync();

    var data = JsonConvert.DeserializeObject<KuduWebHook>(jsonContent);

    if (!data.IsValid())
    {
        return req.CreateResponse(HttpStatusCode.BadRequest, new
        {
            error = "Invalid object passed"
        });
    }

    var messageCard = new
    {
        summary = $"{(data.success ? "Successfully published" : "Failed to publish")} {data.siteName}",
        title = "Azure app service publish",
        sections = new[] {
            new {
                activityTitle = $"{(data.success ? "Successfully published" : "Failed to publish")} {data.siteName}",
                activitySubtitle = "using Kudu",
                activityText = "Details",
                activityImage = data.success
                                    ? "https://cloud.githubusercontent.com/assets/1647294/21986014/cf83fdb0-dbfd-11e6-8d18-617b0fd17597.png"
                                    : "https://cloud.githubusercontent.com/assets/1647294/21986029/ec3256a0-dbfd-11e6-9300-f183681cee85.png",
                facts = new [] {
                    new { name ="Author", value = $"{data.author} <{data.authorEmail}> ({data.deployer})" },
                    new { name ="Message", value = $"{data.message}".Replace("\r", " ").Replace("\n", " ").Replace("  ", " ") },
                    new { name ="Duration", value = $"{data.endTime - data.startTime} ({data.startTime} - {data.endTime})"}
                }
            }
        }
    };

    var statusCode = await PostObjectAsJsonAsync(teamsHookUri, messageCard);

    return req.CreateResponse(statusCode, messageCard);
}
