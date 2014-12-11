using Apprenda.API.Extension.Bootstrapping;
using Apprenda.API.Extension.CustomProperties;
using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace APMBootstrapper
{
    class ApplicationMonitoringBootstrapper : Apprenda.API.Extension.Bootstrapping.BootstrapperBase
    {
        const string NewRelicApplicationPoolTemplate = @"<!--<applicationPool name=""$#IISAPPLICATIONNAME#$"" instrument=""true""/>-->";
        const string NewRelicApplicationPoolValue = @"<applicationPool name=""$#IISAPPLICATIONNAME#$"" instrument=""true""/>";
        /// <summary>
        /// If a Custom Property "APM Application Name" exists, open up the application and add the New Relic
        /// specific configuration inside its *.config files
        /// </summary>
        public override BootstrappingResult Bootstrap(BootstrappingRequest bootstrappingRequest)
        {
            // to get here, it means that the bootstrap policy matched the requirements of an application and we need to
            // make some changes to it based on its custom properties
            try
            {                
                var appName = bootstrappingRequest.ApplicationAlias; // use the ApplicationAlias as a default
                var subscriptionID = string.Empty;
                foreach (CustomProperty property in bootstrappingRequest.Properties)
                {
                    // only save the first property value in case any of these properties are multi-value custom properties                    
                    if (property.Name == "APM Application Name")
                    {
                        foreach (var value in property.Values)
                        {
                            appName = value;
                            break;
                        }
                    }

                    if (property.Name == "APM Subscription ID")
                    {
                        foreach (var value in property.Values)
                        {
                            subscriptionID = value;
                            break;
                        }
                    }
                }
                
                var modifyResult = ModifyConfigFiles(bootstrappingRequest, appName, subscriptionID);
                return !modifyResult.Succeeded ? modifyResult : BootstrappingResult.Success();
            }
            catch (Exception e)
            {
                return BootstrappingResult.Failure(new[] { e.Message });
            }
        }

        /// <summary>
        /// Find all the files ending in *.config and modify them to add the New Relic specific XML elements
        /// Imagine in the future we can use a Custom Property to instruct us which specific .config files to instrument
        /// 
        /// Also, copy the newrelic.config file with the subscription ID in the root folder of the application
        /// </summary>
        /// <param name="appName">The name of the application. This name will show up in the New Relic monitoring dashboard. The developer specifies this name in the Apprenda Custom Properties for her application</param>
        /// <param name="subscriptionID">The license key ID for the New Relic SaaS subscription</param>
        private static BootstrappingResult ModifyConfigFiles(BootstrappingRequest bootstrappingRequest, string appName, string subscriptionID)
        {
            string[] configFiles = Directory.GetFiles(bootstrappingRequest.ComponentPath, "*.config", SearchOption.AllDirectories);
            foreach (string file in configFiles)
            {
                var result = ModifyXML(bootstrappingRequest, file, appName);
                if (!result.Succeeded)
                {
                    return result;
                }
            }

            var srcConfigFilePath = Path.Combine(bootstrappingRequest.BootstrapperPath, @"newrelic.config");
            var dstConfigFilePath = Path.Combine(bootstrappingRequest.ComponentPath, @"newrelic.config");
            File.Copy(srcConfigFilePath, dstConfigFilePath, true);
            File.WriteAllText(dstConfigFilePath, Regex.Replace(File.ReadAllText(dstConfigFilePath), "$#LICENSEKEY#$", subscriptionID));

            // if the app is a web application hosted in IIS, we also need to include the application pool name to be instrumented in the newrelic.config file
            if (bootstrappingRequest.ComponentType == ComponentType.PublicAspNet || bootstrappingRequest.ComponentType == ComponentType.AspNet)
            {
                var applicationPoolName = bootstrappingRequest.ApplicationAlias;
                
                // if the application is in sandbox, we include the version alias in the name of the application pool in IIS
                if (bootstrappingRequest.Stage == Stage.Sandbox)
                {
                    applicationPoolName += "--" + bootstrappingRequest.VersionAlias;
                }
                var appPoolReplacement = Regex.Replace(NewRelicApplicationPoolValue, "$#IISAPPLICATIONNAME#$", applicationPoolName);
                File.WriteAllText(dstConfigFilePath, Regex.Replace(File.ReadAllText(dstConfigFilePath), NewRelicApplicationPoolTemplate, appPoolReplacement));                
            }           

            return BootstrappingResult.Success();
        }

        /// <summary>
        /// Edit the specified .config file and add the New Relic xml elements
        /// Assumes the New Relic agent is installed on all the Apprenda nodes and it is set to instrument all applications (IIS-based and non-IIS-based)
        /// Assumes the newrelic.config contains at least the following content to instrument the WCF container for Apprenda apps:
        ///     <instrumentation>
        ///         <applications>
        ///             <application name="Apprenda.WCFServiceHost.exe" />
        ///         </applications>
        ///     </instrumentation>
        /// </summary>
        private static BootstrappingResult ModifyXML(BootstrappingRequest bootstrappingRequest, string filePath, string appName)
        {
            // because we need to work with any *.config files, we can't use the webconfigurationmanager class            
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(filePath);
            XmlNode appsettingsNode = xmlDoc.SelectSingleNode("//appSettings");
            if (null == appsettingsNode)
            {
                // we didn't find an appSettings Node - returning
                return BootstrappingResult.Success();
            }

            XmlElement appNameElement = (XmlElement)xmlDoc.SelectSingleNode("//appSettings/add[@key = 'NewRelic.AppName']");
            XmlElement agentElement = (XmlElement)xmlDoc.SelectSingleNode("//appSettings/add[@key = 'NewRelic.AgentEnabled']");

            if (null == appNameElement)
            {
                // value does not exist, let's create it
                appsettingsNode.AppendChild(CreateNewElement("key", "NewRelic.AppName", "value", appName, xmlDoc, appsettingsNode));
            }
            else
            {
                // modify an existing value
                appNameElement.Attributes["value"].Value = appName;
            }

            // if the component type is an IIS-hosted web application, we don't need to add the agentenabled tag
            if (bootstrappingRequest.ComponentType == ComponentType.WcfService || bootstrappingRequest.ComponentType == ComponentType.WindowsService)
            {
                if (null == agentElement)
                {
                    appsettingsNode.AppendChild(CreateNewElement("key", "NewRelic.AgentEnabled", "value", "true", xmlDoc, appsettingsNode));
                }
                else
                {
                    agentElement.Attributes["value"].Value = "true";
                }
            }

            xmlDoc.Save(filePath);

            return BootstrappingResult.Success();
        }

        private static XmlNode CreateNewElement(string attribute1Name, string attribute1Value, string attribute2Name, string attribute2Value, XmlDocument xmlDoc, XmlNode node)
        {
            XmlNode newElement = xmlDoc.CreateNode(XmlNodeType.Element, "add", null);
            XmlAttribute attribute1 = xmlDoc.CreateAttribute(attribute1Name);
            attribute1.Value = attribute1Value;
            newElement.Attributes.Append(attribute1);
            XmlAttribute attribute2 = xmlDoc.CreateAttribute(attribute2Name);
            attribute2.Value = attribute2Value;
            newElement.Attributes.Append(attribute2);
            return newElement;
        }
    }
}
