#r "System.Xml.Linq"
#r "Microsoft.WindowsAzure.Storage"

using System;
using System.Xml.Linq;
using System.Text;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

private static HttpClient client;

public static async Task Run(TimerInfo myTimer, TraceWriter log)
{
    log.Info($"C# Timer trigger function executed at: {DateTime.Now}");

    try
    {
        client = new HttpClient();
        var rssFeed = await client.GetStringAsync("http://neotech-solutions.fr/blogs?feed=rss");
        if (string.IsNullOrWhiteSpace(rssFeed))
        {
            log.Info("Impossible de lire le flux");
            return;
        }
        XDocument doc = XDocument.Parse(rssFeed);
        var query = from element in doc.Element("rss").Element("channel").Elements("item")
                    select new { Title = element.Element("title").Value, Date = Convert.ToDateTime(element.Element("pubDate").Value), Id = element.Element("guid").Value };

        var billets = query.OrderByDescending(x => x.Date).ToList();

        CloudStorageAccount storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=function4ccxxxx;AccountKey=yyy==");
        CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
        CloudTable table = tableClient.GetTableReference("billets");

        TableQuery<BilletEntity> allBlogsEntries = new TableQuery<BilletEntity>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "billets"));

        var entries = table.ExecuteQuery(allBlogsEntries).ToList();

        foreach (var billet in billets)
        {
            if (entries.Any(x => x.RowKey == GetHash(billet.Id)))
            {
                // déjà traité
            }
            else
            {
                string json = "{\"text\": \"Bonne nouvelle, un nouveau billet de blog a été publié.\\n" + billet.Id + ".\"}";
                await client.PostAsync("https://hooks.slack.com/services/xxx/yyy/zzz", new StringContent(json, Encoding.UTF8, "application/json"));

                BilletEntity entity = new BilletEntity(GetHash(billet.Id), billet.Id);
                TableOperation insertOperation = TableOperation.Insert(entity);
                table.Execute(insertOperation);
            }
        }
    }
    catch (Exception ex)
    {
        log.Info(ex.ToString());
    }
}

public static string GetHash(string value)
{
    return Convert.ToBase64String(Encoding.Default.GetBytes(value));
}

public class BilletEntity : TableEntity
{
    public BilletEntity(string id, string url)
    {
        PartitionKey = "billets";
        RowKey = id;
        Url = url;
    }

    public string Url { get; set; }

    public BilletEntity() { }
}