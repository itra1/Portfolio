using UnityEngine;

namespace Generals.Network.SocketWeb {
  /// <summary>
  /// Веб сокеты
  /// </summary>
  public class NetWebSocket : MonoBehaviour, WebSocketUnityDelegate {

    private NetworkManager networkManager;                              // Основной менеджер сети
    private WebSocketUnity webSocket;                                   // Дескриптор соединения WebSocket
    private ServerParametrs serverParametrs;                            // Параметры сервера

    public void Init(NetworkManager networkManager, ServerParametrs serverParametrs) {
      this.networkManager = networkManager;
      this.serverParametrs = serverParametrs;
    }

    /// <summary>
    /// Установка соединения
    /// </summary>
    public void Connect() {
      Debug.Log(string.Format("Connecting to {0}", serverParametrs.server));
      Debug.Log(serverParametrs.url);
      webSocket = new WebSocketUnity(serverParametrs.url, this);
      NetworkManager.networkState = NetState.ServerConnecting;
      webSocket.Open();
    }

    /// <summary>
    /// Разрыв соединения
    /// </summary>
    public void Disconnect() {
      webSocket.Close();
    }

    /// <summary>
    /// Отправка пакета
    /// </summary>
    /// <param name="Packet"></param>
    public void OnSendData(INetOutPacket Packet) { }

    /// <summary>
    /// Событие приема текстового сообщения
    /// </summary>
    /// <param name="message"></param>
    public void OnWebSocketUnityReceiveMessage(string message) { }

    /// <summary>
    /// Событие приема байтов на мобильный устройствах
    /// </summary>
    /// <param name="base64EncodedData"></param>
    public void OnWebSocketUnityReceiveDataOnMobile(string base64EncodedData) {
      OnWebSocketUnityReceiveData(webSocket.decodeBase64String(base64EncodedData));
    }

    #region Send package

    public bool SendPacket(INetOutPacket packet) {

      if(!networkManager.isConnect) {
        Debug.LogError("Попытка отправить сообщение при отключенном соединении!");
        return false;
      }

      PacketSizeCalculator calcStream = new PacketSizeCalculator();
      packet.Write(calcStream);
      ByteArrayOutputStream outStream = new ByteArrayOutputStream(calcStream.PacketSize);
      packet.Write(outStream);
      networkManager.byteOut += calcStream.PacketSize;
      //networkManager.byteAll += calcStream.PacketSize;

      webSocket.Send(outStream.GetData());
      return true;
    }

    #endregion

    #region Events

    /// <summary>
    /// Установки соединения
    /// </summary>
    /// <param name="sender"></param>
    public void OnWebSocketUnityOpen(string sender) {
      networkManager.OnSocketOpen(sender);
    }

    /// <summary>
    /// Событие закрытия соединения
    /// </summary>
    /// <param name="reason"></param>
    public void OnWebSocketUnityClose(string reason) {
      networkManager.OnSocketClose(reason);
    }

    /// <summary>
    /// Событие приема данных
    /// </summary>
    /// <param name="data"></param>
    public void OnWebSocketUnityReceiveData(byte[] data) {
			
			networkManager.byteIn += data.Length;
			//networkManager.byteAll += data.Length;

			networkManager.OnSocketReceiveData(data);
    }

    /// <summary>
    /// Событие ошибки сокетов
    /// </summary>
    /// <param name="error"></param>
    public void OnWebSocketUnityError(string error) {
      networkManager.OnSocketError(error);
    }

    #endregion

  }
}