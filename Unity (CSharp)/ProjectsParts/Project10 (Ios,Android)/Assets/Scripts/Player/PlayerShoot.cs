using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player.Jack {

  public class PlayerShoot: PlayerComponent {

    private int shootAllCount = 0;                              // Общее число выстрелов
    public List<GameObject> weapons;                        // Массив орудий
    private bool isShooting = false;                             // Атака выполняется
    private readonly float shootTime;                                    // Время выаолнения атаки
    private bool shootDec = false;                              // Флаг, что декремент счетчика выполнен

    private WeaponTypes activeWeapon;

    private void Start() { }

    /// <summary>
    /// Реакция на кнопку атаку
    /// </summary>
    /// <param name="weaponType"></param>
    public void Shoot(WeaponTypes weaponType) {
      // Если выстрел выполняется, отказываем
      if (isShooting)
        return;
      Invoke(DeactiveShoot, .7f);

      ShootNow(weaponType);
      Invoke(CreatePool, .2f);
    }

    void CreatePool() {
      weapons.ForEach(x => Pooler.Instance.AddPool(x, 3));
    }

    /// <summary>
    /// Отключение состояния атаки
    /// </summary>
    void DeactiveShoot() {
      isShooting = false;
    }

    /// <summary>
    /// Атака
    /// </summary>
    /// <param name="weaponType"></param>
    private void ShootNow(WeaponTypes weaponType) {
      isShooting = true;
      activeWeapon = weaponType;

      animation.timeScale = 1f;

      Questions.QuestionManager.AddUseWeapon(weaponType, transform.position);

      if (weaponType == WeaponTypes.trap) {
        animation.AddAnimation(2, animation.trapShootAnim, false, 0);
        audio.PlayEffect(audio.trapClip, AudioMixerTypes.runnerEffect);
      }

      if (weaponType == WeaponTypes.sabel) {
        animation.skeletonRenderer.skeleton.FindSlot(animation.candySlot).A = 1;
        animation.AddAnimation(2, animation.swordShootAnim, false, 0);
        audio.PlayEffect(audio.swordClip, AudioMixerTypes.runnerEffect);
      }

      if (weaponType == WeaponTypes.gun) {
        animation.AddAnimation(2, animation.pistolShootAnim, false, 0);
      }

      if (weaponType == WeaponTypes.bomb) {
        animation.skeletonRenderer.skeleton.FindSlot(animation.chocolateSlot).A = 1;
        animation.AddAnimation(2, animation.bombShootAnim, false, 0);
        audio.PlayEffect(audio.bombClip, AudioMixerTypes.runnerEffect);
      }

      if (weaponType == WeaponTypes.molotov) {
        animation.AddAnimation(2, animation.coctailShootAnim, false, 0);
        audio.PlayEffect(audio.molotovClip, AudioMixerTypes.runnerEffect);
      }

      if (weaponType == WeaponTypes.ship) {
        animation.AddAnimation(2, animation.cannonShootAnim, false, 0);
        controller.PlayBravoAudio();
      }

      if (weaponType == WeaponTypes.chocolate) {
        animation.skeletonRenderer.skeleton.FindSlot(animation.chocolateSlot).A = 0;
        GameObject wep = Instantiate(weapons[8], transform.position, Quaternion.identity) as GameObject;
        wep.AddComponent<BoneFollower>();
        wep.GetComponent<BoneFollower>().skeletonRenderer = animation.skeletonRenderer;
        wep.GetComponent<BoneFollower>().boneName = animation.WeaponBoneeName;
        wep.GetComponent<BoneFollower>().followBoneRotation = true;
        wep.GetComponent<BoneFollower>().followZPosition = true;

        animation.AddAnimation(2, animation.bombShootAnim, false, 0);
      }

      if (weaponType == WeaponTypes.flowers) {
        animation.skeletonRenderer.skeleton.FindSlot(animation.chocolateSlot).A = 0;
        GameObject wep = Instantiate(weapons[7], transform.position, Quaternion.identity) as GameObject;
        wep.AddComponent<BoneFollower>();
        wep.GetComponent<BoneFollower>().skeletonRenderer = animation.skeletonRenderer;
        wep.GetComponent<BoneFollower>().boneName = animation.WeaponBoneeName;
        wep.GetComponent<BoneFollower>().followBoneRotation = true;
        wep.GetComponent<BoneFollower>().followZPosition = true;
        animation.AddAnimation(2, animation.bombShootAnim, false, 0);
      }

      if (weaponType == WeaponTypes.candy) {
        animation.skeletonRenderer.skeleton.FindSlot(animation.candySlot).A = 0;
        GameObject wep = Instantiate(weapons[6], transform.position, Quaternion.identity) as GameObject;
        wep.AddComponent<BoneFollower>();
        wep.GetComponent<BoneFollower>().skeletonRenderer = animation.skeletonRenderer;
        wep.GetComponent<BoneFollower>().boneName = animation.WeaponBoneeName;
        wep.GetComponent<BoneFollower>().followBoneRotation = true;
        wep.GetComponent<BoneFollower>().followZPosition = true;
        animation.AddAnimation(2, animation.swordShootAnim, false, 0);
        audio.PlayEffect(audio.swordClip, AudioMixerTypes.runnerEffect);
      }

      shootDec = false;
    }

    IEnumerator HideSlot() {
      yield return new WaitForSeconds(0.2f);
      animation.skeletonRenderer.skeleton.FindSlot(animation.candySlot).A = 0;
    }

    IEnumerator ShipShoop() {
      yield return new WaitForSeconds(0.8f);
      audio.PlayEffect(audio.shipClip, AudioMixerTypes.runnerEffect);
      yield return new WaitForSeconds(Random.Range(0.05f, 0.2f));
      audio.PlayEffect(audio.shipClip, AudioMixerTypes.runnerEffect);
      yield return new WaitForSeconds(Random.Range(0.05f, 0.2f));
      audio.PlayEffect(audio.shipClip, AudioMixerTypes.runnerEffect);
    }

    public WeaponTypes GetActiveWeapon() {
      return activeWeapon;
    }

    /// <summary>
    /// Реакция на атаку
    /// </summary>
    public void ShootEvent() {
      WeaponTypes weaponType = activeWeapon;

      shootAllCount++;

      if (!shootDec) {
        controller.ShootPlayer();
        shootDec = true;
      }

      if (weaponType == WeaponTypes.trap) {
        GameObject inst = Pooler.GetPooledObject(weapons[0].name);
        inst.transform.position = new Vector3(transform.position.x - 0.1f, transform.position.y + 1, 0);
        inst.SetActive(true);
      }

      if (weaponType == WeaponTypes.sabel) {
        GameObject inst = Pooler.GetPooledObject(weapons[1].name);
        inst.transform.position = new Vector3(transform.position.x - 0.75f, transform.position.y - 0f, 0);
        inst.SetActive(true);
      }

      if (weaponType == WeaponTypes.gun) {
        GameObject inst = Pooler.GetPooledObject(weapons[2].name);
        inst.transform.position = new Vector3(transform.position.x - 1.35f, transform.position.y, 0);
        inst.SetActive(true);
        audio.PlayEffect(audio.gunClip, AudioMixerTypes.runnerEffect);
      }

      if (weaponType == WeaponTypes.bomb) {
        GameObject inst = Pooler.GetPooledObject(weapons[3].name);
        inst.transform.position = new Vector3(transform.position.x - 0.5f, transform.position.y + 0.8f, 0);
        inst.SetActive(true);

      }

      if (weaponType == WeaponTypes.molotov) {
        GameObject inst = Pooler.GetPooledObject(weapons[4].name);
        inst.transform.position = new Vector3(transform.position.x - 0.5f, transform.position.y + 0.8f, 0);
        inst.SetActive(true);
      }

      if (weaponType == WeaponTypes.ship) {

        for (int i = 0; i < 3; i++) {
          GameObject inst = Pooler.GetPooledObject(weapons[5].name);
          inst.transform.position = new Vector3(CameraController.displayDiff.rightDif(1) + Random.Range(0.3f, 0.7f), CameraController.displayDiff.transform.position.y + CameraController.displayDiff.top + Random.Range(0, 0.3f), 0);
          inst.SetActive(true);
        }
      }
    }
  }
}