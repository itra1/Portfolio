public class KeyPacket : Packet {

  public int randomInt;

  public override void ReadImpl() {
    randomInt = ReadD();
    ReadD();
    ReadD();
  }

  public override void Process() {
    Generals.Network.NetworkManager.SendPacket(new out_AuthLogin(randomInt, Generals.Network.NetworkManager.token));
  }

}
