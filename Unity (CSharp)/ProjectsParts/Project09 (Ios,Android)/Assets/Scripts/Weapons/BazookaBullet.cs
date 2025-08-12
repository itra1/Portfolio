using UnityEngine;

/// <summary>
/// Контроллер снаряда базуки
/// </summary>
public class BazookaBullet : Bullet {
  private Vector2 moveUp;
  private Vector2 moveDown;
  private Vector2 correctVector;

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
		targetPointObject.transform.localScale = new Vector3(damageRadius*2, damageRadius*2, damageRadius*2);
	}

	void CorrectPoint() {
		targetPointObject.transform.position = tagetPosition;
	}

	public override void OnMove(MoveObjecsElem newMove) { }

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

  protected override void Move() {
    //correctVector += moveRight * Time.deltaTime;
    moveParam.velocity += moveDown * Time.deltaTime * 10;
    graphicObject.transform.localEulerAngles = new Vector3(0, 0, Vector3.Angle(Vector3.up, moveParam.velocity) * (moveParam.velocity.x > 0 ? -1 : 1));
    base.Move();
  }

  protected override void DeactiveSFX() {
    base.DeactiveSFX();
    CameraManager.instance.StartVibration();

    GameObject sfx = PoolerManager.GetPooledObject("Boom");
    sfx.transform.position = tagetPosition;
    sfx.SetActive(true);
  }

}
