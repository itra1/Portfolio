using UnityEngine;

/// <summary>
/// Лоадер менеджеров
/// Проверяет наличие экземпляра менеджеров, и при отсутствии создает
/// </summary>
public class ManagersLoader : MonoBehaviour {

	public GameManager manager;
  public GameObject canvas;

	private void Awake() {
		
		// Нашли, ничего не делаем
		if (FindObjectOfType<GameManager>() != null) return;
		// Создаем
		Instantiate(manager.gameObject);
    Instantiate(canvas);
  }
	
}
