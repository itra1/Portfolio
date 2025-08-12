using UnityEngine;
using System.Collections;

public class DeadCloudController : MonoBehaviour {

	Transform player;
	AudioSource sourceCimponent;

	void Start() {
		player = Player.Jack.PlayerController.Instance.transform;
		sourceCimponent = GetComponent<AudioSource>();
		transform.position = player.position;
	}

	void Update() {
		if (player) transform.position = player.position + Vector3.down + Vector3.left;
		//if (transform.position.y < 0) Destroy(gameObject);
	}

	public void NoPlayer() {
		player = null;
		StartCoroutine(changeColor());
	}

	IEnumerator changeColor() {
		while (sourceCimponent.volume > 0) {
			yield return new WaitForSeconds(0.1f);
			sourceCimponent.volume -= 0.05f;
		}
	}
}
