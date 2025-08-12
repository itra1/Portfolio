using System;
using com.ootii.Messages;
using Core.Elements.ScreenModes;
using Core.Messages;
using Core.Network.Api.Consts;
using Core.Network.Http;
using Core.Network.Http.Data;
using Core.Network.Socket.Packets.Outgoing.States.Common.Base;
using Core.Options;
using Elements.Common.Presenter.Factory;
using Elements.Desktops.Controller;
using Elements.Presentations.Controller;
using Elements.ScreenModes.Presenter;
using Elements.Statuses.Controller;
using UnityEngine;
using Zenject;
using Debug = Core.Logging.Debug;

namespace Elements.ScreenModes.Controller
{
	public class ScreenModesController : IScreenModesController, IScreenMode, IDisposable
	{
		private readonly DiContainer _container;
		private readonly IApplicationOptions _options;
		private readonly IStatusesController _statuses;
		private readonly IPresenterFactory _presenterFactory;
		private readonly IHttpRequest _request;
		private readonly IOutgoingStateController _outgoingState;
		
		private IDesktopsController _desktops;
		private IPresentationsController _presentations;
		
		private IScreenModesPresenter _presenter;
		private IScreenModeController _currentController;
		
		public ScreenMode CurrentMode { get; private set; }
		
		public ScreenModesController(DiContainer container,
			IApplicationOptions options,
			IStatusesController statuses, 
			IPresenterFactory presenterFactory,
			IHttpRequest request,
			IOutgoingStateController outgoingState)
		{
			_container = container;
			_options = options;
			_statuses = statuses;
			_presenterFactory = presenterFactory;
			_request = request;
			_outgoingState = outgoingState;
		}
		
		public bool Preload(RectTransform parent)
		{
			if (!_options.IsSumAdaptiveModeActive)
			{
				_desktops = _container.Resolve<IDesktopsController>();
				_presentations = _container.Resolve<IPresentationsController>();
			}
			
			_presenter = _presenterFactory.Create<ScreenModesPresenter>(parent);

			if (_presenter == null)
			{
				Debug.LogError("Failed to instantiate the ScreensPresenter");
				return false;
			}
			
			_presenter.AlignToParent();
			_presenter.Show();

			var content = _presenter.Content;

			_desktops?.Preload(content);
			_presentations?.Preload(content);
			_statuses.Preload(content);
			
			MessageDispatcher.AddListener(MessageType.ScreenSelect, OnScreenModeSelected);
			
			return true;
		}
		
		public void Dispose()
		{
			MessageDispatcher.RemoveListener(MessageType.ScreenSelect, OnScreenModeSelected);
			
			_desktops?.Unload();
			_presentations?.Unload();
			_statuses.Unload();
			
			_currentController = null;
			
			CurrentMode = ScreenMode.None;

			if (_presenter != null)
			{
				_presenter.Unload();
				_presenter = null;
			}
		}
		
		private IScreenModeController FindController(ScreenMode mode)
		{
			return mode switch
			{
				ScreenMode.Desktop when _desktops != null => _desktops,
				ScreenMode.Presentation when _presentations != null => _presentations,
				ScreenMode.Status => _statuses,
				_ => null
			};
		}
		
		private void SetScreenMode(ScreenMode mode)
		{
			var controller = FindController(mode);
			
			if (controller == null)
			{
				const string modeName = ScreenModeName.Home;
				
				if (!_options.IsStateSendingAllowed)
					_request.Request(string.Format(RestApiUrl.CurrentScreenFormat, modeName), HttpMethodType.Patch);
				
				_outgoingState.Context.SetCurrentScreen(modeName);
				_outgoingState.PrepareToSendIfAllowed();
			}
			else if (controller != _currentController)
			{
				CurrentMode = mode;
				
				_currentController?.Hide();
				
				_currentController = controller;
					
				if (_options.IsManagersLogEnabled)
					Debug.Log($"Screen mode \"{mode}\" has been set successfully");
				
				MessageDispatcher.SendMessageData(MessageType.ScreenModeActive, true);
				
				_currentController.Show();
				
				var modeName = mode.GetName();
				
				if (!_options.IsStateSendingAllowed)
					_request.Request(string.Format(RestApiUrl.CurrentScreenFormat, modeName), HttpMethodType.Patch);
				
				_outgoingState.Context.SetCurrentScreen(modeName);
				_outgoingState.PrepareToSendIfAllowed();
				
				MessageDispatcher.SendMessageData(MessageType.ScreenModeChange, mode);
			}
		}
		
		private void OnScreenModeSelected(IMessage message) => SetScreenMode((ScreenMode) message.Data);
	}
}