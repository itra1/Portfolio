using UnityEngine;
using UnityEngine.UI;

public class GamePlay : MonoBehaviour
{

	[SerializeField] MeshRenderer[] ballsQueue;

	void OnEnable()
	{
		//BallPlayerGenerator.addNewBall += AddNewBallInArray;
	}

	void OnDisable()
	{
		//BallPlayerGenerator.addNewBall -= AddNewBallInArray;
	}

	//void AddNewBallInArray(BallProperty newBalllType)
	//{
	//	for (int i = 0; i < ballsQueue.Length; i++)
	//	{
	//		if (i != ballsQueue.Length - 1)
	//			ballsQueue[i].material = ballsQueue[i + 1].material;
	//		else
	//			ballsQueue[i].material = newBalllType.Material;
	//	}
	//}

}
