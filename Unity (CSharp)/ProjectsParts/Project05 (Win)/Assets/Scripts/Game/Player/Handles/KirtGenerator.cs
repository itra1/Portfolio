using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Генерация физики для юбки
/// </summary>
public class KirtGenerator : MonoBehaviourBase
{
	[SerializeField]
	private int m_Level = 2;
	[SerializeField]
	private List<List<SpringJoint>> m_SpringJointList = new List<List<SpringJoint>>();

	[ContextMenu("Generate")]
	public void Generate()
	{
		m_SpringJointList.Clear();
		Spawn();
		ConnectSpringJoint();
	}

	[ContextMenu("Clear")]
	public void Clear()
	{
		ClearComponents();
		m_SpringJointList.Clear();
	}

	private void Spawn()
	{
		int level = -1;
		int colum = -1;
		ReadComponents(transform, ++level, colum);
	}

	private void ConnectSpringJoint()
	{
		for (int i = 0; i < m_SpringJointList[0].Count; i++)
		{

			for (int j = 0; j < m_SpringJointList.Count; j++)
			{

				int contact = j + 1;
				if (contact >= m_SpringJointList.Count - 1)
					contact = 0;
				try
				{
					m_SpringJointList[j][i].connectedBody = m_SpringJointList[contact][i].GetComponent<Rigidbody>();
				}
				catch
				{
					Debug.Log(contact + " : " + m_SpringJointList[j][i].name);
				}
			}

		}
	}

	private void ReadComponents(Transform parent, int level = 0, int colum = 0)
	{
		List<Transform> childrens = GetChildrens<Transform>(parent);

		foreach (var elem in childrens)
		{
			if (level == 0)
				colum++;

			if (level == m_Level - 1)
			{
				AddRigibody(elem, false);
			}
			if (level >= m_Level)
			{
				AddNeedComponents(elem, colum);
			}
			ReadComponents(elem, level + 1, colum);
		}
	}

	private void AddNeedComponents(Transform tr, int index)
	{
		AddRigibody(tr);
		AddCapsulaCollider(tr);
		AddCharacterJoint(tr);
		AddSpringJoin(tr, index);
	}

	private void AddRigibody(Transform tr, bool isGravity = true)
	{
		Rigidbody rb = AddComponentIfNull<Rigidbody>(tr);
		rb.useGravity = isGravity;
		rb.isKinematic = !isGravity;
		rb.drag = 0.2f;
		rb.mass = 10;
		rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
	}

	private void AddCapsulaCollider(Transform tr)
	{
		CapsuleCollider cc = AddComponentIfNull<CapsuleCollider>(tr);
		cc.direction = 0;
		cc.radius = 0.2f;
		cc.center = new Vector3(-0.35f, 0, 0);
	}

	private void AddCharacterJoint(Transform tr, bool excludeParent = false)
	{
		CharacterJoint cj = AddComponentIfNull<CharacterJoint>(tr);
		cj.connectedBody = tr.parent.GetComponent<Rigidbody>();
		cj.anchor = new Vector3(0.5f, 0, 0);
		cj.enableProjection = true;
		cj.twistLimitSpring = new SoftJointLimitSpring()
		{
			spring = 5,
			damper = 0.3f
		};
		cj.lowTwistLimit = new SoftJointLimit()
		{
			limit = -3,
			bounciness = 0,
			contactDistance = 0
		};

		cj.highTwistLimit = new SoftJointLimit()
		{
			limit = 3,
			bounciness = 0,
			contactDistance = 0
		};
		cj.swingLimitSpring = new SoftJointLimitSpring()
		{
			spring = 10,
			damper = .5f
		};
		cj.swing1Limit = new SoftJointLimit()
		{
			limit = 15,
			bounciness = 0,
			contactDistance = 0
		};
		cj.swing2Limit = new SoftJointLimit()
		{
			limit = 15,
			bounciness = 0,
			contactDistance = 0
		};

	}

	private void AddSpringJoin(Transform tr, int index)
	{
		SpringJoint sj = AddComponentIfNull<SpringJoint>(tr);
		sj.spring = 1500;
		sj.anchor = new Vector3(-0.63f, 0f, 0f);
		sj.damper = .3f;

		if (m_SpringJointList.Count <= index)
		{
			m_SpringJointList.Add(new List<SpringJoint>());
		}
		m_SpringJointList[index].Add(sj);
	}

	private void ClearComponents()
	{
		int level = -1;
		ClearRecursive(transform, ++level);
	}

	private void ClearRecursive(Transform parent, int level = 0)
	{
		List<Transform> childrens = GetChildrens<Transform>(parent);

		foreach (var elem in childrens)
		{
			RemoveImmediateIfNotNull<SpringJoint>(elem);
			RemoveImmediateIfNotNull<CharacterJoint>(elem);
			RemoveImmediateIfNotNull<CapsuleCollider>(elem);
			RemoveImmediateIfNotNull<Rigidbody>(elem);

			ClearRecursive(elem, level + 1);
		}
	}

}
