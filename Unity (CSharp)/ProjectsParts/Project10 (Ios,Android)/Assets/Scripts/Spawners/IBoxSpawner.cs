using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBoxSpawner {

	void Init(BoxSpawner boxSpawner);

	void ChangePhase(RunnerPhase runnerPhase);

	void Update();

	void GetConfig();

}
