using Generals.Network;

class in_Pong : Packet {
  public in_Pong() {
  }

  public override void ReadImpl() {
    NetworkManager.instance.Pong();
  }

  public override void Process() {
    
  }
}