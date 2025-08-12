using System.Collections.Generic;
using UnityEngine;

namespace Game.Weapon {


  public class ManagerObrez: WeaponStandart {

    [HideInInspector]
    public int drobCount;
    private Vector3 endTap;
    private bool isShoot;
    private int shootCount = 0;

    private List<Enemy> damageEnemy = new List<Enemy>();

    public override void Inicialized() {
      base.Inicialized();
      isShoot = false;
    }

    protected override void OnClickDown(Vector3 position) {
      if (!CheckReadyShoot(position, position))
        return;

      damageEnemy.Clear();
      isShoot = true;
      shootCount = 2;

      StartShootAnimation();
    }

    protected override void OnClickUp(Vector3 position) {
      isShoot = false;
    }

    protected override void OnShootAnimEvent() {
      Shoot();
    }

    private void Shoot() {
      endTap = pointerDown;

      _oldStart = PlayerController.Instance.ShootPoint;
      _oldEnd = pointerDown;

      if (pointerDown.x < PlayerController.Instance.transform.position.x)
        pointerDown.x = PlayerController.Instance.transform.position.x;

      OnShootObrez();

      shootCount--;

      if (shootCount <= 0) {
        isShoot = false;
        PlayReload();
        BulletDecrement();
        OnShootComplited();
      }


      //float angle = -25;
      //float anglRadian = angle * (Mathf.PI / 180);

      //Vector2 alterTapEnd = pointerDown - PlayerController.Instance.shootPoint.position;
      //_oldEnd2 = new Vector3(alterTapEnd.x * Mathf.Cos(anglRadian) - alterTapEnd.y * Mathf.Sin(anglRadian),
      //             alterTapEnd.x * Mathf.Sin(anglRadian) + alterTapEnd.y * Mathf.Cos(anglRadian),
      //             0) + PlayerController.Instance.shootPoint.position;
      //angle = 25;
      //anglRadian = angle * (Mathf.PI / 180);

      //alterTapEnd = pointerDown - PlayerController.Instance.shootPoint.position;
      //_oldEnd3 = new Vector3(alterTapEnd.x * Mathf.Cos(anglRadian) - alterTapEnd.y * Mathf.Sin(anglRadian),
      //             alterTapEnd.x * Mathf.Sin(anglRadian) + alterTapEnd.y * Mathf.Cos(anglRadian),
      //             0) + PlayerController.Instance.shootPoint.position;

    }
    
    private Vector3 _oldStart;
    private Vector3 _oldEnd;
    //private Vector3 _oldEnd2;
    //private Vector3 _oldEnd3;

    public void OnShootObrez() {
      //for (int i = 0; i < drobCount; i++) {
        GameObject drobMas = PoolerManager.Spawn(bulletPrefab.name);
        //drobMas.SetActive(false);
        drobMas.transform.position = PlayerController.Instance.ShootPoint;
      drobMas.GetComponent<ShotgunBullet>().DrobCount(drobCount);
        drobMas.SetActive(true);
        drobMas.GetComponent<Bullet>().Shot(PlayerController.Instance.ShootPoint, endTap);
        //drobMas.GetComponent<Bullet>().OnDamageEnemy = (enem) => {

        //  if (!damageEnemy.Contains(enem)) {
        //    damageEnemy.Add(enem);
        //    BattleEventEffects.Instance.VisualEffect(BattleEffectsType.hunterObrezShoot, enem.transform.position, this);
        //  }

        //};
        InvokeCustom(0.3f, PlaySleeveDopAudio);
      //}
    }

    public override void GetConfig() {
      base.GetConfig();

      drobCount = (int)wepConfig.param1.Value;

    }

    public AudioBlock sleeveDropAudioBlock;
    protected virtual void PlaySleeveDopAudio() {
      sleeveDropAudioBlock.PlayRandom(PlayerController.Instance);
    }

    public AudioBlock emptyMagazineAudioBlock;
    protected override void PlayEmptyAudioBlock() {
      if(!emptyMagazineAudioBlock.IsPlaying)
        emptyMagazineAudioBlock.PlayRandom(PlayerController.Instance);
    }

    protected override void OnReloadStart()
    {
      base.OnReloadStart();
      InvokeCustom(TimeReaload-1, PlayReloadCompleteBlock);
    }
    public AudioBlock reloadCompleteAudioBlock;
    protected void PlayReloadCompleteBlock() {
      reloadCompleteAudioBlock.PlayRandom(PlayerController.Instance);
    }
  }


}