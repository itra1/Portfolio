using Core.Elements.Status.Data;
using Elements.ScreenModes.Controller;

namespace Elements.Statuses.Controller
{
	public interface IStatusesController : IScreenModeController
	{
		StatusMaterialData ActiveStatusMaterial { get; }
	}
}