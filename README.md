<h1>Apprenda-NewRelic-Integration</h1>

Enhance applications running on Apprenda’s private platform as a service (PaaS) with New Relic monitoring using bootstrap policies. The New Relic dashboard surfaces thousands of metrics and key performance indicators (KPIs) from your apps. New Relic gives Apprenda developers insight into how end-users are interacting with the app and the experience they receive throughout the day.

With New Relic monitoring Apprenda applications, developers can:
- Monitor multi-instance distributed applications in real time
- Monitor how the application is interacting with the database and gain access to query latencies
- Monitor performance metrics and the overall health of an application
- Monitor Key Performance Indicators (KPIs) and define custom metrics that enable insight into the internal behavior of the application
- Monitor how end users are using an app, locations from which they are accessing the app, latency in serving content, and what browsers users are using
- Diagnose bottlenecks in an application
- Receive proactive alerts when the application is falling behind its SLA


This repository contains the bootstrap policy code needed to integrate APM from New Relic with Apprenda Applications

<h2>Requirements</h2>
- Custom Properties "APM Application Name" and "APM Subscription ID" exist on each Apprenda Application
- NewRelic Agent is installed with full instrumentation capabilities for IIS and non-IIS .NET applications on every Apprenda platform node that can host applications. The global New Relic config file should not disable the instrumentation of all application pools.
- All .config files that have an appSettings section will be modified with the NewRelic config (appSettings section has to pre-exist in the .config file without an XML namespace)

<h2>Future Changes</h2>
- Ability to tell the bootstrap policy exactly which .config files to instrument for New Relic through a Custom Property

<h2>Additional Information</h2>
Enhancing Apprenda applications to work with your standardized corporate-wide APM tool is simple through the power of Apprenda extensibility and bootstrap policies. To learn more about bootstrap policies, visit the following links:
- http://docs.apprenda.com/5-5/bootstrap-policies
- http://docs.apprenda.com/5-5/custom-bootstrapper
- http://apprenda.com/blog/customize-paas-application-bootstrap-policies/

To get a better understanding of this integration, visit the following link that includes documentation and videos that guide you step-by-step on this integration.
- http://apprenda.com/partners/integrations/new-relic/
- http://newrelic.com/connect/details#apprenda
- https://apprenda.wistia.com/projects/8pal4dozci
- http://apprenda.com/blog/apprenda-teams-new-relic-monitor-applications-enterprise-paas/
