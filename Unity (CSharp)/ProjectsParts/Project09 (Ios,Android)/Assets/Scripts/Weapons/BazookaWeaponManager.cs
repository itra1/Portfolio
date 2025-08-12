using UnityEngine;

public class BazookaWeaponManager : WeaponManager {

  float damageRadius;

  protected override void GetConfig() {
    base.GetConfig();

    if(configParametrs == null) return;

    if(configParametrs.ContainsKey("damageRadius"))
      damageRadius = float.Parse(configParametrs["damageRadius"]);
  }

  protected override void CreateBullet(ObjectSpawn bulletData) {

    GameObject inst = WeaponSpawner.instance.GenerateBullet(((ObjectSpawn.BulletData)bulletData.data).bulletType);
    inst.transform.position = MapManager.PositionInvers(bulletData.position);
    inst.GetComponent<Bullet>().SetId(bulletData.id);
    inst.GetComponent<Bullet>().targetPoint = MapManager.PositionInvers(((ObjectSpawn.BulletData)bulletData.data).targetPoint);
    MoveObjecsElem move = new MoveObjecsElem();
    move.id = bulletData.id;
    move.velocity = MapManager.VectorInvers(bulletData.velocity);
		move.position = MapManager.PositionInvers(bulletData.position);
		
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
		inst.GetComponent<BazookaBullet>().damageRadius = damageRadius;
		inst.GetComponent<BazookaBullet>().tagetPosition = MapManager.PositionInvers(((ObjectSpawn.BulletData)bulletData.data).targetPoint);
		inst.SetActive(true);

    base.CreateBullet(bulletData);
  }

	bool isLoad;

	protected override void OnStartSendShoot() {
		//return;
		if(isLoad) return;
		isLoad = true;
		playerOwner.StartLoad(0, 1, OnLoadComplited);
	}

	void OnLoadComplited() {
		isLoad = false;
		timeSendPacket = Time.time;
		OnSendShoot();
	}

}
