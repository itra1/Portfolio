using System;
using System.Collections.Generic;
using it.Network.Rest;

[Serializable]
public class SocketEventTable
{
  
  public UserLimited user;
  
  public Table table;
}

[Serializable]
public class SocketEventChinaDistributionSharedData
{
  
  public ChinaDistributionSharedData distribution;
  
  public List<DistributionEvent> events;
}

[Serializable]
public class SocketEventDistributionSharedData
{
  
  public DistributionSharedData distribution;
  
  public List<DistributionEvent> events;
}

[Serializable]
public class SocketEventDistributionUserData
{
  
  public List<DistributionCard> cards;
	
	public ulong total_time_bank;

}
