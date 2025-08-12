namespace Game.Providers.Ui.Attributes
{
	public interface IUiControllerAttribute
	{
		public string PresenterName { get; }
		public string PresenterType { get; }
	}
}