<h1>Apprenda-NewRelic-Integration</h1>

This contains the bootstrap policy code needed to integrate APM from New Relic with Apprenda Applications

<h2>Requirements</h2>
<li> Custom Properties "APM Application Name" and "APM Subscription ID" exist on each Apprenda Application
<li> NewRelic Agent is installed with full instrumentation capabilities for IIS and non-IIS .NET applications on every Apprenda platform node that can host applications. The global New Relic config file should not disable the instrumentation of all application pools.
<li> All .config files that have an appSettings section will be modified with the NewRelic config (appSettings section has to pre-exist in the .config file without an XML namespace)

<h2>Future Changes</h2>
<li> Ability to tell the bootstrap policy exactly which .config files to instrument for New Relic through a Custom Property

Enhancing Apprenda applications to work with your standardized corporate-wide APM tool is simple through the power of Apprenda extensibility and bootstrap policies. To learn more about bootstrap policies, visit the following links:
<li> http://docs.apprenda.com/5-5/bootstrap-policies
<li> http://docs.apprenda.com/5-5/custom-bootstrapper
<li> http://apprenda.com/blog/customize-paas-application-bootstrap-policies/

To get a better understanding of this integration, view the following videos
<li> https://apprenda.wistia.com/medias/t57au22x70
<li> TBD
