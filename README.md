<h1>Apprenda-NewRelic-Integration</h1>

This contains the bootstrap policy code needed to integrate APM from New Relic with Apprenda Applications

<h2>Requirements</h2>
-  Custom Properties "APM Application Name" and "APM Subscription ID" exist on each Apprenda Application
- NewRelic Agent is installed with full instrumentation capabilities for IIS and non-IIS .NET applications on every Apprenda platform node that can host applications. The global New Relic config file should not disable the instrumentation of all application pools.
- All .config files that have an appSettings section will be modified with the NewRelic config (appSettings section has to pre-exist in the .config file without an XML namespace)

<h2>Future Changes</h2>
- Ability to tell the bootstrap policy exactly which .config files to instrument for New Relic through a Custom Property

Enhancing Apprenda applications to work with your standardized corporate-wide APM tool is simple through the power of Apprenda extensibility and bootstrap policies. To learn more about bootstrap policies, visit the following links:
- http://docs.apprenda.com/5-5/bootstrap-policies
- http://docs.apprenda.com/5-5/custom-bootstrapper
- http://apprenda.com/blog/customize-paas-application-bootstrap-policies/

To get a better understanding of this integration, view the following videos
- https://apprenda.wistia.com/medias/t57au22x70
- TBD
