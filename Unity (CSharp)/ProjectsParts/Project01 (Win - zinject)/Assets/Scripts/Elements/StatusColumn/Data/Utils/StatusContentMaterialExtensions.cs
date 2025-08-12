using Core.Elements.StatusColumn.Data;
using UI.Switches.Triggers.Data;
using UI.Switches.Triggers.Data.Enums;

namespace Elements.StatusColumn.Data.Utils
{
    public static class StatusContentMaterialExtensions
    {
        public static TriggerKey GetTriggerKey(this StatusContentMaterialData material)
        {
            return material.Column switch
            {
                1 => new TriggerKey(TriggerType.StatusColumn1),
                2 => new TriggerKey(TriggerType.StatusColumn2),
                3 => new TriggerKey(TriggerType.StatusColumn3),
                4 => new TriggerKey(TriggerType.StatusColumn4),
                5 => new TriggerKey(TriggerType.StatusColumn5),
                6 => new TriggerKey(TriggerType.StatusColumn6),
                7 => new TriggerKey(TriggerType.StatusColumn7),
                8 => new TriggerKey(TriggerType.StatusColumn8),
                9 => new TriggerKey(TriggerType.StatusColumn9),
                0 => new TriggerKey(TriggerType.StatusColumn10),
                _ => default
            };
        }
    }
}