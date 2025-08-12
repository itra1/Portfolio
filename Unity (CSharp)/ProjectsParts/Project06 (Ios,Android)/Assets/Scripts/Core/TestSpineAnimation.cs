using UnityEngine;
using Spine.Unity;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(TestSpineAnimation))]
public class TestSpineAnimationEditor : Editor {

	public override void OnInspectorGUI() {
		base.OnInspectorGUI();

		EditorGUILayout.BeginHorizontal();

		if(GUILayout.Button("Set Animation")) {
			((TestSpineAnimation)target).AnimationSet();
		}

		if(GUILayout.Button("Add Animation")) {
			((TestSpineAnimation)target).AnimationAdd();
		}

		if(GUILayout.Button("Reset Animation")) {
			((TestSpineAnimation)target).AnimationReset();
		}

		EditorGUILayout.EndHorizontal();

	}

}
#endif

public class TestSpineAnimation : MonoBehaviour {

  [SpineAnimation(dataField: "skeletonAnimation")]
  public string spineAnim = "";                 // Анимация бега

  public int layer;
  public bool loop;

  public void Start() { }

  public void AnimationSet() {
    SetAnimation(spineAnim, loop);
  }

  public void AnimationAdd() {
    AddAnimation(layer, spineAnim, loop, 0);
  }

  public void AnimationReset() {
    ResetAnimation();
  }

  #region Animation

  public SkeletonAnimation skeletonAnimation;           // Ссылка на спайн анимацию
  bool isAlterStopAnim;                                 // Активна анимация простоя

  
  /// <summary>
  /// Установка основной анимации
  /// </summary>
  /// <param name="anim">Название анимации</param>
  /// <param name="loop">Циклы</param>
  public void SetAnimation(string anim, bool loop) {
    if(!gameObject) return;
    
    skeletonAnimation.state.SetAnimation(0, anim, loop);
  }
  /// <summary>
  /// Сброс анимации
  /// </summary>
  public void ResetAnimation() {
    skeletonAnimation.Initialize(true);
    SubscribeAnimEvents();
  }
  /// <summary>
  /// Подписываемся на события анимации
  /// </summary>
  protected void SubscribeAnimEvents() {
    skeletonAnimation.state.Event += AnimEvent;
    skeletonAnimation.state.Complete += AnimComplete;
    skeletonAnimation.state.End += AnimEnd;
    skeletonAnimation.state.Dispose += AnimDispose;
    skeletonAnimation.state.Interrupt += AnimInterrupt;
  }
  /// <summary>
  /// Наложение анимации
  /// </summary>
  /// <param name="index">Номар слоя</param>
  /// <param name="animName">Название анимации</param>
  /// <param name="loop">Зациклено</param>
  /// <param name="delay">Задержка запуска</param>
  public void AddAnimation(int index, string animName, bool loop, float delay) {
    skeletonAnimation.state.AddAnimation(index, animName, loop, delay);
  }
  /// <summary>
  /// Привызяка к событию анимации
  /// </summary>
  /// <param name="state"></param>
  /// <param name="trackIndex"></param>
  /// <param name="e"></param>
  void AnimEvent(Spine.TrackEntry trackEntry, Spine.Event e) {
    //Debug.Log("Event ");
  }
  /// <summary>
  /// Привязка к окончанию анимации
  /// </summary>
  /// <param name="state"></param>
  /// <param name="trackIndex"></param>
  void AnimEnd(Spine.TrackEntry trackEntry) {
    Debug.Log("End " + trackEntry.ToString());
  }
  /// <summary>
  /// Событие выполнения анимации при зацикленном воспроизведении
  /// </summary>
  /// <param name="state"></param>
  /// <param name="trackIndex"></param>
  /// <param name="loopCount"></param>
  void AnimComplete(Spine.TrackEntry trackEntry) {
    Debug.Log("Complete " + trackEntry.ToString() + " " + trackEntry.loopCount);
  }

  void AnimDispose(Spine.TrackEntry trackEntry) {
    Debug.Log("Dispose " + trackEntry.ToString());
  }

  void AnimInterrupt(Spine.TrackEntry trackEntry) {
    Debug.Log("Interrupt " + trackEntry.ToString());
  }

  #endregion

}
