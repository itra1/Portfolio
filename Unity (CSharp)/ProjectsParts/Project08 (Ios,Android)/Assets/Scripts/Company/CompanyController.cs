using System;
using System.Collections;
using System.Collections.Generic;
using ExEvent;
using UnityEngine;
using GCSave = GameCompany.Save;
using GC = GameCompany;
using Newtonsoft.Json;

namespace GameCompany {
	[System.Serializable]
	public class CompanyController {
		
		public List<GC.Company> companies;
		public List<GCSave.Company> saveCompanies = new List<GCSave.Company>();
		public List<GC.Company> downloadCompanies;

		public List<int> byeLocationList = new List<int>();
		public bool byeAllLocation = false;
		
		private string _actualCompany = "en";
		public bool isLoadProcess = false;
		private bool needLoad = false;

		private bool _firstLoad;

		public int freePlayLocation;

		public bool firstLoad {
			get { return _firstLoad; }
			set {
				_firstLoad = value;
			}
		}

		public string actualCompany {
			get { return String.IsNullOrEmpty(_actualCompany) ? "en" : _actualCompany ; }
			set {
				_actualCompany = value;
				PlayerManager.Instance.Save();
				ExEvent.PlayerEvents.OnChangeCompany.Call(_actualCompany);
			}
		}

		public List<GC.Company> SaveLevels() {
			return companies;
		}


		public void LoadLevels(List<GC.Company> compamy) {
			companies = compamy;
		}

		public void SaveAllLocations() {
			for (int i = 0; i < saveCompanies.Count; i++) {

				for (int j = 0; j < saveCompanies[i].locations.Count; j++) {
					PlayerPrefs.SetString(String.Format("{0}_{1}", saveCompanies[i].shortCompany, j), JsonConvert.SerializeObject(saveCompanies[i].locations[j]));
				}
			}
		}

		public void SaveOneLocation(string company, int locationNum) {

			PlayerManager.Instance.Save();

			for (int i = 0; i < saveCompanies.Count; i++) {
				if (saveCompanies[i].shortCompany == company) {
					string dataString =
						JsonConvert.SerializeObject(saveCompanies.Find(x => x.shortCompany == company).locations[locationNum]);
					PlayerPrefs.SetString(String.Format("{0}_{1}", company, locationNum), dataString);

				}
			}
		}

		public void LoadAllLocation() {


			for (int i = 0; i < saveCompanies.Count; i++) {

				int num = 0;
				bool loop = true;

				while (loop) {

					string key = String.Format("{0}_{1}", saveCompanies[i].shortCompany, num);

					if (PlayerPrefs.HasKey(key)) {
						string dataString = PlayerPrefs.GetString(key);
						//string[] arr = key.Split(new char[] {'_'});
						var data = saveCompanies.Find(x => x.shortCompany == saveCompanies[i].shortCompany);

						if (data.locations == null)
							data.locations = new List<GCSave.Location>();

						data.locations.Add(JsonConvert.DeserializeObject<GCSave.Location>(dataString));

						num++;
					} else {
						loop = false;
					}

				}
				/*
				if(PlayerPrefs.HasKey(String.Format("{0}_{1}", saveCompanies[i].shortCompany, num)))


				if (saveCompanies[i].shortCompany == company) {
					Debug.Log("SaveOneLocation");
					PlayerPrefs.GetString(String.Format("{0}_{1}", company, locationNum),
						JsonConvert.SerializeObject(saveCompanies.Find(x => x.shortCompany == company).locations[locationNum]));

				}
				*/
			}
		}
//#if UNITY_EDITOR
		public void ForceLevelComplete() {

			var company = GetActualSaveCompany();

			for (int sl = 0; sl < company.locations.Count; sl++) {

				for (int slv = 0; slv < company.locations[sl].levels.Count; slv++) {

					if (!company.locations[sl].levels[slv].isComplited) {

						var level = GetActualCompany().locations[sl].levels[slv];

						for (int i = 0; i < level.words.Count; i++) {
							if (!company.locations[sl].levels[slv].words.Exists(x => x.word == level.words[i].word && level.words[i].primary && !x.isOpen)) {
								ReadWord(level.words[i].word);
							}
						}


						return;
					}

				}

			}

		}
//#endif
		public bool CheckFullInit() {
			return true;
			//try {
			//	//return saveCompanies[0].locations[0].levels.Count > 0;
			//} catch {
			//	return false;
			//}

		}

		public void FirstInitiate() {

			saveCompanies.Clear();

			for (int i = 0; i < companies.Count; i++) {

				GCSave.Company sc = new GCSave.Company();
				sc.bonusLocation = new GCSave.Location();
				sc.id = companies[i].id;
				sc.shortCompany = companies[i].short_name;
				sc.locations = new List<GCSave.Location>();
				saveCompanies.Add(sc);

				for (int c = 0; c < companies[i].locations.Count; c++) {

					GCSave.Location loc = new GCSave.Location();
					loc.levels = new List<GCSave.Level>();
					loc.id = companies[i].locations[c].id;
					sc.locations.Add(loc);

					for (int ll = 0; ll < companies[i].locations[c].levels.Count; ll++) {

						GCSave.Level lvl = new GCSave.Level();
						lvl.id = companies[i].locations[c].levels[ll].id;
						lvl.words = new List<GCSave.Word>();
						loc.levels.Add(lvl);

					}

				}

				GCSave.Location locBonus = new GCSave.Location();
				locBonus.levels = new List<GCSave.Level>();
				locBonus.id = companies[i].bonusLocation.id;
				sc.bonusLocation = locBonus;

				for (int ll = 0; ll < companies[i].bonusLocation.levels.Count; ll++) {

					GCSave.Level lvl = new GCSave.Level();
					lvl.id = companies[i].bonusLocation.levels[ll].id;
					lvl.words = new List<GCSave.Word>();
					locBonus.levels.Add(lvl);

				}

			}

		}


		public SaveProgress SaveCompany() {

			var saveprogress = new SaveProgress() {
				//company = companies,
				//progress = saveCompanies,
				progress = new List<GCSave.Company>(),
				lastBonus = lastBonus,
				activeCompany = actualCompany,
				byeLocationList = byeLocationList,
				byeAll = byeAllLocation ,
				pushLevelNoComplete = pushInfo
			};

			for (int i = 0; i < saveCompanies.Count; i++) {
				saveprogress.progress.Add(GCSave.Company.CreateForSave(saveCompanies[i]));
			}

			return saveprogress;

		}

		public void LoadCompany(SaveProgress progress) {
			//companies = progress.company;
			saveCompanies = progress.progress;
			lastBonus = progress.lastBonus;
			byeLocationList = progress.byeLocationList;
			byeAllLocation = progress.byeAll;
			PlayerManager.Instance.company.LoadAllLocation();
			pushInfo = progress.pushLevelNoComplete;

			actualCompany = progress.activeCompany;

		}

		public void AllDownload(Action OnComplete = null) {

			if (isLoadProcess) return;
			isLoadProcess = true;
			// Первоначальная инициализация активной компании
			//actualCompany = LanguageManager.Instance.activeLanuage.code;

			int locationCount = 0;

			AddDownloadQueue((CallbackFunc) => {
				DownloadCompany(() => {
					locationCount++;

					companies.ForEach((elem) => {

						AddDownloadQueue((CallbackFunc2) => {

							DownloadLocations(elem, () => {

								for (int i = 0; i < elem.locations.Count; i++) {
									if (elem.locations[i].locationType != LevelType.inapp || byeAllLocation || CheckByeLocation(i) || freePlayLocation == i) {
										if (freePlayLocation == i)
											GetSaveLocation(elem.short_name, i).isFreePlay = true;
										locationCount++;

										int num = i;
										AddDownloadQueue((CallbackFunc3) => {
											DownloadLevels(elem, elem.locations[num], () => {
												//PlayerEvents.OnLoad.Call();

												locationCount--;

												if (downloadOrder.Count == 0) {
													PlayerManager.Instance.Save(true);
													firstLoad = true;
													isLoadProcess = false;
													CheckNeedLoad();
													PlayerEvents.OnLoad.Call();
													if (OnComplete != null) OnComplete();
												}
												CallbackFunc3();
											}, () => {
												CallbackFunc3();
											});
										});

									}
								}

								//elem.locations.ForEach((loc) => {


								//});
								AddDownloadQueue((CallbackFunc3) => {
									DownloadLevels(elem, elem.bonusLocation,
										() => {
											locationCount--;

											if (String.IsNullOrEmpty(PlayerManager.Instance.translateLanuage)) {

												var ll = LanguageManager.Instance.lanuageLibrary.Find(x => x.type == Application.systemLanguage);
												if (ll == null) {
													ll = LanguageManager.Instance.lanuageLibrary.Find(x => x.type == SystemLanguage.English);
												}

												var trns = elem.bonusLocation.levels[0].words[0].translations.Find(x => x.lang == ll.code);
												if (trns == null) {
													trns = elem.bonusLocation.levels[0].words[0].translations[0];
												}
												PlayerManager.Instance.translateLanuage = trns.lang;

											}
											if (downloadOrder.Count == 0) {
												PlayerManager.Instance.Save(true);
												PlayerEvents.OnLoad.Call();
												firstLoad = true;
												isLoadProcess = false;
												CheckNeedLoad();
												if (OnComplete != null) OnComplete();
											}
											CallbackFunc3();
										},
										() => {
											CallbackFunc3();
										});
								});
								CallbackFunc2();
							}, () => {
								CallbackFunc2();
							});
						});
					}
					);
					CallbackFunc();
				}, () => {
					CallbackFunc();
					locationCount = 0;

				});
			});
		}

		private Queue<Action<Action>> downloadOrder = new Queue<Action<Action>>();
		private bool downloadProcess = false;
		private void AddDownloadQueue(Action<Action> exec) {
			downloadOrder.Enqueue(exec);
			if (!downloadProcess)
				PlayerManager.Instance.NextDownloadElement();
		}

		public IEnumerator NextDownloadQueue() {
			if (downloadProcess || downloadOrder.Count == 0) yield break;
			downloadProcess = true;
			Action<Action> func = downloadOrder.Dequeue();

			func(() => {
				downloadProcess = false;
				PlayerManager.Instance.NextDownloadElement();
			});
		}

		public void DownloadCompany(Action OnComplete, Action OnFailed) {

			NetManager.Instance.GetCompanies((downloadConpany) => {

				// Отказ при получении локации
				if (downloadConpany == null) {
					if (OnFailed != null) OnFailed();
					return;
				}

				for (int i = 0; i < downloadConpany.Count; i++) {

					GC.Company gc = companies.Find(x => x.short_name == downloadConpany[i].short_name);

					if (gc != null) {
						continue;
					}

					gc = downloadConpany[i];
					gc.locations = new List<GC.Location>();
					companies.Add(gc);

					GCSave.Company saveCompany = saveCompanies.Find(x => x.shortCompany == downloadConpany[i].short_name);

					if (saveCompany == null) {
						saveCompany = new GCSave.Company();
						saveCompany.shortCompany = downloadConpany[i].short_name;
						saveCompany.locations = new List<GCSave.Location>();
						saveCompany.bonusLocation = null;
						saveCompanies.Add(saveCompany);
					}
				}

				bool systemLangExists = false;
				for (int i = 0; i < companies.Count; i++) {
					if (companies[i].short_name == actualCompany)
						systemLangExists = true;
				}
				if (OnComplete != null) OnComplete();

			}
			);
		}
		private void DownloadLocations(GC.Company company, Action OnComplete, Action OnFailed = null) {

			//Debug.Log("DownloadLocations");

			NetManager.Instance.GetLocations(company.short_name, (downloadLocation) => {

				//Debug.Log(downloadLocation);
				if (downloadLocation == null) {
					if (OnFailed != null) OnFailed();
					return;
				}

				GCSave.Company saveCompany = saveCompanies.Find(x => x.shortCompany == company.short_name);
				int useNum = -1;
				for (int i = 0; i < downloadLocation.Count; i++) {

					if (downloadLocation[i].locationType != LevelType.bonus)
						useNum++;
					//Debug.Log("downloadLocation");

					// Если запись отсутствует
					if ((downloadLocation[i].locationType == LevelType.bonus && company.bonusLocation == null) || company.locations.Count < i) {

						GC.Location loc = downloadLocation[i];
						loc.levels = new List<Level>();
						loc.ParseLevelType();

						if (loc.locationType == LevelType.bonus)
							company.bonusLocation = loc;
						else
							company.locations.Add(loc);

						GCSave.Location saveLoc = new GCSave.Location();
						saveLoc.levels = new List<GCSave.Level>();
						saveLoc.id = loc.id;

						if (loc.locationType == LevelType.bonus)
							saveCompany.bonusLocation = saveLoc;
						else
							saveCompany.locations.Add(saveLoc);

						continue;
					}

					if (downloadLocation[i].locationType != LevelType.bonus) {


						if (!String.IsNullOrEmpty(company.locations[useNum].version) &&
						    company.locations[useNum].version == downloadLocation[i].version) {
							company.locations[useNum].isChange = false;
							continue;
						}
					}
					else {
						if (!String.IsNullOrEmpty(company.bonusLocation.version) &&
								company.bonusLocation.version == downloadLocation[i].version) {
							company.bonusLocation.isChange = false;
							continue;
						}
					}


					// Если запись присутствует
					bool existsSave = false;

					if (downloadLocation[i].locationType != LevelType.bonus) {
						for (int s1 = 0; s1 < saveCompany.locations[useNum].levels.Count; s1++) {
							if (saveCompany.locations[useNum].levels[s1].words.Count > 0)
								existsSave = true;
						}
					}

					if (!existsSave) {
						GC.Location loc = downloadLocation[i];
						loc.levels = new List<Level>();
						loc.isChange = true;
						loc.ParseLevelType();

						if (loc.locationType == LevelType.bonus)
							company.bonusLocation = loc;
						else
							company.locations[useNum] = loc;

						GCSave.Location saveLoc = new GCSave.Location();
						saveLoc.levels = new List<GCSave.Level>();
						saveLoc.id = loc.id;

						if (loc.locationType == LevelType.bonus)
							saveCompany.bonusLocation = saveLoc;
						else
							saveCompany.locations[useNum] = saveLoc;

					}


				}

				if (OnComplete != null) OnComplete();
			}
			);

		}

		private void DownloadLevels(GC.Company comp, GC.Location location, Action OnLoad, Action OnFailed = null) {

			if (!location.isChange && location.levels.Count > 0) {

				if (OnLoad != null) OnLoad();
				return;
			}

			string token = "";
#if UNITY_ANDROID
			if (location.locationType == LevelType.inapp) {

				int numLocation = 0;
				for (int i = 0; i < comp.locations.Count; i++) {
					if (comp.locations[i].id == location.id)
						numLocation = i;
				}

				token = BillingManager.Instance.GetTransactionToken(numLocation);
			}
#endif
			GCSave.Location saveLocation = location.locationType == LevelType.bonus
								? GetlSaveCompany(comp.short_name).bonusLocation
								: GetlSaveCompany(comp.short_name).locations.Find(x => x.id == location.id);

			NetManager.Instance.GetLevels(location.id, token, (downloadLevels) => {

				if (downloadLevels == null) {
					if (OnFailed != null) OnFailed();
					return;
				}

				for (int i = 0; i < downloadLevels.Count; i++) {

					// Если запись отсутствует
					if (location.levels.Count <= i) {

						GC.Level lvl = downloadLevels[i];

						location.levels.Add(lvl);
						SelectCoinsWord(lvl);

						GCSave.Level saveLoc = new GCSave.Level();
						saveLoc.words = new List<GCSave.Word>();
						saveLoc.id = lvl.id;
						saveLocation.levels.Add(saveLoc);
						continue;
					}

					// Если запись присутствует
					bool existsSave = saveLocation.levels[i].words.Count > 0;

					if (!existsSave) {

						GC.Level lvl = downloadLevels[i];

						SelectCoinsWord(lvl);
						location.levels[i] = lvl;

						GCSave.Level saveLoc = new GCSave.Level();
						saveLoc.words = new List<GCSave.Word>();
						saveLoc.id = lvl.id;

						saveLocation.levels[i] = saveLoc;
					}
				}

				if (OnLoad != null) OnLoad();

			}
			);

		}

		/// <summary>
		/// Добавляется один отдельно загруженный уровень
		/// </summary>
		public void AddOneLevelIfNeed(int locationId, int levelNum, GC.Level lvl) {
			GC.Location location = GetActualCompany().locations.Find(x => x.id == locationId);
			GC.Save.Location saveLocation = GetActualSaveCompany().locations.Find(x => x.id == locationId);

			if (location.levels.Count > levelNum) {
				if (saveLocation.levels[levelNum].words.Count > 0)
					return;
				OddOneLevel(locationId, levelNum, lvl);
			}

			for (int i = 0; i <= levelNum; i++) {
				if (location.levels.Count <= i) {
					location.levels.Add(GC.Level.CreateInstance());
					saveLocation.levels.Add(GC.Save.Level.CreateInstance());
				}
			}
			OddOneLevel(locationId, levelNum, lvl);
			PlayerManager.Instance.Save(true);

		}

		private void OddOneLevel(int locationId, int levelNum, GC.Level lvl) {
			GC.Location location = GetActualCompany().locations.Find(x => x.id == locationId);
			GC.Save.Location saveLocation = GetActualSaveCompany().locations.Find(x => x.id == locationId);

			location.levels[levelNum] = lvl;

			GCSave.Level saveLoc = new GCSave.Level();
			saveLoc.words = new List<GCSave.Word>();
			saveLoc.id = lvl.id;
			saveLocation.levels[levelNum] = saveLoc;
		}

		public bool billingRestore = false;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="locateNum"></param>
		public bool AddByeLocation(int locateNum, Action OnLoad) {
			if (CheckByeLocation(locateNum)) return false;
			byeLocationList.Add(locateNum);
			if (billingRestore) return false;
			billingRestore = true;
			if (!firstLoad) return false;
			if (isLoadProcess) return false;
			PlayerManager.Instance.Save(true);
			AllDownload(() => {
				billingRestore = false;
				OnLoad();
			});
			return true;
		}

		public bool AddByeAllLocation(Action OnLoad) {
			if (byeAllLocation)
				return false;
			byeAllLocation = true;
			if (billingRestore) return false;
			billingRestore = true;
			if (!firstLoad) return false;
			if (isLoadProcess) return false;
			PlayerManager.Instance.Save(true);
			AllDownload(() => {
				billingRestore = false;
				OnLoad();
			});
			return true;
		}

		public void CheckDownloadIfNeed(Action onComplete = null) {
			try {

				if (!firstLoad) return;
				if (billingRestore) return;

				if (isLoadProcess) {
					needLoad = true;
					return;
				}

				if (Locker.Instance != null)
					Locker.Instance.SetLocker(true);

				billingRestore = false;
				AllDownload(() => {
					try {
						if (Locker.Instance != null)
							Locker.Instance.SetLocker(false);
						if (onComplete != null)
							onComplete();
					} catch {
						if (Locker.Instance != null)
							Locker.Instance.SetLocker(false);
						if (onComplete != null)
							onComplete();
					}
				});
			} catch {
				if (onComplete != null)
					onComplete();
			}
		}

		public void CheckNeedLoad() {
			if (needLoad) {
				needLoad = false;
				CheckDownloadIfNeed();
			}
		}

		private void DownloadAllNewLocations(Action OnLoad) {
			int locations = 0;

			for (int c = 0; c < companies.Count; c++) {
				for (int i = 0; i < companies[c].locations.Count; i++) {
					if (CheckAccessLocation(i) /*&& companies[c].locations[i].levels.Count == 0*/) {
						locations++;
						DownloadLevels(companies[c], companies[c].locations[i], () => {

							locations--;
							if (locations <= 0) {
								PlayerManager.Instance.Save();

								if (OnLoad != null) OnLoad();
							}
						}, () => {

							locations--;
							if (locations <= 0) {
								PlayerManager.Instance.Save();
								//PlayerEvents.OnLoad.Call();
								if (OnLoad != null) OnLoad();
							}
						});
					}

				}
			}
		}

		public bool CheckByeLocation(int locateNum) {

			// Проверка, что локация открыта
			var comp = GetActualCompany();
			if (comp.locations[locateNum].locationType == LevelType.free)
				return true;

			//#if UNITY_IOS
			// На ios первые 2 локации бесплатные
			if (locateNum <= 1)
				return true;
			//#endif

			// Проверка, что куплены все локации
			if (byeAllLocation)
				return true;

			return byeLocationList.Contains(locateNum);
		}

		public bool CheckExistNextLevel() {
			return GetActualLocation().levelsCount > actualLevelNum + 1;
		}

		void SelectCoinsWord(GC.Level lvl) {

			List<GC.Word> lvlWord = lvl.GetAllPrimaryWords();

			int selectWord = UnityEngine.Random.Range(1, 3);
			if (lvlWord.Count < selectWord)
				selectWord = lvlWord.Count;

			List<string> useWordList = new List<string>();

			while (selectWord > 0) {

				string useWord = "";

				do {
					useWord = lvlWord[UnityEngine.Random.Range(0, lvlWord.Count)].word;
				} while (useWordList.Contains(useWord));
				selectWord--;
				useWordList.Add(useWord);

				int coins = UnityEngine.Random.Range(1, 3);
				if (useWord.Length < coins)
					coins = useWord.Length;

				List<int> useLetterList = new List<int>();

				while (coins > 0) {

					int useCoin = 0;
					do {
						useCoin = UnityEngine.Random.Range(0, useWord.Length);
					} while (useLetterList.Contains(useCoin));
					coins--;
					useLetterList.Add(coins);

					var gameWord = lvlWord.Find(x => x.word == useWord);

					if (gameWord != null) {
						if (gameWord.coinsLetters == null)
							gameWord.coinsLetters = new List<int>();
						gameWord.coinsLetters.Add(useCoin);
					}

				}
			}
		}

		public void SaveCompanyWord(GameCompany.Save.Word word, bool isCompleted = false, int starCount = 0) {

			GCSave.Company useCompany = saveCompanies.Find(x => x.shortCompany == actualCompany);
			GCSave.Location useLocation = GetActualSaveLocation();
			GCSave.Level useLevel = GetActualSaveLevel();
			GCSave.Word useWord = useLevel.words.Find(x => x.word == word.word);

			if (useWord != null && useWord.isOpen)
				return;

			if (!PlayerManager.Instance.company.isBonusLevel && !Tutorial.Instance.isTutorial && starCount > 0) {
				//AddStar();
				PlayerManager.Instance.stars += starCount;
				//useCompany.starCount += starCount;
				useLocation.starCount += starCount;
				useLevel.starCount += starCount;
			}

			if (useWord != null) {
				if (word.isOpen) {
					useWord.isOpen = word.isOpen;
					return;
				} else {
					for (int i = 0; i < word.hintLetters.Count; i++)
						if (!useWord.hintLetters.Contains(word.hintLetters[i]))
							useWord.hintLetters.Add(word.hintLetters[i]);
					return;
				}
			}

			useLevel.words.Add(word);
			//PlayerManager.Instance.Save();

		}

		public void CheckAdd3LocationAfter3DayOut() {
			return;
			if (!CheckCompleteLocation(0) && !CheckByeLocation(1)) return;

			foreach (var com in companies) {
				com.locations[1].locationType = LevelType.free;
				DownloadLevels(com, com.locations[1], () => {
					PlayerEvents.OnLoad.Call();
				});
			}

		}

		public bool CheckFullStarLocation(int locationNum) {

			if (GetActualSaveCompany().locations[locationNum].levels.Count == 0)
				return false;

			if (GetActualSaveCompany().locations[locationNum].levels.Count <
					GetActualCompany().locations[locationNum].levelsCount)
				return false;

			return !GetActualSaveCompany().locations[locationNum].levels.Exists(x => !x.isComplited);

		}

		public bool CheckCompleteLocation(int locationNum) {

			GC.Location location = GetActualCompany().locations[locationNum];
			//GCSave.Location saveLocation = GetSaveLocation(actualCompany, actualLocationNum);

			if (location.locationType == LevelType.inapp && !PlayerManager.Instance.company.CheckByeLocation(locationNum))
				return false;

			return CheckOpenLevel(locationNum, location.levelsCount - 1);
		}

		/// <summary>
		/// Праворка доступности уровня
		/// </summary>
		/// <param name="levelId">Идентификатор уровня</param>
		/// <returns>Статус открытого уровня</returns>
		public bool CheckOpenLevel(int locationNum, int levelNum) {

			//if (locationNum == 0) {
			//	if (levelNum <= 2) {
			//		return true;
			//	}
			//	int starCnt = GetActualSaveLocation().starCount;
			//	return ((((levelNum + 1) - 3) * 3) <= starCnt);
			//} else {
			if (locationNum == 0 && levelNum == 0)
				return true;

			GC.Company company = GetActualCompany();

			for (int i = 0; i <= locationNum; i++) {

				if (company.locations[i].levelsCount != company.locations[i].levels.Count)
					return false;

				GCSave.Location saveLoc = GetSaveLocation(i);

				for (int j = 0; j < saveLoc.levels.Count; j++) {
					
					if(i == locationNum && j >= levelNum)
						continue;

					if (!saveLoc.levels[j].isComplited)
						return false;

				}
			}

			return true;


			//GC.Company company = GetActualCompany();
			//int summaryLevels = 0;
			//int starCnt = 0;
			//for (int i = 0; i <= locationNum; i++) {
			//	starCnt += GetSaveLocation(i).starCount;

			//	if (i < locationNum)
			//		summaryLevels += company.locations[i].levelsCount;
			//	else
			//		summaryLevels += levelNum + 1;

			//}


			////return ((((summaryLevels + 1) - 2) * 3) <= starCnt);
			//return (((summaryLevels - 1) * 3) <= starCnt);
			////}

		}

		public GCSave.Level GetSaveLevel() {
			try {

				if (isBonusLevel)
					return saveCompanies.Find(x => x.shortCompany == actualCompany).bonusLocation.levels[lastBonus - 1];

				if (Tutorial.Instance.isTutorial)
					return saveCompanies.Find(x => x.shortCompany == actualCompany).bonusLocation.levels[Tutorial.Instance.tutorialLevel];

				return saveCompanies.Find(x => x.shortCompany == actualCompany).locations[actualLocationNum].levels[actualLevelNum];
			} catch {
				return null;
			}
		}

		public GCSave.Level GetSaveLevel(string shortLanuage, int locNum, int levelNum) {
			try {
				return saveCompanies.Find(x => x.shortCompany == shortLanuage).locations[locNum].levels[levelNum];
			} catch {
				return null;
			}
		}

		public GCSave.Location GetSaveLocation(string shortLanuage, int locationNum) {
			try {

				if (isBonusLevel || Tutorial.Instance.isTutorial)
					return saveCompanies.Find(x => x.shortCompany == actualCompany).bonusLocation;

				return saveCompanies.Find(x => x.shortCompany == shortLanuage).locations[locationNum];
			} catch {
				return null;
			}
		}

		/// <summary>
		/// Получить текущий уровень
		/// </summary>
		/// <returns></returns>
		public GC.Level GetActualLevel() {
			try {

				if (isBonusLevel)
					return GetActualLocation().levels[lastBonus - 1];

				if (Tutorial.Instance.isTutorial)
					return GetActualLocation().levels[Tutorial.Instance.tutorialLevel];

				return GetActualLocation().levels[actualLevelNum];
			} catch {
				return null;
			}
		}

		/// <summary>
		/// Получить текущую локацию
		/// </summary>
		/// <returns></returns>
		public GC.Location GetActualLocation() {
			try {

				if (isBonusLevel || Tutorial.Instance.isTutorial)
					return GetActualCompany().bonusLocation;

				return GetActualCompany().locations[actualLocationNum];
			} catch {
				return null;
			}
		}

		/// <summary>
		/// Получить текущий уровень
		/// </summary>
		/// <returns></returns>
		public GC.Company GetActualCompany() {
			try {
				return companies.Find(x => x.short_name == actualCompany);
			} catch {
				return null;
			}
		}

		public GCSave.Company GetActualSaveCompany() {
			try {
				return saveCompanies.Find(x => x.shortCompany == actualCompany);
			} catch {
				return null;
			}
		}

		public GCSave.Company GetlSaveCompany(string shortCode) {
			try {
				return saveCompanies.Find(x => x.shortCompany == shortCode);
			} catch {
				return null;
			}
		}

		public GCSave.Location GetActualSaveLocation() {
			try {

				if (isBonusLevel || Tutorial.Instance.isTutorial)
					return GetActualSaveCompany().bonusLocation;

				return GetActualSaveCompany().locations[actualLocationNum];
			} catch {
				return null;
			}
		}
		public GCSave.Location GetSaveLocation(int locnum) {
			try {

				return GetActualSaveCompany().locations[locnum];
			} catch {
				return null;
			}
		}

		public GCSave.Level GetActualSaveLevel() {
			try {

				if (isBonusLevel)
					return GetActualSaveLocation().levels[lastBonus - 1];

				if (Tutorial.Instance.isTutorial)
					return GetActualSaveLocation().levels[Tutorial.Instance.tutorialLevel];

				return GetActualSaveLocation().levels[actualLevelNum];
			} catch {
				return null;
			}
		}

		private int _actualLocationNum;
		public int actualLocationNum {
			get { return _actualLocationNum; }
			set {
				_actualLocationNum = value;
			}
		}

		public int actualLevelNum { get; set; }

		/// <summary>
		/// Чтение слова
		/// </summary>
		/// <param name="word"></param>
		public void ReadWord(string word) {

			GC.Level lvl = GetActualLevel();

			GCSave.Level saweLevel = GetSaveLevel();

			bool oldComplete = saweLevel != null && saweLevel.isComplited;

			GC.Word wrd = lvl.words.Find(x => x.word == word);

			if (wrd == null) {
				GameEvents.OnWordSelect.Call(word, SelectWord.no);
				return;
			}

			if (saweLevel != null && saweLevel.words.Exists(x => x.word == word && x.isOpen)) {
				GameEvents.OnWordSelect.Call(word, (wrd.primary ? SelectWord.repeat : SelectWord.conchRepeat));
				return;
			}

			if (!wrd.primary)
				ConchManager.Instance.AddValue();

			GameEvents.OnWordSelect.Call(word, (wrd.primary ? SelectWord.yes : SelectWord.conchYes));

			GCSave.Word saveWord = new GCSave.Word();
			saveWord.word = word;
			saveWord.isOpen = true;


			SaveCompanyWord(saveWord, true, wrd.starCount);

			if (!oldComplete && battlePhase == BattlePhase.game) {
				ChackLevelComplited();
			}

			SaveOneLocation(actualCompany, actualLocationNum);

			GameEvents.OnWordSave.Call();
		}
		/// <summary>
		/// проверка доступа к локации
		/// </summary>
		/// <param name="locationNum">Номер локации</param>
		/// <returns></returns>
		public bool CheckAccessLocation(int locationNum) {

			// Проверка, что локация куплены
			return CheckByeLocation(locationNum);

		}

		public bool CheckBeforeLocationComplete(int locationNum) {
			if (locationNum == 0)
				return true;

			GC.Company company = GetActualCompany();

			for (int i = 0; i < locationNum; i++) {

				if (company.locations[i].levelsCount != company.locations[i].levels.Count)
					return false;

				GCSave.Location saveLoc = GetSaveLocation(i);

				foreach (var lvl in saveLoc.levels) {
					if (!lvl.isComplited)
						return false;
				}
			}
			return true;


			//int summaryLevels = 0;
			//int starCnt = 0;
			//for (int i = 0; i < locationNum; i++) {
			//	starCnt += GetSaveLocation(i).starCount;

			//	if (i < locationNum)
			//		summaryLevels += company.locations[i].levelsCount;

			//}
			//return starCnt == summaryLevels * 3;
		}
		public bool CheckLocationComplete(int locationNum) {
			GC.Company company = GetActualCompany();
			return GetSaveLocation(locationNum).starCount == company.locations[locationNum].levels.Count * 3;
		}

		/// <summary>
		/// Проверка, что уровень выполнен
		/// </summary>
		void ChackLevelComplited() {

			GC.Level level = GetActualLevel();
			GCSave.Level saveLevel = GetSaveLevel();
			GC.Location loc = GetActualLocation();
			
			if (saveLevel.words.Count == 0) return;

			bool isFull = true;

			for (int i = 0; i < level.words.Count; i++) {
				if (!level.words[i].primary) continue;
				if (!saveLevel.words.Exists(c => c.word == level.words[i].word && c.isOpen)) {
					isFull = false;
				}
			}

			if (isFull) {
				saveLevel.isComplited = true;
				saveLevel.starCount = 3;
				RemoveNoCompletePush();
				battlePhase = BattlePhase.win;
				ExEvent.GameEvents.OnBattleChangePhase.Call(battlePhase);
				PlayerManager.Instance.Save();
				PlayerManager.Instance.CreateAllPush();
				try {
					FirebaseManager.Instance.LogEvent("level_complete");
					if (actualLocationNum <= 1)
						FirebaseManager.Instance.LogEvent("level_complete","level",String.Format("{0} - {1}", actualLocationNum + 1, actualLevelNum));

					if (loc.levelsCount == actualLevelNum - 1) 
						FirebaseManager.Instance.LogEvent("location_complete");
					

				} catch {
				}
			}

		}

		BattlePhase battlePhase = BattlePhase.none;
		public void OnLevelLoad() {
			battlePhase = BattlePhase.game;
			ExEvent.GameEvents.OnBattleChangePhase.Call(battlePhase);
		}

		public void BonusResume() {
			battlePhase = BattlePhase.game;
			ExEvent.GameEvents.OnDailyBonusResume.Call();
		}

		#region Бонусный левел

		public bool isBonusLevel;         // Активный бонусный уровень
		[HideInInspector]
		public int lastBonus = 0;       // Активный бонус
		[HideInInspector]
		public GamePhase lastPhase;           // Последняя открытая сцена

		public GC.Level GetNextScene() {
			return GetActualCompany().bonusLocation.levels[lastBonus - 1];
		}

		public void BonusLevelTimeEnd() {
			if (battlePhase != BattlePhase.game) return;
			battlePhase = BattlePhase.full;
			ExEvent.GameEvents.OnBattleChangePhase.Call(battlePhase);
		}

		#endregion

		#region Push

		private PushInfoSave pushInfo;

		public void CreateNoCompleteLevelPush() {

			GC.Level level = GetActualLevel();
			GCSave.Level saveLevel = GetSaveLevel();

			List<string> wordList = new List<string>();

			for (int i = 0; i < level.words.Count; i++) {
				if (!level.words[i].primary) continue;
				if (!saveLevel.words.Exists(c => c.word == level.words[i].word && c.isOpen)) {
					wordList.Add(level.words[i].word);
				}
			}

			if (wordList.Count <= 0) return;
			RemoveNoCompletePush();

			string word = wordList[UnityEngine.Random.Range(0, wordList.Count)];

			string sendText = String.Format("{0} '{1}' {2}", LanguageManager.GetTranslate("push.helpWord1") , word , LanguageManager.GetTranslate("push.helpWord2"));

			string pushId = PushManager.Instance.CreatePush(sendText, DateTime.Now.AddHours(12));

			pushInfo = new PushInfoSave() {
				langCompany = actualCompany,
				locationNum = actualLocationNum,
				levelNum = actualLevelNum,
				word = word,
				pushId = pushId
			};

		}

		private void RemoveNoCompletePush() {

			if (pushInfo == null) return;

			PushManager.Instance.RemovePush(pushInfo.pushId);
			pushInfo = null;
			/*
			if (actualCompany == pushInfo.langCompany
				&& actualLocationNum == pushInfo.levelNum
				&& actualLevelNum == pushInfo.levelNum
				&& !string.IsNullOrEmpty(pushInfo.pushId)) {
				PushManager.Instance.RemovePush(pushInfo.pushId);
				pushInfo = null;
			}
			*/
		}

		#endregion


	}

	public class PushInfoSave {

		public string langCompany;
		public int locationNum;
		public int levelNum;
		public string word;
		public string pushId;
	}

	public class SaveProgress {
		public List<GC.Company> company;
		public List<GCSave.Company> progress;
		public int lastBonus;
		public string activeCompany;
		public List<int> byeLocationList;
		public bool byeAll;
		public PushInfoSave pushLevelNoComplete;
	}


}

public enum BattlePhase {
	none,
	game,
	win,
	full
}