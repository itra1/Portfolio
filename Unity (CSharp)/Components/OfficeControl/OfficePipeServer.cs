using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Leguar.TotalJSON;
using Newtonsoft.Json;
using OfficeControl.Pipes.Base;
using OfficeControl.Pipes.Common;
using OfficeControl.Pipes.Packages;
using UnityEngine;

namespace OfficeControl
{
	internal class OfficePipeServer
	{
		private readonly Queue<RequestData> _requestsQueue = new();
		private readonly Dictionary<string, Type> _packets = new();
		private NamedPipeServerStream _pipeServer;
		private StreamString _streamString;
		private bool _isConnected = false;
		private bool _isWaitRequest = false;
		private CancellationTokenSource _cancelTS = new();
		private const string OfficePipeline = "[OfficePipeline]";

		private class RequestData
		{
			public Package Package;
			public Package Answer;
			public RequestData(Package requestPackage)
			{
				Package = requestPackage;
			}
		}

		public OfficePipeServer()
		{
			_packets = FindActionPackets();
			RunServer().Forget();
		}

		public static Dictionary<string, Type> FindActionPackets()
		{
			Dictionary<string, Type> packs = new();

			var types = System.Reflection.Assembly.GetExecutingAssembly().GetTypes().Where(x => x.IsSubclassOf(typeof(Package))).ToArray();

			for (var i = 0; i < types.Length; i++)
			{
				var ob = types[i].GetCustomAttributes(false);
				for (var x = 0; x < ob.Length; x++)
				{
					if (ob[x] is PackageNameAttribute pName)
						packs.Add(pName.Name, types[i]);
				}
			}
			return packs;
		}

		public async UniTask RunServer()
		{
			var pipeServerName = Settings.PipeServerName;

			_pipeServer = new(pipeServerName, PipeDirection.InOut, 1);
			Debug.Log($"{OfficePipeline} start pipeline");
			await _pipeServer.WaitForConnectionAsync(_cancelTS.Token);

			try
			{
				_streamString = new(_pipeServer);

				_ = _streamString.WriteString("CnpOS file control!");

				if (_streamString.ReadString() == "start")
					_isConnected = true;

			}
			catch (IOException e)
			{
				Debug.LogError($"{OfficePipeline} ERROR: {e.Message}");
				_isConnected = false;
			}
		}

		public async UniTask StopServer()
		{
			Debug.Log($"{OfficePipeline} stop pipeline");
			_cancelTS?.Cancel();
			_cancelTS?.Dispose();
			_cancelTS = null;
			if (_pipeServer == null)
				return;

			if (_isConnected)
				_ = await Request(new CommonQuit());

			_isConnected = false;
			_pipeServer.Close();
		}

		/// <summary>
		/// Запрос на отправку пакета
		/// </summary>
		/// <param name="newPackage"></param>
		/// <returns></returns>
		public async UniTask<Package> Request(Package newPackage)
		{
			RequestData request = new(newPackage);
			_requestsQueue.Enqueue(request);

			ProcessNextMessage();

			await UniTask.WaitUntil(() => request.Answer != null);
			return request.Answer;
		}

		private void ProcessNextMessage()
		{
			if (!_isConnected || _isWaitRequest)
				return;

			if (_requestsQueue.Count <= 0)
				return;

			var request = _requestsQueue.Dequeue();
			_isWaitRequest = true;
			Message(request).Forget();
		}

		private async UniTask Message(RequestData request)
		{
			var packege = MakePackage(request.Package);
			try
			{
				_ = _streamString.WriteString(packege);

				var answer = _streamString.ReadString();

				var jArray = JArray.ParseString(answer);

				var pachageName = jArray.GetString(0).ToString();

				if (!_packets.ContainsKey(pachageName))
					return;

				var packageString = jArray.GetJSON(1).CreateString();

				request.Answer = (Package)(JsonConvert.DeserializeObject(packageString, _packets[pachageName]));
				_isWaitRequest = false;
			}
			catch (ObjectDisposedException e)
			{
				Debug.LogError($"{OfficePipeline} Message {packege} ERROR: {e.Message}");
			}
			await new UniTask();
			return;
		}

		private string MakePackage(Package package)
		{
			var result = new object[2];
			result[0] = "Unknow";

			var ob = package.GetType().GetCustomAttributes(false);
			for (var x = 0; x < ob.Length; x++)
			{
				if (ob[x] is PackageNameAttribute pName)
					result[0] = pName.Name;
			}

			result[1] = package;
			return JsonConvert.SerializeObject(result);
		}
	}
}
