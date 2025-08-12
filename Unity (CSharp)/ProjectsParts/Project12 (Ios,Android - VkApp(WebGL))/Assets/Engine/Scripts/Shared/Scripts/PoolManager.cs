using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Shared
{
	/// <summary>
	/// The pool manager allows you to easily create pools of any game object prefab.
	/// </summary>
	public class PoolManager
	{
		#region Singleton Setup

		private Dictionary<int, Stack<GameObject>> _gameObjectPool = new();
		private Dictionary<int, int> _instantiatedGameObjects = new();
		private IPoolParent _poolParent;

		public static PoolManager Instance { get; private set; }

		public PoolManager(IPoolParent poolParent)
		{
			_poolParent = poolParent;
			Instance = this;
		}

		#endregion

		#region Instantiate

		/// <summary>
		/// Instantiate the game object using the pool.
		/// </summary>
		/// <param name="original">The original game object.</param>
		/// <returns>The instantiated game object.</returns>
		public static GameObject Instantiate(GameObject original)
		{
			return Instantiate(original, Vector3.zero, Quaternion.identity, null);
		}

		/// <summary>
		/// Instantiate the game object using the pool.
		/// </summary>
		/// <param name="original">The original game object.</param>
		/// <param name="parent">The parent of the instantiated game object.</param>
		/// <returns>The instantiated game object.</returns>
		public static GameObject Instantiate(GameObject original, Transform parent)
		{
			return Instance.InstantiateInternal(original, parent.position, original.transform.rotation, parent);
		}

		/// <summary>
		/// Instantiate the game object using the pool.
		/// </summary>
		/// <param name="original">The original game object.</param>
		/// <param name="position">The position to spawn the object at.</param>
		/// <param name="rotation">The rotation to spawn the object in.</param>
		/// <returns>The instantiated game object.</returns>
		public static GameObject Instantiate(GameObject original, Vector3 position, Quaternion rotation)
		{
			return Instance.InstantiateInternal(original, position, rotation, null);
		}

		/// <summary>
		/// Instantiate the game object using the pool.
		/// </summary>
		/// <param name="original">The original game object.</param>
		/// <param name="position">The position to spawn the object at.</param>
		/// <param name="rotation">The rotation to spawn the object in.</param>
		/// <param name="parent">The parent of the instantiated game object.</param>
		/// <returns>The instantiated game object.</returns>
		public static GameObject Instantiate(GameObject original, Vector3 position, Quaternion rotation,
				Transform parent)
		{
			return Instance.InstantiateInternal(original, position, rotation, parent);
		}

		/// <summary>
		/// Instantiate the game object using the pool.
		/// </summary>
		/// <param name="original">The original game object.</param>
		/// <param name="position">The position to spawn the object at.</param>
		/// <param name="rotation">The rotation to spawn the object in.</param>
		/// <param name="parent">The parent of the instantiated game object.</param>
		/// <returns>The instantiated game object.</returns>
		protected virtual GameObject InstantiateInternal(GameObject original, Vector3 position, Quaternion rotation,
				Transform parent)
		{
			var originalInstanceID = original.GetInstanceID();
			var instantiatedObject = ObjectFromPool(originalInstanceID, position, rotation, parent);
			if (instantiatedObject == null)
			{
				instantiatedObject = GameObject.Instantiate(original, position, rotation, parent);
				// Map the newly instantiated instance ID to the original instance ID so when the object is returned it knows what pool to go to.
				_instantiatedGameObjects.Add(instantiatedObject.GetInstanceID(), originalInstanceID);
			}
			else
			{
				instantiatedObject.transform.position = position;
				instantiatedObject.transform.rotation = rotation;
				instantiatedObject.transform.parent = parent;
			}

			return instantiatedObject;
		}

		/// <summary>
		/// Get an existing object from the pool the game object using the pool.
		/// </summary>
		/// <param name="originalInstanceID">The original game object id.</param>
		/// <param name="position">The position to spawn the object at.</param>
		/// <param name="rotation">The rotation to spawn the object in.</param>
		/// <param name="parent">The parent of the instantiated game object.</param>
		/// <returns>The instantiated game object.</returns>
		protected virtual GameObject ObjectFromPool(int originalInstanceID, Vector3 position, Quaternion rotation,
				Transform parent)
		{
			Stack<GameObject> pool;
			if (_gameObjectPool.TryGetValue(originalInstanceID, out pool))
			{
				while (pool.Count > 0)
				{
					var instantiatedObject = pool.Pop();
					// The object may be null if it was removed from an additive scene. Keep popping from the pool until the pool has a valid object or is empty.
					if (instantiatedObject == null)
					{ continue; }

					instantiatedObject.transform.position = position;
					instantiatedObject.transform.rotation = rotation;
					instantiatedObject.transform.parent = parent;
					instantiatedObject.SetActive(true);
					// Map the newly instantiated instance ID to the original instance ID so when the object is returned it knows what pool to go to.
					_instantiatedGameObjects.Add(instantiatedObject.GetInstanceID(), originalInstanceID);
					return instantiatedObject;
				}
			}

			return null;
		}

		#endregion

		#region Getter

		/// <summary>
		/// Checks if the object exists in the pool.
		/// </summary>
		/// <param name="instantiatedObject">The object potentially in the pool</param>
		/// <returns>True if the object is part of the pool</returns>
		public static bool HasObject(GameObject instantiatedObject)
		{
			return Instance.HasObjectInternal(instantiatedObject);
		}

		/// <summary>
		/// Checks if the object exists in the pool.
		/// </summary>
		/// <param name="instantiatedObject">The object potentially in the pool</param>
		/// <returns>True if the object is part of the pool</returns>
		private bool HasObjectInternal(GameObject instantiatedObject)
		{
			var instantiatedInstanceID = instantiatedObject.GetInstanceID();
			return _instantiatedGameObjects.ContainsKey(instantiatedInstanceID);
		}

		/// <summary>
		/// Get multiple objects from the pool.
		/// </summary>
		/// <param name="original">The original game object.</param>
		/// <param name="objects">The array where the objects will be stored.</param>
		/// <returns>The amount of items in the array.</returns>
		public static int GetObjectsFromPool(GameObject original, ref GameObject[] objects)
		{
			var originalInstanceID = original.GetInstanceID();
			return Instance.GetObjectsFromPoolInternal(originalInstanceID, ref objects);
		}

		/// <summary>
		/// Get multiple objects from the pool.
		/// </summary>
		/// <param name="original">The original game object.</param>
		/// <param name="objects">The array where the objects will be stored.</param>
		/// <param name="active">The array where the information whether the objects are active or not.</param>
		/// <returns>The amount of items in the array.</returns>
		public static int GetObjectsFromPool(GameObject original, ref GameObject[] objects, ref bool[] active)
		{
			var originalInstanceID = original.GetInstanceID();
			return Instance.GetObjectsFromPoolInternal(originalInstanceID, ref objects, ref active);
		}

		/// <summary>
		/// Get multiple objects from the pool.
		/// </summary>
		/// <param name="original">The original game object.</param>
		/// <param name="objects">The array where the objects will be stored.</param>
		/// <returns>The amount of items in the array.</returns>
		protected virtual int GetObjectsFromPoolInternal(int originalInstanceID, ref GameObject[] objects)
		{
			if (!_gameObjectPool.TryGetValue(originalInstanceID, out var pool))
			{ return 0; }

			if (pool.Count > objects.Length)
			{ Array.Resize(ref objects, pool.Count); }

			pool.CopyTo(objects, 0);

			return pool.Count;
		}

		/// <summary>
		/// Get multiple objects from the pool.
		/// </summary>
		/// <param name="original">The original game object.</param>
		/// <param name="objects">The array where the objects will be stored.</param>
		/// <param name="active">The array where the information whether the objects are active or not.</param>
		/// <returns>The amount of items in the array.</returns>
		protected virtual int GetObjectsFromPoolInternal(int originalInstanceID, ref GameObject[] objects,
				ref bool[] active)
		{
			var count = GetObjectsFromPoolInternal(originalInstanceID, ref objects);

			if (count == 0)
			{ return 0; }

			if (count > active.Length)
			{ Array.Resize(ref active, count); }

			for (int i = 0; i < count; i++)
			{
				var instantiatedObject = objects[i];

				active[i] = _instantiatedGameObjects.ContainsKey(instantiatedObject.GetInstanceID());
			}

			return count;
		}

		#endregion

		#region Destroy

		/// <summary>
		/// Remove the object from the pool
		/// </summary>
		/// <param name="instantiatedObject">The object to return to the pool.</param>
		/// <param name="force">Force destroy even if the object is not part of a the pool.</param>
		public static void Destroy(GameObject instantiatedObject, bool force)
		{
			if (Instance == null)
			{ return; }

			if (force && !HasObject(instantiatedObject))
			{
				GameObject.Destroy(instantiatedObject);
				return;
			}

			Instance.DestroyInternal(instantiatedObject);
		}

		/// <summary>
		/// Remove the object from the pool
		/// </summary>
		/// <param name="instantiatedObject">The object to return to the pool.</param>
		public static void Destroy(GameObject instantiatedObject)
		{
			PoolManager.Destroy(instantiatedObject, false);
		}

		/// <summary>
		/// Remove the object from the pool
		/// </summary>
		/// <param name="instantiatedObject">The object to return to the pool.</param>
		private void DestroyInternal(GameObject instantiatedObject)
		{
			var instantiatedInstanceID = instantiatedObject.GetInstanceID();
			int originalInstanceID;
			if (!_instantiatedGameObjects.TryGetValue(instantiatedInstanceID, out originalInstanceID))
			{
				Debug.LogErrorFormat("Can't find the object {0} with instance ID {1}", instantiatedObject.name,
						instantiatedInstanceID);
				return;
			}

			_ = _instantiatedGameObjects.Remove(instantiatedInstanceID);

			DestroyLocal(instantiatedObject, originalInstanceID);
		}
		/// <summary>
		/// Remove the object from the pool
		/// </summary>
		/// <param name="instantiatedObject">The object to return to the pool.</param>
		/// <param name="originalInstanceID">The id of the original object.</param>
		private void DestroyLocal(GameObject instantiatedObject, int originalInstanceID)
		{
			instantiatedObject.SetActive(false);
			instantiatedObject.transform.SetParent(_poolParent.PoolParent);

			Stack<GameObject> pool;
			if (_gameObjectPool.TryGetValue(originalInstanceID, out pool))
			{
				pool.Push(instantiatedObject);
			}
			else
			{
				pool = new Stack<GameObject>();
				pool.Push(instantiatedObject);
				_gameObjectPool.Add(originalInstanceID, pool);
			}
		}

		#endregion
	}
}