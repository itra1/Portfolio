#if UNITY_EDITOR

using Settings.SymbolOptions.Base;

namespace Engine.Editor.Options
{
	public class UsingSongsAddressableInEditor : IToggleDefine
	{
		public string Symbol => "ADDRESSABLE_SONGS_EDITOR";

		public string Description => "Загружать треки в редакторе через addressable";

		public void AfterDisable()
		{
		}

		public void AfterEnable()
		{
		}
	}
}
#endif