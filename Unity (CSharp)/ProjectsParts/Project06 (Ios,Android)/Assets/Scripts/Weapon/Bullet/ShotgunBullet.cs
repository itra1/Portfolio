using System.Collections.Generic;
using UnityEngine;

namespace Game.Weapon {
  public class ShotgunBullet: Bullet {
    private int _bulletCount;
    private float distanceShoot;
    private float angleAttack;

    [SerializeField]
    private LineRenderer _lineRenderer;
    private List<LineRenderer> _instanceList = new List<LineRenderer>();

    public void DrobCount(int count) {
      _bulletCount = count;
    }

    public override void Shot(Vector3 tapStart, Vector3 tapEnd) {

      Vector3 direct = tapEnd - PlayerController.Instance.ShootPoint;
      Quaternion direction = Quaternion.LookRotation(tapEnd - PlayerController.Instance.ShootPoint);

      for (int ii = 0; ii < _bulletCount; ii++) {
        LineRenderer line = Game.Utils.InstantiateUtils.GetDisableInstanceFromList<LineRenderer>(this, _lineRenderer, _instanceList);
        line.gameObject.SetActive(true);
        Vector3 targetShoot = (direction * Quaternion.Euler(Random.Range(-angleAttack / 2, angleAttack / 2), 0, 0)) * new Vector3(0, 0, 1) * distanceShoot;
        Debug.Log(targetShoot);
        //Vector3 targetShoot = direction * new Vector3(0,0,1) * distanceShoot;
        line.positionCount = 2;
        line.SetPosition(0, transform.position);

        RaycastHit2D[] allObject = Physics2D.LinecastAll(PlayerController.Instance.ShootPoint, PlayerController.Instance.ShootPoint + targetShoot);

        bool isHit = false;
        for (int i = 0; i < allObject.Length; i++) {
          if (!isHit && LayerMask.LayerToName(allObject[i].collider.gameObject.layer) == "Enemy") {

            BattleEventEffects.Instance.VisualEffect(BattleEffectsType.hunterObrezShoot, allObject[i].collider.transform.position, this);
            DamageEnemy(allObject[i].collider.gameObject.GetComponent<Enemy>().gameObject);
            line.SetPosition(1, allObject[i].point);
            isHit = true;
            break;
          }
        }
        if (!isHit)
          line.SetPosition(1, PlayerController.Instance.ShootPoint + targetShoot);

      }

    }

    //public override void Shot(Vector3 tapStart, Vector3 tapEnd) {

    //  Debug.Log(PlayerController.Instance.ShootPoint);

    //  float angle = 0;
    //  float andleRadian = 0;
    //  Vector3 alterTapEnd;
    //  Vector3 targetShoot;
    //  Vector3 newTap;

    //  for (int ii = 0; ii < _bulletCount; ii++) {

    //    LineRenderer _line = Game.Utils.InstantiateUtils.GetDisableInstanceFromList<LineRenderer>(this, _lineRenderer, _instanceList);
    //    _line.gameObject.SetActive(true);
    //    angle = Random.Range(-1 * angleAttack/2, angleAttack/2);
    //    andleRadian = angle * (Mathf.PI / 180);

    //    alterTapEnd = tapEnd - PlayerController.Instance.ShootPoint;
    //    newTap = new Vector3(alterTapEnd.x * Mathf.Cos(andleRadian) - alterTapEnd.y * Mathf.Sin(andleRadian),
    //                        alterTapEnd.x * Mathf.Sin(andleRadian) + alterTapEnd.y * Mathf.Cos(andleRadian),
    //                        0) + PlayerController.Instance.ShootPoint;
    //    targetShoot = (newTap - PlayerController.Instance.ShootPoint).normalized * distanceShoot;

    //    _line.positionCount = 2;
    //    _line.SetPosition(0, transform.position);

    //    RaycastHit2D[] allObject = Physics2D.LinecastAll(PlayerController.Instance.ShootPoint, PlayerController.Instance.ShootPoint + targetShoot);

    //    bool isHit = false;
    //    for (int i = 0; i < allObject.Length; i++) {
    //      if (!isHit && LayerMask.LayerToName(allObject[i].collider.gameObject.layer) == "Enemy") {

    //        BattleEventEffects.Instance.VisualEffect(BattleEffectsType.hunterObrezShoot, allObject[i].collider.transform.position, this);
    //        DamageEnemy(allObject[i].collider.gameObject.GetComponent<Enemy>().gameObject);
    //        _line.SetPosition(1, allObject[i].point);
    //        isHit = true;
    //        break;
    //      }
    //    }
    //    if(!isHit)
    //      _line.SetPosition(1, PlayerController.Instance.ShootPoint + targetShoot);

    //  }
    //  InvokeCustom(0.1f, () => {
    //    gameObject.SetActive(false);
    //  });
    //}

    private void OnDisable() {
      _instanceList.ForEach(x => x.gameObject.SetActive(false));
    }

    public override void GetConfig() {
      base.GetConfig();

      distanceShoot = wep.param4.Value;
      angleAttack = wep.param2.Value;
    }

  }

}