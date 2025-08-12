using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Leguar.TotalJSON;

namespace it.Game.Managers
{
  public class GameKeys
  {
	 public const string EVT_LOAD = "LOAD_KEYS";

	 private List<KeyData> _keys = new List<KeyData>();
	 public List<KeyData> Keys { get => _keys; set => _keys = value; }


	 public GameKeys()
	 {
	 }

	 public void Initiate()
	 {
		LoadDefault();
		Load();
	 }

	 private void Load()
	 {

		string saveList = PlayerPrefs.GetString("Keys", "");

		if (string.IsNullOrEmpty(saveList))
		  return;

		JSON saveData = JSON.ParseString(saveList, "Keys");

		foreach (var key in saveData.Keys)
		{
		  for (int i = 0; i < Keys.Count; i++)
		  {
			 if (key == Keys[i].KeyAlias)
				Keys[i].Value = saveData.GetInt(key);
		  }
		}
		SendLoadMessage();

	 }

	 public void Save()
	 {

		JSON saveData = new JSON();

		for (int i = 0; i < Keys.Count; i++)
		{
		  saveData.Add(Keys[i].KeyAlias, Keys[i].Value);
		}

		string saveString = saveData.CreateString();
		PlayerPrefs.SetString("Keys", saveString);
		PlayerPrefs.Save();

	 }

	 private void LoadDefault()
	 {
		CreateKeys();
	 }

	 private void CreateKeys()
	 {
		Keys.Add(new KeyData { Title = "Key.MoveUp", KeyAlias = "_MoveUpKey", Value = com.ootii.Input.EnumInput.W });
		Keys.Add(new KeyData { Title = "Key.MoveDown", KeyAlias = "_MoveDownKey", Value = com.ootii.Input.EnumInput.S });
		Keys.Add(new KeyData { Title = "Key.MoveLeft", KeyAlias = "_MoveLeftKey", Value = com.ootii.Input.EnumInput.A });
		Keys.Add(new KeyData { Title = "Key.MoveRight", KeyAlias = "_MoveRightKey", Value = com.ootii.Input.EnumInput.D });
		Keys.Add(new KeyData { Title = "Key.MoveUp", KeyAlias = "Move Up", Value = com.ootii.Input.EnumInput.X });
		Keys.Add(new KeyData { Title = "Key.MoveDown", KeyAlias = "Move Down", Value = com.ootii.Input.EnumInput.Z });
		Keys.Add(new KeyData { Title = "Key.Jump", KeyAlias = "Jump", Value = com.ootii.Input.EnumInput.SPACE });
		Keys.Add(new KeyData { Title = "Key.Interact", KeyAlias = "Interact", Value = com.ootii.Input.EnumInput.F });
		Keys.Add(new KeyData { Title = "Key.Inventary", KeyAlias = "Inventary", Value = com.ootii.Input.EnumInput.I });
		Keys.Add(new KeyData { Title = "Key.Symbols", KeyAlias = "Symbols", Value = com.ootii.Input.EnumInput.O });
		Keys.Add(new KeyData { Title = "Key.Walk", KeyAlias = "Run", Value = com.ootii.Input.EnumInput.LEFT_SHIFT });
		Keys.Add(new KeyData { Title = "Key.Dress1", KeyAlias = "PlayerDress1", Value = com.ootii.Input.EnumInput.ALPHA_1 });
		Keys.Add(new KeyData { Title = "Key.Dress2", KeyAlias = "PlayerDress2", Value = com.ootii.Input.EnumInput.ALPHA_2 });
		Keys.Add(new KeyData { Title = "Key.Dress3", KeyAlias = "PlayerDress3", Value = com.ootii.Input.EnumInput.ALPHA_3 });
		Keys.Add(new KeyData { Title = "Key.Special", KeyAlias = "Special", Value = com.ootii.Input.EnumInput.T });
		Keys.Add(new KeyData { Title = "Key.Attack", KeyAlias = "Attack", Value = com.ootii.Input.EnumInput.MOUSE_RIGHT_BUTTON });
		Keys.Add(new KeyData { Title = "Key.Aiming", KeyAlias = "Aiming", Value = com.ootii.Input.EnumInput.MOUSE_LEFT_BUTTON });
		Keys.Add(new KeyData { Title = "Key.Fly", KeyAlias = "Fly", Value = com.ootii.Input.EnumInput.LEFT_CONTROL });

		// xBox game pad
		//Keys.Add(new KeyData { Title = "Key.Jump", KeyAlias = "Jump", Value = com.ootii.Input.EnumInput.GAMEPAD_0_BUTTON });

	 }

	 [System.Serializable]
	 public class KeyData
	 {
		public string Title;
		public string KeyAlias;
		public int Value;
		public string ValueName
		{
		  get
		  {
			 return com.ootii.Input.EnumInput.EnumNames[Value].Replace("_", " ");
  		}
		}
	 }
	 private void SendLoadMessage()
	 {
		Game.Events.Messages.LoadData loadData = Events.Messages.LoadData.Allocate();
		loadData.Type = EVT_LOAD;
		loadData.Sender = this;
		loadData.Delay = 0.1f;
		Game.Events.EventDispatcher.SendMessage(loadData);
	 }

  }
}