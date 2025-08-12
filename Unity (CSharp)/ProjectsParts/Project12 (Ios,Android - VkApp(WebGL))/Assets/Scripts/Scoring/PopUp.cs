using UnityEngine;

namespace Game.Scripts.Scoring
{
	public class PopUp : MonoBehaviour
	{
		public void Pop(Vector3 position, GameObject Spriteprefab)
		{
			_ = Instantiate(Spriteprefab, position, Quaternion.identity);
		}
	}
}
