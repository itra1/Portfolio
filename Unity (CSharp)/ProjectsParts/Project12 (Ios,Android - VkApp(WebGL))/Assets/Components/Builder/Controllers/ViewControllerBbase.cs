namespace Builder.Controllers
{
	public abstract class ViewControllerBbase
	{
		public abstract string Type { get; }

		public abstract void SetVisible(bool visible);

		protected abstract void PostCreateView();
	}
}
