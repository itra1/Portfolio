using UnityEngine;
using System.Collections;
using Steamworks;
using DG.Tweening;

namespace it.Steam
{
  public class SteamManager : Singleton<SteamManager>
  {

	 //public enum AchievementTypes
	 //{
		//POLYGLOT,
		//LORE_MASTER,
		//NOVICE_LINGUIST
	 //}


	 private uint _appId = 1219000;
	 //private uint _appId = 480;

	 // Use this for initialization
	 void Start()
	 {
#if !UNITY_EDITOR

		Init();
		Application.quitting += () =>
		{
		  SteamClient.Shutdown();
		};
#endif
	 }

	 private void Init()
	 {
		try
		{
		  SteamClient.Init(_appId);

		  DOVirtual.DelayedCall(10, () =>
		  {
			 foreach (var a in SteamUserStats.Achievements)
			 {
				Debug.Log(string.Format("{0} {1} {2}", a.Identifier,  a.Name, a.State));
			 }
		  });
		}
		catch (System.Exception e)
		{
		  Debug.LogError(e.Message);
		}

	 }

  }
}