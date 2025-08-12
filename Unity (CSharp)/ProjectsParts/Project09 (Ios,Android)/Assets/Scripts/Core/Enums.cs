/// <summary>
/// Объекты прилетаемые с сервера
/// </summary>
public enum ObjectType {
  TransparentBlock = 0,
  Character = 1,
  Bullet = 2,
  Barrier = 3,
  Mine = 4
}

public enum BarrierType {
  None,
  Basic,
  Card
}

/// <summary>
/// Тип таймера
/// </summary>
public enum TimerType {
  StartTimer = 0, // 3s ?
  GeneralTimer = 1, // General battle timer, right after StartTimer, 120s?
  BonusTimer = 2
}


/// <summary>
/// Фазы боевой сценыx
/// </summary>
public enum BattlePhase {
  load,
  ready,
  battle,
  finished
}

/// <summary>
/// Статус авторизации
/// </summary>
public enum AuthStatus {
  RESULT_OK = 0,
  RESULT_ACCESS_TOKEN_EMPTY = 1,
  RESULT_ACCESS_TOKEN_INVALID = 2,
  RESULT_NO_CHARACTERS = 3,
  RESULT_UNKNOWN_ERROR = 0xff,
}

/// <summary>
/// Тип ервера
/// </summary>
public enum ServerType {
  developer,
  needle,
  test
}

/// <summary>
/// Позиционирование плеера на карте
/// </summary>
public enum PlayerScenePosition { LEFT, RIGHT }

/// <summary>
/// Тип подключания сокетов
/// </summary>
public enum SocketType {
	tcpSocket,
	webSocket
}

/// <summary>
/// Состояние сети
/// </summary>
public enum NetState {
  None,
  ServerConnecting,
  ServerConnected,
  ServerAuthed,
  Disconnected
}

/// <summary>
/// Тип эффекта
/// </summary>
public enum EffectType {
  NONE = 0,
  FIRING_SPEED = 1,
  POISON = 2,
  HEAL = 3,
  FREEZE = 4
}
