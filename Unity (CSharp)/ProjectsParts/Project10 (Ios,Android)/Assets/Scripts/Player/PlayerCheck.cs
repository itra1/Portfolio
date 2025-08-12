using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public struct WeaponContact {
	public int idObject;
	public bool contact;
	public bool uses;
}

[System.Serializable]
public struct BarrierContact {
	public int idObject;
	public bool contact;
}

public class PlayerCheck : MonoBehaviour {

	private Player.Jack.PlayerController player;
	private bool check;
	private bool hendingStone;
	private bool ghost;
	private bool enemy;
	private bool boomerangWeapon;

	private bool checkDouble;
	private bool checkDoubleYes;

	private List<int> barrierList;
	private List<BarrierContact> barrierDangerList;

	private bool jumpUp;
	private bool jumpBreack;
	private WeaponContact obj;
	private BarrierContact objBarrier;
	public bool endJump;
	
	//public event System.Action OnJumpBreack;

	void OnEnable() {
		player = GetComponent<Player.Jack.PlayerController>();
		barrierList = new List<int>();
		barrierDangerList = new List<BarrierContact>();

		InitParametrs();

	}

	public void InitParametrs() {
		checkQuestBarrier = Questions.QuestionManager.CheckActionQuest(Quest.jumpBarrier)
										|| Questions.QuestionManager.CheckActionQuest(Quest.jumpBarrierDouble)
										|| Questions.QuestionManager.CheckActionQuest(Quest.doubleJumpPadStone)
										|| player.runnerPhase == RunnerPhase.tutorial;

		checkQuestJumpHendingStone = Questions.QuestionManager.CheckActionQuest(Quest.jumpHendingStone);
		checkQuestJumpBoomerang = Questions.QuestionManager.CheckActionQuest(Quest.jumpBoomerang);
		checkQuestStoneDanger = Questions.QuestionManager.CheckActionQuest(Quest.stoneDanger);
		checkQuestJumpGhost = Questions.QuestionManager.CheckActionQuest(Quest.jumpGhost);
		checkQuestJumpEnemy = Questions.QuestionManager.CheckActionQuest(Quest.jumpedEnemy);
		checkQuestUseMagnet = Questions.QuestionManager.CheckActionQuest(Quest.useMagnet);
		checkQuestJumpBreack = Questions.QuestionManager.CheckActionQuest(Quest.jumpBreack) || player.runnerPhase == RunnerPhase.tutorial;
		
	}

	private bool checkQuestBarrier;
	private bool checkQuestJumpHendingStone;
	private bool checkQuestJumpBoomerang;
	private bool checkQuestStoneDanger;
	private bool checkQuestJumpGhost;
	private bool checkQuestJumpEnemy;
	private bool checkQuestUseMagnet;
	private bool checkQuestJumpBreack;

	private Collider2D[] allCollider = new Collider2D[0];
	string layerName;

	bool newCheck;
	int fixCount = 0;

	public LayerMask checkLayers;

	/// <summary>
	/// выполняется каждый кадр
	/// </summary>
	void FixedUpdate() {
		fixCount++;
		if (fixCount % 2 == 0) {
			allCollider = Physics2D.OverlapCircleAll(transform.position, 5f, checkLayers);
			newCheck = true;
		}
	}

	string checkTag;
	//Transform checkTransform;
	Vector2 checkTransPosition;

	private void Update() {

		if (endJump && player.isGround) {
			endJump = false;
			EndJump();
		}

		if (!newCheck)	return;
		newCheck = false;

		foreach (Collider2D collider in allCollider) {

			checkTag = collider.gameObject.tag;
			checkTransPosition = collider.transform.position;
			float distantion = Vector3.Distance(transform.position, checkTransPosition);

			//IdentifiedObject identified = collider.GetComponent<IdentifiedObject>();
			//if(identified) {

			if (checkQuestBarrier && (check /*&& play.isGroungs*/ && (checkTag == "RollingStone" || checkTag == "RollingSkeleton"))
					&& transform.position.x >= checkTransPosition.x - 1f
					&& transform.position.x <= checkTransPosition.x + 1f) {
				if (!barrierList.Exists(x => x == collider.gameObject.GetInstanceID())) {
					barrierList.Add(collider.gameObject.GetInstanceID());
					if (checkDouble) {
						checkDouble = false;
						checkDoubleYes = true;
					}
				}
			}

			// Через висячий камень
			if (checkQuestJumpHendingStone && check /*&& play.isGroungs*/ && checkTag == "HandingStone"
					&& transform.position.y > checkTransPosition.y
					&& transform.position.x >= checkTransPosition.x - 1f
					&& transform.position.x <= checkTransPosition.x + 1f
					 ) {
				hendingStone = true;
			}

			// Через висячий камень
			if (checkQuestJumpBoomerang && check /*&& play.isGroungs*/ && checkTag == "Boomerang"
					&& transform.position.y > checkTransPosition.y
					&& transform.position.x >= checkTransPosition.x - 1f
					&& transform.position.x <= checkTransPosition.x + 1f
					 ) {
				boomerangWeapon = true;
			}

			// Проверка возможного уклонения
			if ((checkQuestStoneDanger || checkQuestBarrier) && check /*&& play.isGroungs*/ && checkTag == "RollingStone") {
				if (distantion > 3f) {
					objBarrier = barrierDangerList.Find(x => x.idObject == collider.gameObject.GetInstanceID());
					if (objBarrier.idObject == collider.gameObject.GetInstanceID() && !objBarrier.contact) {
						Questions.QuestionManager.ConfirmQuestion(Quest.stoneDanger, 1, transform.position);
						barrierDangerList.RemoveAll(x => x.idObject == collider.gameObject.GetInstanceID());
					}
				} else if (distantion <= 3f) {
					objBarrier = barrierDangerList.Find(x => x.idObject == collider.gameObject.GetInstanceID());
					if (objBarrier.idObject != collider.gameObject.GetInstanceID()) {
						barrierDangerList.Add(new BarrierContact() { contact = false, idObject = collider.gameObject.GetInstanceID() });
					}
				}
			}

			//} else 
			if (!player.isGround) {

				if (checkQuestJumpGhost && check && checkTag == "Ghost"
					 && transform.position.y > checkTransPosition.y + 0.5f
					 && transform.position.x >= checkTransPosition.x - 1f
					 && transform.position.x <= checkTransPosition.x + 1f) {
					ghost = true;
				}

				if (checkQuestJumpEnemy && check && checkTag == "Enemy"
						&& transform.position.y > checkTransPosition.y + 0.5f
						&& transform.position.x >= checkTransPosition.x - 1f
						&& transform.position.x <= checkTransPosition.x + 1f) {
					enemy = true;
				}

			}

			// Магнит
			if (checkTag == "Coins") {
				if (player.magnetActive && distantion <= player.magnetRadius) {
					if (!collider.GetComponent<Coin>().toPlayer) {
						collider.GetComponent<Coin>().GoToPlayer();
						if (checkQuestUseMagnet)
							Questions.QuestionManager.ConfirmQuestion(Quest.useMagnet, 1);
					}
				}
			}

			if (checkQuestJumpBreack) {
				// Квест перепрыгивания ямы
				if (checkTag == "jumpUp" && transform.position.x >= checkTransPosition.x - 1 
						&& transform.position.x <= checkTransPosition.x + 0.5f) {
					jumpUp = true;
				}

				if (jumpUp == true && checkTag == "jumpDown" && transform.position.x >= checkTransPosition.x - 1) {

					if (transform.position.y < checkTransPosition.y) {
						jumpBreack = false;
					} else {
						jumpBreack = true;
					}

					jumpUp = false;
				}
			}
			/////////////////////////////
		}
		
	}


	public void ChechDoubleJump() {

		foreach (Collider2D collider in allCollider) {

			IdentifiedObject identified = collider.GetComponent<IdentifiedObject>();
			if (identified != null &&
					(identified.thisObject == identifiedObject.rollingStone || identified.thisObject == identifiedObject.rollingStoneSkeleton)
							&& transform.position.y > collider.transform.position.y + 1f
							&& transform.position.x >= collider.transform.position.x - 1f
							&& transform.position.x <= collider.transform.position.x + 1f) {
				Questions.QuestionManager.ConfirmQuestion(Quest.doubleJumpPadStone, 1, transform.position);

			}
		}
	}

	void CheckJump() {

		if (barrierDangerList.Count > 0) {
			barrierDangerList.Clear();
		}

		if (barrierList.Count > 0) {

			Questions.QuestionManager.ConfirmQuestion(Quest.jumpBarrier, barrierList.Count, transform.position);
			if (barrierList.Count > 1)
				Questions.QuestionManager.ConfirmQuestion(Quest.jumpBarrierDouble, barrierList.Count, transform.position);
		}

		barrierList.Clear();
	}

	void EndJump() {
		check = true;

		if (jumpBreack && player.playerFallWait != true && player.playerFall != true) {

			//if (OnJumpBreack != null)
			//	OnJumpBreack();
			ExEvent.PlayerEvents.PitJump.CallAsync();

			Questions.QuestionManager.ConfirmQuestion(Quest.jumpBreack, 1, transform.position);
			jumpBreack = false;
		}

		if (barrierList.Count > 0 || barrierDangerList.Count > 0)
			Invoke("CheckJump", 0.1f);

		if (hendingStone) {
			Questions.QuestionManager.ConfirmQuestion(Quest.jumpHendingStone, 1, transform.position);
			hendingStone = false;
		}

		if (ghost) {
			Questions.QuestionManager.ConfirmQuestion(Quest.jumpGhost, 1, transform.position);
			ghost = false;
		}

		if (enemy) {
			Questions.QuestionManager.ConfirmQuestion(Quest.jumpedEnemy, 1, transform.position);
			enemy = false;
		}

		if (boomerangWeapon) {
			Questions.QuestionManager.ConfirmQuestion(Quest.jumpBoomerang, 1, transform.position);
			boomerangWeapon = false;
		}

		if (checkDoubleYes) {
			Questions.QuestionManager.ConfirmQuestion(Quest.doubleJumpPadStone, 1, transform.position);
			checkDouble = false;
			checkDoubleYes = false;
		}
	}

	public void ResetJump() {
		check = false;
		hendingStone = false;
		ghost = false;
		enemy = false;
		checkDouble = false;
		checkDoubleYes = false;
		boomerangWeapon = false;
		barrierList.Clear();
	}

	void OnTriggerEnter2D(Collider2D oth) {

		if (oth.gameObject.tag == "RollingStone") {
			if (barrierDangerList.Count > 0) {
				for (int i = 0; i < barrierDangerList.Count; i++) {
					if (barrierDangerList[i].idObject == oth.gameObject.GetInstanceID()) {
						barrierDangerList.RemoveAll(x => x.idObject == oth.gameObject.GetInstanceID());
						barrierDangerList.Add(new BarrierContact() { contact = true, idObject = oth.gameObject.GetInstanceID() });
						return;
					}
				}
			}

			if (barrierList.Count > 0) {
				for (int i = 0; i < barrierList.Count; i++) {
					if (barrierList[i] == oth.gameObject.GetInstanceID()) {
						barrierList.RemoveAll(x => x == oth.gameObject.GetInstanceID());
						return;
					}
				}
				barrierList.Remove(oth.gameObject.GetInstanceID());
			}
		}
	}

}
