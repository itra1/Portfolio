using System.Collections.Generic;
using Core.Materials.Data;
using Core.Materials.Parsing;
using Core.Workers.Material.Coordinator;
using Leguar.TotalJSON;
using Zenject;

namespace Core.Elements.Base.Data.Parsing
{
    public abstract class ElementMaterialDataParserBase
    {
        private IMaterialDataParsingHelper _parsingHelper;
        private IMaterialWorkerCoordinator _workerCoordinator;
        
        [Inject]
        private void Initialize(IMaterialDataParsingHelper parsingHelper, IMaterialWorkerCoordinator workerCoordinator)
        {
            _parsingHelper = parsingHelper;
            _workerCoordinator = workerCoordinator;
        }
        
        protected TMaterialData ParseMaterial<TMaterialData>(JSON json) where TMaterialData : MaterialData
        {
            return _parsingHelper.ParseClass<TMaterialData>(json);
        }

        protected IEnumerable<TMaterialData> ParseMaterial<TMaterialData>(JArray jArray) where TMaterialData : MaterialData
        {
            var jArrayLength = jArray.Length;
            var materials = new TMaterialData[jArrayLength];
            
            for (var i = 0; i < jArrayLength; i++)
                materials[i] = _parsingHelper.ParseClass<TMaterialData>(jArray.GetJSON(i));
            
            return materials;
        }
        
        protected IEnumerable<TMaterialData> ParseMaterial<TMaterialData>(string key, JArray jArray) where TMaterialData : MaterialData
        {
            var jArrayLength = jArray.Length;
            var materials = new TMaterialData[jArrayLength];
            
            for (var i = 0; i < jArrayLength; i++)
                materials[i] = _parsingHelper.ParseClass<TMaterialData>(jArray.GetJSON(i).GetJSON(key));
            
            return materials;
        }
        
        protected MaterialData ParseMaterialBasedOn<TMaterialDataBase>(JSON json) where TMaterialDataBase : MaterialData
        {
            var typeBase = typeof(TMaterialDataBase);
            var concreteType = _workerCoordinator.DefineConcreteTypeFrom(typeBase, json);
            return (MaterialData) _parsingHelper.ParseClass(concreteType, json);
        }
        
        protected IEnumerable<MaterialData> ParseMaterialBasedOn<TMaterialDataBase>(JArray jArray) where TMaterialDataBase : MaterialData
        {
            var jArrayLength = jArray.Length;
            var materials = new MaterialData[jArrayLength];
            var typeBase = typeof(TMaterialDataBase);
            
            for (var i = 0; i < jArrayLength; i++)
            {
                var materialJson = jArray.GetJSON(i);
                var concreteType = _workerCoordinator.DefineConcreteTypeFrom(typeBase, materialJson);
                materials[i] = (MaterialData) _parsingHelper.ParseClass(concreteType, materialJson);
            }
            
            return materials;
        }
    }
}