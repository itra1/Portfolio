using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static it.Diagrams.CurveDiagram;

namespace it.Diagrams
{
	public class BarDiagram : MonoBehaviour
	{
		[SerializeField] private Material _upMaterial;
		[SerializeField] private Material _downMaterial;
		[SerializeField] private RectTransform _diagramRect;
		[SerializeField] private TextMeshProUGUI _verticalLabel;
		[SerializeField] private TextMeshProUGUI _horizontalLabel;
		[SerializeField] private Image _barPrefab;
		[SerializeField] private Image _harizontalSeparatePrefab;
		[SerializeField] private Image _harizontal0SeparatePrefab;
		[SerializeField] private Color _valueZeroColor;
		[SerializeField] private Color _valueColor;

		private List<DiagramItem> _data;
		private PoolList<Image> _barPool;
		private PoolList<Image> _horizontalSeparatePool;
		private PoolList<TextMeshProUGUI> _vecticalLabelsPool;
		private PoolList<TextMeshProUGUI> _horizontalLabelsPool;
		private float _minValue;
		private float _maxValue;
		private float _valuePeriod;
		private System.DateTime _minDate;
		private System.DateTime _maxDate;
		private Vector2[] _positions;
		private float _centerY;
		private float _horizontalMinut;
		private DiagramDateVisible _vo;
		private PokerStatisticDateInticator _dateDelimetr;
		private int _horizontalCountSeparate;

		//private void OnDrawGizmosSelected()
		//{
		//	_data = new List<DiagramItem>();
		//	_data.Add(new DiagramItem() { Date = System.DateTime.Now, Value = 1 });
		//	_data.Add(new DiagramItem() { Date = System.DateTime.Now.AddDays(-1), Value = 10 });
		//	_data.Add(new DiagramItem() { Date = System.DateTime.Now.AddDays(-2), Value = -3 });
		//	_data.Add(new DiagramItem() { Date = System.DateTime.Now.AddDays(-3), Value = 0.5f });
		//	_data.Add(new DiagramItem() { Date = System.DateTime.Now.AddDays(-4), Value = 1 });
		//	_data.Add(new DiagramItem() { Date = System.DateTime.Now.AddDays(-5), Value = -4 });
		//	_data.Add(new DiagramItem() { Date = System.DateTime.Now.AddDays(-6), Value = 1 });

		//	_minDate = System.DateTime.Now.AddDays(-6);
		//	_maxDate = System.DateTime.Now;

		//	SetData(_data
		//	, System.DateTime.Now.AddDays(-6)
		//	, System.DateTime.Now
		//	, PokerStatisticDateInticator.Date
		//	, DiagramDateVisible.None
		//	, _data.Count);
		//}
		public void SetData(List<DiagramItem> data, System.DateTime startDate, System.DateTime endDate, PokerStatisticDateInticator dateDelimetr, DiagramDateVisible visibleOptions, int horiszontalCountSeparate)
		{
			_horizontalCountSeparate = horiszontalCountSeparate + 1;
			_minDate = startDate;
			_maxDate = endDate;
			_dateDelimetr = dateDelimetr;
			_data = data;
			_vo = visibleOptions;
			_horizontalLabel.gameObject.SetActive(false);
			_verticalLabel.gameObject.SetActive(false);
			_barPrefab.gameObject.SetActive(false);
			_harizontalSeparatePrefab.gameObject.SetActive(false);

			_data = _data.OrderBy(x => x.Date).ToList();

			var deltaTime = _maxDate - _minDate;
			var delimetr = deltaTime / _horizontalCountSeparate;

			var oldData = new List<DiagramItem>(_data);
			_data.Clear();

			for (int i = 0; i < _horizontalCountSeparate - 1; i++)
			{
				var startDate1 = _minDate.AddMilliseconds((delimetr * i).TotalMilliseconds);
				var endDate1 = _minDate.AddMilliseconds((delimetr * (i + 1)).TotalMilliseconds);
				float value = 0;
				for (int y = 0; y < oldData.Count; y++)
				{
					if (oldData[y].Date > startDate1 && oldData[y].Date <= endDate1)
						value += oldData[y].Value;
				}
				_data.Add(new DiagramItem() { Date = endDate1, Value = value });
			}


			ConfirmData();
		}

		private void FindValues()
		{
			_minValue = float.MaxValue;
			_maxValue = float.MinValue;
			for (int i = 0; i < _data.Count; i++)
			{
				if (_data[i].Value < _minValue) _minValue = _data[i].Value;
				if (_data[i].Value > _maxValue) _maxValue = _data[i].Value;
			}

			_minValue = Mathf.Min(_minValue, -1f);
			_maxValue = Mathf.Max(_maxValue, 1f);
			_valuePeriod = _maxValue - _minValue;

			it.Logger.Log($"{_minValue} - {_maxValue} : {_valuePeriod}");
		}
		private int _heightCount;
		private float _maxDiagramValue;
		private float _verticalGrad;
		private float _verticalGradItem;
		private float _heightItem = 1;
		private void CalcVerticalValues()
		{
			float visible = _diagramRect.rect.height /** (1 / 1.2f)*/;
			//_heightCount = 8;

			//if (_valuePeriod < 8)
			_maxDiagramValue = Mathf.Ceil(_valuePeriod / 8f) * 8f;

			_verticalGrad = visible / 8f;
			_verticalGradItem = _maxDiagramValue / 8f;

			//float itemValue = Mathf.Ceil(_maxDiagramValue / _heightCount / 50),1)*50;
			//float itemValue = Mathf.Max(Mathf.Ceil(_valuePeriod / _heightCount), 1);
			//_maxDiagramValue = itemValue * _heightCount;
			_heightItem = visible / _maxDiagramValue;

			_centerY = 0;

			if (_minValue < 0)
				_centerY = (float)_heightItem * (MathF.Abs((float)_minValue));

		}

		private void CalcHorizontal()
		{
			System.TimeSpan ts = _maxDate - _minDate;
			_horizontalMinut = _diagramRect.rect.width / (float)ts.TotalMinutes;
			//_horizontalMinut = 100f / 60f;
		}
		private void FillPoints()
		{
			float itemwWidth = (_diagramRect.rect.width * 0.92f) / _data.Count;

			_positions = new Vector2[_data.Count + 2];

			for (int i = 0; i < _data.Count; i++)
			{
				if (i == 0)
				{
					_positions[0] = new Vector2(0, (float)(((_data[i].Value - _minValue) * _heightItem) / (float)_diagramRect.rect.height));
				}

				_positions[i + 1] = new Vector2(((float)i / ((float)_data.Count - 1) * 0.92f) + 0.04f, (float)_data[i].Value);
				if (i == _data.Count - 1)
				{
					_positions[_data.Count + 1] = new Vector2(1, (float)(((_data[i].Value - _minValue) * _heightItem) / (float)_diagramRect.rect.height));
				}
			}

		}
		private void SpawnBars()
		{
			if (_barPool == null)
				_barPool = new PoolList<Image>(_barPrefab, _barPrefab.transform.parent);

			RectTransform pRect = _diagramRect.GetComponent<RectTransform>();
			_barPool.HideAll();

			for (int i = 1; i < _positions.Length - 1; i++)
			{
				var itm = _barPool.GetItem();
				itm.gameObject.SetActive(true);
				float valueY = _positions[i].y;

				if (valueY > 0)
				{
					itm.rectTransform.pivot = new Vector2(0.5f, 0);
					itm.material = _upMaterial;
				}
				else
				{
					itm.rectTransform.pivot = new Vector2(0.5f, 1);
					itm.material = _downMaterial;
				}
				it.Logger.Log(valueY);

				RectTransform rt = itm.GetComponent<RectTransform>();
				rt.anchoredPosition = new Vector2(pRect.rect.width * _positions[i].x, _centerY);
				itm.rectTransform.sizeDelta = new Vector2(itm.rectTransform.sizeDelta.x, Mathf.Abs(_positions[i].y) * (float)_heightItem);
			}
		}
		private void SpawmDates()
		{
			if (_horizontalLabelsPool == null)
				_horizontalLabelsPool = new PoolList<TextMeshProUGUI>(_horizontalLabel.gameObject, _horizontalLabel.transform.parent);

			RectTransform pRect = _diagramRect.GetComponent<RectTransform>();
			_horizontalLabelsPool.HideAll();

			for (int i = 1; i < _positions.Length - 1; i++)
			{
				var itm = _horizontalLabelsPool.GetItem();
				itm.gameObject.SetActive(true);
				RectTransform rt = itm.GetComponent<RectTransform>();
				rt.anchoredPosition = new Vector2(pRect.rect.width * _positions[i].x, pRect.anchoredPosition.y);

				if ((_vo & DiagramDateVisible.ChetClear) != 0 && i % 2 == 0)
					rt.gameObject.SetActive(false);

				if ((_vo & DiagramDateVisible.ChetOffset) != 0 && i % 2 == 0)
					rt.anchoredPosition = new Vector2(pRect.rect.width * _positions[i].x, pRect.anchoredPosition.y - 10);

				switch (_dateDelimetr)
				{
					case PokerStatisticDateInticator.Minute:
						itm.text = _data[i - 1].Date.ToString("mm");
						break;
					case PokerStatisticDateInticator.Time:
						itm.text = _data[i - 1].Date.ToString("t");
						break;
					case PokerStatisticDateInticator.Date:
						itm.text = _data[i - 1].Date.ToString("dd.MM");
						break;
					case PokerStatisticDateInticator.DateTime:
						itm.text = _data[i - 1].Date.ToString("dd.MM") + "\n" + _data[i - 1].Date.ToString("HH:dd");
						break;
					case PokerStatisticDateInticator.Month:
						itm.text = _data[i - 1].Date.ToString("MMM");
						break;
				}

			}

		}
		private void SpawnHorizontalSeparate()
		{
			if (_horizontalSeparatePool == null)
				_horizontalSeparatePool = new PoolList<Image>(_harizontalSeparatePrefab, _harizontalSeparatePrefab.transform.parent);
			_horizontalSeparatePool.HideAll();

			float allH = _diagramRect.rect.height;

			float val = _centerY + _verticalGrad;

			while (val < allH)
			{
				var itm = _horizontalSeparatePool.GetItem();
				itm.rectTransform.anchoredPosition = new Vector2(itm.rectTransform.anchoredPosition.x, val);
				val += _verticalGrad;
			}

			val = _centerY - _verticalGrad;
			while (val > 0)
			{
				var itm = _horizontalSeparatePool.GetItem();
				itm.rectTransform.anchoredPosition = new Vector2(itm.rectTransform.anchoredPosition.x, val);
				val -= _verticalGrad;
			}

			if (_minValue < 0 && _maxValue > 0)
			{
				_harizontal0SeparatePrefab.gameObject.SetActive(true);
				_harizontal0SeparatePrefab.rectTransform.SetAsLastSibling();
				_harizontal0SeparatePrefab.rectTransform.anchoredPosition = new Vector2(_harizontal0SeparatePrefab.rectTransform.anchoredPosition.x, _centerY);
			}
			else
				_harizontal0SeparatePrefab.gameObject.SetActive(false);

		}

		private void SpawnVerticalLabels()
		{

			if (_vecticalLabelsPool == null)
				_vecticalLabelsPool = new PoolList<TextMeshProUGUI>(_verticalLabel, _verticalLabel.transform.parent);
			_vecticalLabelsPool.HideAll();

			float allH = _diagramRect.rect.height;

			float val = _centerY + _verticalGrad;
			float valData = _verticalGradItem;
			while (val < allH)
			{
				var itm = _vecticalLabelsPool.GetItem();
				itm.text = valData.ToString();
				itm.color = _valueColor;
				itm.rectTransform.anchoredPosition = new Vector2(itm.rectTransform.anchoredPosition.x, val);
				val += _verticalGrad;
				valData += _verticalGradItem;
			}

			val = _centerY - _verticalGrad;
			valData = -_verticalGradItem;
			while (val > 0)
			{
				var itm = _vecticalLabelsPool.GetItem();
				itm.text = valData.ToString();
				itm.color = _valueColor;
				itm.rectTransform.anchoredPosition = new Vector2(itm.rectTransform.anchoredPosition.x, val);
				val -= _verticalGrad;
				valData -= _verticalGradItem;
			}

			if (_minValue < 0 && _maxValue > 0)
			{
				var itm = _vecticalLabelsPool.GetItem();
				itm.text = "0";
				itm.color = _valueZeroColor;
				itm.rectTransform.anchoredPosition = new Vector2(itm.rectTransform.anchoredPosition.x, _centerY);
			}

		}

		private void ConfirmData()
		{



			FindValues();
			CalcVerticalValues();
			CalcHorizontal();
			FillPoints();
			SpawnHorizontalSeparate();
			SpawnVerticalLabels();
			SpawmDates();
			SpawnBars();
		}

	}
}