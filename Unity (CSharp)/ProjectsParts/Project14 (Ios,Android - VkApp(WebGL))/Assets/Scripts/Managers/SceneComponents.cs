
using Core.Engine.Components.Game;
using Game.Tools;
using Itra.Utils;
using Scripts.GameItems;
using Scripts.Players;
using UnityEngine;

public class SceneComponents :SceneComponentsBase, IPlayerObject
{
	[SerializeField] private Transform _gameView;
	[SerializeField] private Player _player;
	[SerializeField] private Transform _platformParent;
	[SerializeField] private PlayerHeightListener _cameraParent;
	[SerializeField] private bool _iosTopBorder = true;
	[SerializeField] private LevelResetButton _resetButton;
	[SerializeField] private MeshRenderer _backGround;

	public Scripts.GameItems.Platforms.Platform FinishPlatform;

	private float _topOffset = 200;
	public Transform GameView => _gameView;
	public float HeightBorder => GameView.position.y + Height / 2 - ((Height / Camera.main.pixelHeight) * _topOffset);
	public Vector3 UpRight { get; set; }
	public Vector3 DownLeft { get; set; }
	public float Height { get; set; }
	public Player Player => _player;
	public Transform PlatformParent => _platformParent;
	public PlayerHeightListener CameraParent => _cameraParent;
	public GameObject PlayerGameObject => _player.gameObject;
	public LevelResetButton ResetButton => _resetButton;
	public MeshRenderer BackGround => _backGround;

	private void Awake()
	{
		CalcSize();
		BackGround.FillScreenPerspectiveCamera();
#if UNITY_IOS
		if (_iosTopBorder)
		{
			_topOffset += 76;
			IosOffset();
		}
#endif
	}

	private void CalcSize()
	{
		int displayWidth = Camera.main.pixelWidth;
		int displayHeight = Camera.main.pixelHeight;

		DownLeft = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 10));
		UpRight = Camera.main.ScreenToWorldPoint(new Vector3(displayWidth, displayHeight, 10));
		Height = UpRight.y - DownLeft.y;
	}

	public void IosOffset()
	{
		var baseRect = _screenParent.parent.GetComponent<RectTransform>();

		baseRect.sizeDelta = new(0, -76);
		baseRect.anchoredPosition = new(baseRect.anchoredPosition.x, baseRect.anchoredPosition.y - (76 / 2));
	}

	private void OnDrawGizmos()
	{
		float y = GameView.position.y - Height / 2 + 0.5f + (Height / 2400 * 150);
		Gizmos.DrawLine(new(-100, y, 0), new(100, y, 0));
	}

}