using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.Events;

namespace Core.Engine.Components.Skins
{
	public interface ISkin
	{
		/// <summary>
		/// Идентификатор
		/// </summary>
		public string UUID { get; }
		/// <summary>
		/// Название
		/// </summary>
		public string Title { get; }
		/// <summary>
		/// Описание
		/// </summary>
		public string Description { get; }
		/// <summary>
		/// Группа скинов
		/// </summary>
		public string Type { get; }
		/// <summary>
		/// стандартный
		/// </summary>
		public bool IsDefault { get; }
		/// <summary>
		/// Доступно для выбора
		/// </summary>
		public bool ReadyToSelect { get; }

		/// <summary>
		/// Выбран
		/// </summary>
		public bool IsSelected { get; }

		public Sprite Icone { get; }

		public void SetProvider(ISkinProvider skinProvider);

		bool Confirm();

		void SubscribeChange(UnityAction action);
		void UnSubscribeChange(UnityAction action);
	}
}
