using UnityEngine;
using System.Collections;
namespace it.Game.Environment.Level2.SpeedPit
{
  /// <summary>
  /// Квест с скоростной ямой
  /// </summary>
  public class SpeedPit : Environment
  {
	 [SerializeField]
	 private float _time = 2;
	 [SerializeField]
	 private SpeedPitBlock[] _blocks;

	 [ContextMenu("Player")]
	 public void Player()
	 {
		StartCoroutine(ActiveCor());
	 }

	 private IEnumerator ActiveCor()
	 {

		for (int i = 0; i < _blocks.Length; i++)
		{
		  _blocks[i].SetVisibleStone(_time);
		  yield return null;
		}
	 }

  }
}