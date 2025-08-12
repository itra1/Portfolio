/*
  Макет игрока для примера одежды
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Spine;
using Spine.Unity;

namespace Player.Jack {

  [System.Serializable]
  public struct SkeletonClothes {
    public ShopElementType tupe;
    [SpineAtlasRegion]
    public string cloth;
    [SpineSlot]
    public string region;
    [SpineSlot]
    public string[] execRegion;
    public bool isHand;
  }

  public class PlayerModel: PlayerComponent {

    public AtlasAsset atlasAsset;                   // Рендер кости

    public SkeletonClothes[] clothesParametrs;

    public GameObject fireTorch;

    private bool noHand;

    private bool ShopAnim;

    private readonly string idleAnim = "Shop_Idle";                      // Анимация эмоций Джека
    private readonly string headAnim = "Shop_Hat";                      // Анимация эмоций Джека
    private readonly string spineAnim = "Shop_Back+Acc";                      // Анимация эмоций Джека
    float timeAnimation;

    Atlas atlas;


    bool enableJack;

    void OnEnable() {
      animation.onRebuild += ApplySkine;
      enableJack = true;
      StartCoroutine(Inicialized());
    }

    IEnumerator Inicialized() {
      yield return new WaitForEndOfFrame();
      if (enableJack) {
        enableJack = false;

        if (ShopAnim) animation.SetAnimation(0, idleAnim, true);

        ApplySkine(animation.skeletonRenderer);
        UpdateCloth();
      }
    }

    void OnDisable() {
      animation.onRebuild -= ApplySkine;
    }

    void UpdateCloth() {
      ResetSkin();
    }

    ShopElementType selectedHead;
    ShopElementType selectedSpine;
    ShopElementType selectedAcsessuary;

    public void ShowCloth(ShopElementType element) {
      ClothesPosition clp = Config.GetlothesPosition(element);

      if (element == selectedHead || element == selectedSpine || element == selectedAcsessuary) return;

      Debug.Log(element + " - " + clp);

      switch (clp) {
        case ClothesPosition.head:
          selectedHead = element;
          break;
        case ClothesPosition.spine:
          selectedSpine = element;
          break;
        case ClothesPosition.accessory:
          selectedAcsessuary = element;
          break;
        default:
          selectedHead = element;
          break;

      }
      animation.skeletonRenderer.Initialize(true);
      showCloth(selectedHead, clp);
      showCloth(selectedSpine, clp);
      showCloth(selectedAcsessuary, clp);
    }

    public void showCloth(ShopElementType element, ClothesPosition iClp) {
      Debug.Log(element);

      string cloth = "";
      foreach (SkeletonClothes cl in clothesParametrs) {
        if (cl.tupe == element) {
          cloth = cl.cloth;
          var slot = animation.skeletonRenderer.skeleton.FindSlot(cl.region);
          slot.SetColor(new Color(1, 1, 1, 1));
          animation.skeletonRenderer.skeleton.SetAttachment(cl.region, cloth);

          ClothesPosition clp = Config.GetlothesPosition(element);

          if (clp == ClothesPosition.accessory) {
            fireTorch.SetActive(element == ShopElementType.accessoryTorch);
            fireTorch.GetComponentInChildren<SpriteRenderer>().sortingLayerID = animation.skeletonRenderer.GetComponent<MeshRenderer>().sortingLayerID;
            fireTorch.GetComponentInChildren<SpriteRenderer>().sortingOrder = animation.skeletonRenderer.GetComponent<MeshRenderer>().sortingOrder;
          }

          if (timeAnimation <= Time.time && clp == iClp) {
            Debug.Log(timeAnimation);
            if (clp == ClothesPosition.head) {
              Debug.Log("animacia head");
              animation.AddAnimation(2, headAnim, false, 0);
            } else {
              Debug.Log("animacia oth");
              animation.AddAnimation(2, spineAnim, false, 0);
            }
            timeAnimation = Time.time + 2f;
          }

          if (cl.execRegion.Length > 0) {

            foreach (string exec in cl.execRegion) {
              var slot1 = animation.skeletonRenderer.skeleton.FindSlot(exec);
              slot1.SetColor(new Color(1, 1, 1, 0));
            }

          }
        }
      }
    }

    IEnumerator AnimCorotine(ClothesPosition type) {
      yield return new WaitForEndOfFrame();
      if (timeAnimation <= Time.time) {
        if (type == ClothesPosition.head) {
          animation.AddAnimation(2, headAnim, false, 0);
        } else {
          animation.AddAnimation(2, spineAnim, false, 0);
        }
        timeAnimation = Time.time + 1f;
      }
    }

    public void PickOffCloth(ShopElementType cloth) {
      if (cloth == ShopElementType.accessoryTorch) {
        fireTorch.SetActive(false);
      }
    }

    public void ResetSkin() {
      DeselectAllCloth();
      ApplySkine(animation.skeletonRenderer);
    }

    void ApplySkine(SkeletonRenderer skeletonRenderer1) {
      ShopElementType spine = Config.GetActiveCloth(ClothesPosition.spine);
      ShopElementType head = Config.GetActiveCloth(ClothesPosition.head);
      ShopElementType accessory = Config.GetActiveCloth(ClothesPosition.accessory);

      if (head != ShopElementType.none) {

        string cloth = "";
        foreach (SkeletonClothes cl in clothesParametrs) {
          if (cl.tupe == head) {
            cloth = cl.cloth;
            var slot = animation.skeletonRenderer.skeleton.FindSlot(cl.region);
            if (slot != null) {
              slot.SetColor(new Color(1, 1, 1, 1));
              animation.skeletonRenderer.skeleton.SetAttachment(cl.region, cloth);
              if (ShopAnim && selectedHead != head) {
                StartCoroutine(AnimCorotine(ClothesPosition.head));
                selectedHead = head;
              }
            }
            if (cl.execRegion.Length > 0) {

              foreach (string exec in cl.execRegion) {
                var slot1 = animation.skeletonRenderer.skeleton.FindSlot(exec);
                slot1.SetColor(new Color(1, 1, 1, 0));
              }

            }
          }
        }

      }

      if (spine != ShopElementType.none) {
        string cloth = "";
        foreach (SkeletonClothes cl in clothesParametrs) {
          if (cl.tupe == spine) {
            cloth = cl.cloth;
            var slot = animation.skeletonRenderer.skeleton.FindSlot(cl.region);
            slot.SetColor(new Color(1, 1, 1, 1));
            //if(ShopAnim) skeletonRenderer.GetComponent<SkeletonAnimation>().state.AddAnimation(2, spineAnim, false, 0);

            if (ShopAnim && selectedSpine != spine) {
              StartCoroutine(AnimCorotine(ClothesPosition.spine));
              selectedSpine = spine;
            }

            animation.skeletonRenderer.skeleton.SetAttachment(cl.region, cloth);

            if (cl.execRegion.Length > 0) {

              foreach (string exec in cl.execRegion) {
                var slot1 = animation.skeletonRenderer.skeleton.FindSlot(exec);
                slot1.SetColor(new Color(1, 1, 1, 0));
              }
            }
          }
        }
      }

      if (accessory != ShopElementType.none) {
        if (fireTorch != null) {
          fireTorch.SetActive(accessory == ShopElementType.accessoryTorch && !noHand);
          fireTorch.GetComponentInChildren<SpriteRenderer>().sortingLayerID = animation.skeletonRenderer.GetComponent<MeshRenderer>().sortingLayerID;
          fireTorch.GetComponentInChildren<SpriteRenderer>().sortingOrder = animation.skeletonRenderer.GetComponent<MeshRenderer>().sortingOrder;
        }


        string cloth = "";
        foreach (SkeletonClothes cl in clothesParametrs) {
          if (cl.tupe == accessory && !(noHand && cl.isHand)) {
            cloth = cl.cloth;
            var slot = animation.skeletonRenderer.skeleton.FindSlot(cl.region);
            slot.SetColor(new Color(1, 1, 1, 1));
            //if(ShopAnim) skeletonRenderer.GetComponent<SkeletonAnimation>().state.AddAnimation(2, spineAnim, false, 0);

            if (ShopAnim && selectedAcsessuary != accessory) {
              StartCoroutine(AnimCorotine(ClothesPosition.accessory));
              selectedAcsessuary = accessory;
            }

            animation.skeletonRenderer.skeleton.SetAttachment(cl.region, cloth);

            if (cl.execRegion.Length > 0) {

              foreach (string exec in cl.execRegion) {
                var slot1 = animation.skeletonRenderer.skeleton.FindSlot(exec);
                slot1.SetColor(new Color(1, 1, 1, 0));
              }
            }
          }
        }
      }
    }

    void DeselectAllCloth() {

      ShopElementType spine = Config.GetActiveCloth(ClothesPosition.spine);
      ShopElementType head = Config.GetActiveCloth(ClothesPosition.head);
      ShopElementType accessory = Config.GetActiveCloth(ClothesPosition.accessory);

      List<string> regions = new List<string>();
      List<string> ex = new List<string>();

      foreach (SkeletonClothes cl in clothesParametrs) {
        if (cl.tupe != head && cl.tupe != spine && cl.tupe != accessory) {
          regions.Add(cl.region);

          if (cl.execRegion.Length > 0) {
            foreach (string exec in cl.execRegion) {
              ex.Add(exec);
            }
          }

        }
      }

      if (animation.skeletonRenderer.skeleton == null) return;
      foreach (Slot bon in animation.skeletonRenderer.skeleton.slots) {

        if (regions.Exists(x => x == bon.Data.Name))
          bon.SetColor(new Color(1, 1, 1, 0));

        if (ex.Exists(x => x == bon.Data.Name))
          bon.SetColor(new Color(1, 1, 1, 1));

      }

    }

  }
}