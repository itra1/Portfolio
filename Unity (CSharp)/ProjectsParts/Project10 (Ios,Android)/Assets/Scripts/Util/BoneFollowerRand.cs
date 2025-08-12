using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using Spine;
using Spine.Unity;

/// <summary>Sets a GameObject's transform to match a bone on a Spine skeleton.</summary>
[ExecuteInEditMode]
[AddComponentMenu("Spine/BoneFollower")]
public class BoneFollowerRand : MonoBehaviour
{

    [System.NonSerialized]
    public bool
        valid;
    public SkeletonRenderer skeletonRenderer;
    public Bone bone;
    public bool followZPosition = true;
    public bool followBoneRotation = true;

    public SkeletonRenderer SkeletonRenderer
    {
        get { return skeletonRenderer; }
        set
        {
            skeletonRenderer = value;
            Reset();
        }
    }


    /// <summary>If a bone isn't set, boneName is used to find the bone.</summary>

    [SpineBone(dataField: "skeletonRenderer")]
    public String[] boneName;
    public bool resetOnAwake = true;
    protected Transform cachedTransform;
    protected Transform skeletonTransform;

    public void HandleResetRenderer(SkeletonRenderer skeletonRenderer)
    {
        Reset();
    }

    public void Reset()
    {
        bone = null;
        cachedTransform = transform;
        valid = skeletonRenderer != null && skeletonRenderer.valid;
        if (!valid)
            return;
        skeletonTransform = skeletonRenderer.transform;

        skeletonRenderer.OnRebuild -= HandleResetRenderer;
        skeletonRenderer.OnRebuild += HandleResetRenderer;

        if (Application.isEditor)
            DoUpdate();
    }

    void OnDestroy()
    {
        //cleanup
        if (skeletonRenderer != null)
            skeletonRenderer.OnRebuild -= HandleResetRenderer;
    }

    public void Awake()
    {
        if (resetOnAwake)
            Reset();
    }

    void LateUpdate()
    {
        DoUpdate();
    }

    public void DoUpdate()
    {
        if (!valid)
        {
            Reset();
            return;
        }

        if (bone == null)
        {
            if (boneName == null || boneName.Length == 0)
                return;
            bone = skeletonRenderer.skeleton.FindBone(boneName[UnityEngine.Random.Range(0, boneName.Length)]);
            if (bone == null)
            {
                Debug.LogError("Bone not found: " + boneName, this);
                return;
            }
            else {

            }
        }

        Spine.Skeleton skeleton = skeletonRenderer.skeleton;
        float flipRotation = (skeleton.flipX ^ skeleton.flipY) ? -1f : 1f;

        if (cachedTransform.parent == skeletonTransform)
        {
            cachedTransform.localPosition = new Vector3(bone.worldX, bone.worldY, followZPosition ? 0f : cachedTransform.localPosition.z);

            if (followBoneRotation)
            {
                Vector3 rotation = cachedTransform.localRotation.eulerAngles;
                cachedTransform.localRotation = Quaternion.Euler(rotation.x, rotation.y, bone.Rotation * flipRotation);
            }

        }
        else {
            Vector3 targetWorldPosition = skeletonTransform.TransformPoint(new Vector3(bone.worldX, bone.worldY, 0f));
            if (!followZPosition)
                targetWorldPosition.z = cachedTransform.position.z;

            cachedTransform.position = targetWorldPosition;

            if (followBoneRotation)
            {
                Vector3 rotation = skeletonTransform.rotation.eulerAngles;

                cachedTransform.rotation = Quaternion.Euler(rotation.x, rotation.y, skeletonTransform.rotation.eulerAngles.z + (bone.Rotation * flipRotation));
            }
        }

    }
}
