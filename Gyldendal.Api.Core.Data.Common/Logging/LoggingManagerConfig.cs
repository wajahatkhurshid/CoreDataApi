using Gyldendal.Api.CoreData.Common.ConfigurationManager;
using LoggingManager.Entities;

namespace Gyldendal.Api.CoreData.Common.Logging
{
    public class LoggingManagerConfig : BaseConfigurationManager, ILoggingManagerConfig
    {
        public string RabbitMqLogUser => GetConfigValue("RabbitMqLogUser");

        public string RabbitMqLogUserPassword => GetConfigValue("RabbitMqLogUserPassword");

        public string RabbitMqLogHost => GetConfigValue("RabbitMqLogHost");

        public string RabbitMqLogVirtualHost => GetConfigValue("RabbitMqLogVirtualHost");

        public string RabbitMqLogExchange => GetConfigValue("RabbitMqLogExchange");

        public int LogEventHrs => GetConfigValueAsInt("LogEventHrs", 3);

        public bool LogInfo => GetConfigValueAsBool("LogInfo");

        public string LogName => GetConfigValue("LogName");

        public bool MailExToInfra => GetConfigValueAsBool("MailExToInfra");

        public string SourceName => GetConfigValue("SourceName");

        public string LogDirectoryPath => GetConfigValue("RabbitMqErrorFallbackLogDirectoryPath");

        public bool EnableEventLog => true;

        public bool EnableDebugLog => true;
    }
}