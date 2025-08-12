using System;
using System.Net;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;

public class ServerEntry {
  public string host;
  public IPAddress ip;
  public string webSocket;
  public string webSocketSecure;
  public int port;

  public ServerEntry(string webSockets, string webSocketSecure = null , int port = 80) {
    this.webSocket = webSockets;
    this.webSocketSecure = (webSocketSecure != null ? webSocketSecure : webSockets);
    this.host = this.webSocketSecure;
    this.port = port;
  }
    
}