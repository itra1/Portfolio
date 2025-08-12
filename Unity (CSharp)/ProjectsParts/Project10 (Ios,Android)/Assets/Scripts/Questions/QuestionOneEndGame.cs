using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class QuestionOneEndGame : MonoBehaviour {
    
    public Text titleText;
    public Text countText;

    public GameObject iconStar;
    public GameObject iconConpl;

    public GameObject lineText;
    public AudioClip complClip;

    bool audioActive;

    public void QuestComplAudio() {

        if(audioActive) return;
        audioActive = true;
        if(GetComponent<Animator>().GetBool("complitedNoAnim")) return;
        AudioManager.PlayEffect(complClip, AudioMixerTypes.runnerUi);
    }

    //Animator anim;

    /*
    
    public void SetStatusQuest( bool wait, Question item) {

        //anim = GetComponent<Animator>();

        if (item.needvalue <= item.value && !wait) {
            iconConpl.SetActive(true);
            lineText.SetActive(true);
            GetComponent<Animator>().SetBool("complitedNoAnim", true);
        }
                
    }
    
    public void ShowComplitedAnim() {
        if (!waitComplAnim) {
            gameObject.GetComponentInParent<QuestionsEndGame>().ShowQuestElevAnim();
        } else {
            GetComponent<Animator>().SetTrigger("active");

        }
    }

    public void EndActiveAnim() {
        if(waitComplAnim)
            StartCoroutine(Next());
    }

    IEnumerator Next() {
        yield return new WaitForSeconds(0.5f);
        gameObject.GetComponentInParent<QuestionsEndGame>().ShowQuestElevAnim();
    }

    public void CloseThis() {
        
        anim.SetBool("open", false);
    }
    */
}
