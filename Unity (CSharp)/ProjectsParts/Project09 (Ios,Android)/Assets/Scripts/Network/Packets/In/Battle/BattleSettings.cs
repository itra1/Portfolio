public class BattleSettings : Packet {

  enum BattleType {
    Basic
  }

  private BattleType battleType;
  private float width;
  private float height;

  public override void ReadImpl() {

    battleType = GetType(ReadC());
    width = ReadF();
    height = ReadF();
  }

  BattleType GetType(int num) {
    switch(num) {
      case 0:
        return BattleType.Basic;
    }
    return BattleType.Basic;
  }

  
  public override void Process() {
  }

}
