using System.Collections.Generic;
using DecisionsFramework.Data.ORMapper;
using DecisionsFramework.ServiceLayer;
using DecisionsFramework.ServiceLayer.Actions;
using DecisionsFramework.ServiceLayer.Actions.Common;
using DecisionsFramework.ServiceLayer.Utilities;

namespace Decisions.Monitoring.AzureAppInsights
{
    public class AzureAppInsightsSettings : AbstractModuleSettings
    {
        [ORMField(typeof(KeyFieldConverter))]
        public string InstrumentationKey { get; set; }

        public override BaseActionType[] GetActions(AbstractUserContext userContext, EntityActionType[] types)
        {
            List<BaseActionType> actions = new List<BaseActionType>(base.GetActions(userContext, types));
            actions.Add(new EditObjectAction(typeof(AzureAppInsightsSettings), "Edit Settings", null, "Edit Azure Insights", this,
                () =>
                {
                    MetricsForAzureAppInsights.SetInstrumentationKey(ModuleSettingsAccessor<AzureAppInsightsSettings>.Instance.InstrumentationKey);
                    ModuleSettingsAccessor<AzureAppInsightsSettings>.SaveSettings();
                }));
            return actions.ToArray();
        }
    }
}