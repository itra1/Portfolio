using Game.Providers.Saves.Data;

namespace Game.Providers.Audio.Save
{
	public class AudioProviderSave : SaveProperty<AudioSave>
	{
		public override AudioSave DefaultValue => new() { Sound = true };
	}

	public class AudioSave
	{
		public bool Sound;
	}
}
