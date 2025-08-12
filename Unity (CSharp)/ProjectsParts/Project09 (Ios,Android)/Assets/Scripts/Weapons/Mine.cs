using UnityEngine;
/// <summary>
/// Мина
/// </summary>
public class Mine : Bullet {
  
  protected override void Deactive() {
    GetComponent<Animator>().SetTrigger("hide");
    DeactiveSFX();
    //Helpers.Invoke(this, DeactiveThis, 1f);
  }
  
  /// <summary>
  /// Переопределение родительской фугкции движения
  /// </summary>
  protected override void Move() { }

  protected override void DeactiveSFX() {
    base.DeactiveSFX();

    GameObject sfx = PoolerManager.GetPooledObject("Boom");
    sfx.transform.position = transform.position;
    sfx.SetActive(true);
  }

}
