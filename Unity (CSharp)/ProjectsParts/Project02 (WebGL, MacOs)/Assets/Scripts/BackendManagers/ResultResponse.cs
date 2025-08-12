using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultResponse<T> where T : class
{
    public T Result = null;
    public string ErrorMessage = "";

    public ResultResponse(T result)
    {
        Result = result;
        ErrorMessage = ""; 
    }

    public ResultResponse(string errorMessage)
    {
        Result = null;
        ErrorMessage = errorMessage;
    }

    public bool IsSuccess
    {
        get
        {
            return Result != null;
        }
    }
}

public class ResultResponse
{
  public bool IsSuccess = false;
  public string ErrorMessage = "";
    public ResultResponse(bool result)
    {
        IsSuccess = result;
    }

    public ResultResponse(string errorMessage)
    {
        IsSuccess = false;
        ErrorMessage = errorMessage;
    }
}
