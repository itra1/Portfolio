using UnityEngine;
using Spine.Unity;

public class KingBirdsLogo : MonoBehaviour {

    public SplashController splash;

    public SkeletonAnimation skeletonAnimation;         // Ссылка на скелетную анимацию
    string currentAnimation;                            // Текущая анимация

    [SpineAnimation(dataField: "skeletonAnimation")]
    public string runAnim = "";                        // Анимация 
    [SpineAnimation(dataField: "skeletonAnimation")]
    public string stoleAnim = "";                        // Анимация 
    [SpineAnimation(dataField: "skeletonAnimation")]
    public string stoleRunAnim = "";                        // Анимация 
    [SpineAnimation(dataField: "skeletonAnimation")]
    public string stoleEndingAnim = "";                        // Анимация 


    void Start()
    {
        skeletonAnimation.state.End += EndAnim;
        SetAnimation(runAnim, true);
        skeletonAnimation.timeScale = 1.6f;
    }

    public void StoleCristal()
    {
        skeletonAnimation.timeScale = 1.3f;
        SetAnimation(stoleAnim, false);
    }
    public void RunStole()
    {
        skeletonAnimation.timeScale = 1.3f;
        SetAnimation(stoleRunAnim, true);
    }
    public void RunEnd()
    {
        skeletonAnimation.timeScale = 1.3f;
        SetAnimation(stoleEndingAnim, false);
    }

    public void EndAnim(Spine.AnimationState state, int trackIndex)
    {
        if (state.GetCurrent(trackIndex).ToString() == stoleEndingAnim)
        {
            splash.ShowFullLogo();
        }
    }

    /*
    void Update()
    {
        if (nextAnim != currentAnimation)
            SetAnimation(nextAnim, loopAnim);
    }
    */
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
