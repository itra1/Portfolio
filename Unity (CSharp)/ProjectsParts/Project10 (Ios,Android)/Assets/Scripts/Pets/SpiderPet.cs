using UnityEngine;
using System.Collections;
using Spine.Unity;
using Player.Jack;

namespace Pet {

  /// <summary>
  /// Пет паук
  /// </summary>
  public class SpiderPet: Pet {

    public Transform groundPointUp;

    /// <summary>
    /// Ссылка на объект графики
    /// </summary>
    public GameObject graphic;
    /// <summary>
    /// Ускоренная анимация бега
    /// </summary>
    [SpineAnimation(dataField: "skeletonAnimation")]
    public string runFastAnim = "";
    /// <summary>
    /// Вертикальная дистанция при реверсивном беге
    /// </summary>
    //float heightDistRevert;
    /// <summary>
    /// Типы положения паука
    /// </summary>
    enum SpiderPosition { down, top };
    /// <summary>
    /// Текущее положение паука
    /// </summary>
    [SerializeField]
    SpiderPosition spiderPosition;

    /// <summary>
    /// Маска слоя потолка
    /// </summary>
    public LayerMask groundMaskTop;

    public override void OnEnable() {
      base.OnEnable();
      rb.gravityScale = 5;
      spiderPosition = SpiderPosition.down;
      //heightDistRevert = (CameraController.displayDiff.top * 2) / 3;
    }

    /// <summary>
    /// Рассчет гравитации
    /// </summary>
    //protected override void CalcGravity() {
    //	if (spiderPosition == SpiderPosition.down)
    //		velocity.y -= gravity * Time.deltaTime;
    //	else
    //		velocity.y += gravity * Time.deltaTime;
    //}


    /// <summary>
    /// Апдейт игры
    /// </summary>
    public override void UpdateGame() {
      base.UpdateGame();

      if (!isGround) {

        if (spiderPosition == SpiderPosition.top && graphic.transform.localScale.y == 1) {
          //Collider[] isGrounded2 = Physics.OverlapSphere(transform.position, heightDistRevert, groundMaskTop);
          if (transform.position.y > CameraController.Instance.transform.position.y) {
            graphic.transform.localScale = new Vector3(1, -1, 1);
            if (phase == Phase.uses)
              PlayerController.Instance.pet.ScalingGraphic(true);
          }
        } else if (spiderPosition == SpiderPosition.down && graphic.transform.localScale.y == -1) {
          //Collider[] isGrounded2 = Physics.OverlapSphere(transform.position, heightDistRevert, groundMask);
          if (transform.position.y > CameraController.Instance.transform.position.y) {
            graphic.transform.localScale = new Vector3(1, 1, 1);
            if (phase == Phase.uses)
              PlayerController.Instance.pet.ScalingGraphic(false);
          }
        }
      }
    }

    /// <summary>
    /// Активация пета
    /// </summary>
    /// <param name="jack"></param>
    public override void JackActivate(GameObject jack) {
      spiderPosition = SpiderPosition.top;
      rb.gravityScale = -5;
      base.JackActivate(jack);

      //if (isGround) {
      //	if ((spiderPosition == SpiderPosition.down && velocity.y < 0) || (spiderPosition == SpiderPosition.top && velocity.y > 0))
      //		velocity.y = 0;
      //	transform.position = new Vector3(transform.position.x, isGrounded[0].transform.position.y, transform.position.z);
      //}
    }

    /// <summary>
    /// Проверка пересечения с поверхностью
    /// </summary>
    //protected override void CheckFixedGroung() {
    //	if (isGround) {
    //		if ((spiderPosition == SpiderPosition.down && velocity.y < 0) || (spiderPosition == SpiderPosition.top && velocity.y > 0))
    //			velocity.y = 0;
    //		transform.position = new Vector3(transform.position.x, isGrounded[0].transform.position.y, transform.position.z);
    //	}
    //}

    /// <summary>
    /// Обработка прыжка
    /// </summary>
    protected override void Jump() {
      jumpKey = false;
      if (isGround) {
        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.AddForce(new Vector2(rb.velocity.x, -jumpSpeed), ForceMode2D.Impulse);
      }
    }

    /// <summary>
    /// Проверка пересечения с зеплей
    /// </summary>
    /// <returns>Массив коллайдеров</returns>
    protected override bool CheckGround() {
      //if (spiderPosition == SpiderPosition.down)
      return base.CheckGround();
      //else
      //	return (Physics2D.OverlapCircle(groundPointUp.position, 0.1f, groundMask) != null);
    }

  }
}