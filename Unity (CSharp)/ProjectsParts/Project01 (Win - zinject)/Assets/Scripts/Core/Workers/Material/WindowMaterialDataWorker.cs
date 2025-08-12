using System;
using System.Reflection;
using Core.Elements.Windows.Base.Data;
using Core.Elements.Windows.Base.Data.Attributes;
using Core.Elements.Windows.Browser.Data;
using Core.Elements.Windows.Camera.Data;
using Core.Elements.Windows.Cnp.Data;
using Core.Elements.Windows.Gis.Data;
using Core.Elements.Windows.OfficeDocument.Data.Common.Consts;
using Core.Elements.Windows.OfficeDocument.Data.Microsoft;
using Core.Elements.Windows.OfficeDocument.Data.Web;
using Core.Elements.Windows.Pdf.Data;
using Core.Elements.Windows.Picture.Data;
using Core.Elements.Windows.NSWebView.Data;
using Core.Elements.Windows.Undefined.Data;
using Core.Elements.Windows.VideoFile.Data;
using Core.Elements.Windows.VideoSplit.Data;
using Core.Logging;
using Core.Materials.Data;
using Core.Materials.Parsing;
using Core.Options;
using Leguar.TotalJSON;
using Zenject;

namespace Core.Workers.Material
{
	public class WindowMaterialDataWorker : IDefiningConcreteType, IAfterAddingToStorage
	{
		private const string TypeKey = "type";
		private const string FileKey = "file";
		private const string MimeTypeKeyInLowerCase = "mimetype";
		private const string MimeTypeKeyInCapitalCase = "MimeType";
		private const string ViewKey = "view";
		private const string SubTypeKey = "subType";

		private const string FileTypeValue = "File";
		private const string VideoTypeValue = "Video";
		private const string CameraTypeValue = "Camera";
		private const string KcSystemTypeValue = "KCSystem";
		private const string GisTypeValue = "GIS";
		private const string BrowserTypeValue = "Browser";

		private const string PdfMimeTypeValue = "application/pdf";
		private const string ImageMimeTypeValuePrefix = "image/";
		private const string VideoMimeTypeValuePrefix = "video/";

		private readonly Type _defaultType = typeof(UndefinedMaterialData);

		private IApplicationOptions _options;
		private IMaterialDataParsingHelper _parsingHelper;

		[Inject]
		private void Initialize(IApplicationOptions options, IMaterialDataParsingHelper parsingHelper)
		{
			_options = options;
			_parsingHelper = parsingHelper;
		}

		public Type DefineConcreteTypeFrom(JSON json)
		{
			if (!json.ContainsKey(TypeKey))
				return _defaultType;

			var typeValue = json.GetString(TypeKey);

			if (typeValue == FileTypeValue)
			{
				var mimeType = string.Empty;

				if (json.ContainsKey(FileKey))
				{
					var fileJson = json.GetJSON(FileKey);

					if (fileJson.ContainsKey(MimeTypeKeyInLowerCase))
						mimeType = fileJson.GetString(MimeTypeKeyInLowerCase);
				}

				if (string.IsNullOrEmpty(mimeType))
				{
					if (json.ContainsKey(MimeTypeKeyInLowerCase))
						mimeType = json.GetString(MimeTypeKeyInLowerCase);
					else if (json.ContainsKey(MimeTypeKeyInCapitalCase))
						mimeType = json.GetString(MimeTypeKeyInCapitalCase);
				}

				if (string.IsNullOrEmpty(mimeType))
					return _defaultType;

				var isPdfMimeType = mimeType.Contains(PdfMimeTypeValue);

				if (json.ContainsKey(ViewKey) && !json.IsJNull(ViewKey) && !isPdfMimeType)
				{
					if (_options.IsMSOfficeUsed && json.ContainsKey(SubTypeKey) && !json.IsJNull(SubTypeKey))
					{
						switch (json.GetString(SubTypeKey))
						{
							case OfficeDocumentMaterialSubtype.Word:
								return typeof(MsWordDocumentMaterialData);
							case OfficeDocumentMaterialSubtype.Excel:
								return typeof(MsExcelDocumentMaterialData);
							case OfficeDocumentMaterialSubtype.PowerPoint:
								return typeof(MsPowerPointDocumentMaterialData);
						}
					}
					else if (_options.QtBrowser)
					{
						switch (json.GetString(SubTypeKey))
						{
							case OfficeDocumentMaterialSubtype.Word:
								return typeof(NsWvWordMaterialData);
							case OfficeDocumentMaterialSubtype.Excel:
								return typeof(NsWvExcelMaterialData);
							case OfficeDocumentMaterialSubtype.PowerPoint:
								return typeof(NsWvPowerPointMaterialData);
						}
					}

					return typeof(WebOfficeDocumentMaterialData);
				}

				if (isPdfMimeType)
					return typeof(PdfMaterialData);

				if (mimeType.Contains(ImageMimeTypeValuePrefix))
					return typeof(PictureMaterialData);

				if (mimeType.Contains(VideoMimeTypeValuePrefix))
					return typeof(VideoFileMaterialData);
			}

			if (typeValue == KcSystemTypeValue)
			{
				return _options.QtBrowser
				? typeof(NsWvCnpMaterialData)
				: typeof(CnpMaterialData);
			}

			if (typeValue == GisTypeValue)
			{
				return _options.QtBrowser
				? typeof(NsWvGisMaterialData)
				: typeof(GisMaterialData);
			}

			if (typeValue == BrowserTypeValue)
			{
				return _options.QtBrowser
				? typeof(NsWvBrowserMaterialData)
				: typeof(BrowserMaterialData);
			}

			if (typeValue == CameraTypeValue)
				return typeof(CameraMaterialData);

			if (typeValue == VideoTypeValue)
				return typeof(VideoSplitMaterialData);

			return _defaultType;
		}

		public virtual void PerformActionAfterAddingToStorageOf(MaterialData material)
		{
			if (material is not WindowMaterialData windowMaterial)
				return;

			var statesJson = windowMaterial.StatesJson;

			if (statesJson == null)
			{
				windowMaterial.States = Array.Empty<WindowState>();
				return;
			}

			var statesCount = statesJson.Length;

			var states = new WindowState[statesCount];

			for (var i = 0; i < statesCount; i++)
			{
				var materialType = windowMaterial.GetType();
				var attribute = materialType.GetCustomAttribute<WindowStateAttribute>();

				var stateJson = statesJson[i];

				var state = attribute == null
						? _parsingHelper.Parse<WindowState>(stateJson)
						: _parsingHelper.Parse(attribute.Type, stateJson) as WindowState;

				if (state == null)
				{
					Debug.LogError($"Failed to parse state \"{stateJson}\" for window material {windowMaterial}");
					continue;
				}

				states[i] = state;
			}

			windowMaterial.States = states;
		}
	}
}