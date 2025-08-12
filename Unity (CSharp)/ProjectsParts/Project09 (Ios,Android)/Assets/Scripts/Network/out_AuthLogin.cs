using UnityEngine;
using System.Collections;

public class out_AuthLogin : INetOutPacket {
  private int playerId;
  private string token;

  public out_AuthLogin(int playerId, string tocken) {
    this.playerId = playerId;
    this.token = tocken;
  }

  public void Write(NetOutputStream stream) {
    
    stream.WriteC(0x00);
    stream.WriteC(0x01);
    stream.WriteD(playerId);
    stream.WriteASCII(token);

  }
}
