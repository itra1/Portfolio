using UnityEngine;
using System.Collections;

public interface IPlatformSpawner {

	void Init(PlatformSpawner spawner);

	void Update();

	GameObject Spawn(Vector2 pos, PlatformType platformType = PlatformType.none);

}
