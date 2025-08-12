using System.Collections;
using UnityEngine;

/// <summary>
/// Страницы
/// </summary>
public class Pages : MonoBehaviour
{
	public UnityEngine.Events.UnityAction<int> OnChangePage;

	[SerializeField] private int _maxPagesVisible = 5;
	[SerializeField] private RectTransform _content;
	[SerializeField] private RectTransform _pagesContent;
	[SerializeField] private it.UI.Elements.GraphicButtonUI _leftButton;
	[SerializeField] private it.UI.Elements.GraphicButtonUI _rightButton;
	[SerializeField] private it.UI.Elements.GraphicButtonUI _pageNumberPrefab;

	public int MaxPage { get { return (int)Mathf.Ceil(_recordCount / _recordOnPage); } }

	private PoolList<it.UI.Elements.GraphicButtonUI> _pagePooler;
	private int _recordCount;
	private int _recordOnPage;
	private int _pageIndex;

	public void SetDates(int recordCount, int recordOnPage, int pageIndex = 0)
	{
		if (_pagePooler == null)
			_pagePooler = new PoolList<it.UI.Elements.GraphicButtonUI>(_pageNumberPrefab, _pagesContent);

		_recordCount = recordCount;
		_recordOnPage = recordOnPage;
		_pageIndex = pageIndex;
		DrawPages();
	}

	private void DrawPages()
	{
		int side = (int)Mathf.Floor(_maxPagesVisible / 2);
		int startIndex = Mathf.Max(0, _pageIndex - side);
		int endIndex = Mathf.Min(MaxPage-1, _pageIndex + side);

		int count = endIndex - startIndex + 1;

		_pagePooler.HideAll();

		for (int i = 0; i < count; i++){
			int page = startIndex + i;
			var itm = _pagePooler.GetItem();
			var txt= itm.GetComponentInChildren<TMPro.TextMeshProUGUI>();
			txt.text = (page + 1).ToString();
			itm.OnClick.RemoveAllListeners();
			itm.OnClick.AddListener(() =>
			{
				SetPage(page);
			});
		}

		float deltaW = _content.rect.width - _pagesContent.rect.width;

		_pagesContent.sizeDelta = new Vector2(count * _pageNumberPrefab.GetComponent<RectTransform>().rect.width,_pagesContent.rect.height);
		_content.sizeDelta = new Vector2(_pagesContent.rect.width + deltaW, _content.rect.height);

	}

	public void SetPage(int page)
	{
		_pageIndex = page;
		OnChangePage.Invoke(_pageIndex);
		DrawPages();
	}

	public void LeftArrowButton()
	{
		if (_pageIndex <= 0) return;
		SetPage(_pageIndex - 1);
	}

	public void RightArrowButton()
	{
		if (_pageIndex >= MaxPage - 1) return;
		SetPage(_pageIndex + 1);
	}

}