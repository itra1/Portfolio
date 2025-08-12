using System.Collections.Generic;
using UnityEngine.Events;

namespace Editor.Settings.Components
{
	public abstract class DefineDrawBase
	{
		public string Title { get; set; }

		public abstract bool IsChange { get; }
		
		public abstract void ConfirmChange();
		
		public abstract void Init(List<string> defines);
		
		public abstract void Draw();
		
		public abstract void Save(List<string> defines, UnityAction<List<string>, string, bool> confirm);
	}
}