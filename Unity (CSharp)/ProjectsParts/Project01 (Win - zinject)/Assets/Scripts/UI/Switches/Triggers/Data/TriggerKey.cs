using System;
using System.Text;
using UI.Switches.Triggers.Data.Enums;
using UnityEngine;

namespace UI.Switches.Triggers.Data
{
    [Serializable]
    public struct TriggerKey
    {
        public static bool operator ==(in TriggerKey first, in TriggerKey second) => 
            first.GetHashCode() == second.GetHashCode();
        
        public static bool operator !=(in TriggerKey first, in TriggerKey second) => 
            !first.Equals(second);
        
        [SerializeField] private TriggerType _type;
        [SerializeField] private Component _target;
        
        public TriggerType Type => _type;
        public Component Target => _target;
        
        public TriggerKey(TriggerType type)
        {
            _type = type;
            _target = null;
        }
        
        public override bool Equals(object other) => other is TriggerKey otherKey && this == otherKey;
        
        public readonly bool Equals(TriggerKey other) => this == other;

        public override int GetHashCode()
        {
            var hashCode = _type.GetHashCode();
            
            if (_target == null)
                return hashCode;
            
            hashCode ^= _target.GetHashCode();
            
            return hashCode;
        }

        public override string ToString()
        {
            var buffer = new StringBuilder();
			
            buffer.Append('{');
            buffer.Append($"type: {_type}, ");
            
            if (_target != null)
                buffer.Append($"target: {_target.GetType().Name}, ");
            
            buffer.Append($"hashCode: {GetHashCode()}");
            buffer.Append('}');
			
            return buffer.ToString();
        }
    }
}