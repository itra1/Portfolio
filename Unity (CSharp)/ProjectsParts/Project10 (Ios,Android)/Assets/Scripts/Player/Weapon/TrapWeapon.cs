using UnityEngine;
using System.Collections;

public class TrapWeapon : MonoBehaviour {

	public Rigidbody2D rb;
  public float damagePower;                               // Сила повреждения
	public Transform graphic;                          // Ссылка на фложеный объект графики
	public SpriteRenderer graphicSprite;                          // Ссылка на фложеный объект графики
  public WeaponTypes thisWeaponType;                      // Текущий тип оружия
	public GameObject groundObject;
	
  public Sprite[] graphics;                               // Ммасив состояний спрайта

  public float speedX;                                    // Скорость движения
	public float speed { get { return speedX * _vectorKoeff; } }



	private float _angle;                                            // Угол поворота
  private int _countContact = 0;                                   // Количество контактов с поверхностью
	
  private bool _isActive;                                               // Флаг активности, что может наносить повреждение

  public AudioClip openClip;

	private float _vectorKoeff;

	private void OnEnable() {

		_vectorKoeff = (GameManager.activeLevelData.moveVector == MoveVector.left ? -1 : 1);

		graphicSprite.sprite = graphics[0];
		groundObject.SetActive(true);
		_isActive = true;
		_countContact = 0;
		rb.AddForce(new Vector2((RunnerController.RunSpeed + speed), 8f), 
			ForceMode2D.Impulse);
	}

	private void OnDisable() {
		try {
			if (_isActive) {
				Questions.QuestionManager.ConfirmQuestion(Quest.weaponMiss, 1, Player.Jack.PlayerController.Instance.transform.position);
			}
		} catch { }
	}

	private void FixedUpdate() {
		if (_countContact == 0) {
			_angle = Vector3.Angle(Vector3.up, rb.velocity) * _vectorKoeff + 180;
			rb.MoveRotation(_angle);
			//graphic.localEulerAngles = new Vector3(0f, 0f, angle);
		}
	}
	
	private void OnCollisionEnter2D(Collision2D collision) {
		if (LayerMask.LayerToName(collision.gameObject.layer) == "Ground") {

			switch (_countContact) {
				case 0:
					_countContact = 1;
					rb.velocity = new Vector2(rb.velocity.x, 0);
					rb.AddForce(new Vector2((RunnerController.RunSpeed + speed), 4f), ForceMode2D.Impulse);
					graphicSprite.GetComponent<SpriteRenderer>().sprite = graphics[1];
					graphic.localEulerAngles = new Vector3(0f, 0f, 0);
					AudioManager.PlayEffect(openClip, AudioMixerTypes.runnerEffect);
					break;
				case 1:
					_countContact = 2;
					rb.velocity = new Vector2(rb.velocity.x, 0);
					rb.AddForce(new Vector2((RunnerController.RunSpeed + speed), 2f), ForceMode2D.Impulse);
					break;
			}

		}
	}

	void OnTriggerEnter2D(Collider2D col) {
		
    if(!_isActive) return;
		
    if(LayerMask.LayerToName(col.gameObject.layer) == "Enemy") {
      col.GetComponent<Enemy>().Damage(thisWeaponType, damagePower, Vector3.zero, DamagePowen.level1, 0, false);
      _isActive = false;
      graphicSprite.sprite = graphics[0];
      //Vector3 pos = col.transform.position;
      //transform.position = new Vector3(pos.x, pos.y + 0.15f, 0);
    }
  }
}
