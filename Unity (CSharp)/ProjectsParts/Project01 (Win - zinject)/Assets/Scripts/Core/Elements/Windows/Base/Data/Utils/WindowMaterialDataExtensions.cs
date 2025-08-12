using System;
using Leguar.TotalJSON;
using Debug = Core.Logging.Debug;

namespace Core.Elements.Windows.Base.Data.Utils
{
	public static class WindowMaterialDataExtensions
	{
		public static string DeserializeStates(this WindowMaterialData material, WindowState state)
		{
			var states = material.States;
			
			var isRequiredToAddAsNewState = false;
			
			for (var i = 0; i < states.Length; i++)
			{
				var s = states[i];
				
				if (s == null)
				{
					Debug.LogWarning($"A null state was detected in the states of material: {material}");
					continue;
				}
				
				if (s.StateContentId == state.StateContentId
				    && s.EpisodeId == state.EpisodeId
				    && s.AreaId == state.AreaId
				    && s.PresentationId == state.PresentationId
				    && s.CloneAlias == state.CloneAlias)
				{
					states[i] = state;
					isRequiredToAddAsNewState = true;
					break;
				}
			}
			
			if (!isRequiredToAddAsNewState)
			{
				Array.Resize(ref states, states.Length + 1);
				states[^1] = state;
				material.States = states;
			}
			
			var statesJson = new JArray();
			
			foreach (var s in states)
				statesJson.Add(s.ConvertToJson());
			
			material.StatesJson = statesJson;
			
			return statesJson.CreateString();
		}
	}
}