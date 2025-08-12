using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using Cells;
using ExEvent;

public class MiniMapBehaviour : Singleton<MiniMapBehaviour>, IDragHandler, IScrollHandler, IPointerDownHandler, IPointerUpHandler {

	public RectTransform miniMap;
	public RectTransform parentMap;
	public Image mapSprite;

	public Image playerAncorPrefab;

	private MiniMapConfig _minimapConfig;

	private List<PlayerAncor> players = new List<PlayerAncor>();
	private List<Image> instancePoint = new List<Image>();

	private float _minScaleMiniMap;
	private float _minScale;
	private float _actualScale;

	public Color me;
	public Color meComplete;
	public Color enemy;
	public Color enemyComplete;
	public Color friend;
	public Color friendComplete;

	public void SetMap(MiniMapConfig miniMapConfig) {
		this._minimapConfig = miniMapConfig;
		this.mapSprite.sprite = miniMapConfig.miniMap;
		this.mapSprite.rectTransform.sizeDelta = miniMapConfig.miniMap.rect.size;
		this.parentMap.anchoredPosition = Vector3.zero;
		this.mapSprite.rectTransform.anchoredPosition = Vector3.zero;
		parentMap.localScale = Vector3.one;

		if (this.mapSprite.rectTransform.sizeDelta.x < this.mapSprite.rectTransform.sizeDelta.y) {
			this._minScaleMiniMap = this.mapSprite.rectTransform.sizeDelta.x;
		}
		else {
			this._minScaleMiniMap = this.mapSprite.rectTransform.sizeDelta.y;
		}
		this._minScale = miniMap.sizeDelta.x / this._minScaleMiniMap;
		_actualScale = 1;
	}

	public void AddPlayer(PlayerBehaviour player) {

		PlayerAncor npa = new PlayerAncor();
		npa.ancor = Instantiate(playerAncorPrefab.gameObject, parentMap).GetComponent<RectTransform>();
		npa.ancor.transform.gameObject.SetActive(true);
		npa.player = player;

		if (mainPlayer != null) {

			if (player.playerInfo.pid == mainPlayer.playerInfo.pid) {
				npa.ancor.GetComponent<Image>().color = me;
			}else if (player.playerInfo.army != mainPlayer.playerInfo.army) {
				npa.ancor.GetComponent<Image>().color = enemy;
			}
			else {
				npa.ancor.GetComponent<Image>().color = friend;
			}

		}

		players.Add(npa);
	}

	[ExEvent.ExEventHandler(typeof(BattleEvents.BattleUpdate))]
	public void ChangeStatePlayer(BattleEvents.BattleUpdate battleUpdate) {

		instancePoint.ForEach(x => x.gameObject.SetActive(false));

		foreach (var army in battleUpdate.battleStart.data.army_positions) {

			Image elem = GetInstanceImage();
			elem.gameObject.SetActive(true);

			if (mainPlayer != null) {

				if (army.login == mainPlayer.playerInfo.login) {
					elem.color = (army.complete == 0 ? me : meComplete );
				} else if (army.army != mainPlayer.playerInfo.army) {
					elem.color = (army.complete == 0 ? enemy : enemyComplete); 
				} else {
					elem.color = (army.complete == 0 ? friend : friendComplete);
				}

			}
			Cell cell = CellDrawner.Instance.GetCellByGride(new Vector2(army.posX, army.posY));

			Vector2 playerPercent = new Vector2((cell.position.x - _minimapConfig.regionPointStart.x) / (_minimapConfig.regionPointEnd.x - _minimapConfig.regionPointStart.x),
																				(cell.position.z - _minimapConfig.regionPointStart.y) / (_minimapConfig.regionPointEnd.y - _minimapConfig.regionPointStart.y));

			elem.GetComponent<RectTransform>().anchoredPosition = new Vector2(-_minimapConfig.miniMap.rect.width / 2 + _minimapConfig.miniMap.rect.width * playerPercent.x
																						, -_minimapConfig.miniMap.rect.height / 2 + _minimapConfig.miniMap.rect.height * playerPercent.y);
			
		}

	}

	private Image GetInstanceImage() {
		Image res = instancePoint.Find(x => !x.gameObject.activeInHierarchy);

    float size = (_minimapConfig.miniMap.rect.width / CellDrawner.Instance.grideCountX)/2;

    if (res == null) {
			res = Instantiate(playerAncorPrefab.gameObject, parentMap).GetComponent<Image>();
      res.rectTransform.sizeDelta = new Vector2(size, size);

      instancePoint.Add(res);
		}
		return res;
	}
	
	public void RemovePlayer(PlayerBehaviour player) {
		players.RemoveAll(x => x.player.playerInfo.pid == player.playerInfo.pid);
	}

	private PlayerBehaviour mainPlayer;
	public void SetParentPlayer(PlayerBehaviour player) {
		mainPlayer = player;
		players.ForEach((Action<PlayerAncor>)((el) => {
			if (el.player.playerInfo.pid == mainPlayer.playerInfo.pid) {
				el.ancor.GetComponent<Image>().color = me;
			} else if (el.player.playerInfo.army != mainPlayer.playerInfo.army) {
				el.ancor.GetComponent<Image>().color = enemy;
			} else {
				el.ancor.GetComponent<Image>().color = friend;
			}
		}));
	}

	private void FixedUpdate() {
		if (_minimapConfig == null) return;
		players.ForEach(x => PositionPlayers(x));
	}

	private void PositionPlayers(PlayerAncor pc) {
		
		Vector2 playerPercent = new Vector2((pc.player.tr.position.x - _minimapConfig.regionPointStart.x) / (_minimapConfig.regionPointEnd.x - _minimapConfig.regionPointStart.x),
																				(pc.player.tr.position.z - _minimapConfig.regionPointStart.y)/(_minimapConfig.regionPointEnd.y - _minimapConfig.regionPointStart.y));
		
		pc.ancor.anchoredPosition = new Vector2(-_minimapConfig.miniMap.rect.width/2 + _minimapConfig.miniMap.rect.width* playerPercent.x
																					, -_minimapConfig.miniMap.rect.height / 2 + _minimapConfig.miniMap.rect.height * playerPercent.y);
	}

	public void OnDrag(PointerEventData eventData) {
		parentMap.anchoredPosition += eventData.delta;
		isPointerDown = false;
		CheckPosition();
	}

	public void OnScroll(PointerEventData eventData) {

		Vector3 newScale = parentMap.localScale + new Vector3(0.1f, 0.1f, 0.1f) * eventData.scrollDelta.y;

		if(newScale.x > 2) newScale = Vector3.one * 2;
		if (newScale.x < _minScale) newScale = Vector3.one * _minScale;
		_actualScale = newScale.x;
		
		parentMap.localScale = newScale;
		CheckPosition();
	}

	public void CheckPosition() {
		Vector2 delta = this.mapSprite.rectTransform.sizeDelta / 2 * _actualScale;

		parentMap.anchoredPosition = new Vector2(Mathf.Clamp(parentMap.anchoredPosition.x,-delta.x + miniMap.sizeDelta.x/2, delta.x - miniMap.sizeDelta.x/2)
																					, Mathf.Clamp(parentMap.anchoredPosition.y, -delta.y + miniMap.sizeDelta.y/2, delta.y - miniMap.sizeDelta.y/2)); 
	}
	
	public bool isPointerDown;

	public void OnPointerDown(PointerEventData eventData) {
		isPointerDown = true;
	}

	public void OnPointerUp(PointerEventData eventData) {
		if (!isPointerDown) return;
		isPointerDown = false;
	}

	public void SetCameraClickPosition(Vector2 eventPosition) {
		
		// Определение смещения внитри объекта
		Vector2 localPoint = eventPosition;

		Vector2 targetPoint = Vector2.zero;

		if (localPoint.x < 0) {
			targetPoint.x = mapSprite.rectTransform.sizeDelta.x / 2 - Mathf.Abs(localPoint.x);
		}else {
			targetPoint.x = mapSprite.rectTransform.sizeDelta.x / 2 + localPoint.x;
		}

		if (localPoint.y < 0) {
			targetPoint.y = mapSprite.rectTransform.sizeDelta.y / 2 - Mathf.Abs(localPoint.y);
		} else {
			targetPoint.y = mapSprite.rectTransform.sizeDelta.y / 2 + localPoint.y;
		}
		
		Vector2 pointView = new Vector2(targetPoint.x/ mapSprite.rectTransform.sizeDelta.x, targetPoint.y / mapSprite.rectTransform.sizeDelta.y);

		// Опеределили целевое положение
		Vector3 targetPosition = new Vector3(_minimapConfig.regionPointStart.x + ((_minimapConfig.regionPointEnd.x - _minimapConfig.regionPointStart.x)* pointView.x),
				0,
				_minimapConfig.regionPointStart.y + ((_minimapConfig.regionPointEnd.y - _minimapConfig.regionPointStart.y) * pointView.y));
		
		// Ищем смещение камеры
		Vector3 actialCentr = GameManager.Instance.GetMapPosition(Camera.main.ViewportToScreenPoint(new Vector3(0.5f, 0.5f, 0)));

		CameraController.Instance.gameObject.transform.position = targetPosition + (CameraController.Instance.gameObject.transform.position - actialCentr);

	}

	private struct PlayerAncor {
		public PlayerBehaviour player;
		public RectTransform ancor;
	}

}
