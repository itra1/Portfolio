using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player.Jack {

  public class PlayerMoveManager: PlayerComponent {

    public List<PlayerMove> controllerList;
    public PlayerMove _activeController;
    public PlayerMove activeController {
      get {
        return _activeController;
      }
      set {
        _activeController = value;
        _activeController.Init();
      }
    }

    private void Start() {
      ChangeComponent();
    }

    [ExEvent.ExEventHandler(typeof(ExEvent.RunEvents.RunPhaseChange))]
    private void RunPhaseChange(ExEvent.RunEvents.RunPhaseChange eventData) {
      ChangeComponent();
    }

    public void ChangeComponent() {

      if (pet.IsPet) {

        //PlayerMove sourcePrefab = null;

        //switch (pet.instance.type) {
        //  case PetsTypes.dino:
        //    sourcePrefab = controllerList.Find(x => x.GetType() == typeof(DinoPet));

        //    //activeController = Instantiate(controllerList.Find(x => x.GetType() == typeof(DinoPet)), transform).GetComponent<DinoPet>();
        //    break;
        //  case PetsTypes.bat:
        //    sourcePrefab = controllerList.Find(x => x.GetType() == typeof(BatPet));
        //    //activeController = Instantiate(controllerList.Find(x => x.GetType() == typeof(BatPet)), transform).GetComponent<BatPet>();
        //    break;
        //  case PetsTypes.spider:
        //    sourcePrefab = controllerList.Find(x => x.GetType() == typeof(SpiderPet));
        //    //activeController = Instantiate(controllerList.Find(x => x.GetType() == typeof(SpiderPet)), transform).GetComponent<SpiderPet>();
        //    break;
        //}

        if(typeof(PetMove) != activeController.GetType()) {
          if (activeController != null)
            DeactiveOld();
          activeController = Instantiate(controllerList.Find(x => x.GetType() == typeof(PetMove)), transform).GetComponent(typeof(PetMove)) as PetMove;
        }

        return;
      }

      if (GameManager.activeLevelData.gameFormat == GameMechanic.jetPack) {

        if(activeController == null || (activeController != null && activeController.GetType() != typeof(JetPack))) {
          if (activeController != null)
            DeactiveOld();

          activeController = Instantiate(controllerList.Find(x => x.GetType() == typeof(JetPack)), transform).GetComponent<JetPack>();
        }

        return;

      }

      if(RunnerController.Instance.booster != BoostType.none && ((RunnerController.Instance.runnerPhase & (RunnerPhase.boost | RunnerPhase.postBoost | RunnerPhase.preBoost)) != 0)) {

        System.Type targetType = typeof(Classic);

        switch (RunnerController.Instance.booster) {
          case BoostType.speed:
            targetType = typeof(Force);
            break;
          case BoostType.skate:
            targetType = typeof(Skate);
            break;
          case BoostType.barrel:
            targetType = typeof(Barrel);
            break;
          case BoostType.millWheel:
            targetType = typeof(MillWell);
            break;
          case BoostType.ship:
            targetType = typeof(Ship);
            break;
          case BoostType.planer:
            targetType = typeof(Fly);
            break;
        }


        if (activeController == null || (activeController != null && activeController.GetType() != targetType)) {
          if (activeController != null)
            DeactiveOld();

          activeController = Instantiate(controllerList.Find(x => x.GetType() == targetType), transform).GetComponent(targetType.GetType()) as PlayerMove;
        }

        return;
      }

      if((RunnerController.Instance.runnerPhase & (RunnerPhase.boost | RunnerPhase.postBoost | RunnerPhase.preBoost)) == 0) {

        if (activeController == null || (activeController != null && activeController.GetType() != typeof(Classic))) {
          if (activeController != null)
            DeactiveOld();

           activeController = Instantiate(controllerList.Find(x => x.GetType() == typeof(Classic)), transform).GetComponent<Classic>();
        }

        return;

      }

    }

    private void DeactiveOld() {
      try {
        activeController.DeInit();
      } catch { }
      Destroy(activeController.gameObject);

    }

  }
}