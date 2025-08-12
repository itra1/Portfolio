#if UNITY_ANDROID

using System.Xml;
using UnityEngine;
using VoxelBusters.Utility;

// Генератор главного androidmanifest файла

public class MainAndroidManifestGenerator : AndroidManifestGenerator {

	bool greeEnabled;
	
	
	public MainAndroidManifestGenerator(bool greeEnabled) {
		this.greeEnabled = greeEnabled;
	}
	
	protected override void WriteApplicationProperties(XmlWriter _xmlWriter) {
		base.WriteApplicationProperties(_xmlWriter);

		

		_xmlWriter.WriteStartElement("activity");
		{
			_xmlWriter.WriteAttributeString("android:name", "com.unity3d.player.UnityPlayerNativeActivity");
			_xmlWriter.WriteAttributeString("android:configChanges", "keyboardHidden|orientation");
			_xmlWriter.WriteAttributeString("android:label", "@string/app_name");

			WriteInnerFilter(_xmlWriter, "android.intent.action.MAIN", "android.intent.category.LAUNCHER");
			string[] cats = new[] {
				"android.intent.category.DEFAULT",
				"android.intent.category.BROWSABLE"
			};
			if (greeEnabled)
			{
				WriteInnerFilter(_xmlWriter, "android.intent.action.VIEW", cats, "@string/app_bundle", "host");

				
			}



			WriteInnerFilter(_xmlWriter, "android.intent.action.VIEW", cats, "@string/app_bundle");

			if (greeEnabled) {
				_xmlWriter.WriteStartElement("meta-data");
				{
					_xmlWriter.WriteAttributeString("android:name", "unityplayer.ForwardNativeEventsToDalvik");
					_xmlWriter.WriteAttributeString("android:value", "true");
					_xmlWriter.WriteEndElement();
				}
			}

			_xmlWriter.WriteEndElement();

		}
	}

	protected override void WritePermissions(XmlWriter _xmlWriter) {
		base.WritePermissions(_xmlWriter);

		if (greeEnabled) {
			WriteUsesPermission(_xmlWriter: _xmlWriter, _name: "android.permission.ACCESS_NETWORK_STATE");
			WriteUsesPermission(_xmlWriter: _xmlWriter, _name: "android.permission.INTERNET");
		}

		_xmlWriter.WriteStartElement("uses-sdk");
		{
			_xmlWriter.WriteAttributeString("android:minSdkVersion", "15");
			_xmlWriter.WriteEndElement();
		}

	}




	void WriteInnerFilter(XmlWriter _xmlWriter, string actionName, string category = null, string scheme = null, string host = null) {
		WriteInnerFilter(_xmlWriter,actionName,new string[] {category},scheme,host);
	}


	void WriteInnerFilter(XmlWriter _xmlWriter, string actionName=null, string[] category=null, string scheme = null, string host=null) {
		_xmlWriter.WriteStartElement("intent-filter");
		{
			if (actionName != null) {

				_xmlWriter.WriteStartElement("action");
				{
					_xmlWriter.WriteAttributeString("android:name", actionName);
				}
				_xmlWriter.WriteEndElement();

			}
			if (category != null && category.Length > 0) {
				for (int i = 0; i < category.Length; i++) {
					_xmlWriter.WriteStartElement("category");
					{
						_xmlWriter.WriteAttributeString("android:name", category[i]);
					}
					_xmlWriter.WriteEndElement();
				}
			}


			if (scheme != null) {
				_xmlWriter.WriteStartElement("data");
				{
					_xmlWriter.WriteAttributeString("android:scheme", scheme);
					if (host != null)
						_xmlWriter.WriteAttributeString("android:host", host);
				}
				_xmlWriter.WriteEndElement();
			}
		}
		_xmlWriter.WriteEndElement();
	}

}
#endif