using UnityEngine;
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;

public class AsynchronousClient : MonoBehaviour {

  // адрес и порт сервера, к которому будем подключаться
  static int port = 8002; // порт сервера
  static string address = "52.58.183.112"; // адрес сервера

  static void Connect() {
    try {
      IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse(address), port);

      Socket socket = new Socket(AddressFamily.InterNetwork, System.Net.Sockets.SocketType.Stream, ProtocolType.Tcp);
      // подключаемся к удаленному хосту
      socket.Connect(ipPoint);
      string message = "fgesgf";
      byte[] data = Encoding.Unicode.GetBytes(message);
      socket.Send(data);

      // получаем ответ
      data = new byte[256]; // буфер для ответа
      StringBuilder builder = new StringBuilder();
      int bytes = 0; // количество полученных байт

      do {
        bytes = socket.Receive(data, data.Length, 0);
        builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
      }
      while(socket.Available > 0);
      Debug.Log("ответ сервера: " + builder.ToString());

      // закрываем сокет
      socket.Shutdown(SocketShutdown.Both);
      socket.Close();
    } catch(Exception ex) {
      Debug.Log(ex.Message);
    }
  }

  void Start() {
    Connect();
  }

}