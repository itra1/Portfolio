using UnityEngine;
using System.Collections;
using com.ootii.Messages;
using UnityEngine.SceneManagement;
using UnitySceneManager = UnityEngine.SceneManagement.SceneManager;
using DG.Tweening;

namespace it.Game.Managers
{
  /// <summary>
  /// Менеджер сцены
  /// </summary>
  public class SceneManager : MonoBehaviourBase
  {
	 // Событие загрузки сцены
	 public const string EVT_LOAD_SCENE = "SCENE_LOAD";
	 // Событие выгрузки сцены
	 public const string EVT_UNLOAD_SCENE = "SCENE_UNLOAD";
	 // Прогресс загрузки сцены
	 public const string EVT_LOAD_SCENE_PROGRESS = "SCENE_LOAD_PROGRESS";

	 private string _targetScene;
	 private string _unloadScene;
	 private bool _locationChange = false;

	 private void Awake()
	 {
		UnitySceneManager.sceneLoaded += UnitySceneLoaded;
		UnitySceneManager.sceneUnloaded += UnitySceneUnLoaded;
		UnitySceneManager.activeSceneChanged += UnityActiveSceneChanged;
	 }


	 private string _activeScene = "";

	 public string ActiveScene { get => _activeScene; set => _activeScene = value; }
	 public string TargetScene { get => _targetScene; set => _targetScene = value; }

	 /// <summary>
	 /// Загрузка сцены
	 /// </summary>
	 /// <param name="sceneName">Название сцены</param>
	 /// <param name="addative">догрузка сцены</param>
	 public void LoadScene(string sceneName, bool locationChange = false, UnityEngine.Events.UnityAction onStart = null)
	 {
		TargetScene = sceneName;
		_locationChange = true;
		_unloadScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

		//UnitySceneManager.LoadScene(sceneName, (addative ? LoadSceneMode.Additive : LoadSceneMode.Single));
		//ActiveScene = sceneName;
		//EmitSceneLoad(sceneName);

		Color _black = new Color32(0, 0, 0, 255);
		Color _transtarent = new Color32(0, 0, 0, 0);

		it.Game.Managers.UiManager.Instance.FullColor(Color.black, 1f,false, null, () =>
		{
		  onStart?.Invoke();
		  StartCoroutine(LoadingScene((locationChange ? "LevelChange":"Loader"), true, () =>
		  {
			 UnloadSceneAsync(_unloadScene, () =>
			 {

				UiManager.Instance.FullColor(_transtarent, _black, 0.5f,false, null, 
				  () =>  {
					 DOVirtual.DelayedCall((locationChange ? 5 : 0.5f), 
						() =>	 {
						StartCoroutine(LoadingScene(TargetScene, true, 
						  () =>	{
						  UiManager.Instance.FullColor(_black, _transtarent, 1f,false,null, 
							 () =>  {
								DOVirtual.DelayedCall(0.5f, () =>
								{
								  UnloadSceneAsync((locationChange ? "LevelChange" : "Loader"), () =>
								  {
									 UiManager.Instance.FullColor(_transtarent, _black, 1f, true);
								  });
								});

							 });
						}));
					 });

				  });

			 });

		  }));

		});
	 }
	 /// <summary>
	 /// Асинхронная загрузка сцены
	 /// </summary>
	 /// <param name="sceneName">Название сцены</param>
	 /// <param name="addative">догрузка сцены</param>
	 public void LoadSceneAsync(string sceneName, bool addative = true)
	 {
		StartCoroutine(LoadingScene(sceneName, addative));
	 }

	 private IEnumerator LoadingScene(string sceneName, bool addative = true,
		UnityEngine.Events.UnityAction OnComplete = null)
	 {
		AsyncOperation oper = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName, (addative ? LoadSceneMode.Additive : LoadSceneMode.Single));

		while (!oper.isDone)
		{
		  EmitLoadProgress(sceneName, oper.progress);
		  yield return null;
		}
		ActiveScene = sceneName;
		UnityEngine.SceneManagement.SceneManager.SetActiveScene(UnityEngine.SceneManagement.SceneManager.GetSceneByName(sceneName));

		yield return null;
		OnComplete?.Invoke();
		EmitSceneLoad(sceneName);
	 }

	 public void LoadSceneWishUnloadAsync(string sceneNameLoad, string sceneUnload, bool addative = true)
	 {
		StartCoroutine(LoadingUnloadingScene(sceneNameLoad, sceneUnload, addative));
	 }
	 private IEnumerator LoadingUnloadingScene(string sceneName, string sceneUnload, bool addative = true)
	 {
		AsyncOperation oper = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName, (addative ? LoadSceneMode.Additive : LoadSceneMode.Single));

		while (!oper.isDone)
		{
		  EmitLoadProgress(sceneName, oper.progress);
		  yield return null;
		}
		ActiveScene = sceneName;
		UnityEngine.SceneManagement.SceneManager.SetActiveScene(UnityEngine.SceneManagement.SceneManager.GetSceneByName(sceneName));

		EmitSceneLoad(sceneName);

		AsyncOperation oper2 = UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(sceneUnload);

		while (!oper2.isDone)
		{
		  yield return null;
		}

	 }

	 public void UnloadSceneAsync(string sceneName, UnityEngine.Events.UnityAction OnComplete = null)
	 {
		StartCoroutine(UnloadingAsync(sceneName, OnComplete));
	 }

	 private IEnumerator UnloadingAsync(string sceneName, UnityEngine.Events.UnityAction OnComplete = null)
	 {

		AsyncOperation oper = UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(sceneName);

		while (!oper.isDone)
		{
		  yield return null;
		}

		OnComplete?.Invoke();

		EnitUnloadScene(sceneName);
	 }

	 private void EmitSceneLoad(string sceneName)
	 {
		MessageDispatcher.SendMessage(this, EVT_LOAD_SCENE, sceneName, 0);
	 }

	 private void EnitUnloadScene(string sceneName)
	 {
		MessageDispatcher.SendMessage(this, EVT_UNLOAD_SCENE, sceneName, 0);
	 }

	 private void EmitLoadProgress(string sceneName, float progress)
	 {
		Game.Events.Messages.LoadScenePercent message = Game.Events.Messages.LoadScenePercent.Allocate();
		message.Type = EVT_LOAD_SCENE_PROGRESS;
		message.Sender = this;
		message.SceneName = sceneName;
		message.Progress = progress;
		message.Delay = 0;

		MessageDispatcher.SendMessage(message);
	 }

	 #region Unity events

	 private void UnitySceneLoaded(Scene scene, LoadSceneMode mode)
	 {

	 }
	 private void UnitySceneUnLoaded(Scene scene)
	 {

	 }

	 private void UnityActiveSceneChanged(Scene sceneBefore, Scene sceneAfter)
	 {

	 }

	 #endregion

  }
}