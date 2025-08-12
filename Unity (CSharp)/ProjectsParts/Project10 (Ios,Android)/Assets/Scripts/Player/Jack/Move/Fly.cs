using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Player.Jack {
  public class Fly: PlayerMove {

    public AudioClip activeClip;
    private readonly float graphicDiffY = -2.5f;                              // Ссылка на обхект графики
    private readonly float speedForvard = 6;                 // Скорость горизонтального движения
    private readonly float speedBack = 4;                    // Скорость горизонтального движения

    public float gravity = 65;                              // Графитация, действующая на игрока (только с клавиатуры)
    public float runSpeed = 7;                             // Максимальная скорость горизонтального смещения

    protected override float horizontalSpeed {
      get { return 5;  }
    }

    protected override AudioClip boostClip{
      get { return activeClip; }
    }

    public override void Init() {
      throw new System.NotImplementedException();
    }

    protected override void Move() {

      velocity.x = 0;

      if (horizontalKeyValue != 0) {
        velocity.x = (horizontalKeyValue < 0) ? runSpeed : speedForvard;
        velocity.x = (horizontalKeyValue > 0) ? runSpeed : speedBack;
        velocity.x *= horizontalKeyValue;
      }

      if (controller.isGround && !jumpKey)
        velocity.y = -gravity * Time.deltaTime;

      // Гравитационное действие
      if (velocity.x < 0 || jumpKey)
        velocity.y += (gravity * Time.deltaTime) / 3f;

      if (velocity.x >= 0 && !jumpKey)
        velocity.y -= (gravity * Time.deltaTime) / 4;

      if (velocity.y > 0) {
        if (controller.graphic.transform.eulerAngles.z < 30)
          controller.graphic.transform.eulerAngles = new Vector3(0, 0, controller.graphic.transform.eulerAngles.z + 1f);

        if (controller.graphic.transform.eulerAngles.z == 30)
          controller.graphic.transform.eulerAngles = new Vector3(0, 0, 30);
      } else {
        if (controller.graphic.transform.eulerAngles.z > 0)
          controller.graphic.transform.eulerAngles = new Vector3(0, 0, controller.graphic.transform.eulerAngles.z - 1f);

        if (controller.graphic.transform.eulerAngles.z > 30)
          controller.graphic.transform.eulerAngles = new Vector3(0, 0, 0);
      }

      // Добавим ограничение скорости спуска
      velocity.y = velocity.y <= -4 ? -4 : velocity.y;
      // Ограничение скорости подема
      velocity.y = velocity.y >= 4 ? 4 : velocity.y;

      // Ограничение по максимальной высоте
      velocity.y = (transform.position.y >= 10.5f && velocity.y > 0) ? 0 : velocity.y;

      // Запрет опускаться слишком низко
      bool isGround = Physics.CheckSphere(transform.position, 1f, controller.groundLayer);
      if (controller.isGround && velocity.y < 0) velocity.y = 0;

      // Первоначальное появление
      if (controller.playerStart) velocity.x = 3f;
      if (controller.playerStart) velocity.y = 0f;

      // Двигаем
      //controller.Move(new Vector3(velocity.x, velocity.y, 0) * Time.deltaTime);

      //CheckPosition(transform.position + velocity * Time.deltaTime + new Vector3(0 , -0.858f , 0));
      rigidbody.velocity = velocity;

      if (controller.isGround && velocity.y < 0) {
        velocity.y = 0;
        //transform.position = new Vector3(transform.position.x + velocity.x * Time.deltaTime, player.isGroungsArray[0].transform.position.y + 0.858f, transform.position.z);
      } else
        transform.position += velocity * Time.deltaTime;

      // Ограничения горизонтального перемещения
      if (transform.position.x < boundary.min && velocity.x < 0)
        transform.position = new Vector3(boundary.min, transform.position.y, transform.position.z);

      // Ограничения горизонтального перемещения
      if (transform.position.x > boundary.max && velocity.x > 0)
        transform.position = new Vector3(boundary.max, transform.position.y, transform.position.z);

    }

  }
}