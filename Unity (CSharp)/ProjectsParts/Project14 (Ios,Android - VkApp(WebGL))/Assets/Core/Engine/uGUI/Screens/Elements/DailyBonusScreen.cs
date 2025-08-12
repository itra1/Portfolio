using Core.Engine.App.Base.Attributes;
using Core.Engine.Components.Audio;
using Core.Engine.Components.DailyBonus;
using Core.Engine.Signals;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Core.Engine.uGUI.Screens
{
	[PrefabName(ScreenTypes.DailyBonus)]
	public class DailyBonusScreen :Screen, IDailyBonusScreen
	{
		[SerializeField] private Image[] _items;

		private IDailyBonusProvider _dailyBonusProvider;

		private Color _colorActive = Color.white;
		private Color _colorUnactive = Color.white;

		[Inject]
		public void Initialize(IDailyBonusProvider dailyBonusProvider)
		{
			_colorUnactive.a = 0.5f;
			_dailyBonusProvider = dailyBonusProvider;
		}

		public override void Show()
		{
			base.Show();
			ProcessVisible();

			if (_dailyBonusProvider.ReadyNewGet)
				VisibleNewGet();
		}

		private void ProcessVisible()
		{
			for (int i = 0; i < _items.Length; i++)
			{
				_items[i].color = i + 1 <= _dailyBonusProvider.DaysGet ? _colorActive : _colorUnactive;
			}
		}

		private void VisibleNewGet()
		{
			_items[_dailyBonusProvider.DaysGet].DOColor(_colorActive,0.4f).
			OnComplete(_dailyBonusProvider.AddDay);
		}

		public void OnBackClick()
		{
			PlayAudio.PlaySound("click");
			_signalBus.Fire(new UGUIButtonClickSignal() { Name = ButtonTypes.FirstMenuOpen });
		}
	}
}