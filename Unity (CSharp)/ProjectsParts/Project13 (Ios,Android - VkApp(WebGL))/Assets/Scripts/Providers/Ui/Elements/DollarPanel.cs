using Game.Base;
using Game.Game.Handlers;
using Game.Providers.Profile;
using Game.Providers.Profile.Handlers;
using Game.Providers.Profile.Signals;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Game.Providers.Ui.Elements
{
	public class DollarPanel : MonoBehaviour, IInjection
	{

		[SerializeField] private TMP_Text _coinsLabel;
		[SerializeField] private Button _addButton;

		private SignalBus _signalBus;
		private IProfileProvider _profileProvider;
		private OpenAddDollarHandler _openAddDollarHandler;
		private DollarHandler _dollarHandler;

		[Inject]
		private void Constructor(SignalBus signalBus,
		IProfileProvider profileProvider,
		OpenAddDollarHandler openAddDollarHandler,
		DollarHandler dollarHandler
		)
		{
			_signalBus = signalBus;
			_profileProvider = profileProvider;
			_openAddDollarHandler = openAddDollarHandler;
			_dollarHandler = dollarHandler;
		}

		public void Awake()
		{
			_addButton.onClick.AddListener(AddButtonTouch);
		}

		public void OnEnable()
		{
			_signalBus.Subscribe<DollarChangeSignal>(OnBucksChangeSignal);
			OnBucksChangeSignal();
		}

		public void OnDisable()
		{
			_signalBus.Unsubscribe<DollarChangeSignal>(OnBucksChangeSignal);
		}

		private void OnBucksChangeSignal()
		{
			_coinsLabel.text = _dollarHandler.CurrentValueString;
		}

		private void AddButtonTouch()
		{
			_openAddDollarHandler.OpenAddDollarDialog();
		}
	}
}
