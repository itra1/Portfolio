/*
  Контроллер Змеи на главной странице
*/

using UnityEngine;
using Spine.Unity;

public class SnakeMainController : MonoBehaviour {

    public SceneMainCintroller mainController;

    public SkeletonAnimation skeletonAnimation;         // Ссылка на скелетную анимацию
    string currentAnimation;                            // Текущая анимация

    [SpineAnimation(dataField: "skeletonAnimation")]
    public string idleAnim = "";                        // Анимация в стандартном положении
    [SpineAnimation(dataField: "skeletonAnimation")]
    public string hideAnim = "";                        // Анимация скрытия

    void Start()
    {
        SetAnimation(idleAnim, true);
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

    public void HideSnake()
    {
        SetAnimation(hideAnim, false);
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
