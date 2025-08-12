using UnityEngine;

/// <summary>
/// Контроллер тени
/// </summary>
public class ShadowBehaviour : MonoBehaviour {

	[HideInInspector]
	public Transform matherObject;                      // Ссылка на трансформ родительского объекта

  //private Transform parentObject {
  //  get {
  //    if(matherObject)
  //  }
  //}

  private SpriteRenderer _spriteRenderer;
  private SpriteRenderer spriteRenderer {
    get {
      if (_spriteRenderer == null)
        _spriteRenderer = graphic.GetComponent<SpriteRenderer>();
      return _spriteRenderer;
    }
  }

  public GameObject graphic;                          // Ссылка на графическую часть
	private Color graphicColor;

	public LayerMask groundLayer;                       // Слой С землей
	public float diff;                                  // Смещение высоты центра источника тени
	private Vector3 defScale;                           // Стандартный размер тени
	private Vector3 thisScale;                          // Текущий размер тени
	private Vector3 diffPoss;                           // Смещение размера

	public bool fixedsize;                              // Фиксированный размер тени

	void OnEnable() {
		defScale = graphic.transform.localScale;
		thisScale = defScale;
		graphicColor = graphic.GetComponent<SpriteRenderer>().color;
		transform.position = Vector3.zero;
	}

	public void SetDiff(Vector3 newPos, Vector3 newScale) {
		diffPoss = newPos;
		thisScale = defScale + newScale;
	}

	public void SetDeff() {
		diffPoss = new Vector3(0, 0, 0);
		thisScale = defScale;
	}

	private RaycastHit2D objCheck;
	private Collider2D groundCollider;
	private float diffDist;

	private void FixedUpdate() {
		objCheck = Physics2D.Raycast(new Vector3(matherObject.position.x, matherObject.position.y + 0.3f, matherObject.position.z), Vector3.down ,5, groundLayer);
		if (objCheck && groundCollider != objCheck.collider) {
			groundCollider = objCheck.collider;
			isJumpDown = (objCheck.collider.tag == "jumpDown");
			isJumpUp = (objCheck.collider.tag == "jumpUp");
		}
	}

	bool isJumpUp;
	bool isJumpDown;

	void Update() {
		if (matherObject.position.y <= -5)
			gameObject.SetActive(false);

		// Смещаем тень, за источником

		if (objCheck) {
			transform.position = new Vector3(matherObject.position.x + diffPoss.x, objCheck.transform.position.y + diffPoss.y, matherObject.position.z + diffPoss.z);

      // Меняем прозрачность при приближении к яме
      if (isJumpUp) {
				if (objCheck.transform.position.x + 0.5f - transform.position.x <= 0) {
					isJumpUp = false;
          spriteRenderer.color = new Color(graphicColor.r, graphicColor.g, graphicColor.b, 0);
				}
      } else {
        isJumpDown = false;
        spriteRenderer.color = new Color(graphicColor.r, graphicColor.g, graphicColor.b, 1);
      }
			//if (isJumpDown) {
			//}
		} else {
			transform.position = Vector3.zero;
		}

		// Изменяем размер тени в зависимости от удаления объекта
		if (!fixedsize) {
			diffDist = (matherObject.position.y - diff - transform.position.y);
			if (diffDist > 2f) {
				graphic.transform.localScale = new Vector3(0, 0, 0);
			} else {
				graphic.transform.localScale = new Vector3((thisScale.x / 10) * (10 - (diffDist / 0.2f)),
																										(thisScale.y / 10) * (10 - (diffDist / 0.2f)),
																										(thisScale.z / 10) * (10 - (diffDist / 0.2f)));
			}
		}

	}
}
