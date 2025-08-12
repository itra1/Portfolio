using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Engine.Components.GameQuests
{
	public interface IGameQuestProvider
	{
		public GameQuest ActiveQuest { get; }

		public void SetActiveQuest(int level);
		public void SetActiveQuest(GameQuest quest);

		public void Clear();

	}
}
