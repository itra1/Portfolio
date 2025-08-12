using System.Collections.Generic;
using Game.Scripts.UI.Controllers.Base;
using Game.Scripts.UI.Controllers.Factorys;
using ModestTree;
using UnityEngine.Events;

namespace Game.Scripts.UI
{
	public class UiNavigator : IUiNavigator
	{
		private Stack<IWindowPresenterController> _presentersStack = new();
		public UnityEvent<bool, IWindowPresenterController> OnPresenterVisibleChange { get; set; } = new();

		private readonly IWindowPresenterControllerFactory _presentersFactory;

		public UiNavigator(IWindowPresenterControllerFactory presentersFactory)
		{
			_presentersFactory = presentersFactory;
		}

		T IUiNavigator.GetController<T>()
		{
			T controller = _presentersFactory.GetInstance<T>();

			if (controller != null)
			{
				controller.OnPresenterVisibleChange.RemoveListener(PresenterVisibleChangeListener);
				controller.OnPresenterVisibleChange.AddListener(PresenterVisibleChangeListener);
			}

			return controller;
		}

		private void PresenterVisibleChangeListener(IWindowPresenterController presenterController)
		{
			if (!presenterController.IsOpen)
			{
				if (presenterController.AddInNavigationStack && _presentersStack.Count > 0)
				{
					var closedPresenterController = _presentersStack.Pop();
					((IUiNavigator) this).OnPresenterVisibleChange?.Invoke(closedPresenterController.IsOpen, closedPresenterController);

					var beforeController = _presentersStack.Peek();
					if (beforeController != null)
						((IUiNavigator) this).OnPresenterVisibleChange?.Invoke(beforeController.IsOpen, beforeController);
				}
				return;
			}

			if (presenterController.AddInNavigationStack)
				_presentersStack.Push(presenterController);

			((IUiNavigator) this).OnPresenterVisibleChange?.Invoke(presenterController.IsOpen, presenterController);
		}

		public void ClearStack()
		{
			_presentersStack.Clear();
		}

		public void CloseAll()
		{
			while (!_presentersStack.IsEmpty())
			{
				var targetController = _presentersStack.Pop();
				_ = targetController.Close();
			}
		}

		private bool BackNavigation()
		{
			// самый верхний - текущий
			_ = _presentersStack.Pop();
			if (_presentersStack.IsEmpty())
				return false;
			var targetController = _presentersStack.Pop();
			_ = targetController.Open();
			return true;
		}
	}
}
