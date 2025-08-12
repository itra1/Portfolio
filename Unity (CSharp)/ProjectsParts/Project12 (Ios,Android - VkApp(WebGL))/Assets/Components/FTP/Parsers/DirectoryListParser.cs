using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using FTP.Structs;

namespace FTP.Parsers
{
	public class DirectoryListParser
	{
		private readonly List<FileStruct> _myListArray = new();

		public FileStruct[] FullListing
		{
			get
			{
				return _myListArray.ToArray();
			}
		}

		public FileStruct[] FileList
		{
			get
			{
				List<FileStruct> _fileList = new List<FileStruct>();
				foreach (FileStruct thisstruct in _myListArray)
				{
					if (!thisstruct.IsDirectory)
					{
						_fileList.Add(thisstruct);
					}
				}
				return _fileList.ToArray();
			}
		}

		public FileStruct[] DirectoryList
		{
			get
			{
				List<FileStruct> _dirList = new List<FileStruct>();
				foreach (FileStruct thisstruct in _myListArray)
				{
					if (thisstruct.IsDirectory)
					{
						_dirList.Add(thisstruct);
					}
				}
				return _dirList.ToArray();
			}
		}

		public DirectoryListParser(string responseString)
		{
			_myListArray = GetList(responseString);
		}

		private List<FileStruct> GetList(string datastring)
		{
			List<FileStruct> myListArray = new List<FileStruct>();
			string[] dataRecords = datastring.Split('\n');
			//Получаем стиль записей на сервере
			FileListStyle _directoryListStyle = GuessFileListStyle(dataRecords);
			foreach (string s in dataRecords)
			{
				if (_directoryListStyle != FileListStyle.Unknown && s != "")
				{
					FileStruct f = new()
					{
						Name = ".."
					};
					switch (_directoryListStyle)
					{
						case FileListStyle.UnixStyle:
							f = ParseFileStructFromUnixStyleRecord(s);
							break;
						case FileListStyle.WindowsStyle:
							f = ParseFileStructFromWindowsStyleRecord(s);
							break;
					}
					if (f.Name is not "" and not "." and not "..")
					{
						myListArray.Add(f);
					}
				}
			}
			return myListArray;
		}
		//Парсинг, если фтп сервера работает на Windows
		private FileStruct ParseFileStructFromWindowsStyleRecord(string Record)
		{
			//Предположим стиль записи 02-03-04  07:46PM       <DIR>     Append
			FileStruct f = new FileStruct();
			string processstr = Record.Trim();
			//Получаем дату
			string dateStr = processstr[..8];
			processstr = (processstr[8..]).Trim();
			//Получаем время
			string timeStr = processstr[..7];
			processstr = (processstr[7..]).Trim();
			f.CreateTime = dateStr + " " + timeStr;
			//Это папка или нет
			if (processstr[..5] == "<DIR>")
			{
				f.IsDirectory = true;
				processstr = (processstr[5..]).Trim();
			}
			else
			{
				string[] strs = processstr.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
				processstr = strs[1];
				f.IsDirectory = false;
			}
			//Остальное содержмое строки представляет имя каталога/файла
			f.Name = processstr;
			return f;
		}
		//Получаем на какой ОС работает фтп-сервер - от этого будет зависеть дальнейший парсинг
		public FileListStyle GuessFileListStyle(string[] recordList)
		{
			foreach (string s in recordList)
			{
				//Если соблюдено условие, то используется стиль Unix
				if (s.Length > 10
						&& Regex.IsMatch(s[..10], "(-|d)((-|r)(-|w)(-|x)){3}"))
				{
					return FileListStyle.UnixStyle;
				}
				//Иначе стиль Windows
				else if (s.Length > 8
						&& Regex.IsMatch(s[..8], "[0-9]{2}-[0-9]{2}-[0-9]{2}"))
				{
					return FileListStyle.WindowsStyle;
				}
			}
			return FileListStyle.Unknown;
		}
		//Если сервер работает на nix-ах
		private FileStruct ParseFileStructFromUnixStyleRecord(string record)
		{
			//Предположим. тчо запись имеет формат dr-xr-xr-x   1 owner    group    0 Nov 25  2002 bussys
			FileStruct f = new FileStruct();
			if (record[0] is '-' or 'd')
			{// правильная запись файла
				string processstr = record.Trim();
				f.Flags = processstr[..9];
				f.IsDirectory = (f.Flags[0] == 'd');
				processstr = (processstr[11..]).Trim();
				//отсекаем часть строки
				_ = CutSubstringFromStringWithTrim(ref processstr, ' ', 0);
				f.Owner = CutSubstringFromStringWithTrim(ref processstr, ' ', 0);
				f.CreateTime = GetCreateTimeString(record);
				//Индекс начала имени файла
				int fileNameIndex = record.IndexOf(f.CreateTime) + f.CreateTime.Length;
				//Само имя файла
				f.Name = record[fileNameIndex..].Trim();
			}
			else
			{
				f.Name = "";
			}
			return f;
		}

		private string GetCreateTimeString(string record)
		{
			//Получаем время
			string month = "(jan|feb|mar|apr|may|jun|jul|aug|sep|oct|nov|dec)";
			string space = @"(\040)+";
			string day = "([0-9]|[1-3][0-9])";
			string year = "[1-2][0-9]{3}";
			string time = "[0-9]{1,2}:[0-9]{2}";
			Regex dateTimeRegex = new Regex(month + space + day + space + "(" + year + "|" + time + ")", RegexOptions.IgnoreCase);
			Match match = dateTimeRegex.Match(record);
			return match.Value;
		}

		private string CutSubstringFromStringWithTrim(ref string s, char c, int startIndex)
		{
			int pos1 = s.IndexOf(c, startIndex);
			string retString = s[..pos1];
			s = (s[pos1..]).Trim();
			return retString;
		}
	}
}
