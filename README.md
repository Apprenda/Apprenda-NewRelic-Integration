Apprenda-NewRelic-Integration
=============================

This contains the bootstrap policy code needed to integrate APM from New Relic with Apprenda Applications

**Requirements**
1. Custom Property "APM Application Name" exists on each Apprenda Application
2. NewRelic Agent is installed with full instrumentation capabilities for IIS and non-IIS .NET applications on every Apprenda platform node that can host applications
3. All *.config files that have an appSettings section will be modified with the NewRelic config (appSettings section has to pre-exist in the .config file)

**Future Changes**
1. Ability to create a custom property with the NewRelic Subscription ID and feed that into a custom newrelic.config that is placed in each application's folder
