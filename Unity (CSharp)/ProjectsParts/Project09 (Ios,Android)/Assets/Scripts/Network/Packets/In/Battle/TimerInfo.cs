/// <summary>
/// Пакет таймера с сервера
/// </summary>
public class TimerInfo : Packet {

  public static event Actione<TimeInfoData> OnTimeFromServer;

  TimeInfoData timeInfodata;

  public override void ReadImpl() {
    timeInfodata = new TimeInfoData();
    timeInfodata.type = (TimerType)ReadC();
    timeInfodata.secondTotal = ReadF();
    timeInfodata.secondLeft = ReadF();
  }

  public override void Process() {
    if(OnTimeFromServer != null) OnTimeFromServer(timeInfodata);
  }

}
