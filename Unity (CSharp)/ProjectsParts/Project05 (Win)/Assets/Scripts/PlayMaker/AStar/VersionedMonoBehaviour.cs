using UnityEngine;
using System.Collections.Generic;
using HutongGames.PlayMaker;
using UnityEngine.AI;
using Pathfinding;

namespace it.Game.PlayMaker.AStar
{
  public abstract class VersionedMonoBehaviour : ActorController.ActorDriver, ISerializationCallbackReceiver, IVersionedMonoBehaviourInternal
  {
	 /// <summary>Version of the serialized data. Used for script upgrades.</summary>
	 [SerializeField]
	 [HideInInspector]
	 int version = 0;

	 public override void Awake()
	 {
		base.Awake();
		// Make sure the version field is up to date for components created during runtime.
		// Reset is not called when in play mode.
		// If the data had to be upgraded then OnAfterDeserialize would have been called earlier.
		if (Application.isPlaying) version = OnUpgradeSerializedData(int.MaxValue, true);
	 }

	 /// <summary>Handle serialization backwards compatibility</summary>
	 public override void Reset()
	 {
		base.Reset();
		// Set initial version when adding the component for the first time
		version = OnUpgradeSerializedData(int.MaxValue, true);
	 }

	 /// <summary>Handle serialization backwards compatibility</summary>
	 void ISerializationCallbackReceiver.OnBeforeSerialize()
	 {
	 }

	 /// <summary>Handle serialization backwards compatibility</summary>
	 void ISerializationCallbackReceiver.OnAfterDeserialize()
	 {
		var r = OnUpgradeSerializedData(version, false);

		// Negative values (-1) indicate that the version number should not be updated
		if (r >= 0) version = r;
	 }

	 /// <summary>Handle serialization backwards compatibility</summary>
	 protected virtual int OnUpgradeSerializedData(int version, bool unityThread)
	 {
		return 1;
	 }

	 void IVersionedMonoBehaviourInternal.UpgradeFromUnityThread()
	 {
		var r = OnUpgradeSerializedData(version, true);

		if (r < 0) throw new System.Exception();
		version = r;
	 }
  }
}