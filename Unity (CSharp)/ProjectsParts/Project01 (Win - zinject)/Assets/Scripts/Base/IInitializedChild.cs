using UnityEngine;

namespace Base
{
    public interface IInitializedChild<in TTransform> where TTransform : Transform
    {
        void SetParentOnInitialize(TTransform parent);
    }
}