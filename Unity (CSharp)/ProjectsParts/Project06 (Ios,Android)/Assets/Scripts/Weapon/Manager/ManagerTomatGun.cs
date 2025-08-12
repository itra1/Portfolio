using UnityEngine;

namespace Game.Weapon {

  public class ManagerTomatGun: WeaponStandart {

    private float _timeLastShoot = -1000;
    private bool isShoot = false;

    private Vector3 _shootTarget;
    private float damageKoeff;


    public override void Inicialized() {
      base.Inicialized();
      isShoot = false;
    }

    protected override void OnClickDown(Vector3 position) {
      if (!CheckReadyShoot(position, position))
        return;

      isShoot = true;
      _shootTarget = position;

    }

    protected override void OnClickUp(Vector3 position) {
      isShoot = false;
    }

    protected override void OnClickDrag(Vector3 position, Vector3 delta) {
      base.OnClickDrag(position, delta);
      pointerDown = position;
    }

    protected override void Update() {
      if (!isShoot)
        return;

      if (Magazin <= 0) {
        SetActiveStatus(true);
        isShoot = false;
        PlayReload();
        OnShootComplited();
        return;
      }

      if (_timeLastShoot + TimeBetweenShoot > Time.time)
        return;
      //pointerDown = mousePosition
      if (pointerDown.x < PlayerController.Instance.ShootPoint.x)
        pointerDown.x = PlayerController.Instance.ShootPoint.x;

      PlaygunShootAudio();

      GameObject weapon = PoolerManager.Spawn(bulletPrefab.name);
      weapon.transform.position = PlayerController.Instance.forwardSmoke.position;
      weapon.SetActive(true);
      weapon.GetComponent<Bullet>().Shot(PlayerController.Instance.forwardSmoke.position, pointerDown);
      BulletDecrement();

      PlayShootAudio();

      _timeLastShoot = Time.time;
    }


    //public override void Inicialized() {
    //  base.Inicialized();
    //  isShoot = false;
    //}

    //public override void OnPointerDown(Vector3 pos) {
    //  base.OnPointerDown(pos);
    //  _shootTarget = Camera.main.ScreenToWorldPoint(new Vector3(pos.x, pos.y, 10));
    //}

    //public override void Shoot(Vector3 tapStart, Vector3 tapEnd) {

    //  OnShoot(tapStart, tapEnd);
    //}

    //public override void StartShooting(Vector3 tapStart, Vector3 tapEnd) {
    //  if (isShoot)
    //    return;
    //  if (!CheckReadyShoot(tapStart, tapEnd))
    //    return;

    //  if (Magazin <= 0) {
    //    Reload();
    //    SetActiveStatus(true);
    //    //OnShoot(tapStart, tapEnd);
    //    isShoot = true;
    //  }

    //  base.StartShooting(tapStart, tapEnd);

    //  PlayerController.Instance.playerAnimation.OnAnimComplited += AnimCompleted;
    //  StartShootAnimation();
    //  //PlayerController.Instance.playerAnimation.SetAnimation(0, playerStartAnim, false);
    //}

    //protected override void StartShootAnimation() {

    //  //PlayerController.Instance.SetAnimation(0, tomatoStartAnim, false);
    //  base.StartShootAnimation();
    //}

    //protected override void OnShootComplited(bool isReload = true) {
    //  PlayerController.Instance.playerAnimation.OnAnimComplited -= AnimCompleted;
    //  base.OnShootComplited(isReload);
    //}

    //public virtual void AnimCompleted(string trackEntry) {


    //  if (isTap && Magazin > 0) {
    //    PlayerController.Instance.playerAnimation.SetAnimation(0, playerShootAnim[Random.Range(0, playerShootAnim.Count)], false);
    //  } else {

    //    isShoot = false;

    //    base.OnShoot();
    //    OnShootComplited();
    //    _timeLastShoot = Time.time;

    //    WeaponGenerator.Instance.changeWeapon = true;

    //  }

    //}

    //protected override void OnClickDrag(Vector3 pos, Vector3 delta) {
    //  base.OnClickDrag(pos, delta);
    //  _shootTarget = Camera.main.ScreenToWorldPoint(new Vector3(pos.x, pos.y, 10));
    //}

    //protected override void OnShoot(Vector3 tapStart, Vector3 tapEnd) {

    //  if (tapEnd.x < PlayerController.Instance.shootPoint.position.x)
    //    tapEnd.x = PlayerController.Instance.shootPoint.position.x;

    //  Debug.Log("OnShoot");

    //  PlaygunShootAudio();

    //  GameObject weapon = PoolerManager.Spawn(bulletPrefab.name);
    //  weapon.transform.position = PlayerController.Instance.shootPoint.position;
    //  weapon.SetActive(true);
    //  weapon.GetComponent<Bullet>().Shot(tapStart, tapEnd);
    //  BulletDecrement();
    //}

    //private void Reload() {
    //  WeaponGenerator.Instance.changeWeapon = false;
    //}

    public AudioBlock gunShootudioBlock;
    protected virtual void PlaygunShootAudio() {
      gunShootudioBlock.PlayRandom(this);
    }

    //#region Настройки

    //public override void GetConfig() {
    //  base.GetConfig();

    //}
    //#endregion

  }

}