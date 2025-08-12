using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Первая сцена
/// </summary>
public class FirstSplash : MonoBehaviour {

	void Start() {
		Invoke("Play", 1);
	}

	void Play() {
		GetComponent<Animator>().SetTrigger("play");
	}

	[SerializeField]
	string nextSceneName;              // Имя сцены для загрузки
	void AnimEvent() {
		SceneManager.LoadScene(nextSceneName);
	}
}
