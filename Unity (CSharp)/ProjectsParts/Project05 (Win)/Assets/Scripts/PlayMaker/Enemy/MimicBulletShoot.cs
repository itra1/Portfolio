using HutongGames.PlayMaker;

namespace it.Game.PlayMaker.Enemy
{
  [ActionCategory("Enemy")]
  [HutongGames.PlayMaker.Tooltip("мимик шут")]
  public class MimicBulletShoot : FsmStateAction
  {
	 public FsmGameObject bullet;
	 public FsmGameObject spawnPoint;


	 public override void OnEnter()
	 {
		base.OnEnter();
		bullet.Value.GetComponent<it.Game.NPC.Enemyes.MimicBullet>().Shoot(spawnPoint.Value.transform, Owner.transform);
		Finish();
	 }

  }
}