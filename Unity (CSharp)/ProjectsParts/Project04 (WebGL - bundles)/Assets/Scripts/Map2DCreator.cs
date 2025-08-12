using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(Map2DCreator))]
public class Map2DCreatorEditor: Editor
{
  public override void OnInspectorGUI()
  {
    base.OnInspectorGUI();


    if (GUILayout.Button("Screenshot"))
    {
      ((Map2DCreator)target).CreateMiniMap();
    }

  }
}


#endif

public class Map2DCreator: MonoBehaviour {
  
  [SerializeField]
  private int _scale = 1;
  [SerializeField]
  private string _number;
  [SerializeField]
  private bool _isGrid;

  public void CreateMiniMap()
  {
    if (!Directory.Exists(System.String.Format("{0}\\Levels\\{1}\\2d", Application.dataPath, _number)))
    {
      Directory.CreateDirectory(System.String.Format("{0}\\Levels\\{1}\\2d", Application.dataPath, _number));
    }

    ScreenCapture.CaptureScreenshot(System.String.Format("{0}\\Levels\\{1}\\2d\\map{1}{2}.png"
      , Application.dataPath
      , _number
      , (_isGrid ? "g" : "")), _scale);

  }
}
