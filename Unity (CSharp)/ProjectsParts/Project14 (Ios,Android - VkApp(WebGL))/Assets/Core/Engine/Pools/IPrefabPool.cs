using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

namespace Core.Engine.Utils
{
	public interface IPrefabPool<TComponent>
	: IPool
	where TComponent : Component
	{
		int CountActive { get; }
		IPrefabPool<TComponent> InitInstances(int count);
		IPrefabPool<TComponent> HideAll();
		TComponent GetItem();

	}
	
}
