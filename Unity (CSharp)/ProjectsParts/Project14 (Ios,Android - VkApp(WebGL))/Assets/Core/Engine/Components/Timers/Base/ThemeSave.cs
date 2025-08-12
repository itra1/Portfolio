using System.Collections.Generic;
using Core.Engine.Components.SaveGame;

namespace Core.Engine.Components.Timers.Base
{
	internal class ThemeSave :SaveProperty<ThemeSaveData>
	{
		public override ThemeSaveData DefaultValue => new();
	}
	public class ThemeSaveData
	{
		public Dictionary<string,string> ActiveTheme = new();
		public List<string> ReadyTheme = new();
	}
}
