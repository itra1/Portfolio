using System.IO;
using System.Threading.Tasks;

public class FileHelper {

	public static void WriteAllText(string path, string data) {
		File.WriteAllText(path, data);
	}
	public static async Task WriteAllTextAsync(string path, string data) {
		await File.WriteAllTextAsync(path, data);
	}
	public static string ReadAllText(string path) {
		return File.ReadAllText(path);
	}
	public static async Task<string> ReadAllTextAsync(string path) {
		return await File.ReadAllTextAsync(path);
	}
}

