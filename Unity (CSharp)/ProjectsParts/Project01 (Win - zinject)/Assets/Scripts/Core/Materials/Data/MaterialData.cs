using System.Collections.Generic;
using System.Text;
using Core.Materials.Attributes;
using Core.Messages;
using Core.Network.Socket.Attributes;

namespace Core.Materials.Data
{
	/// <summary>
	/// Устаревшее название - "DataMaterial"
	/// </summary>
	public abstract class MaterialData
	{
		[MaterialDataPropertyParse("id"), SocketProperty(null, "Id", "Идентификатор")]
		public ulong Id { get; set; }
		
		[MaterialDataPropertyParse("name"), MaterialDataPropertyUpdate]
		public string Name { get; set; }
		
		[MaterialDataPropertyParse("description"), MaterialDataPropertyUpdate]
		public string Description { get; set; }
		
		[MaterialDataPropertyParse("subType"), MaterialDataPropertyUpdate]
		public string SubType { get; set; }
		
		[MaterialDataPropertyParse("materialType"), MaterialDataPropertyUpdate]
		public string MaterialType { get; set; }

		[MaterialDataPropertyParse("tags"), MaterialDataPropertyUpdate]
		public List<TagMaterialData> Tags { get; set; }
		
		public string Category { get; protected set; } //Устаревшее имя свойства - "System"
		
		public string Model { get; protected set; }
		
		public string Type { get; set; }
		
		public virtual string UpdateMessageType => MessageType.MaterialDataUpdate;
		
		public override string ToString()
		{
			var buffer = new StringBuilder();
			
			buffer.Append('{');
			buffer.Append($"type: {GetType().Name}, id: {Id}");
			
			if (!string.IsNullOrEmpty(Category))
				buffer.Append($", category: {Category}");
			
			if (!string.IsNullOrEmpty(Model))
				buffer.Append($", model: {Model}");
			
			if (!string.IsNullOrEmpty(Name))
				buffer.Append($", name: {Name}");
			
			buffer.Append('}');
			
			return buffer.ToString();
		}
	}
}