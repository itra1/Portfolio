using UnityEngine;
using System.Collections;

public class PistolWeaponManager : WeaponManager {

  protected override void CreateBullet(ObjectSpawn bulletData) {

    GameObject inst = WeaponSpawner.instance.GenerateBullet(((ObjectSpawn.BulletData)bulletData.data).bulletType);
    inst.transform.position = MapManager.PositionInvers(bulletData.position);
    inst.GetComponent<Bullet>().SetId(bulletData.id);
    inst.GetComponent<Bullet>().targetPoint = MapManager.PositionInvers(((ObjectSpawn.BulletData)bulletData.data).targetPoint);
    MoveObjecsElem move = new MoveObjecsElem();
    move.id = bulletData.id;
    move.velocity = MapManager.VectorInvers(bulletData.velocity);

    if(inst.GetComponent<BoxCollider2D>() != null) {
      inst.GetComponent<BoxCollider2D>().size = bulletData.size;
      inst.GetComponent<BoxCollider2D>().offset = bulletData.offset;
    }

    if(inst.GetComponent<CircleCollider2D>() != null) {
      inst.GetComponent<CircleCollider2D>().radius = bulletData.size.x;
      inst.GetComponent<CircleCollider2D>().offset = bulletData.offset;
    }

    inst.GetComponent<Bullet>().OnMove(move);
    inst.GetComponent<Bullet>().Init(move);
    inst.SetActive(true);

    base.CreateBullet(bulletData);
  }
}
