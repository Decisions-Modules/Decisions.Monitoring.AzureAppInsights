using DecisionsFramework;
using DecisionsFramework.ServiceLayer;
using DecisionsFramework.Utilities.Profiler;
using System;
using System.Collections.Generic;
using Microsoft.ApplicationInsights;

namespace Decisions.Monitoring.AzureAppInsights
{
    public class MetricsForAzureAppInsights : IProfilerDetailWriter, IInitializable
    {
        private static TelemetryClient TEL_CLIENT;
        
        public MetricsForAzureAppInsights()
        {
            
            try
            {
                
            }
            catch (Exception expected)
            {
                // Data Dog throws errors if configuration happens multiple times.  
                // so just swallow this exception. and log it at info level.
                LogConstants.SYSTEM.Info("AzureAppInsights failed to initialize. Detail: {0}", expected);
            }
        }

        public void Initialize()
        {
            ProfilerService.DetailWriter = this;
            // When data dog is in place we want the logs written without
            // json being on new lines.
            Log.WRITE_LOG_FILE_DELIMITER = false;
            Log.WRITE_JSON_WITH_INDENTS = true;
            
            // Setup the static client.
            TEL_CLIENT = new TelemetryClient();
            if (string.IsNullOrEmpty(ModuleSettingsAccessor<AzureAppInsightsSettings>.GetSettings().InstrumentationKey))
            {
                AzureAppInsightsLog.LOG.Warn("Instrumentation Key is NOT set in settings and should be configured.  /System/Settings/AzureAppInsights");
            }
            else
            {

                TEL_CLIENT.InstrumentationKey =
                    ModuleSettingsAccessor<AzureAppInsightsSettings>.GetSettings().InstrumentationKey;

                Dictionary<string, string> props = new Dictionary<string, string>();
                props["Version"] = Version.VERSION;
                TEL_CLIENT.TrackEvent("SHM Started", props);
            }
        }

        public static void SetInstrumentationKey(string instrumentationKey)
        {
            if (TEL_CLIENT != null && string.IsNullOrEmpty(instrumentationKey) == false)
            {
                TEL_CLIENT.InstrumentationKey = instrumentationKey;
            }
        }

        public void WriteDetail(ProfileWriterData header, ProfilerDetail[] details, TimeSpan time)
        {
            if (header.type == ProfilerType.Usage)
            {
                foreach (ProfilerDetail eachEntry in details)
                {
                    TEL_CLIENT.TrackMetric(header.Name, (details == null) ? 1 : details.Length);
                }
            }
        }
    }
}