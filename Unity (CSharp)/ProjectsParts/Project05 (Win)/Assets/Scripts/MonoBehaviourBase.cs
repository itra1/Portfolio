using System.Collections;
using System.Collections.Generic;

using UnityEngine;
public class MonoBehaviourBase : MonoBehaviour
{
  /// <summary>
  /// Получение компонента или добавление при необъодимости
  /// </summary>
  /// <typeparam name="T">Компонент</typeparam>
  /// <returns>Компонент</returns>
  public T AddComponentIfNull<T>() where T : Component
  {
	 return AddComponentIfNull<T>(transform);
  }

  /// <summary>
  /// Получение компонента или добавление при необходимости
  /// </summary>
  /// <param name="tr">Трансформ объекта</param>
  /// <typeparam name="T">Компонент</typeparam>
  /// <returns>Компонент</returns>
  public T AddComponentIfNull<T>(Transform tr) where T : Component
  {
	 T comp = tr.GetComponent<T>();
	 if (comp == null)
		comp = tr.gameObject.AddComponent<T>();
	 return comp;
  }

  /// <summary>
  /// Удаление компонентов
  /// </summary>
  /// <param name="tr">Трансформ объекта</param>
  /// <typeparam name="T">Компонент</typeparam>
  public void RemoveIfNotNull<T>(Transform tr) where T : Component
  {
	 T comp = tr.GetComponent<T>();
	 if (comp != null)
		Destroy(comp);
  }

  /// <summary>
  /// Удаление компонентов
  /// </summary>
  /// <param name="tr">Трансформ объекта</param>
  /// <typeparam name="T">Компонент</typeparam>
  public void RemoveImmediateIfNotNull<T>(Transform tr) where T : Component
  {
	 T comp = tr.GetComponent<T>();
	 if (comp != null)
		DestroyImmediate(comp);
  }
  /// <summary>
  /// Присутствуют дочерние
  /// </summary>
  /// <returns></returns>
  public bool ExistsChildren()
  {
	 return GetComponentsInChildren<Transform>().Length > 0;
  }
  /// <summary>
  /// Присутствуют дочерние
  /// </summary>
  /// <param name="parent">Родитель</param>
  /// <returns></returns>
  public bool ExistsChildren(Transform parent)
  {
	 return GetChildrens<Transform>(parent).Count > 0;
  }
  /// <summary>
  /// Получаем все дочерние компоненты текущего объекта
  /// </summary>
  /// <param name="subChildrens">с поддетьми</param>
  /// <typeparam name="T">Компонент</typeparam>
  /// <returns></returns>
  public List<T> GetChildrens<T>(bool subChildrens = false) where T : Component
  {
	 return GetChildrens<T>(transform, subChildrens);
  }

  /// <summary>
  /// Получаем все дочерние компоненты
  /// </summary>
  /// <param name="parent">роитель</param>
  /// <param name="subChildrens">с поддетьми</param>
  /// <typeparam name="T"></typeparam>
  /// <returns></returns>
  public List<T> GetChildrens<T>(Transform parent, bool subChildrens = false) where T : Component
  {
	 List<T> objects = new List<T>();

	 T[] childrens = GetComponentsInChildren<T>();

	 foreach (var elem in childrens)
	 {
		if (!subChildrens)
		{
		  if (elem.transform.parent.Equals(parent))
		  {
			 objects.Add(elem);
		  }
		}
		else
		{
		  objects.Add(elem);
		}
	 }

	 return objects;

  }

  public GameObject CreateGameObject(Transform parent)
  {
	 var go = new GameObject();
	 go.transform.SetParent(parent);
	 go.transform.localEulerAngles = Vector3.zero;
	 go.transform.position = Vector3.zero;
	 go.transform.localScale = Vector3.one;
	 return go;

  }
  public GameObject InstantiateDefault(GameObject prefab, Transform parent)
  {
	 GameObject inst = Instantiate(prefab, parent);
	 inst.transform.localScale = Vector3.one;
	 inst.transform.localPosition = Vector3.zero;
	 inst.transform.localRotation = Quaternion.identity;
	 return inst;
  }

  /// <summary>
  /// Переопределение инвока
  /// </summary>
  /// <param name="action"></param>
  /// <param name="timeWaint"></param>
  protected void InvokeSeconds(System.Action action, float timeWaint)
  {
	 StartCoroutine(InvokeSecondsCoroutine(action, timeWaint));
  }

  IEnumerator InvokeSecondsCoroutine(System.Action action, float timeWaint)
  {
	 yield return new WaitForSeconds(timeWaint);
	 action();
  }

  protected void InvokeEndFrame(System.Action action)
  {
	 StartCoroutine(InvokeEndFrameCor(action));
  }

  IEnumerator InvokeEndFrameCor(System.Action action)
  {
	 yield return new WaitForEndOfFrame();
	 action();
  }

  protected void InvokeFixedUpdate(System.Action action)
  {
	 StartCoroutine(InvokeFixedUpdateCor(action));
  }

  public string GetNewUUID()
  {
	 return System.Guid.NewGuid().ToString();
  }

  IEnumerator InvokeFixedUpdateCor(System.Action action)
  {
	 yield return new WaitForFixedUpdate();
	 action();
  }


  public void ActiveAllPlayMakerFsm(bool isActive = true)
  {
	 PlayMakerFSM[] fsms = gameObject.GetComponents<PlayMakerFSM>();
	 for (int i = 0; i < fsms.Length; i++)
	 {
		fsms[i].enabled = isActive;

	 }
  }
  public void ActivePlayMakerFsm(string name, bool isActive = true)
  {
	 PlayMakerFSM[] fsms = gameObject.GetComponents<PlayMakerFSM>();
	 for (int i = 0; i < fsms.Length; i++)
	 {
		if (fsms[i].Fsm.Name.Equals(name))
		  fsms[i].enabled = isActive;
	 }
  }

  public PlayMakerFSM GetFsm(string name)
  {

	 PlayMakerFSM[] fsms = gameObject.GetComponents<PlayMakerFSM>();
	 for (int i = 0; i < fsms.Length; i++)
	 {
		if (fsms[i].Fsm.Name.Equals(name))
		  return fsms[i];
	 }
	 return null;
  }

}
