using Core.Materials.Attributes;

namespace Core.Elements.Widgets.Info.Data
{
    public abstract class InfoDataBase : PeriodDataBase
    {
        [MaterialDataPropertyParse("inWork")]
        public int? InWork { get; set; }

        [MaterialDataPropertyParse("inWorkIncrement")]
        public int? InWorkIncrement { get; set; }

        [MaterialDataPropertyParse("monitoring")]
        public int? Monitoring { get; set; }

        [MaterialDataPropertyParse("monitoringIncrement")]
        public int? MonitoringIncrement { get; set; }

        [MaterialDataPropertyParse("toClose")]
        public int? ToClose { get; set; }

        [MaterialDataPropertyParse("toCloseIncrement")]
        public int? ToCloseIncrement { get; set; }

        [MaterialDataPropertyParse("archive")]
        public int? Archive { get; set; }

        [MaterialDataPropertyParse("archiveIncrement")]
        public int? ArchiveIncrement { get; set; }
    }
}