using UnityEngine;
using System.Collections;

/// <summary>
/// События
/// </summary>
public static class EventsConstants
{
  public static string ActionsOrderAdd = "ActionsOrder.Add";
  public static string ActionsOrderRemove = "ActionsOrder.Remove";

  public static string InventaryAddItem = "Inventary.AddItem";
  public static string InventaryGetItem = "Inventary.GetItem";
  public static string InventaryOpenPanel = "Inventary.OpenPanel";

  public static string InventaryNote = "Inventary.Note";
  public static string InventaryLoad = "Inventary.Load";

  public static string UIEnable = "UI.Enable";
  public static string UIDisable = "UI.Disable";

  public static string InteractionFocus = "Interaction.Focus";
  public static string InteractionStart = "Interaction.Start";
  public static string InteractionStop = "Interaction.Stop";

  public static string SymbolLoad = "Symbol.Load";
  public static string SymbolGetItem = "Symbol.GetItem";
  public static string SymbolOpenPanel = "Symbol.OpenPanel";

  public static string PlayerProgressLoad = "PlayerProgress.Load";
  public static string PlayerProgressSave = "PlayerProgress.Save";

  public static string TutorialRun = "Tutorial.Run"; // запуск туториала
  public static string TutorialComplete = "Tutorial.Complete"; // запуск туториала

  public static string PlayerFormChange = "PlayerFormChange"; // Событие изменение формы

  public static string ConsoleOpen = "Console.Open";
  public static string ConsoleClose = "Console.Close";
}
