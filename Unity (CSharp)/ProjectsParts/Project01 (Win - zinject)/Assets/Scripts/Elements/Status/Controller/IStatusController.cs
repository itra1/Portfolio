using System.Collections.Generic;
using Base.Controller;
using Core.Elements.Status.Data;

namespace Elements.Status.Controller
{
	public interface IStatusController : IController, IPreloadingAsync
	{
		StatusMaterialData Material { get; }
		StatusAreaMaterialData AreaMaterial { get; }
		
		void ConfirmPlaylists(IList<int> columns);
	}
}