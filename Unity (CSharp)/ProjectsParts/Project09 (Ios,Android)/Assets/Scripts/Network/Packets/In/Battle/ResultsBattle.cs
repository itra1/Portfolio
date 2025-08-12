using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// Описание результатов боя
/// </summary>
public struct TeamResult {
  public short teamId;
  public ResultType type;
  public int stars;
  public List<PlayerData> players;
  public struct PlayerData {
    public string userId;
    public int rank;
    public long score;
  }
  public enum ResultType {
    none,
    win,
    looser,
    draw
  }
}

/// <summary>
/// Результат боя
/// </summary>
public class ResultsBattle : Packet {

  public static event Action<List<TeamResult>> OnBattleResult;

  List<TeamResult> teamResultList;

  public override void ReadImpl() {

    teamResultList = new List<TeamResult>();

    int arrLenght = ReadC();
    
    for(int i = 0; i < arrLenght; i++) {
      TeamResult res = new TeamResult();
			res.players = new List<TeamResult.PlayerData>();
			res.teamId = ReadC();
      res.type = (TeamResult.ResultType)ReadC();
      res.stars = ReadC();
      int countPlayer = ReadC();
			Debug.Log(res.teamId + " : " + res.type + " : " + res.stars);

			for(int j = 0; j < countPlayer; j++) {
        TeamResult.PlayerData onePlayer = new TeamResult.PlayerData();
        onePlayer.userId = ReadASCII();
				Debug.Log(onePlayer.userId);
        onePlayer.rank = ReadD();
        onePlayer.score = ReadQ();
        res.players.Add(onePlayer);
      }
      teamResultList.Add(res);
    }
  }

  public override void Process() {
    if(OnBattleResult != null) OnBattleResult(teamResultList);
  }

}
