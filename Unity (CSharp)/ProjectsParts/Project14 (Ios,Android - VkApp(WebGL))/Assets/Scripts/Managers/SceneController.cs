using UnityEngine;
using Zenject;
using Game.Base;

/// <summary>
/// Контроллер управления сценой
/// </summary>
public class SceneController
{

	[SerializeField] Transform shootTransform;

	private IGameController _gameController;
	private SceneComponents _sceneComponents;

	public SceneController(SceneComponents sceneComponents, IGameController gameController)
	{
		_gameController = gameController;
		_sceneComponents = sceneComponents;
		SetObjectPosition();
	}

	#region Позициорание

	/// <summary>
	/// Позиционируются игровые объекты относительно экрана
	/// </summary>
	void SetObjectPosition()
	{
	}
	#endregion

}
