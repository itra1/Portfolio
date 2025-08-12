using UnityEngine;
/// <summary>
/// Означает установку в очередь на ожидание боя
/// </summary>
public class QueuedToBattle : Packet {

  public override void Process() {
    GameManager.instance.PlayerAddInQueue();
  }
}
