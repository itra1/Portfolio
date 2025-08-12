using UnityEngine;

namespace Game.Weapon {



  public class ManagerVarganich: WeaponAssistant {

    //float timeShootReady;
    private bool isShoot;


    protected override void OnClickDown(Vector3 position) {
      if (!CheckReadyShoot(PlayerController.Instance.ShootPoint, position))
        return;

      if (isShoot)
        return;

        isShoot = true;
      Shoot();
      PlayReload();
      OnShootComplited();
    }

    private void Shoot() {

      GameObject inst = PoolerManager.Spawn(bulletPrefab.name);
      inst.transform.position = new Vector3(pointerDown.x, DecorationManager.Instance.loaderLocation.roadSize.min + ((DecorationManager.Instance.loaderLocation.roadSize.max - DecorationManager.Instance.loaderLocation.roadSize.min) / 2), pointerDown.z);
      inst.GetComponent<Bullet>().Shot(Vector3.zero, pointerDown);
      inst.SetActive(true);
      SetActiveStatus(true);
      isShoot = false;
      base.OnShoot(Vector3.zero, pointerDown);
      inst.GetComponent<VarganichBullet>().OnDestroyBullet = () => {
        OnShootComplited();
      };
    }




    //public override bool CheckReadyShoot(Vector3 tapStart, Vector3 tapEnd) {
    //  if (tapStart.x > tapEnd.x || tapEnd.x - tapStart.x < 0.5f)
    //    return false;
    //  return base.CheckReadyShoot(tapStart, tapEnd);
    //}

    //protected override void OnShoot(Vector3 tapStart, Vector3 tapEnd) {
    //  if (isShoot)
    //    return;
    //  base.OnShoot(tapStart, tapEnd);
    //  isShoot = true;
    //  CreateBullet(tapEnd);
    //}

    //public override void Shoot(Vector3 tapStart, Vector3 tapEnd) {
    //  if (!CheckReadyShoot(tapStart, tapEnd))
    //    return;
    //  OnShoot(tapStart, tapEnd);
    //  //OnShootComplited();
    //}

    //private void CreateBullet(Vector3 target) {
    //  GameObject inst = PoolerManager.Spawn(bulletPrefab.name);
    //  inst.transform.position = new Vector3(target.x, Decoration.instance.loaderDecoration.roadSize.min + ((Decoration.instance.loaderDecoration.roadSize.max - Decoration.instance.loaderDecoration.roadSize.min) / 2), target.z);
    //  inst.GetComponent<Bullet>().Shot(Vector3.zero, target);
    //  inst.SetActive(true);
    //  SetActiveStatus(true);
    //  isShoot = false;
    //  base.OnShoot(Vector3.zero, target);
    //  inst.GetComponent<VarganichBullet>().OnDestroyBullet = () => {
    //    OnShootComplited();
    //  };
    //}

    //public override void GetConfig() {
    //  base.GetConfig();

    //  //timeShootReady = float.Parse(xmlConfig["Param1"].ToString().Replace(',', '.'));
    //}
  }

}