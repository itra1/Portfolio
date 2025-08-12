using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Network.Input {
	
	/// <summary>
	/// Персонажи на поле
	/// </summary>
	[System.Serializable]
	public class PlayerInfo : InputPackage {
		
		public string playerName;
		
		public float hp;                // Текущее значение жизней
		public float hp_max;             // Максимальное значение жизней
		public string owner_id;					// Родитель
		public int class_id;           // Идентификатор класа
		public int race;                // Расса
		public int model;               // Модель
		public string login;               // Логин
		public int level;               // Уровень
		public float rating;            // 
		public string item_types_list;
		public int army;                // Армия
		public int active;              // Активный
		public int fishka_type;          // 
		public int complete;            // 
		public int pid;                 // 
		public int? x;                   // 
		public int? y;                   // 

		public int posX {
			get { return x.Value; }
		}
		public int posY {
			get { return y.Value; }
		}
		public int? x1;                  // 
		public int? y1;                  // 
		public int killed;              // Убитых
		public int battle_id;            // Идентификатор боя
		public int bitflag;             // 
		public bool attacked;             // 
		public Technic technic;

		public CharacterDress.CharacterDressElement[] GetDress() {

			if (string.IsNullOrEmpty(item_types_list))
				return new CharacterDress.CharacterDressElement[0];

			return Newtonsoft.Json.JsonConvert.DeserializeObject<CharacterDress.CharacterDressElement[]>(item_types_list);
		}


	}
}

[System.Serializable]
public struct CharacterDress {

	public CharacterDressElement[] dress;
	
	[System.Serializable]
	public struct CharacterDressElement {
		public int type_id;
		public int type;
		public int status;
	}
}