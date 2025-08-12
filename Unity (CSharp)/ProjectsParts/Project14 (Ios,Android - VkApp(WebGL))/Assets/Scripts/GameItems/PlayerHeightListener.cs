using UnityEngine;

namespace Scripts.GameItems
{
	public class PlayerHeightListener :MonoBehaviour
	{
		[SerializeField] private Transform _player;
		[SerializeField] private float _offsetY = 2.25f;
		[SerializeField] private float _offsetFinishY = 3.5f;

		private Vector3 _targetPosition;

		public Vector3 PositionFinishPlatform { get; set; }
		public Vector3 TargetPosition { get => _targetPosition; set => _targetPosition = value; }

		private void Update()
		{
			if (_player.transform.position.y < transform.position.y + _offsetY)
			{
				_targetPosition = new Vector3(transform.position.x, _player.transform.position.y - _offsetY, transform.position.z);
			}

			if (PositionFinishPlatform.y + _offsetFinishY > _targetPosition.y)
				_targetPosition.y = PositionFinishPlatform.y + _offsetFinishY;

			transform.position = _targetPosition;
		}

	}
}
