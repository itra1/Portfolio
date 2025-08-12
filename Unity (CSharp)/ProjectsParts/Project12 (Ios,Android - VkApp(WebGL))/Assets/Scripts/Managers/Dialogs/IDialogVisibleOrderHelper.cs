using System;
using Cysharp.Threading.Tasks;

namespace Game.Scripts.Managers.Dialogs
{
	public interface IDialogVisibleOrderHelper
	{
		bool Allowed { get; set; }

		void AddJob(Func<UniTask> dialog);
	}
}