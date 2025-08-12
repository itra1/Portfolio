
/// <summary>
/// Родитель на все панели
/// </summary>
public abstract class PanelUi : PanelUiBase {

  public System.Action OnClose;
  private bool isAddStack;
	
	/// <summary>
	/// Обработка кнопки назад
	/// </summary>
	public abstract void BackButton();
	
	protected virtual void OnEnable() {
		GameManager.OnStartLoadScene += OnStartLoadScene;
		GameManager.Instance.AddStack(this);
		isAddStack = true;
	}

	protected virtual void ClosingStart() {
		if(isAddStack)
			GameManager.Instance.PopStack(this);
	}

	protected virtual void OnDisable() {
		GameManager.OnStartLoadScene -= OnStartLoadScene;
	}

	private void OnStartLoadScene() {
		isAddStack = false;
	}
	
}
