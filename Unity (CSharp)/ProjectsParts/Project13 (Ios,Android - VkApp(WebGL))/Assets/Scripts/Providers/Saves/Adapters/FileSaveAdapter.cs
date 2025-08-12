using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Game.Providers.Saves.Adapters
{
	public class FileSaveAdapter : ISaveAdapter
	{
		private string _saveFileName = "PinWin";
		private readonly Encoding _encoding = new UTF8Encoding(true);
		public string SaveFileName
		{
			get => _saveFileName;
			set => _saveFileName = value;
		}

		public async UniTask Save(string data)
		{
			SaveToDiskInternal(data);
			await UniTask.NextFrame();
		}

		public async UniTask<string> Load()
		{
			await UniTask.NextFrame();
			return GetSaveFromDisk();
		}

		private string GetSaveFolderPath() => Application.persistentDataPath;

		private string GetSaveFilePath()
		{
			return string.Format("{0}/{1}.save",
					GetSaveFolderPath(), _saveFileName);
		}

		public void PrintSaveFolderPath()
		{
			Debug.Log(GetSaveFolderPath());
		}

		[ContextMenu("Delete Save File")]
		public void DeleteFromDisk()
		{
			var saveFilePath = GetSaveFilePath();
			if (!File.Exists(saveFilePath))
				return;

			File.Delete(saveFilePath);
		}

		private string GetSaveFromDisk()
		{
			var saveFilePath = GetSaveFilePath();
			if (!File.Exists(saveFilePath))
				return "{}";

			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(saveFilePath, FileMode.Open);

			var textFromFile = (string) bf.Deserialize(file);
			file.Close();
			return textFromFile;
		}

		private void SaveToDiskInternal(string data)
		{
			var saveFilePath = GetSaveFilePath();

			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Create(saveFilePath);

			bf.Serialize(file, data);
			file.Close();
		}

		private void WriteToFile(FileStream fs, string value)
		{
			var info = _encoding.GetBytes(value);
			fs.Write(info, 0, info.Length);
		}
	}
}
