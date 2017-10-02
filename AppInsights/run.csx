using System;
using Microsoft.ApplicationInsights;
using System.Threading;

public static void Run(TimerInfo myTimer, TraceWriter log)
{
    var appInsights = GetTelemetryClient();
    //track an event
    appInsights.TrackEvent("I am starting now. I'm timer triggered");
    // track a numeric value
    appInsights.TrackMetric("Ticks based on current time", DateTime.Now.Ticks);
    // track an exception
    appInsights.TrackException(new Exception($"Random exception at {DateTime.Now}"));
    // track long running external service call
    RunSlowDependencyCall(appInsights);

    // send data to azure
    appInsights.Flush();
    log.Info($"C# Timer trigger function executed at: {DateTime.Now}");
}

private static TelemetryClient GetTelemetryClient()
{
    var telemetryClient = new TelemetryClient();
    telemetryClient.InstrumentationKey = "<your appInsight Instrumentation key>";
    return telemetryClient;
}

private static void RunSlowDependencyCall(TelemetryClient appInsights)
{
    var success = false;
    var startTime = DateTime.UtcNow;
    var timer = System.Diagnostics.Stopwatch.StartNew();
    try
    {
        Thread.Sleep(2000);
        success = true;
    }
    finally
    {
        timer.Stop();
        appInsights.TrackDependency("myDependency", "myCall", startTime, timer.Elapsed, success);
    }
}