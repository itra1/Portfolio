using OfficeControl.Pipes.Base;

namespace OfficeControl.Common
{
	public abstract class OfficeApplication
	{
		public abstract string AppType { get; }

		protected string _uuid;
		protected string _filePath;
		protected string _fileName;

		public string Uuid { get => _uuid; set => _uuid = value; }
		public string FilePath { get => _filePath; set => _filePath = value; }

		public OfficeApplication(string filePath)
		{
			FilePath = filePath;
			_fileName = FilePath.Replace("\\","/").Split(new char[] { '/' }).Last();

			_uuid = System.Guid.NewGuid().ToString();
		}

		public abstract Package MakeOpenCompletePackage();

		public abstract void Close();

	}
}
