using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace KingBird.Ads {

	public class AdsKingBird : Singleton<AdsKingBird> {

		public static event System.Action<Phases> OnChangePhase;

		[SerializeField] private string iosApiKey;
		[SerializeField] private string androidApiKey;

		private string apiKey {
			get {
#if UNITY_IOS
				return iosApiKey;
#elif UNITY_ANDROID
				return androidApiKey;
#else
				return "undef";
#endif
			}
		}

		public AdsShowResult miniBanner { get; private set; }
		public AdsShowResult fullBanner { get; private set; }

		public List<SaveData> saveList = new List<SaveData>();

		[HideInInspector]
		public bool needShow = false;
		private int countOpenWindow = 0;

		private Phases _phase = Phases.none;


		public Phases phase {
			get { return _phase; }
			set {
				if (_phase == value) return;
				_phase = value;
				if (OnChangePhase != null) OnChangePhase(_phase);
			}
		}

		public enum Phases {
			none,
			ready,
			showing,
			close
		}

		public void OpenWindow() {
			countOpenWindow++;

			if (countOpenWindow >= 2) {
				CloseBanner();
			}

		}

		void Start() {
			KingBirdAds.Initialize(new AdsInitOptions().SetApplicationId(apiKey).SetLocale("en-US").SetFullBannerMinLaunch(0).SetFullBannerMinGameTime(0));
			phase = Phases.none;
		}

		public void Load() {
			if (PlayerPrefs.HasKey("ads"))
				saveList = JsonConvert.DeserializeObject<List<SaveData>>(PlayerPrefs.GetString("ads"));
			
		}

		private void Update() {

			if(miniBanner == null && KingBirdAds.IsReady(BannerPlacement.BannerSquareInApp))
				ShowMiniBanner();

			if (phase == Phases.none) {
				if(KingBirdAds.IsReady(BannerPlacement.BannerSquareInApp))
					phase = Phases.ready;
			}


			//if (fullBanner == null && KingBirdAds.IsReady(BannerPlacement.BannerFullScreenStatic))
			//	ShowFullBanner();

		}

		
		public void Save() {
			PlayerPrefs.SetString("ads", JsonConvert.SerializeObject(saveList));
		}

		public void ShowMiniBanner() {

			KingBirdAds.Show(BannerPlacement.BannerSquareInApp,
					new AdsShowOptions().SetShowOptionsType(AdsShowOptionsType.OnlyInfo).OnComplete(OnLoadMiniBanner));
		}

		public void ShowFullBanner() {
			KingBirdAds.Show(BannerPlacement.BannerFullScreenStatic,
					new AdsShowOptions().SetShowOptionsType(AdsShowOptionsType.FullScreenWithWait).OnComplete(OnLoadFullBanner));
		}

		private void OnLoadMiniBanner(AdsShowResult showResult) {

			miniBanner = showResult;

			bool oldSave = false;

			for (int i = 0; i < saveList.Count; i++) {
				if (saveList[i].id == showResult.BannerData.id) {
					oldSave = true;
					//needShow = (saveList[i].miniShowCount <= 0 && saveList[i].miniPlayStart < PlayerManager.Instance.playCount);

					if (needShow) {
						ClickBannerButton();
					}

				}
			}

			if (!oldSave) {
				var save = new SaveData();
				save.miniShowCount = 0;
				//save.miniPlayStart = PlayerManager.Instance.playCount;
				saveList.Add(save);
			}

			Save();
		}

		private void OnLoadFullBanner(AdsShowResult showResult) {
			fullBanner = showResult;
		}

		public void CloseBanner() {
			phase = Phases.ready;
		}

		public void ClickBannerButton() {
			if (miniBanner == null) return;

			needShow = false;

			for (int i = 0; i < saveList.Count; i++) {
				if (saveList[i].id == miniBanner.BannerData.id) {
					saveList[i].miniShowCount++;
				}
			}

			Save();
			phase = Phases.showing;

		}

		public void OpenBanner() {

			Application.OpenURL(miniBanner.BannerData.storeData.storeUrl);
			KingBirdAds.SendClick(BannerPlacement.BannerSquareInApp);
		}

		public void ClickFullBannerButton() {
			if (fullBanner == null) return;
			KingBirdAds.SendClick(BannerPlacement.BannerFullScreenStatic);
		}

	}

	public class SaveData {
		public int id;
		public int miniShowCount;
		public int miniPlayStart;
		public int maxiShowCount;
		public int maxiPlayStart;
	}

}