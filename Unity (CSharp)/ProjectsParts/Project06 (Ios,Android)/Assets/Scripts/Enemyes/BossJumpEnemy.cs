using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Контроллер врага со средней скоростью
/// </summary>
public class BossJumpEnemy : Enemy {

  protected override void OnEnable() {
    //WeaponManager.weaponTarget += TargetWeapon;
    base.OnEnable();
  }

  public override void OnDisable() {
    //WeaponManager.weaponTarget -= TargetWeapon;
    base.OnDisable();
  }

  public override void Update() {
    if(jump && Time.time > timeStopJump) JumpComplited();
    base.Update();

    if(phase == Phase.attack && attackLastTime + attackWaitTime * 1.2f <= Time.time)
      SetPhase(Phase.run);

  }

  float positionTargetX;
  public void TargetWeapon(float time, float pointX) {
    positionTargetX = transform.position.x - (speedX * time);
    if(Vector3.Distance(new Vector3(0f, positionTargetX, 0f), new Vector3(0f, pointX, 0f)) < 6f) Jump();
  }

  bool jump;
  float timeStopJump;
  float x;
  protected void Jump() {
    if(jump) return;
    timeStopJump = Time.time + 0.5f;
    jump = true;
    if(transform.position.x > (CameraController.rightPoint.x - 10f)) x = -1f;
    else if(transform.position.x < (CameraController.leftPointX.x + 5f)) x = 1f;
    else {
      x = Random.Range(-1, 1);
      if(x == 0) x += 1;
    }
    SetDirectionVelocity(1 * x);
  }

  protected void JumpComplited() {
    jump = false;
    SetDirectionVelocity(-1);
  }

  public override void Move() {

    if(!jump) velocity.x = Mathf.Abs(speedX) * directionVelocity;
    else velocity.x = Mathf.Abs(speedX * 15f) * directionVelocity;
    if(stunTime >= Time.time) velocity.x *= 1 - stunDelay;
    if(EnemysSpawn.maxLeftPositionX > transform.position.x + velocity.x * Time.deltaTime) velocity.x = 0;
		transform.position += velocity * Time.deltaTime;
		//rb.MovePosition(transform.position + velocity * Time.deltaTime);
  }
      
}
