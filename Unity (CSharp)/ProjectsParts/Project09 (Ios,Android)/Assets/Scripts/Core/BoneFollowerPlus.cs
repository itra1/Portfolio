using UnityEngine;
using System.Collections;

namespace Spine.Unity {
  public class BoneFollowerPlus : MonoBehaviour {

    #region Inspector
    public SkeletonRenderer skeletonRenderer;
    public SkeletonRenderer SkeletonRenderer {
      get { return skeletonRenderer; }
      set {
        skeletonRenderer = value;
        Initialize();
      }
    }

    public Transform targetTransform;
    public Transform targetTrfm { get { return targetTransform != null ? targetTransform : transform; } }

    public float diffAngle;

    /// <summary>If a bone isn't set in code, boneName is used to find the bone.</summary>
    [SpineBone(dataField: "skeletonRenderer")]
    public string boneName;

    public bool followZPosition = true;
    public bool followBoneRotation = true;

    [Tooltip("Follows the skeleton's flip state by controlling this Transform's local scale.")]
    public bool followSkeletonFlip = false;

    [UnityEngine.Serialization.FormerlySerializedAs("resetOnAwake")]
    public bool initializeOnAwake = true;
    #endregion
    
    public bool valid;

    public Bone bone;
    Transform skeletonTransform;

    public void Awake() {
      if(initializeOnAwake) Initialize();
    }

    public void HandleRebuildRenderer(SkeletonRenderer skeletonRenderer) {
      Initialize();
    }

    public void Initialize() {
      bone = null;
      valid = skeletonRenderer != null && skeletonRenderer.valid;
      if(!valid) return;

      skeletonTransform = skeletonRenderer.transform;
      skeletonRenderer.OnRebuild -= HandleRebuildRenderer;
      skeletonRenderer.OnRebuild += HandleRebuildRenderer;

#if UNITY_EDITOR
      if(Application.isEditor)
        LateUpdate();
#endif
    }

    void OnDestroy() {
      if(skeletonRenderer != null)
        skeletonRenderer.OnRebuild -= HandleRebuildRenderer;
    }

    public void LateUpdate() {
      if(!valid) {
        Initialize();
        return;
      }

      if(bone == null) {
        if(string.IsNullOrEmpty(boneName)) return;

        bone = skeletonRenderer.skeleton.FindBone(boneName);
        if(bone == null) {
          Debug.LogError("Bone not found: " + boneName, this);
          return;
        }
      }

      Transform thisTransform = targetTrfm;
      if(thisTransform.parent == skeletonTransform) {
        // Recommended setup: Use local transform properties if Spine GameObject is the immediate parent
        thisTransform.localPosition = new Vector3(bone.worldX, bone.worldY, followZPosition ? 0f : thisTransform.localPosition.z);
        transform.localPosition = new Vector3(bone.worldX, bone.worldY, followZPosition ? 0f : thisTransform.localPosition.z);
        if(followBoneRotation) thisTransform.localRotation = Quaternion.Euler(0f, 0f, bone.WorldRotationX);

      } else {
        // For special cases: Use transform world properties if transform relationship is complicated
        Vector3 targetWorldPosition = skeletonTransform.TransformPoint(new Vector3(bone.worldX, bone.worldY, 0f));
        if(!followZPosition) targetWorldPosition.z = thisTransform.position.z;
        thisTransform.position = targetWorldPosition;
        transform.position = targetWorldPosition;

        if(followBoneRotation) {
          Vector3 worldRotation = skeletonTransform.rotation.eulerAngles;
          thisTransform.rotation = Quaternion.Euler(worldRotation.x, worldRotation.y, skeletonTransform.rotation.eulerAngles.z + bone.WorldRotationX + diffAngle);
        }
      }

      if(followSkeletonFlip) {
        float flipScaleY = bone.skeleton.flipX ^ bone.skeleton.flipY ? -1f : 1f;
        thisTransform.localScale = new Vector3(1f, flipScaleY, 1f);
      }
    }
  }

}