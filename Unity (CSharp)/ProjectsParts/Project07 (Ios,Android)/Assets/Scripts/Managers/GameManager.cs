using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DarkTonic.MasterAudio;

public class GameManager: Singleton<GameManager> {

  protected override void Awake() {
    DontDestroyOnLoad(gameObject);
    base.Awake();
    SceneManager.sceneLoaded += SceneLoaded;

  }

  private void Start() {

    Screen.sleepTimeout = SleepTimeout.NeverSleep;

    {
#if UNITY_IPHONE || UNITY_IOS
    UnityEngine.iOS.NotificationServices.RegisterForNotifications (
      UnityEngine.iOS.NotificationType.Alert |
      UnityEngine.iOS.NotificationType.Badge |
      UnityEngine.iOS.NotificationType.Sound, true);
#endif
    }

    StartCoroutine(PlaySound());
    //audioBackGround.PlayBackGround();
  }

  IEnumerator PlaySound() {
    yield return new WaitForEndOfFrame();
    PlayBackGroundSound("BackGround/menuBack");
  }

  string _lasetSourceNameSoundFile;
  public void PlayBackGroundSound(string sourceNameSoundFile) {

    if (_lasetSourceNameSoundFile == sourceNameSoundFile)
      return;
    _lasetSourceNameSoundFile = sourceNameSoundFile;
    MasterAudio.TriggerPlaylistClip("PlaylistController", _lasetSourceNameSoundFile);

  }

  public AudioClipData audioBackGround;

  private void SceneLoaded(Scene scene, LoadSceneMode loadScene) {

    if (scene.name == "Loader") return;

    SceneManager.SetActiveScene(scene);

  }


  /// <summary>
  /// Изменение активной сцены
  /// </summary>
  /// <param name="sceneName"></param>
  public void LoadScene(string sceneName) {

    BlackRound.Instance.Play(() => {
      SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    });

  }

  public void StartBattle(int level)
  {
    Location loc;
    do
    {
      loc = LocationManager.Instance.FindLocation(level);
      level--;
    } while (loc == null && level > 0);

    StartBattle(loc);
  }
  
  public void StartBattle(Location battleLocation) {

    if (battleLocation == null)
      return;

    var wepList = WeaponManager.Instance.GetWeaponsEquipped();

    if (wepList.Count <= 0) {
      InfoDialog dialog = UiController.GetUi<InfoDialog>();
      dialog.gameObject.SetActive(true);
      dialog.SetData("Не выбрано ни одно оружие. Экипируйтесь оружием в арсенале");
    }

    UserEnergy.Instance.UseGame();

    UserManager.Instance.ActiveLocation = battleLocation;
    GameManager.Instance.LoadScene("Game");
  }

}
