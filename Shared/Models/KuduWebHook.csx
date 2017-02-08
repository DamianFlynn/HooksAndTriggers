public class KuduWebHook
{
    public string id { get; set; }
    public string status { get; set; }
    public string statusText { get; set; }
    public string authorEmail { get; set; }
    public string author { get; set; }
    public string message { get; set; }
    public string progress { get; set; }
    public string deployer { get; set; }
    public DateTimeOffset receivedTime { get; set; }
    public DateTimeOffset startTime { get; set; }
    public DateTimeOffset endTime { get; set; }
    public DateTimeOffset lastSuccessEndTime { get; set; }
    public bool complete { get; set; }
    public string siteName { get; set; }
    public string hostName { get; set; }
    public bool success => StringComparer.OrdinalIgnoreCase.Equals(status, "success");
    public bool IsValid()
    {
        return  !string.IsNullOrEmpty(id) &&
                !string.IsNullOrEmpty(status) &&
                !string.IsNullOrEmpty(hostName);
    }
}
