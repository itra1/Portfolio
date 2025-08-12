using UnityEngine;
using Spine.Unity;

namespace Player.Jack {

  public class PlayerAnimation: PlayerComponent {

    public readonly string leftDamageAnim = "Damage_Back";                  // Анимация дамага сзади
    public readonly string rightDamageAnim = "Damage_Front";                // Анимация дамага спереди
    public readonly string wonderAnim = "Emo_Wonder";                       // Анимация эмоций Джека
    public readonly string weaponAttackMagicAnim = "Weapon_Attack_Panic";   // Анимация анимация магической атаки
    public readonly string spearDefenderAnim = "Spear_Defence";             // Анимация отбивания копья
    public readonly string gemGetAnim = "JumpToGem";                        // Анимация хватания кристала
    public readonly string gemIdleAnim = "JumpToGem_Idle";                  // Анимация хватания кристала
    public readonly string runIdleAnim = "Run_Common";                      // Анимация бега
    public readonly string jumpIdleAnim = "Jump_Idle";                      // Анимация полета после прыжка
    public readonly string jumpDoubleIdleAnim = "Jump_Backfip_Idle";        // Анимация отталкивания прри двойном прыжке
    public readonly string deadAnim = "Death_Fall_Long_No_Move";            // Анимация смерти на земле
    public readonly string deadJumpAnim = "Death_Fall_Long_No_Move";        // Анимация смерти в полете

    public readonly string boostBarrierAnim = "Boost_Barrel";                    // Анимация бега на бочке
    public readonly string fastSpeedsAnim = "Boost_FastSpeed";                      // Анимация ускоренного бега
    public readonly string bigWheeAnim = "Boost_Wheel";                   // Анимация бега на бочке
    public readonly string skateAnim = "Boost_Skate";                            // Анимация бега на бочке
    public readonly string shipAnim = "Boost_Ship";                            // Анимация бега на бочке
    public readonly string flyAnim = "Boost_Fly";                             // Анимация бега на бочке
                                                                              //public readonly string bigWheeBarrierAnim = "Boost_Whell";                   // Анимация бега на бочке

    public readonly string trapShootAnim = "Weapon_Trap";                   // Анимация атака капканом
    public readonly string swordShootAnim = "Weapon_Saber";                  // Анимация атаки саблей
    public readonly string pistolShootAnim = "Weapon_Gun";                 // Анимация выстрела из пистолета
    public readonly string bombShootAnim = "Weapon_Bomb";                   // Анимация бросания бомбы
    public readonly string coctailShootAnim = "Weapon_Coctail";                // Анимация броска коктейля
    public readonly string cannonShootAnim = "Weapon_Attack_Brave";                 // Анмиация атаки с корабля
    public readonly string WeaponBoneeName = "Hand_L_3";
    public readonly string chocolateSlot = "Bomb";
    public readonly string flowersSlot = "Bomb";
    public readonly string candySlot = "Sabre_2";

    public readonly string mountsFastAnim = "Mounts_Fast";                              // Анимация посадки на пета при быстром беге
    public readonly string mountsSlowAnim = "Mounts_Slow";                              // Анимация посадки на пета при шаге
    public readonly string mountsFlyAnim = "Mount_Fly";                               // Анимация посадки на пета при полете

    public SkeletonAnimation skeletonAnimation;         // Ссылка на скелетную анимацию
    public SkeletonRenderer skeletonRenderer;
    public AtlasAsset atlasAsset;                   // Рендер кости

    public event Spine.AnimationState.StartEndDelegate onAnimEnd;
    public event Spine.AnimationState.EventDelegate onAnimEvent;
    public event Spine.Unity.SkeletonRenderer.SkeletonRendererDelegate onRebuild;

    private string currentAnimation;

    private void Start() { }

    public float timeScale {
      get { return skeletonAnimation.timeScale; }
      set { skeletonAnimation.timeScale = value; }
    }

    public Spine.TrackEntry AddAnimation(int trackIndex, string animationName, bool loop = false, float delay = 0) {
      return skeletonAnimation.state.AddAnimation(trackIndex, animationName, loop, delay);
    }

    public Spine.TrackEntry SetAnimation(string animationName, bool loop, bool necessary = false) {


      if (necessary || currentAnimation != animationName) {
        Debug.Log(animationName);
        currentAnimation = animationName;
        animation.SetAnimation(0, animationName, loop);
        return skeletonAnimation.state.SetAnimation(0, animationName, loop);
      }

      return skeletonAnimation.state.GetCurrent(0);
    }

    public Spine.TrackEntry SetAnimation(int trackIndex, string animationName, bool loop, bool necessary = false) {

      if (necessary || currentAnimation != animationName) {
        currentAnimation = animationName;
        animation.SetAnimation(0, animationName, loop);
        return skeletonAnimation.state.SetAnimation(trackIndex, animationName, loop);
      }

      return skeletonAnimation.state.GetCurrent(trackIndex);
    }

    public void ResetAnimation() {

      skeletonAnimation.Initialize(true);
      skeletonAnimation.state.Event += AnimEvent;
      skeletonAnimation.state.End += AnimEnd;
      skeletonAnimation.OnRebuild += OnRebuild;
      currentAnimation = null;
    }

    /// <summary>s
    /// Событие анимации
    /// </summary>
    /// <param name="state"></param>
    /// <param name="trackIndex"></param>
    /// <param name="e"></param>
    void AnimEvent(Spine.AnimationState state, int trackIndex, Spine.Event e) {
      if (onAnimEvent != null) onAnimEvent(state, trackIndex, e);
    }

    /// <summary>
    /// Окончание анимации
    /// </summary>
    /// <param name="state"></param>
    /// <param name="trackIndex"></param>
    void AnimEnd(Spine.AnimationState state, int trackIndex) {
      if (onAnimEnd != null) onAnimEnd(state, trackIndex);
    }

    void OnRebuild(SkeletonRenderer skeletonRenderer) {
      if (onRebuild != null) onRebuild(skeletonRenderer);
    }

  }
}