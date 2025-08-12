/* *********************
 * Основной контроллер вступительной сцены
 * *********************/

using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public class SceneMainCintroller : MonoBehaviour
{
    public GameObject cameraObject;                               // Камера
    public GameObject animCamera;                                 // Камера участующая в анимации
    public JackMainController jack;
    public SnakeMainController snake;

    #region Main
    bool main;
    public GameObject mainUi;                               // Главный интерфейс
    public GameObject mainObjects;                          // Объекты на главном экране
    public GameObject bgPerehod;                            // Объект шторки переходи
    public GameObject bgPerehodRadial;                            // Объект шторки переходи
    public GameObject shopUI;                               // Интерфейс магазина
    public GameObject cristal;

    public ParticleSystem[] particles;
    public ParticleSystem[] particles1;

    public GameObject bestPanel;                            // Панель лучшего результата
    public GameObject mainBottomPanel;                      // Панель на главной странице
    #endregion

    public AdController adContent;
    
    #region Colors
    public Color whiteColor;
    public Color yellowColor;
    #endregion

    bool firsFrame = true;

    bool goToLevelReady = false;
    
    public AudioClip clickClip;
    public AudioClip introClip;

    string toScene;

    void Awake()
    {
        Time.timeScale = 1f;
    }

    void FixedUpdate()
    {
        if (!firsFrame) return;
        firsFrame = false;
        bgPerehod.GetComponent<Animator>().SetTrigger("hide");
    }

    void Start()
    {        
        Time.timeScale = 1f;
        foreach (ParticleSystem part in particles1) part.Stop();

        // Показываем шторку
        if (!bgPerehod.activeInHierarchy) bgPerehod.SetActive(true);

        main = true;
        creditPanel.SetActive(false);
        shopUI.SetActive(false);
        mainUi.SetActive(true);

        // Подписываемся на пуши
#if UNITY_IOS
        Apikbs.SubscribePushIOS();
#endif
    }
    

    // Активация или дизактивация объектов
    IEnumerator ActiveObject(GameObject obj, float time, bool act)
    {
        yield return new WaitForSeconds(time);
        if (obj.GetComponent<Animator>()) obj.GetComponent<Animator>().Rebind();
        obj.SetActive(act);
        yield return 0;
    }



    /* **********************
     * Загрузка главной сцены
     * 
     * show - флаг активации или дизактивации сцены
     * **********************/
    void LoadMainScenes(bool show = true)
    {
        if (show)
        {
            
            if (!mainUi.activeInHierarchy) mainUi.SetActive(true);
            main = true;
        }
        else
        {
            mainUi.GetComponent<Animator>().SetBool("hide",true);
            //StartCoroutine(ActiveObject(FBPanel, 0.5f, false));
            StartCoroutine(ActiveObject(mainUi, 1f, false));
            main = false;
        }
    }
    
    #region playGame
    // Кнопка кристала
    public void ButtonCristal()
    {
        if (!main) return;
        toPlay();
    }

    public void ShowBlackBg()
    {
        if (toScene == "Runner") {
            if (!goToLevelReady) return;
            SceneManager.LoadScene(toScene);
        }

        if (toScene == "Map") {
            SceneManager.LoadScene(toScene);
        }
    }

    public void ButtomToMap() {

        if (!bgPerehod.activeInHierarchy) bgPerehod.SetActive(true);
        bgPerehod.GetComponent<Animator>().SetTrigger("show");
        if (toScene == null || toScene == "") toScene = "Map";
    }

    public void GoRun()
    {
        if (!main) return;
        mainObjects.GetComponent<Animator>().SetTrigger("cristalReady");
        mainUi.GetComponent<Animator>().SetBool("hide", true);
        adContent.HideAd();
    }

    public void ShowAd(string url) {
        if (!main) return;
        Application.OpenURL(url);
    }

    public void JackStart() {
        AudioManager.PlayEffect(introClip, AudioMixerTypes.runnerEffect);
        jack.StartAnim();
    }

    public void StartParticles()
    {
        foreach (ParticleSystem part in particles1) part.Play();

        mainObjects.GetComponent<Animator>().SetTrigger("warningMain");
        mainObjects.GetComponent<Animator>().SetTrigger("warning");
        mainObjects.GetComponent<Animator>().SetTrigger("hideLight");
        snake.HideSnake();
    }

    public void StartWarning() {
        // mainObjects.GetComponent<Animator>().SetTrigger("hideLight");
    }
    public void StartWarningMain() {
        goToLevelReady = true;
        StartCoroutine(toPlayGo());
    }

    public void toPlay()
    {
        if (!bgPerehodRadial.activeInHierarchy) bgPerehodRadial.SetActive(true);
        bgPerehodRadial.GetComponent<Animator>().SetTrigger("show");
        if(toScene == null || toScene == "") toScene = "Runner";
    }

    IEnumerator toPlayGo()
    {
        yield return new WaitForSeconds(0.5f);
        //if (!bgPerehod.activeInHierarchy) bgPerehod.SetActive(true);
        //bgPerehod.GetComponent<Animator>().SetTrigger("show");
        if (!bgPerehodRadial.activeInHierarchy) bgPerehodRadial.SetActive(true);
        bgPerehodRadial.GetComponent<Animator>().SetTrigger("show");
        if (toScene == null || toScene == "") toScene = "Runner";
        AudioManager.HideAllSound();
        yield return 0;
        
    }
    #endregion

    #region MainToShop
    
    public void MainToShop() {
        AudioManager.PlayEffect(clickClip, AudioMixerTypes.runnerEffect);
        main = false;
        animControl(false);
        StartCoroutine(MainToShopGo());
    }

    public void ShopToMain()
    {
        AudioManager.PlayEffect(clickClip, AudioMixerTypes.runnerEffect);

        main = true;
        //animControl(false);
        StartCoroutine(ShopToMainGo());
    }

    void animControl(bool flag)
    {
        if (flag)
        {
            mainObjects.GetComponent<Animator>().speed = 1;
            jack.pause(false);
            snake.pause(false);
            adContent.pause(false);
            //StartCoroutine(PlayAnim(mainObjects.GetComponent<Animator>()));
            for (int i = 0; i < particles.Length; i++)
                particles[i].Play();
        }
        else
        {
            jack.pause(true);
            snake.pause(true);
            adContent.pause(true);
            mainObjects.GetComponent<Animator>().speed = 0;
            //StartCoroutine(StopAnim(mainObjects.GetComponent<Animator>()));
            for (int i = 0; i < particles.Length; i++)
                particles[i].Pause();
        }
    }


    // Переход в магазин
    IEnumerator MainToShopGo()
    {
        LoadMainScenes(false);
        yield return new WaitForSeconds(0.5f);
        if (animCamera.activeInHierarchy)
        {
            cameraObject.GetComponent<Camera>().orthographicSize = animCamera.GetComponent<Camera>().orthographicSize;
            cameraObject.SetActive(true);
            animCamera.SetActive(false);
        }
        shopUI.SetActive(true);
    }

    // Плавное торможение анимации
    IEnumerator StopAnim(Animator anim)
    {
        while(anim.speed > 0)
        {
            yield return new WaitForSeconds(0.05f);
            if(anim.speed > 0.1f)
                anim.speed -= 0.1f;
            else
                anim.speed = 0;
        }
        yield return 0;
    }

    // Плавное торможение анимации
    IEnumerator PlayAnim(Animator anim)
    {
        while (anim.speed < 1)
        {
            yield return new WaitForSeconds(0.05f);
            if (anim.speed < 0.90f)
                anim.speed += 0.1f;
            else
                anim.speed = 1;
            
        }
        yield return 0;
    }

    IEnumerator ShopToMainGo()
    {
        yield return new WaitForSeconds(0.7f);
        if (cameraObject.activeInHierarchy)
        {
            animCamera.SetActive(true);
            cameraObject.SetActive(false);
        }
        shopUI.SetActive(false);
        
        mainUi.SetActive(true);
        animControl(true);
        mainObjects.GetComponent<Animator>().speed = 1;
    }
    #endregion
     
    

    #region Credits

    public GameObject creditPanel;                                  // Окно с кредитами
    /*
    public void OpenCredits()
    {
        AudioEffects.PlayEffects(clickClip);
        creditPanel.SetActive(true);
    }
    */
    public void MainToCredits() {
        AudioManager.PlayEffect(clickClip, AudioMixerTypes.runnerEffect);
        main = false;
        animControl(false);
        StartCoroutine(MainToCreditsGo());
    }

    public void CreditsToMain() {
        AudioManager.PlayEffect(clickClip, AudioMixerTypes.runnerEffect);

        main = true;
        //animControl(false);
        StartCoroutine(CreditsToMainGo());
    }

    IEnumerator MainToCreditsGo() {
        LoadMainScenes(false);
        yield return new WaitForSeconds(0.5f);
        if (animCamera.activeInHierarchy) {
            cameraObject.GetComponent<Camera>().orthographicSize = animCamera.GetComponent<Camera>().orthographicSize;
            cameraObject.SetActive(true);
            animCamera.SetActive(false);
        }
        creditPanel.SetActive(true);
    }

    IEnumerator CreditsToMainGo() {
        yield return new WaitForSeconds(0.5f);
        if (cameraObject.activeInHierarchy) {
            animCamera.SetActive(true);
            cameraObject.SetActive(false);
        }
        creditPanel.SetActive(false);

        mainUi.SetActive(true);
        animControl(true);
        mainObjects.GetComponent<Animator>().speed = 1;
    }



    #endregion
}
