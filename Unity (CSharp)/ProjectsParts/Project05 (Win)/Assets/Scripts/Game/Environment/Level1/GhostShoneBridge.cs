using UnityEngine;
using System.Collections;
using DG.Tweening;
using HutongGames.PlayMaker.Actions;

namespace it.Game.Environment.Level1
{
  /// <summary>
  /// Призрачный мост
  /// </summary>
  public class GhostShoneBridge : Environment
  {
	 /*
     * Состояния
     * 1 - не активен
     * 2 - постепенная активация
     * 3 - постоянно активна
     * 
     */

	 [SerializeField]
	 [ColorUsage(false, true)]
	 private Color _lightColor;

	 [SerializeField]
	 private float _timeVisiblePlatform = 6;
	 [SerializeField]
	 private float _timeBetweetPlatforms = 1;

	 protected override void Start()
	 {
		for(int i = 0; i < _platformData.Length; i++)
		{
		  _platformData[i].startLocalY = _platformData[i].plane.position.y;
		}


		base.Start();

		if (State == 0)
		{
		  HideAll();
		}
	 }

	 protected override void ConfirmState(bool isForce = false)
	 {
		base.ConfirmState(isForce);

		if (isForce)
		{
		  if (State == 1)
			 State = 0;
		}
		if (State == 2)
		  FixedOpen();

		if (State == 0)
		{
		  HideAll();
		}

	 }
	 private void HideAll()
	 {
		for (int i = 0; i < _platformData.Length; i++)
		{
		  var collider = _platformData[i].platform.GetComponentInChildren<Collider>(true);
		  collider.enabled = false;
		  Vector3 targetPosition = _platformData[i].plane.position;
		  targetPosition.y = _platformData[i].startLocalY;
		  _platformData[i].plane.position = targetPosition;
		}
	 }
	 //private void HideAll()
	 //{
	 //for (int i = 0; i < _platforms.Length; i++)
	 //{
	 //  var rederer = _platforms[i].GetComponentInChildren<Renderer>();
	 //  var material = rederer.material;
	 //  var color = material.GetColor("_Color");
	 //  color.a = 0;
	 //  material.SetColor("_Color", color);

	 //  var collider = _platforms[i].GetComponentInChildren<Collider>(true);
	 //  collider.enabled = false;

	 //}
	 //}
	 private void ShowAll(bool force = true)
	 {
		for (int i = 0; i < _platformData.Length; i++)
		{

		  var collider = _platformData[i].platform.GetComponentInChildren<Collider>(true);
		  collider.enabled = false;


		  if (force)
		  {
			 Vector3 localposition = _platformData[i].plane.position;
			 localposition.y = _platformData[i].startLocalY;
			 _platformData[i].plane.position = localposition;
		  }
		  else
		  {
			 _platformData[i].plane.DOMoveY(_platformData[i].startLocalY - 3.5f, 1);
		  }

		}

	 }
	 //private void ShowAll(bool force = true)
	 //{
	 //for (int i = 0; i < _platforms.Length; i++)
	 //{
	 //  var rederer = _platforms[i].GetComponentInChildren<Renderer>();
	 //  var color = rederer.material.GetColor("_Color");
	 //  var collider = _platforms[i].GetComponentInChildren<Collider>(true);
	 //  if (force)
	 //  {
	 //	 rederer.material.SetColor("_Color", new Color(color.r, color.g, color.b, 1));

	 //	 collider.enabled = true;
	 //  }
	 //  else
	 //  {
	 //	 DOTween.To(() => rederer.material.GetColor("_EmissionColor"), (x) => rederer.material.SetColor("_EmissionColor", x), _lightColor, 0.5f).OnComplete(() =>
	 //	 {

	 //		DOTween.To(() => rederer.material.GetColor("_EmissionColor"), (x) => rederer.material.SetColor("_EmissionColor", x), Color.black, 0.5f);
	 //	 });
	 //	 DOTween.To(() => rederer.material.GetColor("_Color"), (x) => rederer.material.SetColor("_Color", x), new Color(color.r, color.g, color.b, 1), 1f);
	 //  }

	 //}

	 //}

	 [SerializeField]
	 private PlatformData[] _platformData;

	 [System.Serializable]
	 private struct PlatformData
	 {
		public GameObject platform;
		public Transform plane;
		public float startLocalY;
	 }

	 public void Play()
	 {
		if (State == 1)
		  return;

		State = 1;
		StartCoroutine(PlayProcess());
	 }

	 private IEnumerator PlayProcess()
	 {
		for (int i = 0; i < _platformData.Length; i++)
		{
		  int index = i;
		  var collider = _platformData[i].platform.GetComponentInChildren<Collider>(true);

		  _platformData[i].plane.DOMoveY(_platformData[i].startLocalY - 3.5f, 1).OnComplete(() =>
		  {
			 collider.enabled = true;
			 _platformData[i].plane.DOMoveY(_platformData[i].startLocalY, 1).OnComplete(() =>
			 {
				if (index == _platformData.Length - 1)
				{
				  State = 0;
				}
				collider.enabled = false;
			 }).SetDelay(_timeVisiblePlatform);

		  });

		  //var rederer = _platforms[i].GetComponentInChildren<Renderer>();
		  //var material = rederer.material;
		  //rederer.material = Instantiate(material);


		  //var color = rederer.material.GetColor("_Color");
		  //color.a = 1;

		  //collider.enabled = true;
		  //DOTween.To(() => rederer.material.GetColor("_EmissionColor"), (x) => rederer.material.SetColor("_EmissionColor", x), _lightColor, 0.5f).OnComplete(() =>
		  //{

			 //DOTween.To(() => rederer.material.GetColor("_EmissionColor"), (x) => rederer.material.SetColor("_EmissionColor", x), Color.black, 0.5f);
		  //});
		  //DOTween.To(() => rederer.material.GetColor("_Color"), (x) => rederer.material.SetColor("_Color", x), new Color(color.r, color.g, color.b, 1), 1f).OnComplete(() =>
		  //{
			 //DOTween.To(() => rederer.material.GetColor("_Color"), (x) => rederer.material.SetColor("_Color", x), new Color(color.r, color.g, color.b, 0), 1f).OnComplete(() =>
			 //{
				//if (i == _platforms.Length - 1)
				//{
				//  State = 0;
				//}
				//collider.enabled = false;
			 //}).SetDelay(_timeVisiblePlatform);
		  //});
		  yield return new WaitForSeconds(_timeBetweetPlatforms);
		}
	 }

	 //private IEnumerator PlayProcess()
	 //{
	 //for (int i = 0; i < _platforms.Length; i++)
	 //{
	 //  var rederer = _platforms[i].GetComponentInChildren<Renderer>();
	 //  var material = rederer.material;
	 //  rederer.material = Instantiate(material);

	 //  var collider = _platforms[i].GetComponentInChildren<Collider>();

	 //  var color = rederer.material.GetColor("_Color");
	 //  color.a = 1;

	 //  collider.enabled = true;
	 //  DOTween.To(() => rederer.material.GetColor("_EmissionColor"), (x) => rederer.material.SetColor("_EmissionColor", x), _lightColor, 0.5f).OnComplete(() =>
	 //  {

	 //	 DOTween.To(() => rederer.material.GetColor("_EmissionColor"), (x) => rederer.material.SetColor("_EmissionColor", x), Color.black, 0.5f);
	 //  });
	 //  DOTween.To(() => rederer.material.GetColor("_Color"), (x) => rederer.material.SetColor("_Color", x), new Color(color.r, color.g, color.b, 1), 1f).OnComplete(() =>
	 //	{
	 //	  DOTween.To(() => rederer.material.GetColor("_Color"), (x) => rederer.material.SetColor("_Color", x), new Color(color.r, color.g, color.b, 0), 1f).OnComplete(() =>
	 //	  {
	 //		 if (i == _platforms.Length - 1)
	 //		 {
	 //			State = 0;
	 //		 }
	 //		 collider.enabled = false;
	 //	  }).SetDelay(_timeVisiblePlatform);
	 //	});
	 //  yield return new WaitForSeconds(_timeBetweetPlatforms);
	 //}
	 //}

	 public void FixedOpen()
	 {
		State = 2;
		Save();
		ShowAll(false);
	 }

  }
}