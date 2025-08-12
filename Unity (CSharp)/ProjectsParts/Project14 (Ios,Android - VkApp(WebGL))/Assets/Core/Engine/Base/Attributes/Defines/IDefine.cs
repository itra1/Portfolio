namespace Core.Engine.App.Base.Attributes.Defines
{
	public interface IDefine
	{
		string Description { get; }

		void AfterEnable();
		void AfterDisable();
	}
}
