/*
  Контроллер Джека на главной странице
*/
using UnityEngine;
using Spine.Unity;

public class JackMainController : MonoBehaviour {

    public SceneMainCintroller mainController;

    public SkeletonAnimation skeletonAnimation;         // Ссылка на скелетную анимацию
    string currentAnimation;                            // Текущая анимация

    [SpineAnimation(dataField: "skeletonAnimation")]
    public string idleAnim = "";                        // Анимация в стандартном положении
    [SpineAnimation(dataField: "skeletonAnimation")]
    public string cristalConnectAnim = "";              // Анимация xватания кристала
    [SpineAnimation(dataField: "skeletonAnimation")]
    public string idleFunAnim = "";                     // Анимация удерживания кристала
    [SpineAnimation(dataField: "skeletonAnimation")]
    public string idleFunEyesAnim = "";                 // Анимация удерживания кристала + глаза
    [SpineAnimation(dataField: "skeletonAnimation")]
    public string idleWonderAnim = "";                  // Анимация удивления
    [SpineAnimation(dataField: "skeletonAnimation")]
    public string idleScaredAnim = "";                  // Анимация дрожи

    string nextAnim;
    bool loopAnim;

    void Start()
    {
        skeletonAnimation.state.End += AnimEnd;
        skeletonAnimation.state.Start += AnimStart;
        skeletonAnimation.state.Event += AnimEvent;
        loopAnim = true;
        nextAnim = idleAnim;
    }

    public void StartAnim()
    {
        loopAnim = false;
        nextAnim = cristalConnectAnim;
    }

    public void pause(bool flag)
    {
        if (flag)
        {
            skeletonAnimation.timeScale = 0;
        }
        else
        {
            skeletonAnimation.timeScale = 1;
        }
    }

    void Update()
    {
        if(nextAnim != currentAnimation)
            SetAnimation(nextAnim, loopAnim);
    }

    /* ***************************
     * Событие на окончание анимации
     * ***************************/
    void AnimEnd(Spine.AnimationState state, int trackIndex)
    {

        if (state.GetCurrent(trackIndex).ToString() == idleWonderAnim)
        {
            loopAnim = true;
            nextAnim = idleScaredAnim;
        }
        if (state.GetCurrent(trackIndex).ToString() == idleFunEyesAnim)
        {
            loopAnim = false;
            nextAnim = idleWonderAnim;
        }
        if (state.GetCurrent(trackIndex).ToString() == idleFunAnim)
        {
            loopAnim = false;
            nextAnim = idleFunEyesAnim;
        }
        if (state.GetCurrent(trackIndex).ToString() == cristalConnectAnim)
        {
            loopAnim = false;
            nextAnim = idleFunAnim;
        }
    }

    /* ***************************
     * Событие на Начало
     * ***************************/
    void AnimStart(Spine.AnimationState state, int trackIndex)
    {

        if (state.GetCurrent(trackIndex).ToString() == idleFunAnim)
        {
            mainController.StartParticles();
        }

        if (state.GetCurrent(trackIndex).ToString() == idleWonderAnim)
        {
            mainController.StartWarning();
        }

        if (state.GetCurrent(trackIndex).ToString() == idleScaredAnim)
        {
            mainController.StartWarningMain();
        }

    }

    void AnimEvent(Spine.AnimationState state, int trackIndex, Spine.Event e)
    {
        if (state.GetCurrent(trackIndex).ToString() == cristalConnectAnim)
        {
            mainController.cristal.SetActive(false);
        }
    }

    #region Animation
    /* ***************************
     * Применение анимации
     * ***************************/
    public void SetAnimation(string anim, bool loop)
    {
        if (currentAnimation != anim)
        {
            skeletonAnimation.state.SetAnimation(0, anim, loop);
            currentAnimation = anim;
        }
    }


    /* ***************************
     * Резет анимации
     * ***************************/
    public void ResetAnimation()
    {
        skeletonAnimation.Initialize(true);
        currentAnimation = null;
    }


    /* ***************************
     * Добавленная анимация
     * ***************************/
    public void AddAnimation(int index, string animName, bool loop, float delay)
    {
        skeletonAnimation.state.AddAnimation(index, animName, loop, delay);
    }

    /* ***************************
     * Установка скорости
     * ***************************/
    public void SpeedAnimation(float speed)
    {
        skeletonAnimation.timeScale = speed;
    }
    #endregion
}
