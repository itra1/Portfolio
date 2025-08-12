using UnityEngine;
using System.Collections;

/// <summary>
/// Обработка полета комня от голубя
/// </summary>
public class PigeonStone : MonoBehaviour {

  /// <summary>
  /// Твердое тело
  /// </summary>
  Rigidbody2D rb;

  /// <summary>
  /// Камень свободен
  /// </summary>
  public bool isFree;

  /// <summary>
  /// сила гравитации действующая на предмет
  /// </summary>
  public float gravity;

  Vector3 velocity = Vector3.zero;

  void OnEnable() {
    isFree = false;
    rb = GetComponent<Rigidbody2D>();
  }

  void Update() {
    Move();
  }

  void Move() {
    if (!isFree) return;
    velocity.y -= gravity * Time.deltaTime;
    rb.MovePosition(transform.position + velocity * Time.deltaTime);
  }
}
