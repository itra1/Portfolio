using UnityEngine;

namespace Game.Weapon {

  public class ManagerBazooka: WeaponStandart {

    [SerializeField]
    private LayerMask targetMask;

    [SerializeField]
    private GameObject targetObject;
    private bool isShoot = false;
    private Transform targetBullet = null;

    public override void Inicialized() {
      base.Inicialized();

      targetObject.SetActive(isSelected);
      TargetPosition();
    }

    protected override void OnClickDown(Vector3 position) {
      if (!CheckReadyShoot(position, position))
        return;

      StartShootAnimation();
    }

    protected override void OnShootAnimEvent() {

      if (isShoot)
        return;

      Shoot();
      PlayReload();
      BulletDecrement();
      OnShootComplited();
    }

    protected override void Update() {
      base.Update();

      if (targetBullet == null) {
        TargetPosition();
      } else {
        targetObject.transform.position = targetBullet.position + Vector3.up / 2;
      }

    }

    private void Shoot() {

      RaycastHit2D hit = Physics2D.CircleCast(pointerDown, 0.3f, Vector2.one, 0.3f, targetMask);
      if (hit.collider != null && hit.collider.GetComponent<Enemy>() != null) {
        targetBullet = hit.collider.transform;
        isShoot = true;

        GameObject inst = PoolerManager.Spawn(bulletPrefab.name);
        inst.transform.position = PlayerController.Instance.ShootPoint;
        inst.SetActive(true);
        inst.GetComponent<Bullet>().OnDestroyBullet = OnDestroyBullet;
        inst.GetComponent<BazookaRocketWeapon>().SetTarget(targetBullet);


      }
    }

    
    protected override void DeInicialized() {
      base.DeInicialized();
      if (targetBullet == null)
        targetObject.SetActive(isSelected);
    }

    private void TargetPosition() {
      targetObject.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10));
    }

    //protected override void OnShoot(Vector3 tapStart, Vector3 tapEnd) {

    //  base.OnShoot(tapStart, tapEnd);
    //  GameObject inst = PoolerManager.Spawn(bulletPrefab.name);
    //  inst.transform.position = PlayerController.Instance.shootPoint.position;
    //  inst.SetActive(true);
    //  inst.GetComponent<Bullet>().OnDestroyBullet = OnDestroyBullet;
    //  inst.GetComponent<BazookaRocketWeapon>().SetTarget(targetBullet);
    //}

    private void OnDestroyBullet() {
      targetBullet = null;
      targetObject.SetActive(isSelected);
    }

    protected override void OnShootComplited(bool isReload = true) {
      if (isShoot) {
        base.OnShootComplited(isReload);
        isShoot = false;
      }
    }

  }

}