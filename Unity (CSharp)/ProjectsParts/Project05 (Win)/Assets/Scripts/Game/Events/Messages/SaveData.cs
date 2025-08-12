using System.Collections.Generic;
using com.ootii.Collections;
using com.ootii.Messages;
using Leguar.TotalJSON;

namespace it.Game.Events.Messages
{
  /// <summary>
  /// Событие изменения параметров объекта для сохранения
  /// </summary>
  public class SaveData : Message
  {
	 private EntityType _entity = EntityType.none;
	 public EntityType Entity { get => _entity; set => _entity = value; }
	 
	 private string _spawnPosition = "";
	 public string SpawnPosition { get => _spawnPosition; set => _spawnPosition = value; }
	 private float _spawnRotetionY = 0;
	 public float SpawnRotetionY { get => _spawnRotetionY; set => _spawnRotetionY = value; }

	 private JSON _inventary = new JSON();
	 public JSON Inventary { get => _inventary; set => _inventary = value; }

	 private Dictionary<string, JValue> _environment = new Dictionary<string, JValue>();
	 public Dictionary<string, JValue> Environment { get => _environment; set => _environment = value; }
	 private JArray _symbols = new JArray();
	 public JArray Symbols { get => _symbols; set => _symbols = value; }

	 private JArray _tutorial = new JArray();
	 public JArray Tutorial { get => _tutorial; set => _tutorial = value; }

	 private float _energy;
	 public float Energy { get => _energy; set => _energy = value; }

	 public enum EntityType
	 {
		none,
		environment,
		levelPosition,
		inventary,
		symbol,
		energy,
		tutorial
	 }

	 public override void Clear()
	 {
		base.Clear();

		Entity = EntityType.none;
		SpawnPosition = null;
		Inventary.Clear();
		Environment.Clear();
		Symbols = null;
		Energy = 0;
	 }

	 public override void Release()
	 {
		Clear();

		IsSent = true;
		IsHandled = true;

		if (this is SaveData)
		{
		  sPool.Release(this);
		}
	 }


	 // ******************************** OBJECT POOL ********************************

	 private static ObjectPool<SaveData> sPool = new ObjectPool<SaveData>(40, 10);


	 public new static SaveData Allocate()
	 {
		SaveData lInstance = sPool.Allocate();

		lInstance.IsSent = false;
		lInstance.IsHandled = false;

		if (lInstance == null) { lInstance = new SaveData(); }
		return lInstance;
	 }

	 public static void Release(SaveData rInstance)
	 {
		if (rInstance == null) { return; }
		rInstance.Clear();

		rInstance.IsSent = true;
		rInstance.IsHandled = true;

		sPool.Release(rInstance);
	 }

	 public new static void Release(IMessage rInstance)
	 {
		if (rInstance == null) { return; }

		rInstance.Clear();

		rInstance.IsSent = true;
		rInstance.IsHandled = true;

		if (rInstance is SaveData)
		  sPool.Release((SaveData)rInstance);
	 }


  }
}