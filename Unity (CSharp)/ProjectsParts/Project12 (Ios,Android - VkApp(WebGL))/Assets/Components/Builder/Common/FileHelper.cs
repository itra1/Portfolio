using System.IO;

namespace Builder.Common
{
	public class FileHelper
	{
		public static void MoveFiles(string sourcePath, string targetPath)
		{
			Directory.Move(sourcePath, targetPath);
		}
	}
}
