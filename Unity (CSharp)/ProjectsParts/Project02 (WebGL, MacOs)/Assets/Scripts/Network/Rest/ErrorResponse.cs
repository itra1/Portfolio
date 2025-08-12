using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace it.Network.Rest
{
  public class ErrorResponse
  {
    
    public Error[] errors;
  }

  public class Error
  {
    
    public string id;
    
    public int status;
    
    public string code;
    
    public string title;

	}
	public class ErrorHttp
	{
		
		public string error;

	}



}