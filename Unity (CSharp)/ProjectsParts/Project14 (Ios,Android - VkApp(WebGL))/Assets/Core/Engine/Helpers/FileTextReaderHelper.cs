using Core.Engine.Helpers;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Engine.Helpers
{
	public static class FileTextReaderHelper
	{
		public static List<string> ReadFileNicknames(string filePath)
		{
			List<string> result = new();

			var lines = File.ReadAllLines(filePath);

			foreach (var line in lines) {

				var readLine = line.Trim();

				if(InputValidate.UserName(readLine) == 0 && !result.Contains(readLine)) {
					result.Add(readLine);
				}

			}

			return result;
		}
	}
}
