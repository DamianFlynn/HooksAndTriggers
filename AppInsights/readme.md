# AppInsights

Functions by default generate plenty of logs which you can view in the portal or download using the Kudu REST API. This is great and it comes out of the box without you having to configure a single thing.

![AppInsights Splash](..\images\function-appinsights.png)

However, what if you have some custom, complex logic that you want to capture and log. And I guess that in most instances there will be more than one Function per AppService and potentially a lot more applications and services deployed on Azure. So how to do manage the logs for all these. How do you consolidate and how do you make sense of all the telemetry?

Well Azure's got your back with Application Insights(AppInsights). AppInsights started as a standalone tool that developers would opt-in (some times) to get useful telemetry from their applications. However, fast-forward to today and AppInsights has become an invaluable tool for developers and support staff alike. AppInsights helps you triage, prevent and react to issues, performance problems, crashes, availability etc. Yes, there are other products in the market but there's one thing they miss. Full integration with all your Azure services and products! And, of course, managing all these events in one central place, your AppInsights dashboard.

It's only logical then that our Azure Functions should take advantage of this service. A few reasons you should consider AppInsights:
* monitor performance issues
* exception tracking
* custom events
* analytics

Again, you could easily achieve this with other tools, but AppInsights brings all your logging under one central dashboard. DevOps becomes a lot easier with a centralized command post and don't forget the extremely powerful analytics which are part of the service!

> Note: This blog post will soon may be redundant because the team has plans to make AppInsights a first-class citizen and full integrated into the service. Until then, this post can fill in the gap.
Integrate AppInsights (.NET) with you Azure Function

1. Create an AppInsights Instance

   If you don't have AppInsights running on Azure already, you can spin an new one up from the portal. It should take a few minutes to create. Once up and running, navigate to the Properties blade and copy the Instrumentation Key

   ![AppInsights Instrumentation Key](..\images\function-appinsights-2.png)

The InstrumentationKey is required to instantiate a TelemetryClient and is used to capture your events and send them to your unique instance of AppInsights. This key can be shared by many applications and Functions. Remember that your AppInsights is your central dashboard. This key is your common point of reference. 