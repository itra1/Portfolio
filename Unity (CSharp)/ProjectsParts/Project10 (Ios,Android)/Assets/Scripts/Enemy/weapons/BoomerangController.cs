using UnityEngine;
using System.Collections;

public class BoomerangController : MonoBehaviour {
	//enum BoomerangState {
	//	forward, stay, back
	//}

	public Rigidbody2D rb;
	public BoxCollider2DHelper box;

	[HideInInspector]
	public GameObject targets;															// Ссылка на объект возврата
	Vector3 target;

	//BoomerangState stateThis;

	public float rotateSpeed;                               // Скорость вращения
	float thisRotateSpeed;
	Vector3 velocity;                                       // Движение
	public float speed;                                     // Скорость смещения
	float thisSpeed;                                        // Текущая скорость

	public int power;                                       // Сила атаки

	void OnEnable() {
		velocity.y = 0;
		//stateThis = BoomerangState.forward;
		thisSpeed = speed + RunnerController.RunSpeed;
		velocity.x = thisSpeed;
		thisRotateSpeed = Random.Range(rotateSpeed - 50, rotateSpeed + 50);

		box.OnEnter = OnEnter2D;

		rb.AddForce(velocity, ForceMode2D.Impulse);

		//Invoke("SetStay" , 4);
	}

	//void SetStay() {
	//  stateThis = BoomerangState.stay;
	//}

	private void FixedUpdate() {

		if(transform.position.x >= CameraController.displayDiff.rightDif(0.7f))
			rb.AddForce(new Vector2(-RunnerController.RunSpeed*3,0), ForceMode2D.Force);
		rb.angularVelocity = thisRotateSpeed;

	}

	//void Update() {
	//	// Вращаем
	//	transform.localEulerAngles = new Vector3(0f, 0f, transform.localEulerAngles.z + thisRotateSpeed * Time.deltaTime);

	//	if (stateThis == BoomerangState.forward) {
	//		thisSpeed = speed + RunnerController.RunSpeed;
	//		velocity.x = thisSpeed;
	//	}

	//	// В стадии остановки, постепенно замедляем скорость
	//	if (stateThis == BoomerangState.stay) {

	//		thisSpeed -= speed * Time.deltaTime * 3;

	//		// Скорость не меньше нуля
	//		if (thisSpeed <= RunnerController.RunSpeed) {
	//			thisSpeed = RunnerController.RunSpeed;
	//			stateThis = BoomerangState.back;
	//		}
	//	}

	//	if (stateThis == BoomerangState.back) {

	//		// Поэтапно меняем скорость
	//		if (Mathf.Abs(thisSpeed) < speed)
	//			thisSpeed -= speed * Time.deltaTime * 3;
	//		velocity.x = thisSpeed;
	//	}

	//	transform.position += velocity * Time.deltaTime;

	//	// При приближении краю меняем фазу
	//	if (stateThis == BoomerangState.forward & transform.position.x >= CameraController.displayDiff.transform.position.x + CameraController.displayDiff.right * 0.7f) {
	//		//if (IsInvoking("SetStay")) CancelInvoke("SetStay");
	//		stateThis = BoomerangState.stay;
	//	}

	//	if (transform.position.x <= CameraController.displayDiff.transform.position.x + CameraController.displayDiff.left * 2)
	//		Destroy(gameObject);
	//}

	void OnTriggerEnter2D(Collider2D col) {
		if (targets) {
			if (rb.velocity.x <= 0 & col.gameObject.Equals(targets))
				Destroy(gameObject);
		}
	}

	void OnEnter2D(Collider2D col) {
		if (LayerMask.LayerToName(col.gameObject.layer) == "Player")
			col.GetComponent<Player.Jack.PlayerController>().ThisDamage(WeaponTypes.bumerang, Player.Jack.DamagType.live, 1, transform.position);
	}

}
