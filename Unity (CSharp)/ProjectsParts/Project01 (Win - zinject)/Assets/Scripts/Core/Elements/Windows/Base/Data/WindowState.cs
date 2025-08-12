using System;
using Core.Materials.Attributes;
using Leguar.TotalJSON;

namespace Core.Elements.Windows.Base.Data
{
	public abstract class WindowState
	{
		[MaterialDataPropertyParse("AreaId")] 
		public ulong? AreaId { get; set; }
		
		[MaterialDataPropertyParse("StateContentId")]
        public ulong? StateContentId { get; set; }
        
        [MaterialDataPropertyParse("EpisodeId")] 
        public ulong? EpisodeId { get; set; }
        
        [MaterialDataPropertyParse("PresentationId")]
        public ulong? PresentationId { get; set; }
        
        [MaterialDataPropertyParse("CloneAlias")]
        public string CloneAlias { get; set; }
        
        public bool IsFloatingWindow => AreaId == null
										&& StateContentId == null
                                        && EpisodeId == null
                                        && PresentationId == null
                                        && CloneAlias == null;
		
		public virtual JSON ConvertToJson()
        {
            var json = new JSON();
            
            json.Add("AreaId", AreaId);
            json.Add("StateContentId", StateContentId);
            json.Add("EpisodeId", EpisodeId);
            json.Add("PresentationId", PresentationId);
            json.Add("CloneAlias", CloneAlias);
            
            return json;
        }
		
		public virtual WindowState GetCopyIfInPresentationExcept(ulong areaId, 
			ulong? episodeId,
			ulong? presentationId,
			string cloneAlias)
		{
			var copy = (WindowState) Activator.CreateInstance(GetType());
			
			copy.AreaId = areaId;
			copy.StateContentId = StateContentId;
			copy.EpisodeId = episodeId ?? EpisodeId;
			copy.PresentationId = presentationId ?? PresentationId;
			copy.CloneAlias = cloneAlias ?? CloneAlias;
			
			return copy;
		}
		
		public virtual WindowState GetCopyIfInStatusExcept(ulong areaId, ulong? stateContentId)
		{
			var copy = (WindowState) Activator.CreateInstance(GetType());
			
			copy.AreaId = areaId;
			copy.StateContentId = stateContentId ?? StateContentId;
			copy.EpisodeId = null;
			copy.PresentationId = null;
			copy.CloneAlias = null;
			
			return copy;
		}
	}
}