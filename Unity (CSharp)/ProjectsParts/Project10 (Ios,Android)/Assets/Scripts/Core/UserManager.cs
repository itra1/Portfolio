using UnityEngine;
using System;
using System.Collections;
using ExEvent;

/// <summary>
/// Менеджер игрока
/// </summary>
public class UserManager : Singleton<UserManager> {
	
	public static event Action OnLoadData;
	
	protected override void Awake() {
		base.Awake();
		LoadData();

    CreateLiveCompany();
  }

  private void CreateLiveCompany() {

    GameObject gameobject = new GameObject();
    Company.Live.LiveCompany liveCompany = gameobject.AddComponent<Company.Live.Heart>();
    gameobject.transform.SetParent(transform);
  }
  
	void OnApplicationQuit() {
		PlayerPrefs.SetString("timeLastExit", DateTime.Now.ToString());
		SaveData();
	}

	private void LoadData() {
		if (OnLoadData != null) OnLoadData();
	}

	public void SaveData() {
		PlayerPrefs.SetFloat("maxDistance", _survivleMaxRunDistance.Value);
	}

	public void ConfirmSave() {
		PlayerPrefs.Save();
	}
  
  /// <summary>
  /// Монеты
  /// </summary>
	private static int? _coins;
	public static int coins {
		get {
      if(_coins == null)
        _coins = PlayerPrefs.GetInt("coins", 0);
      return _coins.Value; }
		set {
			_coins = value;
			PlayerPrefs.SetInt("coins", _coins.Value);
			RunEvents.CoinsChange.CallAsync(_coins.Value);
		}
	}

  /// <summary>
  /// Кристалы
  /// </summary>
	private int? _cristall;
	public int cristall {
		get {
      if (_cristall == null)
        _cristall = PlayerPrefs.GetInt("cristall", 0);
      return _cristall.Value; }
		set {
			_cristall = value;
			PlayerPrefs.SetInt("cristall", _cristall.Value);
			RunEvents.CristallChange.CallAsync(_cristall.Value);
		}
	}

  /// <summary>
  /// Черная метка
  /// </summary>
	private int? _blackMark;
	public int blackMark {
		get {
      if (_blackMark == null)
        _blackMark = PlayerPrefs.GetInt("blackMark", 0);
      return _blackMark.Value; }
		set {
			_blackMark = value;
			PlayerPrefs.SetInt("blackMark", _blackMark.Value);
			RunEvents.BlackMarkChange.CallAsync(_blackMark.Value);
		}
	}

  /// <summary>
  /// Ключи
  /// </summary>
	private int? _keys;
	public int keys {
		get {
      if (_keys == null)
        _keys = PlayerPrefs.GetInt("keys", 0);
      return _keys.Value; }
		set {
			_keys = value;
			PlayerPrefs.SetInt("keys", _keys.Value);
			RunEvents.KeysChange.CallAsync(_keys.Value);
		}
	}
  
	/// <summary>
	/// Максимальная дистанция в выживании
	/// </summary>
	private float? _survivleMaxRunDistance;
	public float survivleMaxRunDistance {
		get {
      if(_survivleMaxRunDistance == null)
        _survivleMaxRunDistance = PlayerPrefs.GetFloat("maxDistance", 0);
      return _survivleMaxRunDistance.Value; }
		set {
			_survivleMaxRunDistance = value;
		}
	}

  /// <summary>
  /// Регистрация пушей
  /// </summary>
	private static bool? _pushRegister;
	public static bool pushRegister {
		get {
      if(_pushRegister == null)
      _pushRegister = Boolean.Parse(PlayerPrefs.GetString("pushRegister", "false"));
      return _pushRegister.Value;
		}
		set {
			_pushRegister = value;
			PlayerPrefs.SetString("pushRegister",_pushRegister.Value.ToString());
		}
	}

  /// <summary>
  /// Перк жизней
  /// </summary>
	private int? _livePerk;
	public int livePerk {
		get {
      if(_livePerk == null)
        _livePerk = PlayerPrefs.GetInt("livePerk", 0);
      return _livePerk.Value; }
		set {
			if(_livePerk == value) return;
			_livePerk = value;
			PlayerPrefs.SetInt("livePerk", _livePerk.Value);
		}
	}

  /// <summary>
  /// Бонус оружия
  /// </summary>
  private int? _liveAddPerk;
  public int liveAddPerk {
    get {
      if (_liveAddPerk == null)
        _liveAddPerk = PlayerPrefs.GetInt("liveAddPerk", 0);
      return _liveAddPerk.Value;
    }
    set {
      if (_liveAddPerk == value) return;
      _liveAddPerk = value;
      PlayerPrefs.SetInt("liveAddPerk", _liveAddPerk.Value);
    }
  }

  /// <summary>
  /// Перк спасения из ямы
  /// </summary>
  private int? _savesPerk;
  public int savesPerk {
    get {
      if (_savesPerk == null)
        _savesPerk = PlayerPrefs.GetInt("savesPerk", 0);
      return _savesPerk.Value;
    }
    set {
      if (_savesPerk == value) return;
      _savesPerk = value;
      PlayerPrefs.SetInt("savesPerk", _savesPerk.Value);
    }
  }

  /// <summary>
  /// Буст бега
  /// </summary>
  private int? _runBoost;
  public int runBoost {
    get {
      if (_runBoost == null)
        _runBoost = PlayerPrefs.GetInt("runBoost", 0);
      return _runBoost.Value;
    }
    set {
      _runBoost = value;
      PlayerPrefs.SetInt("runBoost", _runBoost.Value);
    }
  }

  /// <summary>
  /// Буст скейта
  /// </summary>
  private int? _skateBoost;
  public int skateBoost {
    get {
      if (_skateBoost == null)
        _skateBoost = PlayerPrefs.GetInt("skateBoost", 0);
      return _skateBoost.Value;
    }
    set {
      _skateBoost = value;
      PlayerPrefs.SetInt("skateBoost", _skateBoost.Value);
    }
  }

  /// <summary>
  /// Буст бочки
  /// </summary>
  private int? _barrelBoost;
  public int barrelBoost {
    get {
      if (_barrelBoost == null)
        _barrelBoost = PlayerPrefs.GetInt("barrelBoost", 0);
      return _barrelBoost.Value;
    }
    set {
      _barrelBoost = value;
      PlayerPrefs.SetInt("barrelBoost", _barrelBoost.Value);
    }
  }

  /// <summary>
  /// Буст мельнечного колеса
  /// </summary>
  private int? _millWhellBoost;
  public int millWhellBoost {
    get {
      if (_millWhellBoost == null)
        _millWhellBoost = PlayerPrefs.GetInt("millWhellBoost", 0);
      return _millWhellBoost.Value;
    }
    set {
      _millWhellBoost = value;
      PlayerPrefs.SetInt("millWhellBoost", _millWhellBoost.Value);
    }
  }

  /// <summary>
  /// Буст корабля
  /// </summary>
  private int? _shipBoost;
  public int shipBoost {
    get {
      if (_shipBoost == null)
        _shipBoost = PlayerPrefs.GetInt("shipBoost", 0);
      return _shipBoost.Value;
    }
    set {
      _shipBoost = value;
      PlayerPrefs.SetInt("shipBoost", _shipBoost.Value);
    }
  }

  /// <summary>
  /// Двойное оружие
  /// </summary>
  private int? _doubleWeapon;
  public int doubleWeapon {
    get {
      if (_doubleWeapon == null)
        _doubleWeapon = PlayerPrefs.GetInt("doubleWeapon", 0);
      return _doubleWeapon.Value;
    }
    set {
      _doubleWeapon = value;
      PlayerPrefs.SetInt("doubleWeapon", _doubleWeapon.Value);
    }
  }

  /// <summary>
  /// Воскрешение
  /// </summary>
  private int? _ressurection;
  public int ressurection {
    get {
      if (_ressurection == null)
        _ressurection = PlayerPrefs.GetInt("ressurection", 0);
      return _ressurection.Value;
    }
    set {
      _ressurection = value;
      PlayerPrefs.SetInt("ressurection", _ressurection.Value);
    }
  }

}