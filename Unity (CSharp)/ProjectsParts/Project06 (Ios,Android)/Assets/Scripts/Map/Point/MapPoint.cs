using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(MapPoint))]
public class MapPointEditor: Editor {

  int allSection;
  int section;

  public override void OnInspectorGUI() {
    base.OnInspectorGUI();

    EditorGUILayout.BeginHorizontal();

    section = EditorGUILayout.IntField(section);
    allSection = EditorGUILayout.IntField(allSection);

    if (GUILayout.Button("Set")) {
      ((MapPoint)target).SetSections(section, allSection);
    }
    EditorGUILayout.EndHorizontal();

  }

}

#endif

public class MapPoint : MonoBehaviour {

  /// <summary>
  /// Цвет фарм индикатора в не законченном состоянии
  /// </summary>
  [SerializeField]
  private Color farmColor;
  /// <summary>
  /// Цвет фарм индикатора в законченном состоянии
  /// </summary>
  [SerializeField]
  private Color protectColor;
  /// <summary>
  /// Радиальный спрайт фарм индикатора
  /// </summary>
  [SerializeField]
  private Image colorSections;
  /// <summary>
  /// Цвет границ долей фарм индикатора в верхнем положении
  /// </summary>
  [SerializeField]
  private Color borderLibeColorLight;
  /// <summary>
  /// Цвет границ долей фарм индикатора в нижнем положении
  /// </summary>
  [SerializeField]
  private Color borderLibeColorDark;
  /// <summary>
  /// Граница фарм индикатора
  /// </summary>
  [SerializeField]
  private Transform sectionBorder;
  /// <summary>
  /// Индикация флага
  /// </summary>
  [SerializeField]
  private GameObject flag;
  /// <summary>
  /// Босс на локации
  /// </summary>
  [SerializeField]
  public Image enemyImage;
  /// <summary>
  /// Текст счетчика
  /// </summary>
  [SerializeField]
  private TMPro.TextMeshPro countText;
  /// <summary>
  /// Заголовок
  /// </summary>
  [SerializeField]
  public TextMesh title;
  /// <summary>
  /// Рендерер для
  /// </summary>
  [SerializeField]
  private SpriteRenderer spriteRenderer;
  /// <summary>
  /// Сприт на 1 линию
  /// </summary>
  [SerializeField]
  private Sprite spriteTitle1Lint;
  /// <summary>
  /// Спрайт на 2 линии
  /// </summary>
  [SerializeField]
  private Sprite spriteTitle2Lint;

  private int actualSection;

  public void SetSections(int active, int all) {

    if (all == 0) {
      colorSections.fillAmount = 0;
      return;
    }
    float value = (all - active) * (1f / all);

    colorSections.fillAmount = (float)value;
    colorSections.Rebuild(CanvasUpdate.PreRender);
    colorSections.color = farmColor;

    if(actualSection != all) {
      actualSection = all;
      SetSectionBorder();
    }
  }


  private List<Transform> sectionBorderList = new List<Transform>();
  private void SetSectionBorder() {
    sectionBorderList.ForEach(x => {
      Destroy(x.gameObject);
    });
    sectionBorderList.Clear();
    sectionBorder.gameObject.SetActive(false);

    if (actualSection <= 1) return;

    if (sectionBorder.parent.GetComponent<UnityEngine.Rendering.SortingGroup>() == null) {
      var rend = sectionBorder.parent.gameObject.AddComponent<UnityEngine.Rendering.SortingGroup>();
      rend.sortingLayerName = "MapPoint";
      rend.sortingOrder = 15;
    }

    for (int i = 0; i < actualSection; i++) {
      GameObject inst = Instantiate(sectionBorder.gameObject, sectionBorder.parent);
      inst.transform.localEulerAngles = new Vector3(0, 0, -i * (360 / actualSection));
      inst.SetActive(true);
      sectionBorderList.Add(inst.transform);

      float angleNoFull = inst.transform.localEulerAngles.z % 360;
      float perc = 0;

      if (angleNoFull < 180) {
        perc = angleNoFull / 180;
      } else {
        perc = 1 - ((angleNoFull - 180) / 180);
      }
      inst.GetComponentInChildren<SpriteRenderer>().color = new Color(Mathf.Lerp(borderLibeColorLight.r, borderLibeColorDark.r, perc), Mathf.Lerp(borderLibeColorLight.g, borderLibeColorDark.g, perc), Mathf.Lerp(borderLibeColorLight.b, borderLibeColorDark.b, perc), 1);

    }
  }

  public void ShowTitle(bool isShow, string text = null) {

    if (text != null) {

      if(text.Length >= 11) {
        string[] strArr = text.Split(' ');
        text = "";
        if (strArr.Length == 2) {

          for (int i = 0; i < strArr.Length; i++) {
            if (i != 0)
              text += '\n';
            text += strArr[i];
          }
          
        } else {
          for (int i = 0; i < strArr.Length; i++) {
            if(i == 2)
              text += '\n';
            else if(i > 0)
              text += " ";
            text += strArr[i];
          }
        }

        if (strArr.Length == 1) {
          spriteRenderer.sprite = spriteTitle1Lint;
        } else {
          spriteRenderer.sprite = spriteTitle2Lint;
        }

      } else {
        spriteRenderer.sprite = spriteTitle1Lint;
      }

      title.text = text;
    }
    title.transform.parent.gameObject.SetActive(isShow);
  }

  [ContextMenu("Resize")]
  public void ResizeBorder() {

    SpriteRenderer sr = sectionBorder.GetComponentInChildren<SpriteRenderer>();

    sr.transform.localScale = new Vector3(.03f, 0.127f, 1);

  }
  [ContextMenu("Change")]
  public void Change() {

    title.transform.parent.GetComponent<UnityEngine.Rendering.SortingGroup>().sortingOrder = 17;

  }

}
