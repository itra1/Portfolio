using Base.Presenter;
using com.ootii.Messages;
using Core.Messages;
using UnityEngine;
using Zenject;

namespace Elements.FloatingWindows.Presenter
{
	public class FloatingWindowsPresenter : PresenterBase, IFloatingWindowsPresenter
	{
		[SerializeField] private CanvasGroup _group;
		
		[Inject]
		private void Initialize() => SetName("FloatingWindows");
		
		private void Awake() => 
			MessageDispatcher.SendMessage(this, MessageType.HiddenCanvasGroupItemAdd, _group, EnumMessageDelay.IMMEDIATE);
	}
}
