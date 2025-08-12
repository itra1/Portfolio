using System.Collections;
using System.Collections.Generic;
using ExEvent;
using UnityEngine;
using Cells;

public class MagicManager : Singleton<MagicManager> {

	public MagicData[] magicPrefabs;

	public List<MagicData> instanceList = new List<MagicData>();
	public List<MagicInstanceData> activeList = new List<MagicInstanceData>();

	[ExEvent.ExEventHandler(typeof(BattleEvents.BattleUpdate))]
	public void ChangeStatePlayer(BattleEvents.BattleUpdate battleUpdate) {


		if (battleUpdate.battleStart.data.magic.Length <= 0) {

			if (activeList.Count > 0) {
				for (int i = 0; i < activeList.Count; i++) {
					activeList[i].instant.SetActive(false);
				}
				activeList.Clear();
			}
			return;
		}

		for (int i = 0; i < battleUpdate.battleStart.data.magic.Length; i++) {

			MagicType useMagicType = (MagicType) System.Int32.Parse(battleUpdate.battleStart.data.magic[i].magic_id);

			// Магия уже стоит
			if (
				activeList.Exists(
					x =>
						x.x == battleUpdate.battleStart.data.magic[i].x && x.y == battleUpdate.battleStart.data.magic[i].y &&
						x.type == (MagicType)System.Int32.Parse(battleUpdate.battleStart.data.magic[i].magic_id)))
				continue;

			// Устанавливаем магию
			MagicData data = GetInstance((MagicType)System.Int32.Parse(battleUpdate.battleStart.data.magic[i].magic_id));

			if (data == null) continue;

			activeList.Add(new MagicInstanceData() {
												x = battleUpdate.battleStart.data.magic[i].x,
												y = battleUpdate.battleStart.data.magic[i].y,
												type = data.type,
												instant = data.prefab
											}
			);

			data.prefab.SetActive(true);

			Cell tmpCell = CellDrawner.Instance.GetCellByGride(new Vector2(battleUpdate.battleStart.data.magic[i].x, battleUpdate.battleStart.data.magic[i].y));

			if (tmpCell == null) continue;

			data.prefab.transform.position = tmpCell.position;
		}
	}

	private MagicData GetInstance(MagicType type) {

		MagicData inst = instanceList.Find(x => x.type == type && !x.prefab.gameObject.activeInHierarchy);

		if (inst == null) {

			MagicData magicData = null;

			for (int j = 0; j < magicPrefabs.Length; j++) {
				if (magicPrefabs[j].type == type)
					magicData = magicPrefabs[j];
			}

			if (magicData == null)
				return null;

			GameObject elem = Instantiate(magicData.prefab);

			inst = new MagicData() {
				prefab = elem,
				type = type
			};

			instanceList.Add(inst);

		}

		return inst;
	}
	public enum MagicType {
		poison = 41,
		lava = 42
	}

	[System.Serializable]
	public class MagicData {
		public GameObject prefab;
		public MagicType type;
	}

	public class MagicInstanceData {
		public GameObject instant;
		public int x;
		public int y;
		public MagicType type;
	}

}
