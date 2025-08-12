using UnityEngine;
using System.Collections;
using Spine.Unity;

public class SplashController : MonoBehaviour {

    public KingBirdsLogo king;                          // Логотип кингбирд
    public GameObject cristal;                          // Объект кристалл
    Animator anim;                                      
    AudioSource audi;

    public string muveToScene;                          // Переход в сцену

    public AudioClip stepAudi;
    public AudioClip golosAudi;

    void Start()
    {
        anim = GetComponent<Animator>();
        audi = GetComponent<AudioSource>();
    }
    
    public void StartAnim()
    {
        anim.SetTrigger("start");
        audi.loop = true;
        audi.clip = stepAudi;
        audi.Play();
    }
    

	public void EndFirstAnim()
    {
        audi.Stop();
        anim.SetTrigger("endCristalAnim");
        king.StoleCristal();
    }

    public void RunStole()
    {
        audi.loop = true;
        audi.pitch = 1.5f;
        audi.clip = stepAudi;
        audi.Play();
        BoneFollower bon = cristal.GetComponent<BoneFollower>();
        bon.enabled = true;
        bon.boneName = "Axe";
        king.RunStole();
    }

    public void EndNewWiew()
    {
        audi.Stop();
        king.RunEnd();
    }

    public void ShowFullLogo()
    {
        isEnd = true;
        anim.SetTrigger("showLogo");
        audi.pitch = 1;
        audi.loop = false;
        audi.PlayOneShot(golosAudi, 1);
    }

    public void HideCristal() {
        cristal.SetActive(false);
    }

    public void FullShow() {
        StartCoroutine(showSplashBlack());
    }

    IEnumerator showSplashBlack() {
        yield return new WaitForSeconds(1.5f);
        GameManager.LoadScene(muveToScene);
        //anim.SetTrigger("nextLevel");
    }

    public void LoadFirstScene()
    {
        GameManager.LoadScene(muveToScene);
    }

    bool isEnd;

    public void SpeedEnd() {
        if (isEnd) return;
        isEnd = true;
        anim.SetTrigger("endNow");
        king.RunEnd();
        cristal.SetActive(false);
    }
}
