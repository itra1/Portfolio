using System.Collections.Generic;
using System.Linq;
using Core.Elements.Desktop.Data;
using Core.Elements.Presentation.Data;
using Core.Elements.PresentationEpisode.Data;
using Core.Elements.StatusColumn.Data;
using Core.Materials.Data;
using Core.Materials.Storage;
using Zenject;

namespace Core.Workers.Material
{
	public class AreaMaterialDataWorker : IAfterAddingToStorage, IUpdatingChildren
	{
		private IMaterialDataStorage _materials;
		
		[Inject]
		private void Initialize(IMaterialDataStorage materials) => 
			_materials = materials;

		public void PerformActionAfterAddingToStorageOf(MaterialData material)
		{
			if (material is not AreaMaterialData area)
				return;
			
			switch (area)
			{
				case DesktopAreaMaterialData desktop:
				{
					UpdateChildrenAt(desktop, _materials.GetList<ContentAreaMaterialData>());
					return;
				}
				case PresentationAreaMaterialData presentation:
				{
					UpdateChildrenAt(presentation, _materials.GetList<PresentationEpisodeAreaMaterialData>());
					SortChildrenAt(presentation, _materials.GetList<PresentationEpisodeMaterialData>());
					return;
				}
				case PresentationEpisodeAreaMaterialData presentationEpisode:
				{
					UpdateChildrenAt(presentationEpisode, _materials.GetList<ContentAreaMaterialData>());
					return;
				}
				case StatusContentAreaMaterialData status:
				{
					UpdateChildrenAt(status, _materials.GetList<ContentAreaMaterialData>());
					return;
				}
				case ContentAreaMaterialData content:
				{
					if (content.IsContainer)
						UpdateChildrenAt(content, _materials.GetList<AreaMaterialData>());
					
					return;
				}
				default:
				{
					UpdateChildrenAt(area, _materials.GetList<AreaMaterialData>());
					return;
				}
			}
		}
		
		public void UpdateChildrenAt<TParentMaterialData, TChildMaterialData>(TParentMaterialData parent, IReadOnlyList<TChildMaterialData> allChildren) 
			where TParentMaterialData : AreaMaterialData
			where TChildMaterialData : AreaMaterialData
		{
			var parentId = parent.Id;
			var childIds = parent.ChildIds;
			var children = parent.Children;
			
			childIds.Clear();
			children.Clear();
			
			var allChildrenCount = allChildren.Count;
			
			for (var i = 0; i < allChildrenCount; i++)
			{
				var child = allChildren[i];
				
				if (child.ParentId != parentId)
					continue;
				
				childIds.Add(child.Id);
				children.Add(child);
			}
		}

		private void SortChildrenAt(PresentationAreaMaterialData parent, IReadOnlyList<PresentationEpisodeMaterialData> allChildren)
		{
			var children = parent.Children;
			var childrenCount = children.Count;
			
			if (childrenCount == 0)
				return;
			
			children = children
				.Cast<PresentationEpisodeAreaMaterialData>()
				.OrderBy(episodeArea =>
				{
					if (episodeArea == null)
						return 0;
					
					var episodeId = episodeArea.EpisodeId;
					var episode = allChildren.FirstOrDefault(episode => episode.Id == episodeId);
					
					return episode?.Order ?? 0;
				})
				.OfType<AreaMaterialData>()
				.ToList();
			
			var childIds = parent.ChildIds;
			
			childIds.Clear();
			
			for (var i = 0; i < childrenCount; i++)
				childIds.Add(children[i].Id);
			
			parent.Children = children;
		}
	}
}