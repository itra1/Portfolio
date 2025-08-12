using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine;
using Spine.Unity;

[ExecuteInEditMode]
public class PetFollower : MonoBehaviour {
  
  #region Inspector

  public SkeletonRenderer sourceSkeletonRenderer;
  public SkeletonRenderer SourceSkeletonRenderer {
    get { return sourceSkeletonRenderer; }
    set {
      sourceSkeletonRenderer = value;
      Initialize();
    }
  }

  /// <summary>If a bone isn't set in code, boneName is used to find the bone.</summary>
  [SpineBone(dataField: "targetSkeletonRenderer")]
  public String boneName;
  [SpineBone(dataField: "sourceSkeletonRenderer")]
  public String boneNameContact;

  public bool followZPosition = true;

  [Tooltip("Follows the skeleton's flip state by controlling this Transform's local scale.")]
  public bool followSkeletonFlip = false;

  public bool followScaleBone = false;

  [UnityEngine.Serialization.FormerlySerializedAs("resetOnAwake")]
  public bool initializeOnAwake = true;
  #endregion

  [NonSerialized] public Bone boneSource;
  [NonSerialized] public Bone rootBoneSource;
  Transform skeletonTransform;

  public void Awake() {
    if (initializeOnAwake) Initialize();
  }

  public void HandleRebuildRenderer(SkeletonRenderer skeletonRenderer) {
    Initialize();
  }

  public void Initialize() {

#if UNITY_EDITOR
    if (Application.isEditor)
      LateUpdate();
#endif
  }

  public void Reset() {
    Initialize();
  }


  void OnDestroy() {
  }

  public void LateUpdate() {
    

    FingBones();

    Transform thisTransform = this.transform;
    if (thisTransform.parent == skeletonTransform) {
      // Recommended setup: Use local transform properties if Spine GameObject is the immediate parent

    } else {
      // For special cases: Use transform world properties if transform relationship is complicated
      Vector3 sourceWorldPosition = SourceSkeletonRenderer.transform.TransformPoint(new Vector3(boneSource.worldX, boneSource.worldY, 0f));
      Vector3 rootWorldPosition = SourceSkeletonRenderer.transform.TransformPoint(new Vector3(rootBoneSource.worldX, rootBoneSource.worldY, 0f));
      //Debug.Log(boneSource.worldX + " : " + boneSource.worldY);
      //Debug.Log(rootBoneSource.worldX + " : " + rootBoneSource.worldY);
      //Debug.Log(sourceWorldPosition);
      //Debug.Log(rootWorldPosition);
      //Debug.Log(sourceWorldPosition - rootWorldPosition);

      Vector3 delta = sourceWorldPosition - rootWorldPosition;

      thisTransform.position = Player.Jack.PlayerController.Instance.transform.position - delta;

    }



  }

  private void FingBones() {

    if (boneSource == null) {
      if (string.IsNullOrEmpty(boneNameContact)) return;

      boneSource = sourceSkeletonRenderer.skeleton.FindBone(boneNameContact);
      if (boneSource == null) {
        Debug.LogError("Bone not found: " + boneNameContact, this);
        return;
      }
    }

    if (rootBoneSource == null) {
      if (string.IsNullOrEmpty("root")) return;

      rootBoneSource = sourceSkeletonRenderer.skeleton.FindBone("root");
      if (rootBoneSource == null) {
        Debug.LogError("Bone not found: " + "root", this);
        return;
      }
    }

  }

}
