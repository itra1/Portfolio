using System.Collections;
using System.IO;
using UnityEngine;

#if UNITY_EDITOR

using UnityEditor;

[CustomEditor(typeof(PlayerScreenShoter))]
public class PlayerScreenShoterEditor :Editor
{



	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		if (GUILayout.Button("Ready"))
		{
			((PlayerScreenShoter)target).Ready();
		}

		if (GUILayout.Button("Screen"))
		{
			((PlayerScreenShoter)target).ScreenShow();
		}

		if (GUILayout.Button("Screen single"))
		{
			((PlayerScreenShoter)target).SingleShoot();
		}

		if (GUILayout.Button("Dead"))
		{
			((PlayerScreenShoter)target).SetDead();
		}

		//if (GUILayout.Button("Test"))
		//{
		//  string tst = "player_high_1_1_1";
		//   Debug.Log(Application.dataPath + "/Editor/Render/Players/" + tst.Substring(12));
		//}

	}

}

#endif

[ExecuteInEditMode]
public class PlayerScreenShoter :MonoBehaviour
{

	public GameObject player;
	public GameObject horse;
	private GameObject target;
	public Animator templateAnimation;
	public Animator templateAnimationHorse;
	public bool isHorse;

	private Animator _useAnim;

	[SerializeField]
	private int scale = 1;
	[SerializeField]
	private int folderIndex = 1;
	private int number;

	private int step = -1;
	private bool _isDead;

	public void Ready()
	{

		if (!Application.isPlaying)
			return;

		number = 0;
		step = -1;

		if (isHorse)
		{
			player.GetComponent<PlayerBehaviour>().SetHorse(true);
			horse = GameObject.Find("Horse(Clone)");
			_useAnim = horse.GetComponentInChildren<Animator>();
			_useAnim.runtimeAnimatorController = templateAnimationHorse.runtimeAnimatorController;
			_useAnim.applyRootMotion = false;
			target = horse;
		}
		else
		{
			_useAnim = player.GetComponentInChildren<Animator>();
			_useAnim.runtimeAnimatorController = templateAnimation.runtimeAnimatorController;
			target = player;
		}

	}

	public void ScreenShow()
	{
		_isDead = false;

		if (number % 8 == 0 || step > 3)
			CheckState();

		switch (step)
		{
			case 0:
				{
					target.transform.localEulerAngles = new Vector3(0, 90, 0);
					_useAnim.SetTrigger("event" + number % 8);
					break;
				}
			case 1:
				{
					target.transform.localEulerAngles = new Vector3(0, -90, 0);
					_useAnim.SetTrigger("event" + number % 8);
					break;
				}
			case 2:
				{
					target.transform.localEulerAngles = new Vector3(0, 135, 0);
					_useAnim.SetTrigger("event" + number % 8);
					break;
				}
			case 3:
				{
					target.transform.localEulerAngles = new Vector3(0, -35, 0);
					_useAnim.SetTrigger("event" + number % 8);
					break;
				}
			case 4:
			case 5:
				{
					target.transform.localEulerAngles = new Vector3(0, 135, 0);
					_useAnim.SetTrigger("stay");
					break;
				}

			case 6:
				{
					//_useAnim.SetTrigger("die");
					SetDead();

					break;
				}

			default:
				{
					return;
				}
		}


		StartCoroutine(WainCor());
	}

	public void SetDead()
	{
		_isDead = true;
		target.transform.localEulerAngles = new Vector3(0, 90, 0);
		player.transform.localEulerAngles = new Vector3(0, 70, 0);
		if (isHorse)
			horse.GetComponent<HorseBehaviour>().parent.position = new Vector3(-9.38f, 0, 7.91f);
		else
			player.transform.position = new Vector3(-7.66f, 0, 0);
		//player.transform.position = new Vector3(-9.31f, 0, 4.3f);
		//player.GetComponent<PlayerBehaviour>().SetHorse(false);
		if (isHorse)
			horse.GetComponentInChildren<Animator>().SetTrigger("dead");
		player.GetComponentInChildren<Animator>().SetTrigger("dead");
	}

	private IEnumerator WainCor()
	{

		yield return new WaitForSecondsRealtime(_isDead ? 1.5f : 0.5f);

		var playerName = player.name.Substring(12);
		var pathGreate = Application.dataPath + "/Editor/Render/Players/" + playerName + "/" +
												folderIndex + "/";

		if (!Directory.Exists(pathGreate))
			Directory.CreateDirectory(pathGreate);

		ScreenCapture.CaptureScreenshot(pathGreate + playerName + "_" + number.ToString("000") + ".png", scale);
		yield return new WaitForSecondsRealtime(6f);
		number++;
		ScreenShow();
	}

	private int indexShoot = -1;
	public void SingleShoot()
	{
		indexShoot++;
		var playerName = player.name.Substring(12);
		var pathGreate = Application.dataPath + "/Editor/Render/Players/" + playerName + "/000/";

		if (!Directory.Exists(pathGreate))
			Directory.CreateDirectory(pathGreate);

		ScreenCapture.CaptureScreenshot(pathGreate + playerName + "_" + indexShoot.ToString("000") + ".png", scale);
	}

	private void CheckState()
	{
		step++;



	}

}
