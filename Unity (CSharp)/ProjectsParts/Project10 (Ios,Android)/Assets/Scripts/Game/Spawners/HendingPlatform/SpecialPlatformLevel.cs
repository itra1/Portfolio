using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class SpecialPlatformLevel : SpecialPlatforms {
	
	public List<HandingPlatformParametrs> handingPlatformParametrs;
	private RunSpawner runSpawner;
	private bool isGenerateReady;
	private float generateNext;
	private float calcNext;
	private bool isGenerateUse;
	private int activeHandPlatform;

	protected override void Init(RunSpawner runSpawner) {
		this.runSpawner = runSpawner;
		isGenerateReady = false;
		isGenerateUse = false;
	}

	public void Update() {
		UpdateHandingPlatform();
	}

	void UpdateHandingPlatform() {
		if (runSpawner.runnerPhase != RunnerPhase.run) return;

		if (isGenerateReady && generateNext <= RunnerController.playerDistantion && runSpawner.CheckGenerate(5))
			ActivateHandingPlatform();
		if (calcNext <= RunnerController.playerDistantion) HandingPlatformCalcDist();
	}

	void ActivateHandingPlatform() {
		isGenerateReady = false;
		runSpawner.lockedGenerate = true;

		isGenerateUse = true;

		ExEvent.RunEvents.SpecialPlatform.Call(true, (int)activeHandPlatform, () => {
			runSpawner.lockedGenerate = false;
			ExEvent.RunEvents.SpecialPlatform.Call(false, (int)activeHandPlatform, null);
		});
	}

	void HandingPlatformCalcDist() {

		if (handingPlatformParametrs[handingPlatformParametrs.Count - 2].distance <= RunnerController.playerDistantion)
			InfinityIncrementHandingPlatform();

		HandingPlatformParametrs nextCheck = handingPlatformParametrs.Find(x => x.distance > calcNext);

		if (nextCheck.types.Count > 0) {
			activeHandPlatform = nextCheck.GetPlatformType();
			isGenerateReady = true;
			generateNext = Random.Range(calcNext + 10, nextCheck.distance - 30);
		}
		calcNext = nextCheck.distance;
	}

	void InfinityIncrementHandingPlatform() {
		HandingPlatformParametrs tmp = new HandingPlatformParametrs();
		tmp.distance = handingPlatformParametrs[handingPlatformParametrs.Count - 1].distance
									 + (handingPlatformParametrs[handingPlatformParametrs.Count - 2].distance
									 - handingPlatformParametrs[handingPlatformParametrs.Count - 3].distance);
		tmp.types = handingPlatformParametrs[handingPlatformParametrs.Count - 1].types;
		tmp.probability = handingPlatformParametrs[handingPlatformParametrs.Count - 1].probability;
		handingPlatformParametrs.Add(tmp);
	}

	protected override void GetConfig() {
		List<Configuration.Levels.Platform> itemsData = Config.Instance.config.activeLevel.platforms;

		handingPlatformParametrs.Clear();

		for (int i = 0; i < itemsData.Count; i++) {

			HandingPlatformParametrs onePlatform = new HandingPlatformParametrs();
			onePlatform.distance = itemsData[i].distantion;
			onePlatform.probability = itemsData[i].hendingPlatform * 0.01f;
			onePlatform.types = new List<HandingPlatformParametrs.HandingPlatform>();
      
      if (itemsData[i].tipe1 > 0) {
        HandingPlatformParametrs.HandingPlatform oneHand = new HandingPlatformParametrs.HandingPlatform();
        oneHand.type = 1;
        oneHand.probability = itemsData[i].tipe1;
        onePlatform.types.Add(oneHand);
      }
      if (itemsData[i].tipe2 > 0) {
        HandingPlatformParametrs.HandingPlatform oneHand = new HandingPlatformParametrs.HandingPlatform();
        oneHand.type = 2;
        oneHand.probability = itemsData[i].tipe2;
        onePlatform.types.Add(oneHand);
      }
      if (itemsData[i].tipe3 > 0) {
        HandingPlatformParametrs.HandingPlatform oneHand = new HandingPlatformParametrs.HandingPlatform();
        oneHand.type = 3;
        oneHand.probability = itemsData[i].tipe3;
        onePlatform.types.Add(oneHand);
      }
      if (itemsData[i].tipe4 > 0) {
        HandingPlatformParametrs.HandingPlatform oneHand = new HandingPlatformParametrs.HandingPlatform();
        oneHand.type = 4;
        oneHand.probability = itemsData[i].tipe4;
        onePlatform.types.Add(oneHand);
      }
      if (itemsData[i].tipe5 > 0) {
        HandingPlatformParametrs.HandingPlatform oneHand = new HandingPlatformParametrs.HandingPlatform();
        oneHand.type = 5;
        oneHand.probability = itemsData[i].tipe5;
        onePlatform.types.Add(oneHand);
      }

      handingPlatformParametrs.Add(onePlatform);
		}
	}
	
}
