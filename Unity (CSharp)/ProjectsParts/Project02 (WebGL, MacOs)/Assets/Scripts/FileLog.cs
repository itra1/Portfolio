using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

using Cysharp.Threading.Tasks;

using UnityEngine;

public class FileLog : Singleton<FileLog>
{
	private string _pathRecord;
	private bool _process;
	private string _writeToLog;
	private string _lastFilename;
	private void Awake()
	{
		//#if !UNITY_EDITOR
		//Init("NNN");
		//#endif
	}

	public void Init(string name)
	{
		_lastFilename = name;
		Init();
		//Clear();
	}

	private void Init()
	{

		string dictionary = "/../logs/";

		if (!System.IO.Directory.Exists(Application.dataPath + dictionary))
			System.IO.Directory.CreateDirectory(Application.dataPath + dictionary);


		_pathRecord = Application.dataPath + dictionary + (System.DateTime.Now.ToString("dd.MM.yyyy_HH.mm.ss")) + $"_{_lastFilename}.txt";

		//if (!System.IO.File.Exists(_pathRecord))
		//	System.IO.File.Create(_pathRecord);

		Application.logMessageReceived -= LogCallback;
		Application.logMessageReceived += LogCallback;


		_writeToLog = "////////////////////////////////////\n"
		+ "/// " + System.DateTime.Now.ToString() + " \n"
		+ "////////////////////////////////////\n\n\n";
	}

	private void OnDestroy()
	{
		//		#if !UNITY_EDITOR
		Application.logMessageReceived -= LogCallback;
		//		#endif
	}

	private async void ProcessRecord()
	{
		_process = true;

		while (_writeToLog.Length > 0)
		{

			if (_writeToLog.Length == 0) break;

			//yield return new WaitForSeconds(5f);
			await UniTask.Yield(PlayerLoopTiming.Update);

			Write();

		}
		_process = false;

	}

	private void Write()
	{
		try
		{
			byte[] array = System.Text.Encoding.Default.GetBytes(_writeToLog);

			FileStream SourceStream = File.Open(_pathRecord, FileMode.Append, FileAccess.Write);
			SourceStream.Seek(0, SeekOrigin.End);
			Task t = SourceStream.WriteAsync(array, 0, array.Length);
			t.ContinueWith(_ =>
			{
				_writeToLog = "";
				SourceStream.Dispose();
				if (t.Exception != null)
				{
					Init();
				}
			});
		}catch
		{
			Init();
		}
	}

	private void Clear()
	{
		File.WriteAllText(_pathRecord, "");
	}

	void LogCallback(string condition, string stackTrace, LogType type)
	{
		string record = System.DateTime.Now.ToString("O") + " ";

		switch (type)
		{
			case LogType.Log:
				record += "LOG: ";
				break;
			case LogType.Warning:
				record += "WARNING: ";
				break;
			case LogType.Error:
				record += "ERROR: ";
				break;
			case LogType.Exception:
				record += "EXCEPTION: ";
				break;
			case LogType.Assert:
				record += "ASSERT: ";
				break;
		}
		record += condition + "\n";
		record += stackTrace + "\n";

		_writeToLog += record;

		if (_process) return;

		ProcessRecord();

	}

}
