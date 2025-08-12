using UnityEngine;

public class SceneBorderColliders : MonoBehaviour {

	public static SceneBorderColliders instance;

	public Transform leftCollider;
	public Transform rightCollider;

	public static float LeftBorder {
		get {
			return instance.leftCollider.position.x;
		}
	}

	public static float RightBorder {
		get {
			return instance.rightCollider.position.x;
		}
	}

	private void Awake() {
		instance = this;
		leftCollider.GetComponent<BoxCollider2DHelper>().OnEnter = OnEnter2D;
		rightCollider.GetComponent<BoxCollider2DHelper>().OnEnter = OnEnter2D;
		
		leftCollider.gameObject.SetActive(GameManager.activeLevelData.moveVector != MoveVector.left);
		rightCollider.gameObject.SetActive(GameManager.activeLevelData.moveVector == MoveVector.left);

	}


	private void OnEnter2D(Collider2D collision) {

		collision.gameObject.SetActive(false);

	}

}
