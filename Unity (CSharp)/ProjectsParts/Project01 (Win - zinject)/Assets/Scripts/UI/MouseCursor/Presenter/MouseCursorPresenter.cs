using System;
using System.Collections.Generic;
using FileResources;
using Settings;
using Settings.Data;
using UI.MouseCursor.Data;
using UI.MouseCursor.Presenter.Adapter;
using UI.MouseCursor.Presenter.Components;
using UnityEngine;
using Debug = Core.Logging.Debug;

namespace UI.MouseCursor.Presenter
{
    public class MouseCursorPresenter : IMouseCursorPresenter, IMouseCursorAccess, IMouseCursorAdapter, IDisposable
    {
	    private readonly ITextureProvider _textures;
        private readonly IDictionary<MouseCursorState, MouseCursorInfo> _infoListByState;
        private readonly IList<MouseCursorInitiatorInfo> _initiatorInfoList;
        private readonly IMouseCursorUpdater _customUpdater;
        
        public MouseCursorState CurrentState { get; private set; }
        
        public MouseCursorPresenter(IUISettings settings, ITextureProvider textures)
        {
	        _textures = textures;
	        _infoListByState = new Dictionary<MouseCursorState, MouseCursorInfo>();
	        _initiatorInfoList = new List<MouseCursorInitiatorInfo>();
	        _customUpdater = new CustomMouseCursorUpdater("Unnamed", 50);
	        
	        foreach (var info in settings.MouseCursors)
	        {
		        var state = info.State;
		        
		        if (!_infoListByState.TryAdd(state, info))
		        {
			        Debug.LogError($"An attempt was detected to add a duplicate mouse cursor state \"{state}\" as a key to the dictionary");
			        continue;
		        }
		        
		        if (state == MouseCursorState.Arrow)
			        SetCursor(state, info);
	        }
	        
	        MouseCursorMediator.Register(this);
        }
        
        public void SetInfo(MouseCursorInfo info)
        {
            var state = info.State;
            
            if (_infoListByState.ContainsKey(info.State))
                _infoListByState[state] = info;
            else
                _infoListByState.Add(state, info);
            
            if (CurrentState == state)
                SetCursor(state, info);
        }
        
        public void SetCursor(Texture2D texture, Vector2 hotspot)
        {
	        if (texture != null)
	        {
		        const MouseCursorState customState = MouseCursorState.Custom;
				
		        var info = new MouseCursorInfo
		        {
			        State = MouseCursorState.Custom,
			        Texture = texture,
			        HotSpot = hotspot
		        };
				
		        if (_infoListByState.ContainsKey(customState))
			        _infoListByState[customState] = info;
		        else
			        _infoListByState.Add(customState, info); 
                
		        Set(MouseCursorState.Custom, _customUpdater);
	        }
	        else
	        {
		        Remove(_customUpdater);
	        }
        }
        
        public void Set(in MouseCursorState state, IMouseCursorUpdater initiator)
        {
        	var isInitiatorInfoFound = false;
        	
        	for (var i = _initiatorInfoList.Count - 1; i >= 0; i--)
        	{
        		var initiatorInfo = _initiatorInfoList[i];
        		
        		if (initiatorInfo.Initiator != initiator)
        			continue;
        		
        		if (initiatorInfo.State == state)
        			return;
        		
        		_initiatorInfoList[i] = new MouseCursorInitiatorInfo
        		{
        			Initiator = initiator,
        			State = state
        		};
        		
        		isInitiatorInfoFound = true;
        		break;
        	}
        	
        	if (!isInitiatorInfoFound)
        	{
        		if (_initiatorInfoList.Count == 0)
        		{
        			_initiatorInfoList.Add(new MouseCursorInitiatorInfo
        			{
        				Initiator = initiator,
        				State = state
        			});
        		}
        		else
        		{
        			var isInitiatorInfoInserted = false;
        			
        			var priority = initiator.Priority;
        			
        			for (var i = _initiatorInfoList.Count - 1; i >= 0; i--)
        			{
        				var initiatorInfo = _initiatorInfoList[i];
        				var anotherInitiator = initiatorInfo.Initiator;
        				
        				if (anotherInitiator != initiator && anotherInitiator.Priority <= priority)
        				{
        					_initiatorInfoList.Insert(i + 1, new MouseCursorInitiatorInfo
        					{
        						Initiator = initiator,
        						State = state
        					});
        					
        					isInitiatorInfoInserted = true;
        					break;
        				}
        			}
        			
        			if (!isInitiatorInfoInserted)
        			{
        				_initiatorInfoList.Insert(0, new MouseCursorInitiatorInfo
        				{
        					Initiator = initiator,
        					State = state
        				});
        			}
        		}
        	}
        	
        	AttemptToSetCursor();
        }
        
        public void Remove(IMouseCursorUpdater initiator)
        {
        	var isInitiatorInfoRemoved = false;
        	
        	for (var i = _initiatorInfoList.Count - 1; i >= 0; i--)
        	{
        		if (_initiatorInfoList[i].Initiator != initiator)
        			continue;
        		
        		_initiatorInfoList.RemoveAt(i);
        		
        		isInitiatorInfoRemoved = true;
        	}
        	
        	if (isInitiatorInfoRemoved)
        		AttemptToSetCursor();
        }
        
        public void Dispose()
        {
	        MouseCursorMediator.Unregister(this);
	        
	        _initiatorInfoList.Clear();
	        
	        foreach (var info in _infoListByState.Values)
				_textures.Release(info.Texture);
	        
	        _infoListByState.Clear();
        }

        private void SetCursor(in MouseCursorState state, in MouseCursorInfo info)
        {
            CurrentState = state;
            
            var texture = info.Texture;
            var hotSpot = info.HotSpot;
            
            if (hotSpot == default)
	            hotSpot = new Vector2(texture.width * 0.5f, texture.height * 0.5f);

            Cursor.SetCursor(texture, hotSpot, CursorMode.Auto);
        }
        
        private bool ValidateState(in MouseCursorState state, out MouseCursorInfo info)
		{
			if (_infoListByState.TryGetValue(state, out info))
				return true;

			Debug.LogError("The mouse cursor has not yet been initialized");
			return false;
		}
        
		private void AttemptToSetCursor()
		{
			var state = _initiatorInfoList.Count > 0 ? _initiatorInfoList[^1].State : MouseCursorState.Arrow;
			
			if (ValidateState(state, out var info))
				SetCursor(state, info);
		}
    }
}