using UnityEngine;
using System.Collections;

namespace it.Game.Utils
{
  /// <summary>
  /// Отрисовка стрелки поворота
  /// </summary>
  public class DrawRotateArrow : MonoBehaviourBase
  {
	 [SerializeField]
	 private Transform _arrowSource;

	 public Transform Source => _arrowSource == null ? transform : _arrowSource;

	 private void OnDrawGizmos()
	 {
		Game.Utils.DrawArrow.ForGizmo(Source.position, Source.forward);
	 }

  }
}