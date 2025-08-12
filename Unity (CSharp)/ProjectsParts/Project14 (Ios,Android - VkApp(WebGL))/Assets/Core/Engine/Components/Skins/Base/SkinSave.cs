using System.Collections.Generic;
using Core.Engine.Components.SaveGame;

namespace Core.Engine.Components.Skins
{
	public class SkinSave :SaveProperty<SkinSaveData>
	{
		public override SkinSaveData DefaultValue => new();
	}
	public class SkinSaveData
	{
		public Dictionary<string,string> ActiveSkins = new();
		public List<string> ReadySkins = new();
	}
}
