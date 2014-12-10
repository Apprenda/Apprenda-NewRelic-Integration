using Apprenda.API.Extension.Bootstrapping;
using Apprenda.API.Extension.CustomProperties;
using System;
using System.IO;
using System.Text;
using System.Xml;

namespace APMBootstrapper
{
    class ApplicationMonitoringBootstrapper : Apprenda.API.Extension.Bootstrapping.BootstrapperBase
    {
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
                }
                
                var modifyResult = ModifyWebConfig(bootstrappingRequest, appName);
                return !modifyResult.Succeeded ? modifyResult : BootstrappingResult.Success();
            }
            catch (Exception e)
            {
                return BootstrappingResult.Failure(new[] { e.Message });
            }
        }

        /// <summary>
        /// Find all the files ending in *.config and modify them to add the New Relic specific XML elements
        /// </summary>
        /// <param name="appName">The name of the application. This name will show up in the New Relic monitoring dashboard. The developer specifies this name in the Apprenda Custom Properties for her application</param>
        private static BootstrappingResult ModifyWebConfig(BootstrappingRequest bootstrappingRequest, string appName)
        {
            string[] configFiles = System.IO.Directory.GetFiles(bootstrappingRequest.ComponentPath, "*.config", SearchOption.AllDirectories);
            foreach (string file in configFiles)
            {
                var result = ModifyXML(file, appName);
                if (!result.Succeeded)
                {
                    return result;
                }
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
        private static BootstrappingResult ModifyXML(string filePath, string appName)
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

            if (null == agentElement)
            {
                appsettingsNode.AppendChild(CreateNewElement("key", "NewRelic.AgentEnabled", "value", "true", xmlDoc, appsettingsNode));
            }
            else
            {
                agentElement.Attributes["value"].Value = "true";
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
