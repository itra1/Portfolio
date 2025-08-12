using Engine.Engine.Scripts.Settings;
using Game.Scripts.App;
using UnityEngine.Events;

namespace Game.Scripts.Controllers
{
	public interface IApplicationSettingsController : IApplicationLoaderItem
	{
		IApplicationSettings ApplicationSettings { get; }
		UnityEvent<IApplicationSettings> OnLoadSettings { get; set; }
	}
}