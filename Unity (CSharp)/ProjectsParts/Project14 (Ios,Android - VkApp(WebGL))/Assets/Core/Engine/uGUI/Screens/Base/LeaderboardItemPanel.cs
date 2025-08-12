using Core.Engine.Components.Leaderboard;
using TMPro;

using UnityEngine;

namespace Core.Engine.uGUI.Screens
{
	public class LeaderboardItemPanel : MonoBehaviour
	{
		[SerializeField] private TMP_Text _titleLabel;
		[SerializeField] private TMP_Text _valueLabel;
		[SerializeField] private Color _isMe;

		private RectTransform _rt;

		public RectTransform RT => _rt ??= GetComponent<RectTransform>();
		public void Set(LeaderboarItem item)
		{
			_titleLabel.text = $"{item.Index}. {item.Name}";
			_titleLabel.color = item.IsMe ? _isMe : Color.white;
			_valueLabel.text = item.Value.ToString();
			_valueLabel.color = item.IsMe ? _isMe : Color.white;

		}
	}
}
