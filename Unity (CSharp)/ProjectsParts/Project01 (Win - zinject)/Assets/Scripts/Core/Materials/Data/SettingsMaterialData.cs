using Core.Materials.Attributes;
using Core.Materials.Consts;
using Leguar.TotalJSON;

namespace Core.Materials.Data
{
	/// <summary>
	/// Устаревшее название - "SettingsMaterial"
	/// </summary>
	[MaterialDataLoader("/settings")]
	public class SettingsMaterialData : MaterialData
	{
		[MaterialDataPropertyParse("avayaLink"), MaterialDataPropertyUpdate]
		public string AvayaLink { get; set; }
		
		[MaterialDataPropertyParse("socio"), MaterialDataPropertyUpdate]
		public string Socio { get; set; }
		
		[MaterialDataPropertyParse("lib"), MaterialDataPropertyUpdate]
		public string Lib { get; set; }
		
		[MaterialDataPropertyParse("cnp"), MaterialDataPropertyUpdate]
		public string Cnp { get; set; }
		
		[MaterialDataPropertyParse("gis"), MaterialDataPropertyUpdate]
		public string Gis { get; set; }
		
		[MaterialDataPropertyParse("cams"), MaterialDataPropertyUpdate]
		public string Cams { get; set; }
		
		[MaterialDataPropertyParse("HDMI"), MaterialDataPropertyUpdate]
		public string Hdmi { get; set; }
		
		[MaterialDataPropertyParse("kc"), MaterialDataPropertyUpdate]
		public string Kc { get; set; }
		
		[MaterialDataPropertyParse("timerMinuteSoundId"), MaterialDataPropertyUpdate]
		public ulong TimerMinuteSoundId { get; set; }
		
		[MaterialDataPropertyParse("timerHourSoundId"), MaterialDataPropertyUpdate]
		public ulong TimerHourSoundId { get; set; }
		
		[MaterialDataPropertyParse("extra"), MaterialDataPropertyUpdate]
		public JSON Others { get; set; }

		public SettingsMaterialData() => Model = MaterialModel.Settings;
	}
}