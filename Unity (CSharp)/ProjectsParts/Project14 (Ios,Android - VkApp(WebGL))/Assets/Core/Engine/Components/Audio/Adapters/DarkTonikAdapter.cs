namespace Core.Engine.Components.Audio
{
	public class DarkTonikAdapter :IAudioAdapter
	{
		public void PlaySound(string type)
		{
			DarkTonic.MasterAudio.MasterAudio.PlaySound(type);
		}
	}
}
