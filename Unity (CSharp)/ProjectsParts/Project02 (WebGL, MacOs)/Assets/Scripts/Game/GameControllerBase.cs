using it;
using it.Main.SinglePages;
using it.Network.Rest;
using it.Network.Socket;
using it.UI;
using it.UI.Elements;

using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

namespace Garilla.Games
{
	public abstract class GameControllerBase : MonoBehaviour
	{
		public class PacketProcessItem
		{
			//public string PacketType;
			public SocketIn Package;
			public object Data;
			public UnityEngine.Events.UnityAction<object> Action;
		}
		protected PacketProcessItem _processedPackage;
		public bool IsProcessedPackage => _processedPackage != null;

		public List<Bets> Bets { get => _bets; set => _bets = value; }
		public GamePanel GamePanel { get => _gamePanel; set => _gamePanel = value; }
		public Dictionary<int, PlayerGameIcone> Players { get => _players; set => _players = value; }
		public bool IsHasGame => _isGame || (SelectTable != null && SelectTable.has_active_distribution);
		public int? HandsBeforeUpdate { get; set; } = null;

		[SerializeField] protected List<it.UI.Elements.Bets> _bets;
		[SerializeField] protected List<Image> _dealers;
		[SerializeField] protected GameObject _bankContainer;
		[SerializeField] protected it.UI.Elements.Bets _totalBank;
		[SerializeField] protected Image _dealerChip;
		public List<PlaceController.DropCards> CardDrop;

		private Queue<PacketProcessItem> _packetQueue = new Queue<PacketProcessItem>();
		private Table _selectTable;
		public Table SelectTable
		{
			get
			{
				return _selectTable;
			}
			set
			{
				_selectTable = value;
			}
		}
		protected Dictionary<int, GamePlaceUI> placesFree = new Dictionary<int, GamePlaceUI>();
		protected string _chanel;
		protected it.UI.GamePanel _gamePanel;
		protected bool _isWeatFullLoad;
		protected bool _isGame;
		protected Dictionary<int, PlayerGameIcone> _players = new Dictionary<int, PlayerGameIcone>();
		protected System.DateTime _lastSocketEventTime = System.DateTime.Now;

		private bool _isSubscribe;

		/// <summary>
		/// я за столом
		/// </summary>
		/// <returns></returns>
		public bool ISit()
		{
			if (_players == null)
				return false;

			foreach (var elem in _players)
				if (elem.Value.UserId == UserController.User.id)
					return true;
			return false;

		}
		public abstract void WSConnect();
		public abstract void WSDisconnect();
		public abstract void WSSocketOpen();

		protected virtual void StartSubscribe()
		{
			if (_isSubscribe) return;
			_isSubscribe = true;

			com.ootii.Messages.MessageDispatcher.RemoveListener(EventsConstants.NetworkTokenUpdate, NetworkTokenUpdate);
			com.ootii.Messages.MessageDispatcher.AddListener(EventsConstants.NetworkTokenUpdate, NetworkTokenUpdate);
		}

		public virtual void StopSubscribe()
		{
			com.ootii.Messages.MessageDispatcher.RemoveListener(EventsConstants.NetworkTokenUpdate, NetworkTokenUpdate);
		}

		private void NetworkTokenUpdate(com.ootii.Messages.IMessage handler)
		{
			it.Logger.Log("[WS] tokenUpdate event");
			_lastSocketEventTime = System.DateTime.Now.AddMinutes(-3);
		}

		private void Update()
		{
#if UNITY_STANDALONE

			// Ќа ѕ  присутствует проблема:
			// если игрок сидит за пустым столом длительное врем€, его незаметно выкидывает
			// «аплатка: при отсутствии новых сокет пакетов, повторно подписываемс€ на стол каждые 2 минуты
			if (SelectTable != null && (System.DateTime.Now - _lastSocketEventTime).TotalMinutes >= 2)
			{
				_lastSocketEventTime = System.DateTime.Now;
				BestSocketIO.Instance.EnterTableChanel(SelectTable.id, null);
			}
#endif

		}

		/// <summary>
		/// —ейчас идет игра
		/// </summary>
		/// <returns></returns>
		public bool InGame()
		{
			if (SelectTable == null)
				return false;

			if (!ISit())
				return false;
			int count = 0;
			foreach (var elem in _players)
				if (elem.Value != null)
					count++;

			if (count < SelectTable.players_autostart_count)
				return false;

			return true;
		}

		protected virtual void OnDestroy()
		{
			ClearPackages();
			StopSubscribe();
		}

		public virtual void InitGame(Table table, it.UI.GamePanel gameInitManager)
		{
			_chanel = SocketClient.GetChanelTable(table.id);
			this._gamePanel = gameInitManager;
		}

		public void ClearCarsDrop()
		{
			foreach (var elem in CardDrop)
				elem.Items.ForEach(x => x.Item.gameObject.SetActive(false));
		}

		public void AddPackageToProcessedQueue(PacketProcessItem item)
		{
			_packetQueue.Enqueue(item);
			if (!IsProcessedPackage)
				NextProcessedPackage();
		}

		public void ClearIsResting(ulong userid)
		{

			if (_packetQueue.Count <= 0) return;

			Queue<PacketProcessItem> newQueue = new Queue<PacketProcessItem>();

			while (_packetQueue.Count > 0)
			{
				var itm = _packetQueue.Dequeue();

				if (itm.Package is DistributonEvents)
				{
					var pack = (DistributonEvents)itm.Package;

					foreach (var pl in pack.SocketEventSharedData.distribution.players)
					{
						if (pl.user.id == userid)
							pl.is_resting = false;
					}

				}
				newQueue.Enqueue(itm);
			}
			_packetQueue = newQueue;

		}


		public void ClearPackages()
		{
			_packetQueue.Clear();

			while (_packetQueue.Count > 0)
			{
				var elem = _packetQueue.Dequeue();
				elem.Package.IsLockDispose = false;
			}
		}

		public void CompletePackageProcessed()
		{
			if (_processedPackage != null)
			{
				_processedPackage.Package.IsLockDispose = false;
				it.Logger.Log("Complete event");
			}
			_processedPackage = null;
			NextProcessedPackage();
		}

		IEnumerator DelayFullLoad()
		{
			yield return null;
			NextProcessedPackage();
		}

		private void NextProcessedPackage()
		{
			if (_packetQueue.Count == 0) return;
			if (_isWeatFullLoad)
			{
				StartCoroutine(DelayFullLoad());
				return;
			}

			try
			{
				if (_processedPackage != null)
					_processedPackage.Package.IsLockDispose = false;
				_processedPackage = null;
				//System.GC.Collect();
				_processedPackage = _packetQueue.Dequeue();
				//System.GC.Collect();
				_processedPackage.Action(_processedPackage.Data);
			}
			catch (System.Exception ex)
			{
				it.Logger.LogError("Error queue qction " + ex.Message);
			}
		}

	}
}