using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace StringDrop.Editor
{
	[CustomPropertyDrawer(typeof(StringDropListAttribute))]
	public class StringListDrawer : PropertyDrawer
	{
		private string[] _properties = null;
		private Dictionary<string, int> _props = new();

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			var _currentIndex = -1;
			if (property.propertyType != SerializedPropertyType.String)
			{
				EditorGUI.LabelField(position, label.text, "Only string support.");
				return;
			}

			MakeProperties(property, ref _currentIndex);

			position.width -= (40 + 5);
			var newIndex = EditorGUI.Popup(position, label.text, _currentIndex, _properties);
			if (newIndex != _currentIndex)
			{
				_currentIndex = newIndex;
				property.stringValue = _properties[_currentIndex];
			}

			position.x += position.width + 5;
			position.width = 40;
			if (GUI.Button(position, "Clear"))
			{
				_currentIndex = -1;
				property.stringValue = "";
			}
		}

		private void MakeProperties(SerializedProperty property, ref int _currentIndex)
		{
			_props.Clear();
			var targetType = ((StringDropListAttribute) attribute).Type;
			var checkAttribute = ((StringDropListAttribute) attribute).CheckAttribute;
			var propertyList = targetType.GetProperties();

			var inst = Activator.CreateInstance(targetType);
			foreach (var elemProp in propertyList)
			{
				int orderIndex = -1;
				if (checkAttribute)
				{
					var attr = elemProp.GetCustomAttribute<StringDropItemAttribute>();
					if (attr == null)
						continue;
					orderIndex = attr.Index;
				}

				_props.Add(elemProp.GetConstantValue().ToString(), orderIndex);
			}

			var membbersList = targetType.GetFields();
			foreach (var elemMember in membbersList)
			{
				int orderIndex = -1;
				if (checkAttribute)
				{
					var attr = elemMember.GetCustomAttribute<StringDropItemAttribute>();
					if (attr == null)
						continue;
					orderIndex = attr.Index;
				}

				_props.Add(elemMember.GetValue(inst).ToString(), orderIndex);
			}

			_properties = new string[_props.Count];

			var index = -1;
			foreach (var propValue in _props)
			{
				index++;
				_properties[index] = propValue.Key;

				if (property.stringValue == _properties[index])
					_currentIndex = index;
			}
		}
	}
}
