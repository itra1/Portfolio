using UnityEngine;

public class GrenadeFreezBullet : Bullet {

	public GameObject targetPointObject;
	[HideInInspector]
	public float damageRadius;
	[HideInInspector]
	public Vector3 tagetPosition;

	public override void OnEnable() {
    base.OnEnable();
    correctVector = Vector3.zero;
		InitTargetPoint();
	}

	protected override void Update() {
		base.Update();
		CorrectPoint();
	}

	void InitTargetPoint() {
		CorrectPoint();
		targetPointObject.transform.localScale = new Vector3(damageRadius * 2, damageRadius * 2, damageRadius * 2);
	}

	void CorrectPoint() {
		targetPointObject.transform.position = tagetPosition;
	}

	public override void OnMove(MoveObjecsElem newMove) { }

  Vector2 moveUp;
  Vector2 moveDown;

  Vector2 correctVector;

  public override void Init(MoveObjecsElem moveParamNew) {
    base.Init(moveParam);
    moveParam = moveParamNew;

    Vector3 moveVect = targetPoint - transform.position;
    moveUp = (Quaternion.Euler(0, 0, (moveVect.x >= 0 ? 90 : -90)) * moveVect).normalized;
    moveDown = (Quaternion.Euler(0, 0, -(moveVect.x >= 0 ? 90 : -90)) * moveVect).normalized;
    float timeMove = Vector3.Distance(targetPoint, transform.position) / moveParam.velocity.magnitude;
    correctVector = moveUp * timeMove * 5;
    moveParam.velocity += correctVector;
		targetPointObject.SetActive(true);
		CorrectPoint();
	}
  float angle = 0;
  protected override void Move() {
    //correctVector += moveRight * Time.deltaTime;
    moveParam.velocity += moveDown * Time.deltaTime * 10;
    angle += 1000 * Time.deltaTime;
    graphicObject.transform.localEulerAngles = new Vector3(0, 0, angle);
    base.Move();
  }

  protected override void DeactiveSFX() {
    base.DeactiveSFX();

    CameraManager.instance.StartVibration();
    GameObject sfx = PoolerManager.GetPooledObject("BoomFreez");
    sfx.transform.position = tagetPosition;
    sfx.SetActive(true);
  }
}
