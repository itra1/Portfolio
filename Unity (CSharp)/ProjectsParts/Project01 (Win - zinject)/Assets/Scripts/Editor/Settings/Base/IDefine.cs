namespace Editor.Settings.Base
{
	public interface IDefine
	{
		string Description { get; }
		
		void AfterEnable();
		void AfterDisable();
	}
}
