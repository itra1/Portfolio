using System;
using UnityEngine.SceneManagement;
using ZbCatScene;

public class CatSceneHelper36: ExEvent.EventBehaviour {

  private bool _activeScene;
  private string _activeId;

  [ExEvent.ExEventHandler(typeof(ExEvent.CatSceneEvent.StartCatScene))]
  void StartCastScene(ExEvent.CatSceneEvent.StartCatScene cs) {
    if (cs.id == "36_1" || cs.id == "36_2" || cs.id == "36_3" || cs.id == "36_4") {
      _activeScene = true;
      _activeId = cs.id;
    }
  }

  [ExEvent.ExEventHandler(typeof(ExEvent.CatSceneEvent.StartCatFrame))]
  void StartCastFrame(ExEvent.CatSceneEvent.StartCatFrame cs) {
    if (!_activeScene) return;
  }

  [ExEvent.ExEventHandler(typeof(ExEvent.CatSceneEvent.EndCatFrame))]
  void EndCastFrame(ExEvent.CatSceneEvent.EndCatFrame cs) {
    if (!_activeScene) return;

  }

  [ExEvent.ExEventHandler(typeof(ExEvent.CatSceneEvent.EndCatScene))]
  void EndCastScene(ExEvent.CatSceneEvent.EndCatScene cs) {
    if (!_activeScene) return;

    if (cs.id == "36_1") {
      CatSceneManager.Instance.isSpecLevel = true;

      Game.User.UserManager.Instance.ActiveBattleInfo = LevelsManager.Instance.LevelsList.Find(x => x.Group == 40 && x.Level == 1);

      CloseBlack(() => {
        SceneManager.LoadScene("Battle");
      });
    }

    if (cs.id == "36_3") {
      Game.User.UserManager.Instance.ActiveBattleInfo = LevelsManager.Instance.LevelsList.Find(x => x.Group == 1 && x.Level == 3);

      CloseBlack(() => {
        SceneManager.LoadScene("Map");
      });
    }

    if (cs.id == "36_4") {
      CatSceneManager.Instance.isSpecLevel = false;
      CatSceneManager.Instance.library.catSceneList.Find(x => x.id == "36_1").isShow = true;
      CatSceneManager.Instance.library.catSceneList.Find(x => x.id == "36_2").isShow = true;
      CatSceneManager.Instance.library.catSceneList.Find(x => x.id == "36_3").isShow = true;
      CatSceneManager.Instance.library.catSceneList.Find(x => x.id == "36_4").isShow = true;
    }

  }

  private void CloseBlack(Action OnComplited) {
    FillBlack inst = UIController.ShowUi<FillBlack>();
    inst.gameObject.SetActive(true);
    inst.PlayAnim(FillBlack.AnimType.full, FillBlack.AnimVecor.close, OnComplited);
  }
}
