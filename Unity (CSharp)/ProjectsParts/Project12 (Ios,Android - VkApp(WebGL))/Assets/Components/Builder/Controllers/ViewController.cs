using System;
using Builder.Common;
using Builder.Views;
using UnityEngine.UIElements;

namespace Builder.Controllers
{
	public abstract class ViewController<TView> : ViewControllerBbase
	where TView : ViewBase
	{
		protected BuildSession _session;

		public override string Type => View.Type;
		public TView View { get; set; }

		public ViewController(BuildSession session)
		{
			_session = session;

			View = (TView) Activator.CreateInstance(typeof(TView), session);
			PostCreateView();
		}

		public override void SetVisible(bool visible)
		{
			View.View.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;

			if (visible)
				View.Show();
		}

		protected override void PostCreateView()
		{

		}
	}
}
