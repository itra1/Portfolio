using HutongGames.PlayMaker;

namespace it.Game.PlayMaker.Enemy
{
  [ActionCategory("Enemy")]
  [HutongGames.PlayMaker.Tooltip("Поиск enemy")]
  public class FindEnemyGameObject : FsmStateAction
  {
	 public FsmBool findParent;

	 public FsmGameObject _storeObject;


	 public override void OnEnter()
	 {
		var lEnemy = Owner.gameObject.GetComponentInChildren<it.Game.NPC.Enemyes.Enemy>();

		if(lEnemy == null && findParent.Value)
		  lEnemy = Owner.gameObject.GetComponentInParent<it.Game.NPC.Enemyes.Enemy>();

		if (lEnemy != null)
		  _storeObject.Value = lEnemy.gameObject;

	 }

  }
}