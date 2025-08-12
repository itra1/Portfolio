using UnityEngine;
using System.Collections;

/// <summary>
/// Помошник в работе sfx эффектов
/// </summary>
public class sfx : MonoBehaviour {

	[SerializeField]
	GameObject parentObject;

	/// <summary>
	/// Событие анимации по отключению объекта
	/// </summary>
	public void DeactiveObject() {

		if (parentObject != null)
			parentObject.SetActive(false);
		else
			gameObject.SetActive(false);
	}
}
