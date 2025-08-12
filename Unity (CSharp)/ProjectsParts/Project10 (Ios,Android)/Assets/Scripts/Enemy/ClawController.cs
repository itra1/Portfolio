using UnityEngine;
using System.Text.RegularExpressions;
using Spine.Unity;

public class ClawController : SpawnAbstract {

	public SkeletonRenderer skeletonRenderer;                   // Рендер кости
	public LayerMask groundMask;


	void OnEnable() {

		transform.localScale = new Vector3((GameManager.activeLevelData.moveVector == MoveVector.left ? -1 : 1),1,1);

		CheckPosition();
	}

	void CheckPosition() {

		RaycastHit2D[] grnd = Physics2D.RaycastAll(transform.position + new Vector3(0, 0.2f, 0), Vector3.down, groundMask);
		if (grnd.Length > 0) transform.position = new Vector3(transform.position.x, grnd[0].transform.position.y, 0);

		Regex regex = new Regex("Log|Wood");

		foreach (RaycastHit2D ont in grnd) {

			Match match = regex.Match(ont.transform.name);
			if (match.Success) {
				if (match.Success && (match.Groups[0].Value == "Wood" || match.Groups[0].Value == "Log")) {
					Debug.Log("disable");
					gameObject.SetActive(false);
				}
			}
		}
	}

}
