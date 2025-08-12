using UnityEngine;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using NetCollections;

namespace Generals.Network.SocketTcp {

  /// <summary>
  /// общий контроллер сокетов
  /// </summary>
  public class NetTcpSocket : MonoBehaviour {

    private Socket mSocket;                                             // Дескриптор соединения Socket
    private NetworkManager networkManager;
    private MemoryStream memoryStream;
    private const int MAX_RCV_SIZE = 65535;
    private byte[] MessageBuffer = new byte[MAX_RCV_SIZE];
    private byte[] RecvBuffer = new byte[MAX_RCV_SIZE];
    private CircularBuffer InputBuffer = new CircularBuffer(1024 * 512);
    private ServerParametrs serverParametrs;                            // Параметры сервера

    public void Init(NetworkManager networkManager, ServerParametrs serverParametrs) {
      this.networkManager = networkManager;
      this.serverParametrs = serverParametrs;
      memoryStream = new MemoryStream(MessageBuffer, true);
    }

    /// <summary>
    /// Установка соединения
    /// </summary>
    public void Connect() {
      NetworkManager.networkState = NetState.ServerConnecting;
      Debug.Log("Starting connection to Login Server " + serverParametrs.url + "[" + serverParametrs.port + "]");
      ConnectImpl(serverParametrs.url, serverParametrs.port);
    }

    private void ConnectImpl(string addressStr, int portnum) {

      InputBuffer.clear();
      try {

        IPAddress ipAddress;
        Match match = Regex.Match(addressStr, @"\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}");
        if(!match.Success) {
          IPHostEntry ipHostInfo = Dns.GetHostEntry(addressStr);
          ipAddress = ipHostInfo.AddressList[0];
        } else {
          IPAddress.TryParse(addressStr, out ipAddress);
        }

        if(ipAddress.AddressFamily == AddressFamily.InterNetworkV6)
          mSocket = new Socket(AddressFamily.InterNetworkV6, System.Net.Sockets.SocketType.Stream, ProtocolType.Tcp);
        else
          mSocket = new Socket(AddressFamily.InterNetwork, System.Net.Sockets.SocketType.Stream, ProtocolType.Tcp);

        IPEndPoint remoteEP = new IPEndPoint(ipAddress, portnum);

        mSocket.SendTimeout = NetworkManager.SOCKET_TIMEOUT;
        mSocket.ReceiveTimeout = NetworkManager.SOCKET_TIMEOUT;

        if(mSocket.SendTimeout < NetworkManager.SOCKET_TIMEOUT) {
          Debug.LogWarning("Socket timeout bug, expected: " + NetworkManager.SOCKET_TIMEOUT + " got: " + mSocket.SendTimeout + " setting to the overvalues");
          mSocket.SendTimeout = NetworkManager.SOCKET_TIMEOUT * 1000;
          mSocket.ReceiveTimeout = NetworkManager.SOCKET_TIMEOUT * 1000;
        }
        Debug.Log("Begin connection to " + remoteEP.ToString() + " send timeout: " + mSocket.SendTimeout + "ms recv timeout: " + mSocket.ReceiveTimeout + "ms");
        mSocket.BeginConnect(
                remoteEP,
                new AsyncCallback(OnConnect),
                mSocket
            );

      } catch(Exception ex) {
        //setDisconnectState(0xff, ex.Message);
        OnClose("Catch in ConnectImpl: " + ex.Message);
      }
    }

    void OnConnect(IAsyncResult asyncResult) {
      Socket clientSocket = (Socket)asyncResult.AsyncState;
      try {
        clientSocket.EndConnect(asyncResult);
        clientSocket.NoDelay = true;

        if(clientSocket.Connected == false) {
          networkManager.OnSocketError("Network Unavailable");
          //setDisconnectState(0xff, "Network Unavailable");
          Debug.Log(".client is not connected.");
          OnDisconnect("!connected in OnConnect ");
          return;
        }

        networkManager.OnSocketOpen("Client connected");
        Debug.Log("Connected, receiving");
        BeginReceiveData();
      } catch(Exception ex) {
        networkManager.OnSocketError(ex.Message);
        //setDisconnectState(0xff, ex.Message);
        Debug.LogError(ex.StackTrace.ToString());
        OnDisconnect("catch in OnConnect: " + ex.Message + "  netstate: " + NetworkManager.networkState.ToString());
      }
    }

    void BeginReceiveData() {
      if(mSocket != null && mSocket.Connected)
        mSocket.BeginReceive(RecvBuffer, 0, MAX_RCV_SIZE, SocketFlags.None, EndReceiveData, null);
    }

    void EndReceiveData(System.IAsyncResult iar) {
      try {
        int BytesRcv = mSocket.EndReceive(iar);

        if(BytesRcv == 0) {
          throw new Exception("Server closed connection");
        }

        lock(InputBuffer) {
          InputBuffer.put(RecvBuffer, 0, BytesRcv);
          bool gotAPacket;
          do {
            gotAPacket = ProcessInputBuffer();
          }
          while(gotAPacket);
        }

        BeginReceiveData();
      } catch {
        Disconnect();
      }
    }

    private bool ProcessInputBuffer() {
      if(InputBuffer.Count < 2)
        return false;

      int PacketSize = InputBuffer.peekUShort();
      if(InputBuffer.Count < PacketSize)
        return false;

      // Skip size
      InputBuffer.get();
      InputBuffer.get();

			networkManager.byteIn += PacketSize;
			//networkManager.byteAll += PacketSize;

      int SignPacketSize = PacketSize - 2;
      memoryStream.Position = 0;
      memoryStream.SetLength(SignPacketSize);

      InputBuffer.get(MessageBuffer, SignPacketSize);

      networkManager.OnSocketReceiveData(MessageBuffer);
      return true;
    }

    /// <summary>
    /// Разрыв соединения
    /// </summary>
    public void Disconnect() {
      DisconnectSocket();
    }

    /// <summary>
    /// Отключение от сокетов
    /// </summary>
    public void DisconnectSocket() {
      if(mSocket == null) return;

      if(mSocket.Connected) {
        Debug.Log("Calling socket disconnect");
        mSocket.Disconnect(false);
      }

      if(mSocket != null) {
        mSocket.Close();
        mSocket = null;
      }
      OnDisconnect("Disconnect from server");
    }

    private void OnDisconnect(string debugString) {
      Debug.Log("NetworkManager::OnDisconnect() disconnect reason: " + debugString);

      InputBuffer.clear();
      networkManager.OnSocketClose(debugString);
    }

    #region Send package

    public bool SendPacket(INetOutPacket packet) {

      if(!networkManager.isConnect) {
        Debug.LogError("Trying send packet to closed socket!!");
        return false;
      }

      PacketSizeCalculator calcStream = new PacketSizeCalculator();
      packet.Write(calcStream);
      ByteArrayOutputStream outStream = new ByteArrayOutputStream(calcStream.PacketSize);
      packet.Write(outStream);

      networkManager.byteOut += calcStream.PacketSize;
      //networkManager.byteAll += calcStream.PacketSize;

      byte[] dataToSend = outStream.GetData();
      short PacketId = System.BitConverter.ToInt16(dataToSend, 2);
      System.BitConverter.GetBytes((short)dataToSend.Length).CopyTo(dataToSend, 0);
      try {
        mSocket.BeginSend(dataToSend, 0, dataToSend.Length, SocketFlags.None, null, mSocket);
      } catch(SocketException ex) {
        OnDisconnect("catch in SendPacket: " + ex.Message + " packetID:" + PacketId);
      }

      return true;
    }

    #endregion

    #region Events

    /// <summary>
    /// Установки соединения
    /// </summary>
    /// <param name="sender"></param>
    public void OnOpen(string sender) {
      networkManager.OnSocketOpen(sender);
    }

    /// <summary>
    /// Событие закрытия соединения
    /// </summary>
    /// <param name="reason"></param>
    public void OnClose(string reason) {
      networkManager.OnSocketClose(reason);
    }

    /// <summary>
    /// Событие приема данных
    /// </summary>
    /// <param name="data"></param>
    public void OnReceiveData(byte[] data) {
      networkManager.OnSocketReceiveData(data);
    }

    /// <summary>
    /// Событие ошибки сокетов
    /// </summary>
    /// <param name="error"></param>
    public void OnError(string error) {
      networkManager.OnSocketError(error);
    }

    #endregion

  }
}