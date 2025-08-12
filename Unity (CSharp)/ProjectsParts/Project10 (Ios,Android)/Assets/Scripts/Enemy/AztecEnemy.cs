using UnityEngine;

/// <summary>
/// Контроллер ацтека
/// </summary>
public class AztecEnemy : ClassicEnemy {

  /// <summary>
  /// Флаг бросания
  /// </summary>
  [HideInInspector]
  public bool thisToss;
  
  public override void OnEnable() {
    base.OnEnable();
    transform.localEulerAngles = Vector3.zero;
		
	}
  
  /// <summary>
  /// Бросок
  /// </summary>
  public void Toss() {
    thisToss = true;
		play.Toss();
    //MoveFunction = play.Toss;
  }

  public override void OnTriggerEnter2D(Collider2D oth) {

    base.OnTriggerEnter2D(oth);

    if(oth.tag == "Player") {
      if(thisToss) {
        Player.Jack.PlayerController playerCont = oth.GetComponent<Player.Jack.PlayerController>();
        if(playerCont)
          playerCont.ThisDamage(WeaponTypes.none, playerDamage.type, playerDamage.damagePower, transform.position);
      }
    }
  }

  public override bool CheckPlayer() {
    if(thisToss)
      return false;
    else
      return base.CheckPlayer();
  }

}
