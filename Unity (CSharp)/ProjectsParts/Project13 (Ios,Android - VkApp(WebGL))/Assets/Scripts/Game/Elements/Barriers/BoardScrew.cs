using UnityEngine;

namespace Game.Game.Elements.Barriers
{
	public class BoardScrew : MonoBehaviour
	{
		[SerializeField] private Screw _screw;

		public void SetVisible(bool isVisible)
		{
			_screw.transform.Rotate(Vector3.forward, Random.Range(0, 360));
		}
	}
}
