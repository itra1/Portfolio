using UnityEngine;
using Facebook.Unity;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Контроллер Facebook
/// </summary>
public class SocialFb : MonoBehaviour {

  /// <summary>
  /// Событие по изменению состояния авторизации
  /// </summary>
  public static event System.Action OnAuthChange;
  public delegate void WritePermission();
  public WritePermission OnWritePermission;

  public static SocialFb instance;

  /// <summary>
  /// Проверка авторизации
  /// </summary>
  public static bool CheckFbLogin {
    get {
#if PLUGIN_FACEBOOK
      return FB.IsLoggedIn;
#else
      return false;
#endif
    }
  }
  /// <summary>
  /// Текущий id facebook
  /// </summary>
  public static string GetUserId {
    get {
#if PLUGIN_FACEBOOK
      return AccessToken.CurrentAccessToken.UserId;
#else
      return "";
#endif
    }
  }
  /// <summary>
  /// Токен фейсбука
  /// </summary>
  public static string FBToken {
    get {
#if PLUGIN_FACEBOOK
      return AccessToken.CurrentAccessToken.TokenString;
#else
      return "";
#endif
    }
  }
  private void Awake() {
    if(instance != null) {
      Destroy(this);
      return;
    }
    instance = this;

#if PLUGIN_FACEBOOK

    DontDestroyOnLoad(this);
    instance = this;

    if(!FB.IsInitialized) FB.Init(OnInit);
#else
    Destroy(this);
    return;
#endif

  }

  void Start() { }

  void OnInit() {
    Debug.Log("OnInit");
		//FBlogin();
		StartCoroutine(WaitAndDownload());
	}

	IEnumerator WaitAndDownload() {
#if PLUGIN_FACEBOOK
		int ii = 0;
		while(ii < 240) {
			ii++;
			yield return new WaitForSeconds(0.5f);
			if(FB.IsInitialized && FB.IsLoggedIn) {
				FbLoginCalbac();
			}else {
				ii = 240;
			}
		}
#else
        yield return null;
#endif
	}

	/// <summary>
	/// Авторизация
	/// </summary>
	/// <param name="delegateFunction">Функция вызова ответной авторизации</param>
	public static void FBlogin() {
    if(instance == null)
      return;
    instance.FBlogin_();
  }
  /// <summary>
  /// Авторизация с правом чтения
  /// </summary>
  /// <param name="delegateFunction">Функция вызова ответной авторизации</param>
  public void FBlogin_() {
    FB.LogInWithReadPermissions(new List<string>() { "public_profile", "email", "user_friends" }, FbLoginCalbac);
  }
  /// <summary>
  /// Авторизация с правом редактирования данных
  /// </summary>
  void LoginInWritePermission() {
    FB.LogInWithPublishPermissions(new List<string>() { "publish_actions" }, FbLoginWriteCalbac);
  }
  /// <summary>
  /// Ответ при авторизации с правом чтения
  /// </summary>
  /// <param name="loginresult"></param>
  void FbLoginCalbac(ILoginResult loginresult = null) {
    if(loginresult != null && loginresult.Error != null) {
      Debug.LogError(loginresult.Error);
      return;
    }

		StopAllCoroutines();

    if(OnAuthChange != null) OnAuthChange();
    if(OnWritePermission != null) OnWritePermission();
  }
  /// <summary>
  /// Ответ при авторизации с правом на запись
  /// </summary>
  /// <param name="loginresult"></param>
  void FbLoginWriteCalbac(ILoginResult loginresult = null) {

    if(loginresult.Error != null) return;
    if(OnWritePermission != null) OnWritePermission();
  }

}
