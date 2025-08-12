using System;
using System.Collections.Generic;
using Zenject;

namespace Environment.Netsoft.WebView
{
	public class WebViewFactory : IWebViewFactory, IDisposable
	{
		private DiContainer _diContainer;
		private List<IWebViewApplication> _instances = new();

		public WebViewFactory(DiContainer diContainer)
		{
			_diContainer = diContainer;
		}
		public IWebViewApplication Create()
		{
			var browser = new WebViewApplication();
			_instances.Add(browser);
			_diContainer.Inject(browser);

			return browser;
		}

		public void Dispose()
		{
			for (int i = 0; i < _instances.Count; i++)
				_instances[i].Close();
			_instances.Clear();
		}

		public void Remove(IWebViewApplication application)
		{
			application.Close();
			_ = _instances.Remove(application);
		}
	}
}
