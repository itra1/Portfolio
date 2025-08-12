using Game.Common.Factorys.Base;
using Game.Game.Settings;
using UnityEngine;
using Zenject;

namespace Game.Game.Elements.Boards
{
	public class BoardFactory : MultiInstanceFactory<string, Board>
	{
		private BoardsSettings _boardSettings;

		public BoardFactory(
			DiContainer container,
			GameSettings gameSettings,
			BoardsSettings boardSettings
		) : base(container)
		{
			_boardSettings = boardSettings;
			var allScreens = Resources.LoadAll<Board>(gameSettings.BoardFolder);
			foreach (var wItem in allScreens)
			{
				var type = wItem.Type;

				if (!_prefabs.ContainsKey(type))
					_prefabs.Add(type, wItem);
			}
		}

		public override Board GetInstance(string key, Transform parent)
		{
			var inst = base.GetInstance(key, parent);
			inst.SetData(_boardSettings.Boards.Find(x => x.Name == key));
			return inst;
		}
	}
}
