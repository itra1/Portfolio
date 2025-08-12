using UnityEngine;
using System.Collections;
using System;
using Leguar.TotalJSON;

namespace it.Network.Socket
{
	/// <summary>
	/// Входящие событие сокетов
	/// </summary>
	public abstract class SocketIn : IDisposable
	{
		public JSON JSource
		{
			get
			{
				if (_jSource == null)
					_jSource = JSON.ParseString(sSource);
				return _jSource;
			}
			set
			{
				_jSource = value;
			}
		}

		public virtual float PackageLive { get; private set; } = 5;
		public bool IsLockDispose { get; set; } = false;
		public float TimeCreate { get; set; }

		public string sSource;

		private JSON _jSource;

		public string id;
		public string Chanel;

		public void SetData(string chanel, JSON JSource)
		{
			this.Chanel = chanel;
			this.JSource = JSource;
		}
		public void SetData(string chanel, object fObjec)
		{
			TimeCreate = Time.timeSinceLevelLoad;
			this.Chanel = chanel;
			this.sSource = fObjec.ToString();
			//this.JSource = JSource;
		}
		public abstract void Parse();
		public abstract void Process();

		public void Dispose()
		{
			Disposing();
				_jSource = null;
			sSource = null;
		}

		protected virtual void Disposing()
		{
			_jSource = null;
			sSource = null;
			Chanel = null;
		}

	}

	public interface GameUpdate
	{
		SocketEventTable TableEvent { get; }
	}

}