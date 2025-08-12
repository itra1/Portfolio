using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEditor;
using UnityEngine;
using UniRx;

public class BuildArchivate
{

	public class ArchiveData
	{
		public string Name;
		public string Path;
		public string PathBase;
	}

	private static IEnumerator CompressorProcess(System.IObserver<string> observer, CancellationToken cancellationToken, ArchiveData arData)
	{
		//Init();


		yield return null;

		string filezip = arData.Name;

		lzip.compressDir(arData.Path, 9, arData.PathBase + "\\" + filezip, false);

		observer.OnNext("");
		observer.OnCompleted();
	}

	public static void Compression(ArchiveData archiveData, UnityEngine.Events.UnityAction OnComplete)
	{
		it.Logger.Log("Compression start");

		var obs = UniRx.Observable.FromCoroutine<string>((observer, cancellationToken) => CompressorProcess(observer, cancellationToken, archiveData));

		obs.Subscribe((x) =>
		{
			it.Logger.Log("Complete archive base");
			OnComplete?.Invoke();
		});
	}

}
