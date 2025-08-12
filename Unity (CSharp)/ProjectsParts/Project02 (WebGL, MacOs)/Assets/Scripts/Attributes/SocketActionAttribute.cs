using System;
using System.Collections;
using UnityEngine;


[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class SocketActionAttribute : System.Attribute
{
  public string AliasName;
  public string Name;

  public SocketActionAttribute(string AliasName, string name = null)
  {
	 this.AliasName = AliasName;
	 this.Name = name;
  }

}