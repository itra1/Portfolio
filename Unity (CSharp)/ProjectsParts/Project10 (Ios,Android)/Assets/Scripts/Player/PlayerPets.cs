using Spine.Unity;
using System;
using UnityEngine;
namespace Player.Jack {
  [Flags]
  public enum PetsTypes {
    none = 0,
    bat = 1,
    dino = 2,
    spider = 4
  }

  public class PlayerPets: PlayerComponent {

    public bool IsPet {
      get {
        return instance != null && ((instance.type & (PetsTypes.dino | PetsTypes.spider | PetsTypes.bat)) != 0);
      }
    }

    public PetsTypes Type {
      get {
        return instance == null ? PetsTypes.none : instance.type;
      }
    }

    /// <summary>
    /// Контроллер пета
    /// </summary>
    public Pet.Pet instance;

    public void Shoot() {
      instance.Shoot();
    }

    [HideInInspector]
    public bool playerMuveUp;
    private Vector3 lastPosition;
    private Vector3 deltaPosition;

    void Update() {

      if (!IsPet) return;

      deltaPosition = transform.position - lastPosition;
      if (deltaPosition.y >= 0.1f)
        playerMuveUp = true;
      else
        playerMuveUp = false;

      lastPosition = transform.position;
    }

    public void EnablePet(Pet.Pet controller, SkeletonAnimation skeletonAnimation, string slotEnable, float timeUse) {
      SaveJackParametrs();
      instance = controller;

      animation.SetAnimation(animation.mountsSlowAnim, true);

      BoneFollower bonFollow = gameObject.AddComponent<BoneFollower>();
      bonFollow.skeletonRenderer = skeletonAnimation;
      bonFollow.boneName = slotEnable;
      bonFollow.followZPosition = true;
      bonFollow.followBoneRotation = false;
      bonFollow.Reset();

      GetComponent<ShadowController>().Show(false);
      RunnerController.petActivate(instance.type, true, timeUse);

      switch (instance.type) {
        case PetsTypes.bat:
          this.controller.boxDamageCollider.offset = new Vector3(0, 0.21f, 0);
          this.controller.boxDamageCollider.size = new Vector3(1.69f, 1.52f, 0.6f);
          animation.skeletonAnimation.transform.localPosition = new Vector3(-0.086f, -0.313f, 0);
          if (this.controller != null)
            animation.SetAnimation(animation.mountsFlyAnim, true);
          break;
        case PetsTypes.spider:
          animation.skeletonAnimation.transform.localPosition = new Vector3(0.171f, -0.637f, 0);
          break;
        default:
          animation.skeletonAnimation.transform.localPosition = new Vector3(0.22f, -0.632f, 0);
          break;

      }
      moveManager.ChangeComponent();
    }

    /// <summary>
    /// Отключаем пета, если активный
    /// </summary>
    public void PetDisconnect(bool damage = false) {
      if (instance == null) return;

      ReturnJackParametrs();

      BoneFollower boneFollow = GetComponent<BoneFollower>();
      if (boneFollow != null) Destroy(boneFollow);

      RunnerController.petActivate(instance.type, false);
      instance.JackDisActive(damage);
      controller.rigidbody.velocity = new Vector2(controller.rigidbody.velocity.x, 0);

      controller.SetGraceDamage(true);

      gameObject.transform.Rotate(Vector3.zero);
      if (controller != null)
        animation.ResetAnimation();
      else
        animation.ResetAnimation();
      GetComponent<ShadowController>().Show(true);

      instance = null;
      ScalingGraphic(false);
      moveManager.ChangeComponent();
    }

    #region Позиционирование компонентов

    private Vector3 boxCenter;
    private Vector3 boxSize;
    private Vector3 spineLocalPosition;

    void SaveJackParametrs() {
      boxCenter = controller.boxDamageCollider.offset;
      boxSize = controller.boxDamageCollider.size;
      spineLocalPosition = animation.skeletonAnimation.transform.localPosition;
    }

    public void ScalingGraphic(bool top) {
      controller.graphic.transform.localScale = top 
        ? new Vector3(1, -1, 1) 
        : new Vector3(1, 1, 1);
    }

    void ReturnJackParametrs() {
      controller.boxDamageCollider.offset = boxCenter;
      controller.boxDamageCollider.size = boxSize;
      animation.skeletonAnimation.transform.localPosition = spineLocalPosition;
    }

    #endregion

    //#region Реакция на управление джеком

    ///// <summary>
    ///// Реакция на нажание прыжка
    ///// </summary>
    ///// <param name="flag"></param>
    //public void CanJump(bool flag) {
    //  base.pet.CanJump(flag);
    //}

    //public void Movement(float mov) {
    //  base.pet.Movement(mov);
    //}

    //#endregion

  }
}