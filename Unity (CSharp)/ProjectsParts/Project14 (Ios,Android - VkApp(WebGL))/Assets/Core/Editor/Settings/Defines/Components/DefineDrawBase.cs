using System.Collections.Generic;

namespace Core.Editor.Settings
{
	public abstract class DefineDrawBase
	{
		public string Title;
		public abstract bool IsChange { get; }

		public abstract void ConfirmChange();

		public abstract void Init(List<string> defines);

		public abstract void Draw();
		public abstract void Save(List<string> defines, UnityEngine.Events.UnityAction<List<string>,string,bool> confirm);

	}
}