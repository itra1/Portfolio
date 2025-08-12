using System;
using System.Reflection;
using Core.Elements.Windows.Base.Data;
using Core.Elements.Windows.Base.Data.Attributes;
using Core.Network.Socket.Packets.Incoming.Actions.Consts;
using Cysharp.Threading.Tasks;
using Elements.FloatingWindow.Presenter;
using Elements.FloatingWindow.Presenter.Factory;
using UnityEngine;
using Debug = Core.Logging.Debug;

namespace Elements.FloatingWindow.Controller
{
	public class FloatingWindowController : IFloatingWindowController
	{
		private readonly IFloatingWindowPresenterFactory _presenterFactory;
		private readonly bool _isAdaptiveSizeRequired;
		
		private IFloatingWindowPresenter _presenter;
		
		public WindowMaterialData Material { get; }
		public IFloatingWindowOptions Options => _presenter;
		
		public bool Active => _presenter is { Active: true };
		public bool Visible => _presenter is { Visible: true };
		
		public FloatingWindowController(WindowMaterialData material,
			IFloatingWindowPresenterFactory presenterFactory,
			bool isAdaptiveSizeRequired)
		{
			Material = material;
			
			_presenterFactory = presenterFactory;
			_isAdaptiveSizeRequired = isAdaptiveSizeRequired;
		}
		
		public async UniTask<bool> PreloadAsync(RectTransform parent)
		{
			_presenter = _presenterFactory.Create(Material, parent, _isAdaptiveSizeRequired);
			
			if (_presenter == null)
				return false;
			
			if (!await _presenter.PreloadAsync())
				return false;
			
			FindOrCreateState();
			return true;
		}
		
		public void Activate() => _presenter?.Activate();
		public void Deactivate() => _presenter?.Deactivate();
		
		public bool Show() => _presenter != null && _presenter.Show();
		public bool Hide() => _presenter != null && _presenter.Hide();
		
		public void Unload()
		{
			if (_presenter == null) 
				return;
			
			_presenter.Unload();
			_presenter = null;
		}
		
		public void PerformAction(string actionAlias, ulong materialId)
		{
			if (_presenter == null)
			{
				Debug.LogError("Presenter is null when trying to perform an action");
				return;
			}
			
			if (!_presenter.Visible)
				Show();
			
			if (actionAlias == WindowMaterialActionAlias.Focus)
			{
				if (!_presenter.InFocus)
					_presenter.Focus();
				else
					_presenter.Unfocus();
			}
			else
			{
				_presenter.PerformAction(actionAlias, materialId);
			}
		}
		
		public void PerformFloatingWindowAction(string actionAlias)
		{
			if (_presenter == null)
			{
				Debug.LogError("Presenter is null when trying to perform a floating window action");
				return;
			}
			
			if (!_presenter.Visible)
				Show();
			
			_presenter.PerformFloatingWindowAction(actionAlias);
		}

		private void FindOrCreateState()
		{
			if (_presenter == null)
			{
				Debug.LogError("Presenter is null when trying to find or create a state");
				return;
			}
			
			var attribute = Material.GetType().GetCustomAttribute<WindowStateAttribute>();
			
			if (attribute == null)
				return;
			
			var states = Material.States;
			
			WindowState state = null;
			
			for (var i = states.Length - 1; i >= 0; i--)
			{
				var s = states[i];
				
				if (s == null)
				{
					Debug.LogWarning($"A null state was detected in the states of material {Material}");
					continue;
				}
				
				if (!s.IsFloatingWindow)
					continue;

				state = s;
				break;
			}
			
			if (state == null)
				state = (WindowState) Activator.CreateInstance(attribute.Type);
			
			_presenter.SetState(state);
		}
	}
}
