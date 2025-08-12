using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tutorial {

  public class TutorialManager: Singleton<TutorialManager> {

    public List<Tutorial> tutorialList;

    private Queue<TutorialQueue> tutorialQueue = new Queue<TutorialQueue>();

    TutorialQueue activeTutor;

    private bool isVisible = false;

    private bool isActive = false;

    private bool isComplete = false;

    public bool IsActive {
      get {
        return !isComplete;
      }
    }

    private void Start() {

      Load();

    }

    private void Save() {
      SaveData saveData = new SaveData {
        isComplete = isComplete
      };
      PlayerPrefs.SetString("tutorial", Newtonsoft.Json.JsonConvert.SerializeObject(saveData));
    }

    private void Load() {

      if (!PlayerPrefs.HasKey("tutorial")) {
        isComplete = false;
        return;
      }

      SaveData saveData = Newtonsoft.Json.JsonConvert.DeserializeObject<SaveData>(PlayerPrefs.GetString("tutorial"));
      isComplete = saveData.isComplete;
    }

    public void Show(Type type, System.Action OnComplete = null, IFocusObject focusObject = null) {

      if (!TutorialManager.Instance.IsActive) {
        if (OnComplete != null) OnComplete();
        return;
      }

      if(type == Type.intro)
        AppMetrica.Instance.ReportEvent("tutorial_start");
      if (type == Type.complete)
        AppMetrica.Instance.ReportEvent("tutorial_end");

      tutorialQueue.Enqueue(new TutorialQueue {
        type = type,
        onComplete = OnComplete,
        focusObject = focusObject
      });

      NextShow();
    }

    Tutorial tutorialInstance;

    private void NextShow() {

      if (isVisible) return;

      if (tutorialQueue.Count <= 0) return;


      if (tutorialInstance != null)
        Destroy(tutorialInstance.gameObject);

      activeTutor = tutorialQueue.Dequeue();

      Tutorial tutor = tutorialList.Find(x => x.type == activeTutor.type);

      tutor.Load();

      if (tutor.isShowing) {
        Complete();
        StartCoroutine(NextShowTutorCor());
        return;
      }
      isVisible = true;

      GameObject instTutor = Instantiate(tutor.gameObject);
      tutorialInstance = instTutor.GetComponent<Tutorial>();
      if (activeTutor.focusObject != null)
        activeTutor.focusObject.Focus(true, () => {
          tutorialInstance.Complete();
        });
      tutorialInstance.Show();
      
    }

    public void Complete() {
      isVisible = false;

      if (activeTutor.onComplete != null) {
        activeTutor.onComplete();
      }

      if (activeTutor.focusObject != null)
        activeTutor.focusObject.Focus(false);

      StartCoroutine(NextShowTutorCor());
    }

    IEnumerator NextShowTutorCor() {
      yield return null;
      NextShow();
    }

    public void Finished() {
      isComplete = true;
      Save();
    }


    private struct TutorialQueue {
      public Type type;
      public System.Action onComplete;
      public IFocusObject focusObject;
    }
    
  }

  public class SaveData {
    public bool isComplete;
  }

}