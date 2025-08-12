using System;
using System.Collections.Generic;
using Core.Network.Socket.ActionTypes;
using Core.Network.Socket.IgnoredPackets;
using Core.Network.Socket.Packets.Incoming.Base;
using Core.Network.Socket.Packets.Incoming.States;
using Zenject;

namespace Core.Network.Socket.Packets.Incoming.Actions
{
	public class ActionPacket : IncomingPacket
	{
		private readonly ISocketActionPackets _actionPackets;
		private readonly IIgnoredIncomingPackets _ignoredPackets;

		private IncomingAction _action;
		private string _replaceableAlias;
		private Queue<IIncomingPacket> _actions;

		public ActionPacket()
		{
			var container = ProjectContext.Instance.Container;
			
			_actionPackets = container.Resolve<ISocketActionPackets>();
			_ignoredPackets = container.Resolve<IIgnoredIncomingPackets>();
		}

		public override bool Parse()
		{
			var actionType = PacketType;
			
			if (!string.IsNullOrEmpty(actionType)) 
			{ 
				if (_ignoredPackets.IsPacketIgnored(actionType))
					return false;
				
				(_action, _replaceableAlias) = CreateAction(actionType);
				
				if (_action == null) 
					throw new Exception($"Missing action handling with type \"{actionType}\"");
				
				_action.Parse(DataJson);
				
				if (!string.IsNullOrEmpty(_replaceableAlias))
					_action.ReplaceAlias(_replaceableAlias);
			}
			else
			{
				_actions = new Queue<IIncomingPacket>();
				
				CreateActionIfAvailable<ScreenSelect>("screen_select");
				CreateActionIfAvailable<WindowAction>("window_action");
				CreateActionIfAvailable<ScreensaverVisibility>("screensaver_visibility");
				CreateActionIfAvailable<MapTextureChange>("map_texture");
			}
			
			return true;
		}
		
		public override bool Process()
		{
			if (!base.Process())
				return false;
			
			if (_action != null) 
				return _action.Process();
			
			if (_actions != null)
			{
				var result = true;
				
				while (_actions.Count > 0)
					result = result && _actions.Dequeue().Process();

				return result;
			}
			
			return true;
		}
		
		private (IncomingAction, string) CreateAction(string actionType)
		{
			var types = _actionPackets.PacketTypes;
			
			return types.TryGetValue(actionType, out var data) 
				? ((IncomingAction) Activator.CreateInstance(data.TargetType), ReplaceAlias: data.ReplaceableAlias) 
				: default;
		}
		
		private void CreateActionIfAvailable<TAction>(string actionType) where TAction : IDataJsonSetter, IIncomingPacket, new()
		{
			if (_ignoredPackets.IsPacketIgnored(actionType) || !DataJson.ContainsKey(actionType))
				return;
			
			var action = new TAction
			{
				DataJson = DataJson
			};
			
			action.Parse();
			
			_actions.Enqueue(action);
		}
	}
}