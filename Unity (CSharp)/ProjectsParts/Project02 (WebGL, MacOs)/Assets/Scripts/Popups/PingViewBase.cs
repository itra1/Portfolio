using System;
using UnityEngine;

namespace it.Popups
{
  public abstract class PingViewBase : MonoBehaviour, Garilla.Ping.IPingListener
	{
    public  string Host { get; set; }

    public virtual void Awake()
    {
      Clear();
    }
    private void Start()
    {
			Garilla.Ping.PingListener.AddListener(this);
		}

		public virtual void Await(){

    }
    public virtual void OnDestroy()
    {
      Clear();
			Garilla.Ping.PingListener.RemoveListeners(this);
		}
    public abstract void Clear();
    public virtual void SetPing(long ping)
		{
		}

	}
}
