using Core.Elements.Widgets.Base.Attributes;
using Core.Elements.Widgets.Base.Consts;
using Core.Elements.Widgets.KcTable.Data;

namespace Core.Elements.Widgets.KcIncidentsTable.Data
{
    [WidgetDataTypeKey(WidgetDataTypeKey.KcIncidentsTable)]
    public class KcIncidentsTableData : KcTableDataBase
    {
        protected override int GetStatusOrder(string status)
        {
            return status switch
            {
                "Новый" => 0,
                "В работе" => 1,
                "Закрыт (мониторинг)" => 2,
                "Закрыт (архив)" => 3,
                "Закрыт (в архиве)" => 4,
                "Удален" => 5,
                _ => 10
            };
        }
    }
}