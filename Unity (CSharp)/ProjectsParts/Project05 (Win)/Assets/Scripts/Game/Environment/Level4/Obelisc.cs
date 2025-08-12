using UnityEngine;
using System.Collections;
using DG.Tweening;
namespace it.Game.Environment.Level4
{
  [System.Serializable]
  public class ObeliskEvent : UnityEngine.Events.UnityEvent<Obelisc> { }

  public class Obelisc : MonoBehaviour
  {
	 private const float TIME_LOAD = 7f;
	 private bool _usePlayer;
	 private Transform _crystal;
	 private bool _isComplete = false;
	 public bool IsComplete => _isComplete;

	 private float _actualTime;

	 [SerializeField]
	 private Transform _lineTarget;

	 [SerializeField]
	 private LineRenderer _line;
	 private Vector3[] _positionsLines = new Vector3[2];

	 [SerializeField]
	 private Material _crystalMat;
	 [SerializeField]
	 private Material _baseMaterial;
	 [SerializeField]
	 [ColorUsage(false, true)]
	 private Color _crystalEmissColor;

	 [SerializeField]
	 private Light _light;

	 private bool _materialInit = false;

	 public ObeliskEvent _onComplete;

	 private float _progress = 0;

	 private void Start()
	 {
		_progress = 0;
	 }

	 /// <summary>
	 /// Мессадж от контроллера
	 /// </summary>
	 public void ResetStateBossArena()
	 {
		_isComplete = false;
		_line.positionCount = 0;
		SetEmissionValue(0);
		_usePlayer = false;
	 }

	 public bool UsePlayer { get => _usePlayer; set => _usePlayer = value; }

	 public void PlayerConnect(Transform crystal)
	 {
		if (_isComplete)
		  return;

		_usePlayer = true;
		this._crystal = crystal;
		_positionsLines[0] = _crystal.transform.position;
		_positionsLines[1] = _lineTarget.position;
		_line.positionCount = 2;
		_line.SetPositions(_positionsLines);
		_actualTime = 0;
	 }

	 public void PlayerDisconnect()
	 {
		if (_isComplete)
		  return;

		_usePlayer = false;
		_line.positionCount = 0;
	 }

	 private void Update()
	 {
		if (_isComplete)
		  return;

		if (!_usePlayer)
		{
		  if (_actualTime > 0)
		  {
			 _actualTime -= Time.deltaTime * 3;
			 if (_actualTime < 0)
				_actualTime = 0;
			 _progress = _actualTime / TIME_LOAD;
			 SetEmissionValue(_progress);
		  }
		  return;
		}
		_actualTime += Time.deltaTime;
		_progress = _actualTime / TIME_LOAD;



		_positionsLines[0] = _crystal.transform.position;
		_line.SetPositions(_positionsLines);

		SetEmissionValue(_progress);

		if (_progress >= 1)
		{
		  SetComplete();
		}

	 }


	 private void SetEmissionValue(float percent)
	 {
		if (!_materialInit)
		  ReadyMaterial();

		_light.intensity = Mathf.Lerp(0, 10, percent);

		for (int i = 0; i < rend.Length; i++)
		{
		  if (rend[i].material.name.Contains("ObelisksBase"))
			 rend[i].material.SetFloat("_EmisDiff", Mathf.Lerp(0, 10, percent));
		  else
			 rend[i].material.SetColor("_EmissionColor", Color.Lerp(Color.black, _crystalEmissColor, percent));
		}


	 }
	 Renderer[] rend;
	 private void ReadyMaterial()
	 {
		rend = GetComponentsInChildren<Renderer>();
		for (int i = 0; i < rend.Length; i++)
		{
		  rend[i].material = Instantiate(rend[i].material);
		}
		_materialInit = true;
	 }

	 private void SetComplete()
	 {
		_isComplete = true;
		_line.positionCount = 0;
		_onComplete?.Invoke(this);
	 }

#if UNITY_EDITOR

	 [ContextMenu("SpawnMaterials")]
	 private void SpawnMaterials()
	 {
		Material crysMat = Instantiate(_crystalMat);
		Material baseMat = Instantiate(_baseMaterial);

		Renderer[] rends = GetComponentsInChildren<Renderer>();

		for(int i = 0; i < rends.Length; i++)
		{
		  for(int j = 0; j < rends[i].materials.Length; j++)
		  {
			 if (rends[i].materials[j].name == _baseMaterial.name)
				rends[i].materials[j] = baseMat;
			 if (rends[i].materials[j].name == _crystalMat.name)
				rends[i].materials[j] = crysMat;
		  }

		}
		_crystalMat = crysMat;
		_baseMaterial = baseMat;
	 }

	 [ContextMenu("SetMaxEmition")]
	 private void SetMaxEmition()
	 {
		_progress = 0;
		SetEmissionValue(_progress);
		DOTween.To(() => _progress, (x) => {
		  _progress = x;
		  SetEmissionValue(_progress);
		},1, 1);
	 }

	 [ContextMenu("SetMinEmition")]
	 private void SetMinEmition()
	 {
		_progress = 1;
		SetEmissionValue(_progress);
		DOTween.To(() => _progress, (x) => {
		  _progress = x;
		  SetEmissionValue(_progress);
		}, 0, 1);
	 }

	 [ContextMenu("ACtivate obelisk")]
	 private void ActivateObelisk()
	 {
		DOTween.To(()=> { return _progress; },
		  (x)=> {
			 _progress = x;
			 SetEmissionValue(_progress);

		  },1,5f);
	 }


#endif

  }
}