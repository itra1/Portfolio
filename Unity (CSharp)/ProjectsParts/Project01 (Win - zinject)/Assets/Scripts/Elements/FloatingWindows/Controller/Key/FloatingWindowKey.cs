namespace Elements.FloatingWindows.Controller.Key
{
    public readonly struct FloatingWindowKey
    {
        public static bool operator ==(in FloatingWindowKey first, in FloatingWindowKey second) => 
            first.GetHashCode() == second.GetHashCode();
        
        public static bool operator !=(in FloatingWindowKey first, in FloatingWindowKey second) => 
            !first.Equals(second);
        
        public static FloatingWindowKey Get(string tag, ulong materialId) => new (tag, materialId);
        
        private readonly string _tag;
        
        public ulong MaterialId { get; }

        private FloatingWindowKey(string tag, ulong materialId)
        {
            _tag = tag;
            MaterialId = materialId;
        }
        
        public override bool Equals(object other) => other is FloatingWindowKey otherKey && this == otherKey;
        
        public bool Equals(FloatingWindowKey other) => this == other;

        public override int GetHashCode()
        {
            var hashCode = 0;
            
            if (string.IsNullOrEmpty(_tag))
                return hashCode;
            
            hashCode = _tag.GetHashCode() ^ MaterialId.GetHashCode();
            
            return hashCode;
        }
        
        public override string ToString() => 
            $"{{tag: {_tag}, materialId: {MaterialId}, hashCode: {GetHashCode()}}}";
    }
}