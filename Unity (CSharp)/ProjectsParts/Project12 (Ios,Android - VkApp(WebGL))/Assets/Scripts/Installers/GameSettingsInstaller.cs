using Engine.Scripts.Settings;
using Game.Scripts.Controllers.Inputs.Settings;
using UnityEngine;
using Zenject;

namespace Game.Scripts.Installers
{
	[CreateAssetMenu(fileName = "GameSettingsInstaller", menuName = "Installers/GameSettings")]
	public class GameSettingsInstaller : ScriptableObjectInstaller
	{
		[SerializeField] private ScoreSettings _scoreSettings;
		[SerializeField] private RhythmSettings _rhythmSettings;
		[SerializeField] private InputsSettings _inputsSettings;
		[SerializeField] private ModesSettings _modes;
		[SerializeField] private TemplateSettings _templates;
		[SerializeField] private PrefabsLibrary _prefabsLibrary;
		[SerializeField] private SpecialPointsSettings _specialPointsSettings;
		[SerializeField] private GameColorsSettings _gameColorsSettings;

		public override void InstallBindings()
		{
			_ = Container.BindInterfacesTo<ScoreSettings>().FromInstance(_scoreSettings).NonLazy();
			_ = Container.Bind<RhythmSettings>().FromInstance(_rhythmSettings).NonLazy();
			_ = Container.Bind<ModesSettings>().FromInstance(_modes).NonLazy();
			_ = Container.Bind<TemplateSettings>().FromInstance(_templates).NonLazy();
			_ = Container.Bind<PrefabsLibrary>().FromInstance(_prefabsLibrary).NonLazy();
			_ = Container.Bind<SpecialPointsSettings>().FromInstance(_specialPointsSettings).NonLazy();
			_ = Container.Bind<GameColorsSettings>().FromInstance(_gameColorsSettings).NonLazy();
			_ = Container.Bind<InputsSettings>().FromInstance(_inputsSettings).NonLazy();
		}
	}
}
