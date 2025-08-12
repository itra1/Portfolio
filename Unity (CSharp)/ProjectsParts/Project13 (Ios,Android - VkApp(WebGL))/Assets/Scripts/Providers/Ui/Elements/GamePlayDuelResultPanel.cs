using UnityEngine;
using UnityEngine.UI;

namespace Game.Providers.Ui.Elements
{
	public class GamePlayDuelResultPanel : MonoBehaviour
	{
		[SerializeField] private Image _backgroundImage;
		[SerializeField] private Image _lightImage;
		[SerializeField] private Image _titleImage;

		public Image BackgroundImage => _backgroundImage;
		public Image LightImage => _lightImage;
		public Image TitleImage => _titleImage;
	}
}
