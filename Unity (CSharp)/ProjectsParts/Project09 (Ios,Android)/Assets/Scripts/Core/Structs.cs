[System.Serializable]
public struct FloatSpan {
  public float min;
  public float max;
}

[System.Serializable]
public struct IntSpan {
  public int min;
  public int max;
}

/// <summary>
/// Структура воемени прилетаемая с сервера
/// </summary>
public struct TimeInfoData {
  public TimerType type;
  public float secondTotal;
  public float secondLeft;
}

/// <summary>
/// Параметры сервера
/// </summary>
[System.Serializable]
public struct ServerParametrs {
  public SocketType connect;
  public ServerType server;
  public string url;
  public int port;
}