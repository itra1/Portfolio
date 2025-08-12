using UnityEngine;
using System.Collections;

namespace it.Game.Player.Handlers
{
  /// <summary>
  /// Нанесение урона игроку
  /// </summary>
  public class PlayerDamage : MonoBehaviourBase, IPlayerDamage
  {
	 [Tooltip("Размер наносимого урона")]
	 [SerializeField]
	 private float _value = 34;


	 [Tooltip("Игнорировать временной промежуток между уроном")]
	 [SerializeField]
	 private bool _ignoreGracePerion = false;

	 public float PlayerDamageValue => _value;

	 public bool IgnoreGracePeriod => _ignoreGracePerion; 
  
  }
}