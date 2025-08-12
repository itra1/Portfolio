using UnityEngine;
using SharpJson;
using System.Collections.Generic;

/// <summary>
/// Информация о игроке
/// </summary>
public class ProfileInfo : Packet {

  public static event Actione<object> OnProfile;
  public static event Actione<object> OnCharacter;

  object profile;
  object character;

  public override void ReadImpl() {
    profile = JsonDecoder.DecodeText(ReadUTF8());
    character = JsonDecoder.DecodeText(ReadUTF8());
  }

  public override void Process() {
    if(OnProfile != null) OnProfile(profile);
    if(OnCharacter != null) OnCharacter(character);
  }

}
