Apprenda-NewRelic-Integration
=============================

This contains the bootstrap policy code needed to integrate APM from New Relic with Apprenda Applications

**Requirements**
<ol> Custom Properties "APM Application Name" and "APM Subscription ID" exist on each Apprenda Application
<ol> NewRelic Agent is installed with full instrumentation capabilities for IIS and non-IIS .NET applications on every Apprenda platform node that can host applications. The global New Relic config file should not disable the instrumentation of all application pools.
<ol> All *.config files that have an appSettings section will be modified with the NewRelic config (appSettings section has to pre-exist in the .config file without an *XML namespace*)

**Future Changes**
<ol> Ability to tell the bootstrap policy exactly which .config files to instrument for New Relic through a Custom Property

Enhancing Apprenda applications to work with your standardized corporate-wide APM tool is simple through the power of Apprenda extensibility and bootstrap policies. To learn more about bootstrap policies, visit the following links:
<ol> http://docs.apprenda.com/5-5/bootstrap-policies
<ol> http://docs.apprenda.com/5-5/custom-bootstrapper
<ol> http://apprenda.com/blog/customize-paas-application-bootstrap-policies/

To get a better understanding of this integration, view the following videos
<ol> https://apprenda.wistia.com/medias/t57au22x70
<ol> <tbd>
