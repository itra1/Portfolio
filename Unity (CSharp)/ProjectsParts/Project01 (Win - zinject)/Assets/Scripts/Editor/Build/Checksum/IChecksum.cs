namespace Editor.Build.Checksum
{
	public interface IChecksum
	{
		string Calculate(string path);
	}
}