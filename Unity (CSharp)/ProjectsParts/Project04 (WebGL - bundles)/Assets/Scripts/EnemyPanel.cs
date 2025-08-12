using System;
using System.Collections;
using System.Collections.Generic;
using ExEvent;
using Network.Input;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyPanel : Singleton<EnemyPanel> {

	public GamePlayBattle gamePlayBattle;

	public GameObject backBlack;

	public RectTransform attackBlock;
	public RectTransform buttonBlock;

  public GameObject helperRegionAttack;

	[System.Serializable]
	public struct BodyElementIcons {
		public BodyElement bodyElement;
		public Image leftIcon;
		public Image rightIcon;
		public Image backImage;
		public Image allIcon;
	}

	private void OnEnable() {
		UpdateIconsAttack();
	}

	public Image avatar;
	public TextMeshProUGUI enemyName;

	public RectTransform healthLine;
	public TextMeshProUGUI healthValue;

	public RectTransform powerLine;
	public TextMeshProUGUI powerValue;

	private PlayerBehaviour selectPlayer;
	private EnemyData enemyData;

	public List<BodyElementIcons> bodyElement;

	public Button attackButton;
	public Button endButton;
	public bool isRearyNextRound = false;

	public RectTransform topPanel;
	public List<RectTransform> topPanelList = new List<RectTransform>();
	public RectTransform bottonPanel;
	public List<RectTransform> bottonPanelList = new List<RectTransform>();

	private int _place35Count;

  public Button roundEndButton;

  public class ItemStruct {
		public string itemId;
		public Texture2D testrure;
	}

	private List<ItemStruct> itemsList = new List<ItemStruct>();

	public void SetSelectPlayer(PlayerBehaviour selectPlayer) {
		
		this.selectPlayer = selectPlayer;

		backBlack.gameObject.SetActive(true);
		LoadPlayerInfo();
	}
  
	public void LoadPlayerInfo() {

		if (this.selectPlayer == null) return;

		NetworkManager.Instance.GetEnemyInfo(this.selectPlayer.playerInfo.pid, (enemyData) => {

			if (enemyData.enemy_info == null) {
				selectPlayer = null;
				gamePlayBattle.DeactivePanel();
				return;
			}

			this.enemyData = enemyData;
			enemyName.text = string.Format("{0} ({1}) {2}", enemyData.enemy_info.login, enemyData.enemy_info.level, enemyData.enemy_info.rating);
			backBlack.gameObject.SetActive(false);
			healthValue.text = String.Format("{0} / {1}", enemyData.enemy_info.hp, enemyData.enemy_info.hp_max);
			powerValue.text = String.Format("{0} / {1}", enemyData.enemy_info.mp, enemyData.enemy_info.mp_max);
			healthLine.sizeDelta = new Vector2(healthLine.sizeDelta.x, 192 * (enemyData.enemy_info.hp / enemyData.enemy_info.hp_max));
			powerLine.sizeDelta = new Vector2(powerLine.sizeDelta.x, 192 * (enemyData.enemy_info.mp / enemyData.enemy_info.mp_max));

			var elem = bodyElement.Find(x => x.bodyElement == BodyElement.head).backImage;
			elem.color = new Color(elem.color.r, elem.color.g, elem.color.b, 1f / 4 * int.Parse(enemyData.enemy_info.trauma_head));

			elem = bodyElement.Find(x => x.bodyElement == BodyElement.body).backImage;
			elem.color = new Color(elem.color.r, elem.color.g, elem.color.b, 1f / 4 * int.Parse(enemyData.enemy_info.trauma_body));

			elem = bodyElement.Find(x => x.bodyElement == BodyElement.leftHand).backImage;
			elem.color = new Color(elem.color.r, elem.color.g, elem.color.b, 1f / 4 * int.Parse(enemyData.enemy_info.trauma_left));

			elem = bodyElement.Find(x => x.bodyElement == BodyElement.rightHand).backImage;
			elem.color = new Color(elem.color.r, elem.color.g, elem.color.b, 1f / 4 * int.Parse(enemyData.enemy_info.trauma_right));

			elem = bodyElement.Find(x => x.bodyElement == BodyElement.leg).backImage;
			elem.color = new Color(elem.color.r, elem.color.g, elem.color.b, 1f / 4 * int.Parse(enemyData.enemy_info.trauma_foots));

			headArmor.text = enemyData.enemy_info.armor_head;
			bodyArmor.text = enemyData.enemy_info.armor_body;
			leftArmor.text = enemyData.enemy_info.armor_left;
			rightArmor.text = enemyData.enemy_info.armor_right;
			legArmor.text = enemyData.enemy_info.armor_foots;

			string[] arr = enemyData.enemy_info.player_img.Split(new char[] {'_'});

			if (arr.Length == 4) {

				while (avatar.transform.childCount > 0) DestroyImmediate(avatar.transform.GetChild(0).gameObject);

				if (itemsList.Exists(x => x.itemId == arr[3])) {

					Texture2D texture2d = itemsList.Find(x => x.itemId == arr[3]).testrure;

					Sprite spr = Sprite.Create(texture2d, new Rect(0, 0, texture2d.width, texture2d.height), new Vector2(0.5f, 0.5f));
					spr.texture.mipMapBias = 0;
					avatar.color = new Color32(255, 255, 255, 255);
					avatar.sprite = spr;
				}
				else {

					NetworkManager.Instance.GetItemImage(selectPlayer.playerInfo.class_id.ToString(), selectPlayer.playerInfo.model.ToString(), arr[3], "1", (texture2d) => {

						itemsList.Add(new ItemStruct() {
							itemId = arr[3],
							testrure = texture2d
						});

						Sprite spr = Sprite.Create(texture2d, new Rect(0, 0, texture2d.width, texture2d.height), new Vector2(0.5f, 0.5f));
						spr.texture.mipMapBias = 0;
						avatar.color = new Color32(255, 255, 255, 255);
						avatar.sprite = spr;

					});
				}

			}
			else {

				LoadItems(enemyData);
				try {
					NetworkManager.Instance.GetEnemyImage(this.selectPlayer.playerInfo.class_id.ToString(),
						this.selectPlayer.playerInfo.model.ToString(), this.selectPlayer.playerInfo.race.ToString(), (texture2d) => {

							Sprite spr = Sprite.Create(texture2d, new Rect(0, 0, texture2d.width, texture2d.height), new Vector2(0.5f, 0.5f));
							spr.texture.mipMapBias = 0;
							avatar.color = new Color32(255, 255, 255, 255);
							avatar.sprite = spr;

						});
				} catch { }
			}


		});
	}

	private void LoadTutorialInfo() {
		healthValue.text = String.Format("{0} / {1}", this.selectPlayer.playerInfo.hp, this.selectPlayer.playerInfo.hp_max);
		healthLine.sizeDelta = new Vector2(healthLine.sizeDelta.x, 192 * (this.selectPlayer.playerInfo.hp / this.selectPlayer.playerInfo.hp_max));
	}

	void LoadItems(EnemyData enemyInfo) {

		while (avatar.transform.childCount > 0) DestroyImmediate(avatar.transform.GetChild(0).gameObject);

		var tmpTop = topPanel.GetComponentsInChildren<Image>();
		for (int i = 0; i < tmpTop.Length; i++)
			DestroyImmediate(tmpTop[i].gameObject);

		var tmpBot = bottonPanel.GetComponentsInChildren<Image>();
		for (int i = 0; i < tmpBot.Length; i++)
			DestroyImmediate(tmpBot[i].gameObject);
		_place35Count = -1;
		foreach (var item in enemyInfo.enemy_items) {

			ItemStruct it = itemsList.Find(x => x.itemId == item.item_id);

			if (it != null) {
				SetItemInfo(item, it.testrure);
			} else {
				NetworkManager.Instance.GetItemImage(item.class_id, item.model, item.item_id, item.status, (texture2d) => {
					itemsList.Add(new ItemStruct() {
						itemId = item.item_id,
						testrure = texture2d
					});
					SetItemInfo(item, texture2d);
				});
			}


		}
	}

	public void SetItemInfo(ItemData item, Texture2D texture) {

		GameObject itemIcon = new GameObject();
		itemIcon.name = item.id;
		Image imageItem = itemIcon.AddComponent<Image>();
		itemIcon.transform.SetParent(avatar.transform);
		itemIcon.transform.localScale = Vector3.one;

		Sprite spr = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
		spr.texture.mipMapBias = 0;
		imageItem.sprite = spr;

		int typeId = int.Parse(item.type);
		ItemLibrary il = ItemManager.Instance.itemLibrary.Find(x => x.typeBegin <= typeId && x.typeEnd >= typeId);

		imageItem.rectTransform.sizeDelta = new Vector2(texture.width, texture.height);

		switch (item.place_id) {
			case "50": {
					imageItem.rectTransform.pivot = new Vector2(0.5f, 0.5f);
					imageItem.transform.SetParent(topPanel.transform);
					imageItem.rectTransform.anchoredPosition = topPanelList[2].anchoredPosition;
					return;
				}
			case "45": {
					imageItem.transform.SetParent(bottonPanel.transform);
					imageItem.rectTransform.anchoredPosition = bottonPanelList[2].anchoredPosition;
					return;
				}
			case "35": {
					imageItem.rectTransform.pivot = new Vector2(0.5f, 0.5f);
					Item35position(imageItem);
					return;
				}
			default:
				break;
		}

		imageItem.rectTransform.pivot = new Vector2(0, 1);
		imageItem.rectTransform.anchorMin = Vector2.up;
		imageItem.rectTransform.anchorMax = Vector2.up;


		if (item.status == "1") {
			imageItem.rectTransform.anchoredPosition = new Vector2(float.Parse(item.bind1_x) - 1, -float.Parse(item.bind1_y) + 2);
		} else
			imageItem.rectTransform.anchoredPosition = new Vector2(float.Parse(item.bind2_x) - 1, -float.Parse(item.bind2_y) + 2);

	}

	private void Item35position(Image itemImage) {
		_place35Count++;

		if (_place35Count <= 1) {
			itemImage.transform.SetParent(topPanel.transform);
			itemImage.rectTransform.anchoredPosition = topPanelList[_place35Count].anchoredPosition;
		}else if (_place35Count <= 3) {
			itemImage.transform.SetParent(topPanel.transform);
			itemImage.rectTransform.anchoredPosition = topPanelList[_place35Count+1].anchoredPosition;
		} else if (_place35Count <= 5) {
			itemImage.transform.SetParent(bottonPanel.transform);
			itemImage.rectTransform.anchoredPosition = bottonPanelList[_place35Count - 4].anchoredPosition;
		} else if (_place35Count <= 7) {
			itemImage.transform.SetParent(bottonPanel.transform);
			itemImage.rectTransform.anchoredPosition = bottonPanelList[_place35Count - 3].anchoredPosition;
		}

	}

	[ExEvent.ExEventHandler(typeof(ExEvent.BattleEvents.BattleRoundChange))]
	public void OnRoundChange(ExEvent.BattleEvents.BattleRoundChange roundChange) {

    roundEndButton.interactable = !Tutorial.Tutorial.Instance.isActive;


    ActiveButton(true);

		if (selectPlayer == null) {
			selectPlayer = null;
			gamePlayBattle.DeactivePanel();
			return;
		}

		ClearSelect();

		LoadPlayerInfo();

	}

	/// <summary>
	/// Атака
	/// </summary>
	public void AttackButton() {

		ActiveButton(false);

		PlayersManager.Instance.AttackButton(false, (res2) => {
			LoadPlayerInfo();
		}, () => {
			ActiveButton(true);
		});
	}

	/// <summary>
	/// Завершение раунда
	/// </summary>
	public void EndRoundButton() {
    if (Tutorial.Tutorial.Instance.isActive) return;
		ActiveButton(false);
		PlayersManager.Instance.EndRoundButton(LoadPlayerInfo, () => {
			ActiveButton(true);
		});
	}

	public void ClearSelect() {
		UpdateIconsAttack();
	}

	public TextMeshProUGUI headArmor;
	public TextMeshProUGUI bodyArmor;
	public TextMeshProUGUI leftArmor;
	public TextMeshProUGUI rightArmor;
	public TextMeshProUGUI legArmor;

	[ExEventHandler(typeof(GameEvents.ChangeAttackTargets))]
	public void ChangeAttackTargets(GameEvents.ChangeAttackTargets data) {
		UpdateIconsAttack();
	}

	private void ClickLeftButton(BodyElement element) {
		if (isRearyNextRound) return;

		PlayersManager.Instance.SetAttack(element, AttackHand.left);
	}
	private void ClickRightButton(BodyElement element) {
		if (isRearyNextRound) return;

		PlayersManager.Instance.SetAttack(element, AttackHand.right);
	}

	void ActiveButton(bool isActive) {
    if (!Tutorial.Tutorial.Instance.isActive) {
      attackButton.interactable = isActive;
      endButton.interactable = isActive && !Tutorial.Tutorial.Instance.isActive;
    }
		isRearyNextRound = !isActive;
	}

	private void UpdateIconsAttack() {

		foreach (var VARIABLE in bodyElement) {
			VARIABLE.leftIcon.gameObject.SetActive(false);
			VARIABLE.rightIcon.gameObject.SetActive(false);
			VARIABLE.allIcon.gameObject.SetActive(false);

			if (PlayersManager.Instance.attackLeft == VARIABLE.bodyElement)
				VARIABLE.leftIcon.gameObject.SetActive(true);

			if (PlayersManager.Instance.attackRight == VARIABLE.bodyElement)
				VARIABLE.rightIcon.gameObject.SetActive(true);

			if (PlayersManager.Instance.attackHand2 == VARIABLE.bodyElement)
				VARIABLE.allIcon.gameObject.SetActive(true);

		}
	}

	/// <summary>
	/// Тап по голове
	/// </summary>
	public void HeadMouseDown() {
    
		if (Input.GetMouseButtonDown(0)) {
			ClickLeftButton(BodyElement.head);
		}

		if (Input.GetMouseButtonDown(1)) {
			ClickRightButton(BodyElement.head);
		}
	}

	/// <summary>
	/// Тап по телу
	/// </summary>
	public void BodyMouseDown() {
    
		if (Input.GetMouseButtonDown(0)) {
			ClickLeftButton(BodyElement.body);
		}

		if (Input.GetMouseButtonDown(1)) {
			ClickRightButton(BodyElement.body);
		}
	}

	/// <summary>
	/// Тап по левой руке
	/// </summary>
	public void LeftHendMouseDown() {
    
		if (Input.GetMouseButtonDown(0)) {
			ClickLeftButton(BodyElement.leftHand);
		}

		if (Input.GetMouseButtonDown(1)) {
			ClickRightButton(BodyElement.leftHand);
		}
	}

	/// <summary>
	/// Тап по правой руке
	/// </summary>
	public void RightHandMuseDown() {
    
		if (Input.GetMouseButtonDown(0)) {
			ClickLeftButton(BodyElement.rightHand);
		}

		if (Input.GetMouseButtonDown(1)) {
			ClickRightButton(BodyElement.rightHand);
		}
	}

	/// <summary>
	/// Тап по ногам
	/// </summary>
	public void LegMouseDown() {
    
		if (Input.GetMouseButtonDown(0)) {
			ClickLeftButton(BodyElement.leg);
		}

		if (Input.GetMouseButtonDown(1)) {
			ClickRightButton(BodyElement.leg);
		}
	}

	public TextTemprary errorText;

	[ExEventHandler(typeof(BattleEvents.OnErrorAttack))]
	private void OnErrorAttack(BattleEvents.OnErrorAttack cl) {
		errorText.ShowText(cl.text);
	}


	[ExEventHandler(typeof(BattleEvents.OnPlayerWeaponChange))]
	private void OnErrorAttack(BattleEvents.OnPlayerWeaponChange cl) {

	}


}
