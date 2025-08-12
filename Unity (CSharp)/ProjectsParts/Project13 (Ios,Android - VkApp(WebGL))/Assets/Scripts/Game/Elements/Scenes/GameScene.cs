using UnityEngine;
using UnityEngine.Splines;

namespace Game.Game.Elements.Scenes
{
	public class GameScene : MonoBehaviour, IGameScene
	{
		[SerializeField] private Transform _boardPoint;
		[SerializeField] private Transform _playerWeaponPoint;
		[SerializeField] private Transform _opponentWeaponPoint;
		[SerializeField] private SpriteRenderer _backSprite;
		[SerializeField] private SplineContainer _spline;

		public Transform PlayerWeaponPoint => _playerWeaponPoint;
		public Transform OpponentWeaponPoint => _opponentWeaponPoint;
		public Transform BoardPoint => _boardPoint;
		public SplineContainer Spline => _spline;

		[ContextMenu("Resize back")]
		private void ResizeBack()
		{
			_backSprite.SpriteScaleToOrtoScreenVisible();
		}

		public void Awake()
		{
			ResizeBack();
		}
	}
}
