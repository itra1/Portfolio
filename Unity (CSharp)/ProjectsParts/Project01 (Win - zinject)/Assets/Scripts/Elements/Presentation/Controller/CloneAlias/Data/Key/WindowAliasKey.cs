using System.Text;
using Core.Elements.Windows.Base.Data;

namespace Elements.Presentation.Controller.CloneAlias.Data.Key
{
    public readonly struct WindowAliasKey
    {
        public static bool operator ==(in WindowAliasKey first, in WindowAliasKey second) => 
            first.GetHashCode() == second.GetHashCode();
        
        public static bool operator !=(in WindowAliasKey first, in WindowAliasKey second) => 
            !first.Equals(second);
        
        public static WindowAliasKey Get(string cloneAlias, ulong? presentationId, WindowMaterialData material) => 
            new (cloneAlias, presentationId, material);
        
        private readonly string _cloneAlias;
        private readonly ulong? _presentationId;
        private readonly WindowMaterialData _material;
        
        private WindowAliasKey(string cloneAlias, ulong? presentationId, WindowMaterialData material)
        {
            _cloneAlias = cloneAlias;
            _presentationId = presentationId;
            _material = material;
        }
        
        public override bool Equals(object other) => other is WindowAliasKey otherKey && this == otherKey;
        
        public bool Equals(WindowAliasKey other) => this == other;
        
        public override int GetHashCode()
        {
            var hashCode = 0;
            
            if (string.IsNullOrEmpty(_cloneAlias))
                return hashCode;
            
            hashCode = _cloneAlias.GetHashCode();
            
            if (_presentationId != null)
                hashCode ^= _presentationId.GetHashCode();
            
            if (_material != null)
                hashCode ^= _material.GetHashCode();
            
            return hashCode;
        }

        public override string ToString()
        {
            var buffer = new StringBuilder();
			
            buffer.Append('{');
            
            if (!string.IsNullOrEmpty(_cloneAlias))
            {
                buffer.Append($"cloneAlias: {_cloneAlias}, ");
                
                if (_presentationId != null)
                    buffer.Append($"presentationId: {_presentationId}, ");
                
                if (_material != null)
                {
                    buffer.Append("material: {");
                    buffer.Append($"type: {_material.GetType().Name}, id: {_material.Id}");
                    
                    var materialType = _material.MaterialType;
                    var subType = _material.SubType;
                    
                    if (!string.IsNullOrEmpty(materialType))
                        buffer.Append($", materialType: {materialType}");
                
                    if (!string.IsNullOrEmpty(subType))
                        buffer.Append($", subType: {subType}");
                    
                    buffer.Append("}, ");
                }
            }
            
            buffer.Append($"hashCode: {GetHashCode()}");
            
            buffer.Append('}');
			
            return buffer.ToString();
        }
    }
}