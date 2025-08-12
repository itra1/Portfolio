using OfficeControl.Common;
using OfficeControl.Excels;
using OfficeControl.PowerPoints;
using OfficeControl.Words;

namespace OfficeControl.Controllers
{
	/// <summary>
	/// Основной запускаемый файл
	/// </summary>
	internal class Apps
	{
		public static Apps Instance { get; private set; }
		private Dictionary<string, OfficeApplication> _applicationDictionary = new();
		public Apps()
		{
			Instance = this;
		}

		public OfficeApplication GetOfficeApplication(string uuidApp)
		{
			if (!_applicationDictionary.ContainsKey(uuidApp))
			{
				return null;
			}

			return _applicationDictionary[uuidApp];

		}

		public OfficeApplication OpenApp(string type, string path)
		{
			path = path.Replace("\\", "/");
			OfficeApplication officeApp = type switch
			{
				OfficeAppType.PowerPoint => new PowerPoint(path),
				OfficeAppType.Word => new Word(path),
				OfficeAppType.Excel => new Excel(path),
				_ => new PowerPoint(path)
			};

			_applicationDictionary.Add(officeApp.Uuid, officeApp);

			return officeApp;

		}

		public void CloseAll()
		{
			System.Console.WriteLine("CloseAll");
			foreach (KeyValuePair<string, OfficeApplication> elem in _applicationDictionary)
			{
				elem.Value.Close();
			}
			_applicationDictionary.Clear();
		}

		public void CloseApp(string uuid)
		{
			System.Console.WriteLine("CloseApp " + uuid);
			if (!_applicationDictionary.ContainsKey(uuid))
				return;

			OfficeApplication? app = _applicationDictionary[uuid];
			_ = _applicationDictionary.Remove(uuid);
			app.Close();
		}
	}
}
