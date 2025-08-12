using Core.Elements.Widgets.Base.Attributes;
using Core.Elements.Widgets.Base.Consts;
using Core.Elements.Widgets.KcTable.Data;

namespace Core.Elements.Widgets.KcProjectTable.Data
{
    [WidgetDataTypeKey(WidgetDataTypeKey.KcProjectV2)]
    public class KcProjectsTableV2Data : KcTableDataBase
    {
        protected override int GetStatusOrder(string status)
        {
            return status switch
            {
                "Активный" => 0,
                "На паузе" => 1,
                "Завершен" => 2,
                _ => 10
            };
        }
    }
}