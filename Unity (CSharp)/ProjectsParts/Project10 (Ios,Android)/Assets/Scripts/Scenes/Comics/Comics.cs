using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// Комикс
/// </summary>
public class Comics : MonoBehaviour {

  /// <summary>
  /// Время на появление текущего кадра
  /// </summary>
  [System.Serializable]
  public struct ComicsCadr {
    public float time;                  // Время на появление кадра
    public Image cadr;                  // Ссылка на сам кадр
  }

  /// <summary>
  /// Структура комикса
  /// 
  /// <para>Время на страницу дроится на количество кадров</para>
  /// </summary>
  [System.Serializable]
  public struct ComicsParam {

    public float timeShow;        // Время показа
		public ComicsCadr[] card;     // Кадры
		public AudioClip clip;        // Сам клип
	}

  /// <summary>
  /// Стартовый комикс
  /// </summary>
  public ComicsParam[] startComics;
  /// <summary>
  /// Конечный комикс
  /// </summary>
  public ComicsParam[] endComics;
  /// <summary>
  /// Активный комикс
  /// </summary>
  ComicsParam[] activeComics;
  /// <summary>
  /// ссылка на кмеры
  /// </summary>
  public Camera camObject;
  /// <summary>
  /// Панель с картинками
  /// </summary>
  public GameObject imagesPanel;

  public delegate void CloseComics();
  public static event CloseComics OnCloseComics;
  
  [SerializeField]
  Animator tapAnim;
  
  public GameObject blackScreen;

  /// <summary>
  /// Номер страницы
  /// </summary>
  int pageNum;

  /// <summary>
  /// Время новой страницы
  /// </summary>
  float nextPageTime;
  
  [SerializeField]
  Canvas mainCanvas;

  AudioSource audioComp;
  
  void Start() {
    
    audioComp = GetComponent<AudioSource>();
    audioComp.loop = false;

    pageNum = 0;

    if (GameManager.isFirstStart) {
      activeComics = startComics;
    } else {
      camObject.GetComponent<AudioListener>().enabled = false;
      activeComics = endComics;
    }
    Init();
    ShowVideo();
  }

  void OnEnable() {
    //blackScreen.GetComponent<FillBlack>().bg.color = new Color(0 , 0 , 0 , 0);
  }

  /// <summary>
  /// Событие окончания комикса
  /// </summary>
  void ComicsTheEnd() {
    if (OnCloseComics != null)
      OnCloseComics();
  }

  /// <summary>
  /// Показать видео
  /// </summary>
  public void ShowVideo() {
    float delay = 0;
    for(int i = 0 ; i < activeComics[pageNum].card.Length ; i++) {
      LightTween.SpriteColorTo(activeComics[pageNum].card[i].cadr , new Color(1 , 1 , 1 , 1) , activeComics[pageNum].card[i].time , delay, LightTween.EaseType.linear,gameObject,
	      () => {
		      Invoke("OnTap",0.7f);
	      });
      delay += activeComics[pageNum].card[i].time;
    }
    audioComp.PlayOneShot(activeComics[pageNum].clip);

    tapAnim.SetBool("show" , false);
    //nextPageTime = Time.time + activeComics[pageNum].timeShow;
    pageNum++;
  }

  void GetReadyVideo() {
    if (pageNum < 3) {
      tapAnim.SetBool("show" , true);
    }
  }
  /// <summary>
  /// Так по экрану
  /// </summary>
  public void OnTap() {

		if(IsInvoking("OnTap")) CancelInvoke("OnTap");

    //if (nextPageTime > Time.time) return;

    if(pageNum < activeComics.Length)
      ShowVideo();
    else {
      if (GameManager.isFirstStart) {
        GameManager.isFirstStart = false;
        //blackScreen.GetComponent<FillBlack>().bg.color = new Color(0, 0, 0, 0);

        FillBlack.Instance.PlayAnim(FillBlack.AnimType.full, FillBlack.AnimVecor.swich, Vector3.zero, () => {
          GameManager.LoadScene("Menu", true);
        }, () => { }, () => { });


        //LightTween.SpriteColorTo(blackScreen.GetComponent<FillBlack>().bg, new Color(0 , 0 , 0 , 1) , 0.5f , 0, LightTween.EaseType.linear,gameObject, ()=> { GameManager.LoadScene("Menu", true); });
      } else {
				SceneManager.UnloadSceneAsync("Comics");
				//SceneManager.UnloadScene(SceneManager.GetSceneByName("comics").buildIndex);
        //LightTween.SpriteColorTo(blackScreen.GetComponent<Image>() , new Color(0 , 0 , 0 , 1) , 0.5f , 0);
        ComicsTheEnd();
      }
    }
  }
	
  
  void Update() {

    if (nextPageTime <= Time.time)
      GetReadyVideo();
  }
  
  void Init() {
    SetScale();
  }
  
  bool waitUpdate;

  public void Skip() {
    GameManager.isFirstStart = false;
    GameManager.LoadScene("Loader");
    LightTween.SpriteColorTo(blackScreen.GetComponent<Image>(), new Color(0, 0, 0, 1), 0.5f, 0);
  }
  
  /// <summary>
  /// Изменение размера по экрану
  /// </summary>
  void SetScale() {

    Vector2 etalonSize = new Vector3(1280 , 720);
    Vector2 canvasSize = mainCanvas.GetComponent<RectTransform>().sizeDelta;
    Vector2 needSize = etalonSize;

    if (etalonSize.y != canvasSize.y)
      needSize = new Vector2(etalonSize.x * ( canvasSize.y / etalonSize.y ) , canvasSize.y);

    if (needSize.x > canvasSize.x)
      needSize = new Vector2(canvasSize.x , ( canvasSize.x / needSize.x ) * needSize.y);

    float scale = needSize.x/ 1280;
    imagesPanel.GetComponent<RectTransform>().localScale = new Vector3(scale , scale , scale);
  }
  
}
