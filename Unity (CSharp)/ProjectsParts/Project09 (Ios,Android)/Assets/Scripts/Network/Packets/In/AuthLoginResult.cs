using UnityEngine;
using System;

public class AuthLoginResult : Packet {

  public static event Action<AuthStatus> OnAuthLogin;

  AuthStatus authResult;
  string gameProfileId;
  
  public override void ReadImpl() {
    authResult = (AuthStatus)ReadD();
    gameProfileId = ReadASCII();
  }

  public override void Process() {
    if(OnAuthLogin != null) OnAuthLogin(authResult);
  }

}
