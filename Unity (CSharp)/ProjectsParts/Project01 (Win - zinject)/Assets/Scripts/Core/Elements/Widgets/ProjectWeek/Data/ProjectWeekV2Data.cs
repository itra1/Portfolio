using Core.Elements.Widgets.Base.Attributes;
using Core.Elements.Widgets.Base.Consts;
using Core.Elements.Widgets.Info.Data;
using Core.Materials.Attributes;

namespace Core.Elements.Widgets.ProjectWeek.Data
{
    [WidgetDataTypeKey(WidgetDataTypeKey.KcStatusWeekV2)]
    public class ProjectWeekV2Data : PeriodDataBase
    {
        [MaterialDataPropertyParse("active")]
        public int? Active { get; set; }

        [MaterialDataPropertyParse("activeIncrement")]
        public int? ActiveIncrement { get; set; }

        [MaterialDataPropertyParse("paused")]
        public int? Paused { get; set; }

        [MaterialDataPropertyParse("pausedIncrement")]
        public int? PausedIncrement { get; set; }

        [MaterialDataPropertyParse("toClose")]
        public int? ToClose { get; set; }

        [MaterialDataPropertyParse("toCloseIncrement")]
        public int? ToCloseIncrement { get; set; }

        [MaterialDataPropertyParse("preproject")]
        public int? Preproject { get; set; }

        [MaterialDataPropertyParse("preprojectIncrement")]
        public int? PreprojectIncrement { get; set; }
    }
}