using Core.Materials.Attributes;
using Core.Materials.Consts;
using Core.Workers.Material;
using Leguar.TotalJSON;

namespace Core.Materials.Data
{
    [MaterialDataLoader("/profile/me")]
    [MaterialDataWorker(typeof(ArUserProfileMaterialDataWorker))]
    public class ArUserProfileMaterialData : MaterialData
    {
        [MaterialDataPropertyParse("calendarDataSource"), MaterialDataPropertyUpdate]
        public JSON DataJson { get; set; }
        
        public CalendarDataSourceMaterialData CalendarDataSource { get; set; }

        public ulong CalendarId => CalendarDataSource.Id;
     
        public ArUserProfileMaterialData()
        {
            Model = MaterialModel.UserProfile;
        }
    }
}