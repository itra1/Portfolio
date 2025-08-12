using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using UnityEngine;

/// <summary>
/// ПАрсер XML и запись в кеш
/// </summary>
public class XMLParcer: Singleton<XMLParcer>
{

  //public delegate void ParsingComplitedDelegate(object data);
  public static event System.Action<Configuration.Config> ParsingComplited;

  public bool recordPlayerPrefs;
  public bool recordFile;

  public TextAsset sourceFile;

  protected override void Awake()
  {
    base.Awake();
    ReadData();
  }

  private void Start() { }

  public void ReadData()
  {
//DownloadFile();
// #if UNITY_WEBGL && !UNITY_EDITOR
// 		DownloadFile();
// #else
    ReadFile();
// #endif

  }

  public void ReadXmlButtonEditor()
  {
    ReadData();
  }

  private void ReadFile()
  {
    //ReadXml(sourceFile.text);
    ReadJson(sourceFile.text);
  }

  private void DownloadFile()
  {
    StartCoroutine(DownloadXml());
  }

  private void ReadJson(string textData)
  {

    Configuration.Config conf = Newtonsoft.Json.JsonConvert.DeserializeObject<Configuration.Config>(textData);

    if (ParsingComplited != null)
      ParsingComplited(conf);
    
  }


  /// <summary>
  /// Считываем файл с ГД
  /// </summary>
  public void ReadXml(string textData)
  {
    ClearParsText(ref textData);
    XElement fileGD = XElement.Parse(textData);
    Dictionary<string, object> allConfig = new Dictionary<string, object>();

    foreach (XElement one in fileGD.Elements())
    {
      ParsingXmlText(ref allConfig, one);
    }
    //result = Json.Serialize(allConfig);
    //if (ParsingComplited != null) ParsingComplited(allConfig);

    Debug.Log("Загрузка конфигурации завершена");

  }

  /// <summary>
  /// Чистка отл ишних текоы
  /// </summary>
  /// <param name="text"></param>
  private void ClearParsText(ref string text)
  {
    string replacement1 = "";
    string pattern = @"o:";
    text = Regex.Replace(text, pattern, replacement1);
    pattern = @"x:";
    text = Regex.Replace(text, pattern, replacement1);
    pattern = @"ss:";
    text = Regex.Replace(text, pattern, replacement1);
    pattern = @"html:";
    text = Regex.Replace(text, pattern, replacement1);
  }

  private void ParsingXmlText(ref Dictionary<string, object> conf, XElement oneElement)
  {

    if (oneElement.Name.LocalName == "Worksheet")
    {
      if (oneElement.Attribute("Name").Value == "Волны врагов")
      {
        conf.Add("chapters", ParsingTableLevels(oneElement));
      }
      if (oneElement.Attribute("Name").Value == "Общее")
      {
        conf.Add("summary", ParsingTable(oneElement));
      }
      if (oneElement.Attribute("Name").Value == "Противники")
      {
        conf.Add("enemy", ParsingTable(oneElement));
      }
      if (oneElement.Attribute("Name").Value == "Оружия_суперок_помощников")
      {
        conf.Add("weapon", ParsingTable(oneElement));
      }
      if (oneElement.Attribute("Name").Value == "Shop")
      {
        conf.Add("shop", ParsingTable(oneElement));
      }
      if (oneElement.Attribute("Name").Value == "Фоны")
      {
        conf.Add("levels", ParsingTable(oneElement));
      }
      if (oneElement.Attribute("Name").Value == "Сурвавайл")
      {
        conf.Add("survival", ParsingTable(oneElement));
      }
      if (oneElement.Attribute("Name").Value == "Сурвавайл (дроп)")
      {
        conf.Add("survivalDrop", ParsingTable(oneElement));
      }
    }
  }

  private object ParsingTable(XElement element)
  {
    List<object> enemyes = new List<object>();
    List<string> names = new List<string>();

    foreach (XElement one1 in element.Elements())
    {
      if (one1.Name.LocalName == "Table")
      {
        int rowNum = 0;
        foreach (XElement one2 in one1.Elements())
        {
          if (one2.Name.LocalName == "Row")
          {
            rowNum++;

            Dictionary<string, object> oneEnemy = new Dictionary<string, object>();
            int cellNum = 0;
            foreach (XElement cell in one2.Elements())
            {
              foreach (XElement cellData in cell.Elements())
              {
                if (cellData.Name.LocalName == "Data")
                {
                  if (rowNum == 1)
                  {
                    names.Add(cellData.Value);
                  } else
                  {
                    oneEnemy.Add(names[cellNum], cellData.Value);
                  }
                }
              }
              cellNum++;
            }

            if (rowNum > 1 && oneEnemy.Keys.Count >= 2)
            {
              enemyes.Add(oneEnemy);
            }
          }
        }
      }
    }
    return enemyes;
  }

  private object ParsingTableLevels(XElement element)
  {

    List<Dictionary<string, string>> allTable = new List<Dictionary<string, string>>();

    /// Парсим таблицу
    foreach (XElement one1 in element.Elements())
    {
      if (one1.Name.LocalName == "Table")
      {
        int rowNum = 0;
        foreach (XElement one2 in one1.Elements())
        {
          if (one2.Name.LocalName == "Row")
          {
            rowNum++;
            if (rowNum == 1)
              continue;
            int cellNum = 0;
            Dictionary<string, string> oneRow = new Dictionary<string, string>();
            foreach (XElement cell in one2.Elements())
            {
              if (cell.Value == "")
                continue;
              cellNum++;
              if (cellNum == 1)
              {
                oneRow.Add("chapter", cell.Value);
              }
              if (cellNum == 2)
              {
                oneRow.Add("level", cell.Value);
              }
              if (cellNum == 3)
              {
                oneRow.Add("mobId", cell.Value);
              }
              if (cellNum == 4)
              {
                oneRow.Add("timeSpawn", cell.Value);
              }
              if (cellNum == 5)
              {
                oneRow.Add("levelEnemy", cell.Value);
              }
              if (cellNum == 6)
              {
                oneRow.Add("baffId", cell.Value);
              }
              if (cellNum == 7)
              {
                oneRow.Add("mobType", cell.Value);
              }
            }
            if (oneRow.ContainsKey("chapter"))
              allTable.Add(oneRow);
          }
        }
      }
    }

    // Разбираем на уровни
    int chapter = 0;
    int level = 0;

    List<object> allEnemy = new List<object>();
    List<object> allLevels = new List<object>();
    List<object> allChapters = new List<object>();
    Dictionary<string, object> oneLevel = new Dictionary<string, object>();
    Dictionary<string, object> oneChapter = new Dictionary<string, object>();

    foreach (Dictionary<string, string> tableRow in allTable)
    {

      try
      {
        if (chapter.ToString() != tableRow["chapter"])
        {
          chapter++;
          level = 0;
          if (chapter != 1)
          {

            oneLevel.Add("enemy", allEnemy);
            allLevels.Add(oneLevel);
            oneLevel = new Dictionary<string, object>();

            oneChapter.Add("levels", allLevels);
            allChapters.Add(oneChapter);
            //allLevels.Add(oneLevel);
            allLevels = new List<object>();
            oneChapter = new Dictionary<string, object>();
          }
        }

        if (level.ToString() != tableRow["level"])
        {
          level++;
          if (level != 1)
          {
            oneLevel.Add("enemy", allEnemy);
            allLevels.Add(oneLevel);
            oneLevel = new Dictionary<string, object>();
          }
          allEnemy = new List<object>();
        }


        Dictionary<string, string> oneEnemy = new Dictionary<string, string>();

        oneEnemy.Add("mobId", tableRow["mobId"]);
        oneEnemy.Add("timeSpawn", tableRow["timeSpawn"]);
        oneEnemy.Add("levelEnemy", tableRow["levelEnemy"]);
        oneEnemy.Add("baffId", tableRow["baffId"]);
        try
        {
          oneEnemy.Add("mobType", tableRow["mobType"]);
        } catch { }
        allEnemy.Add(oneEnemy);
      } catch { }
    }

    oneLevel.Add("enemy", allEnemy);
    allLevels.Add(oneLevel);

    oneChapter.Add("levels", allLevels);
    allChapters.Add(oneChapter);

    return allChapters;
  }

  public IEnumerator DownloadXml()
  {
    Debug.Log("Старт загрузки Json");
    WWW read = new WWW("http://kuzmich.netarchitect.ru/GameDesign/GameDesign.json?v=" + Core.unixTime);
    yield return read;
    Debug.Log("Загрузка json окончена");

    if (read.text != "")
    {
      ReadJson(read.text);
    }

  }

}
