using System;
using System.Collections;
using System.Collections.Generic;
using Network.Input;
using UnityEngine;

namespace Tutorial.Steps {

	public class Step0 : Step {
		
		public int mapNumber = 117;


		private int loadCount = 0;

		public override int step {
			get { return 1; }
		}

		public override void StartStep() {
			base.StartStep();

			MapManager.Instance.SceneLoader(mapNumber, () => {

				BattleManager.Instance.phase = BattleManager.BattlePhase.battle;
				//BattleManager.Instance._mapLoaded = true;
				//StartCoroutine(BattleStateUpdate());

				//ParceMovePosition(battleStart.poss_move);
				//ParceAttackCell(battleStart.attack_cell);
				//ParceMagicCell(battleStart.magic_cell);

				GenerateMyPlayer();
				GenerateOpponent();
				ExEvent.BattleEvents.LoadAllModels.CallAsync();

				ExEvent.BattleEvents.StartBattle.Call(null);
			});
			
		}

		private void GenerateMyPlayer() {
			PlayerInfo playerInf = new PlayerInfo();
			playerInf.class_id = 1;
			playerInf.model = 1;
			playerInf.race = 1;
			playerInf.pid = 007;
			playerInf.playerName = "Я";
			playerInf.login = "Я";
			playerInf.x = 15;
			playerInf.y = 16;
			playerInf.army = 0;
			playerInf.owner_id = "1";

			PlayersManager.Instance.myPlayer = new MyPlayer() {
				pid = playerInf.pid
			};

			PlayersManager.Instance.CreatePlayer(playerInf);
		}

		private void GenerateOpponent() {
			PlayerInfo playerInf = new PlayerInfo();
			playerInf.class_id = 110;
			playerInf.model = 1;
			playerInf.race = 0;
			playerInf.pid = 008;
			playerInf.playerName = "Злой дядя";
			playerInf.login = "Злой дядя";
			playerInf.x = 17;
			playerInf.y = 19;
			playerInf.army = 1;
			playerInf.hp_max = 100;
			playerInf.hp = 100;


			PlayersManager.Instance.CreatePlayer(playerInf);
		}

		[ExEvent.ExEventHandler(typeof(ExEvent.BattleEvents.LoadAllModels))]
		private void LoadAllModels(ExEvent.BattleEvents.LoadAllModels eventData) {
			loadCount++;

			if (loadCount >= 2) {
				StepComplete();
			}

			//PlayersManager.Instance.instancePlayers["007"].team = 0;
		}
	}

}