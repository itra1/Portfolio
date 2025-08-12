using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif


#if UNITY_EDITOR

[CustomEditor(typeof(MeshBackerHelper))]
[CanEditMultipleObjects]
public class MeshBackerHelperEditor : Editor
{

	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		if (GUILayout.Button("All backer"))
		{
			for (int i = 0; i < targets.Length; i++)
			{
				((MeshBackerHelper)targets[i]).AllBacker();
			}
		}
		if (GUILayout.Button("Renderers enabled"))
		{
			for (int i = 0; i < targets.Length; i++)
			{
				((MeshBackerHelper)targets[i]).EnableAll();
			}
		}
		if (GUILayout.Button("Renderers disabled"))
		{
			for (int i = 0; i < targets.Length; i++)
			{
				((MeshBackerHelper)targets[i]).DisableAll();
			}
		}
		if (GUILayout.Button("Active Children"))
		{
			for (int i = 0; i < targets.Length; i++)
			{
				((MeshBackerHelper)targets[i]).ActiveChildren();
			}
		}
		if (GUILayout.Button("Deactive Children"))
		{
			for (int i = 0; i < targets.Length; i++)
			{
				((MeshBackerHelper)targets[i]).DeactiveChildren();
			}
		}


	}

}

#endif


public class MeshBackerHelper : MonoBehaviour
{
	[SerializeField]
	private MB3_MeshBaker _meshBacker;

#if UNITY_EDITOR


#endif

	[ContextMenu("Find Children Meshes")]
	public void FindMeshRenderers()
	{

#if UNITY_EDITOR

		if (_meshBacker == null)
			_meshBacker = GetComponent<MB3_MeshBaker>();
		_meshBacker.objsToMesh = new System.Collections.Generic.List<GameObject>();

		MeshRenderer[] meshes = transform.GetComponentsInChildren<MeshRenderer>(true);

		for (int i = 0; i < meshes.Length; i++)
		{
			if (meshes[i].transform.GetComponentInParent<LODGroup>() == null)
				_meshBacker.objsToMesh.Add(meshes[i].gameObject);
		}
#endif
	}

	[ContextMenu("Clear")]
	public void Clear()
	{
		_meshBacker.objsToMesh.Clear();
	}

	[ContextMenu("FindLodMeshRenderer 0")]
	public void FindLodMeshRenderer0()
	{
		FindLodMeshRenderer(0);
	}

	[ContextMenu("FindLodMeshRenderer 1")]
	public void FindLodMeshRenderer1()
	{
		FindLodMeshRenderer(1);
	}

	[ContextMenu("FindLodMeshRenderer 2")]
	public void FindLodMeshRenderer2()
	{
		FindLodMeshRenderer(2);
	}

	public void FindLodMeshRenderer(int lodNum)
	{
#if UNITY_EDITOR
		if (_meshBacker == null)
			_meshBacker = GetComponent<MB3_MeshBaker>();

		_meshBacker.objsToMesh = new System.Collections.Generic.List<GameObject>();

		LODGroup[] lods = transform.GetComponentsInChildren<LODGroup>(true);

		if (lods.Length == 0)
			return;

		for (int i = 0; i < lods.Length; i++)
		{
			int targetlod = lodNum;
			LOD[] lod = lods[i].GetLODs();

			while (lod.Length - 1 < targetlod)
				targetlod--;

			LOD l = lod[targetlod];

			for (int x = 0; x < l.renderers.Length; x++)
			{
				if (l.renderers[x].GetComponent<MeshRenderer>() != null)
					_meshBacker.objsToMesh.Add(l.renderers[x].gameObject);
			}
		}
#endif
	}

	public void RemoveBacker()
	{
		Transform b1 = transform.Find("MeshBackerLOD");
		if (b1 != null)
			DestroyImmediate(b1.gameObject);

		Transform b2 = transform.Find("MeshBackerSingle");
		if (b2 != null)
			DestroyImmediate(b2.gameObject);

		Transform b3 = transform.Find("Mesh Backer");
		if (b3 != null)
			DestroyImmediate(b3.gameObject);
	}

	[ContextMenu("EnableAll")]
	public void EnableAll()
	{
		SetActived(true);
	}

	[ContextMenu("DisableAll")]
	public void DisableAll()
	{
		SetActived(false);
	}

	public void SetActived(bool isActive)
	{
		//if (_meshBacker == null)
		//return;

		//if (_meshBacker.objsToMesh.Count == 0)
		//return;

		Renderer[] r = GetComponentsInChildren<Renderer>();

		for (int i = 0; i < r.Length; i++)
		{
			if (r[i].enabled != isActive)
				r[i].enabled = isActive;
		}

		//for (int i = 0; i < _meshBacker.objsToMesh.Count; i++)
		//{
		//_meshBacker.objsToMesh[i].GetComponent<MeshRenderer>().enabled = isActive;
		//}

	}

	[ContextMenu("DeactiveChildren")]
	public void DeactiveChildren()
	{
		for (int i = 0; i < transform.childCount; i++)
			transform.GetChild(i).gameObject.SetActive(false);
	}

	[ContextMenu("ActiveChildren")]
	public void ActiveChildren()
	{
		for (int i = 0; i < transform.childCount; i++)
			transform.GetChild(i).gameObject.SetActive(true);
	}

	[ContextMenu("AllBacker")]
	public void AllBacker()
	{
		RemoveBacker();
		LOD3Backer();
		SingleBacker();
	}

	[ContextMenu("SingleBacker")]
	public void SingleBacker()
	{
#if UNITY_EDITOR

		MB3_MeshBaker bb = gameObject.GetComponent<MB3_MeshBaker>();
		if (bb != null)
			DestroyImmediate(bb);

		MB3_MeshBaker baker = gameObject.AddComponent<MB3_MeshBaker>();
		baker.useObjsToMeshFromTexBaker = false;
		baker.meshCombiner.pivotLocationType = DigitalOpus.MB.Core.MB_MeshPivotLocation.boundsCenter;
		//baker.meshCombiner.pivotLocation = transform.position;
		FindMeshRenderers();

		if (baker.objsToMesh.Count == 0)
			return;

		bool createDimmy;
		baker.meshCombiner.resultSceneObject.transform.position = transform.position;
		MB3_MeshBakerEditorFunctions.BakeIntoCombined(baker, out createDimmy);
		baker.meshCombiner.resultSceneObject.transform.SetParent(transform);
		baker.EnableDisableSourceObjectRenderers(false);
		DestroyImmediate(baker);
		baker.meshCombiner.resultSceneObject.name = "MeshBackerSingle";
		LODGroup lg = baker.meshCombiner.resultSceneObject.AddComponent<LODGroup>();
		LOD[] lod = lg.GetLODs();
		lod = new LOD[1];
		lod[0].renderers = new Renderer[1];
		lod[0].renderers[0] = baker.meshCombiner.resultSceneObject.GetComponentInChildren<MeshRenderer>();
		lod[0].screenRelativeTransitionHeight = 0.004f;
		lod[0].fadeTransitionWidth = 0.1f;
		lg.SetLODs(lod);
		lg.enabled = true;
		lg.gameObject.isStatic = true;
		var flags = StaticEditorFlags.BatchingStatic
		 | StaticEditorFlags.NavigationStatic
		 | StaticEditorFlags.OccludeeStatic
		 | StaticEditorFlags.OccluderStatic
		 | StaticEditorFlags.OffMeshLinkGeneration
		 | StaticEditorFlags.ReflectionProbeStatic;
		GameObjectUtility.SetStaticEditorFlags(lg.gameObject, flags);
		DestroyImmediate(baker);
#endif
	}

	[ContextMenu("LOD3Backer")]
	public void LOD3Backer()
	{
#if UNITY_EDITOR

		MB3_MeshBaker bb = gameObject.GetComponent<MB3_MeshBaker>();
		if (bb != null)
			DestroyImmediate(bb);

		LOD[] _lods = new LOD[3];

		GameObject[] lodsResult = new GameObject[3];

		bool isExists = false;

		for (int lod = 0; lod < 3; lod++)
		{
			MB3_MeshBaker baker = gameObject.AddComponent<MB3_MeshBaker>();
			baker.useObjsToMeshFromTexBaker = false;
			FindLodMeshRenderer(lod);

			if (baker.objsToMesh.Count == 0)
			{
				DestroyImmediate(baker);
				continue;
			}

			isExists = true;

			bool createDimmy;
			MB3_MeshBakerEditorFunctions.BakeIntoCombined(baker, out createDimmy);

			baker.EnableDisableSourceObjectRenderers(false);

			//baker.meshCombiner.resultSceneObject.transform.SetParent(res.transform);
			_lods[lod].renderers = new Renderer[1];
			_lods[lod].renderers[0] = baker.meshCombiner.resultSceneObject.GetComponentInChildren<MeshRenderer>();

			lodsResult[lod] = baker.meshCombiner.resultSceneObject;
			DestroyImmediate(baker);
		}

		if (!isExists)
			return;

		GameObject res = new GameObject();
		res.name = "MeshBackerLOD";
		res.transform.SetParent(transform);
		res.transform.localPosition = Vector3.zero;
		res.transform.localScale = Vector3.one;
		res.transform.localRotation = Quaternion.identity;

		for (int i = 0; i < lodsResult.Length; i++)
		{
			lodsResult[i].transform.SetParent(res.transform);
		}

		LODGroup lGroun = res.AddComponent<LODGroup>();
		LOD[] newLods = lGroun.GetLODs();

		for (int i = 0; i < _lods.Length; i++)
		{
			newLods[i].renderers = _lods[i].renderers;
		}
		lGroun.fadeMode = LODFadeMode.CrossFade;

		newLods[0].screenRelativeTransitionHeight = 0.25f;
		newLods[0].fadeTransitionWidth = 0.1f;
		newLods[1].screenRelativeTransitionHeight = 0.075f;
		newLods[1].fadeTransitionWidth = 0.1f;
		newLods[2].screenRelativeTransitionHeight = 0.004f;
		newLods[2].fadeTransitionWidth = 0.1f;

		lGroun.SetLODs(newLods);
		lGroun.gameObject.isStatic = true;
		var flags = StaticEditorFlags.BatchingStatic
		 | StaticEditorFlags.NavigationStatic
		 | StaticEditorFlags.OccludeeStatic
		 | StaticEditorFlags.OccluderStatic
		 | StaticEditorFlags.OffMeshLinkGeneration
		 | StaticEditorFlags.ReflectionProbeStatic;
		GameObjectUtility.SetStaticEditorFlags(lGroun.gameObject, flags);

#endif
	}

}
