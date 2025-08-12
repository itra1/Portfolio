using UnityEngine;
using System.Collections;

public class EnergyUpdate : Packet {

  public struct EnergyData {
    public string playerId;
    public float currentEnergy;
    public float maxEnergy;
    public float recoverySpeed;
  }

  EnergyData energyData = new EnergyData();

  public static event Actione<EnergyData> OnEnergy;
  
  public override void ReadImpl() {
    energyData.playerId = ReadASCII();
    energyData.currentEnergy = ReadF();
    energyData.maxEnergy = ReadF();
    energyData.recoverySpeed = ReadF();
  }

  public override void Process() {
    if(OnEnergy != null) OnEnergy(energyData);
  }
}
