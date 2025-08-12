using System;
using System.Collections;

namespace Core.Materials.Storage.Utils
{
    public static class MaterialDataPropertyHelper
    {
        public static bool IsDeeplyEquals(object first, object second)
        {
            if (first == null && second == null)
                return true;
            
            if (first == null || second == null) 
                return false;
            
            if (first is ICollection firstCollection && second is ICollection secondCollection)
            {
                IEnumerator firstEnumerator = null;
                IEnumerator secondEnumerator = null;
                
                try
                {
                    firstEnumerator = firstCollection.GetEnumerator();
                    secondEnumerator = secondCollection.GetEnumerator();
                    
                    while (firstEnumerator.MoveNext())
                    {
                        if (!secondEnumerator.MoveNext())
                            return false;
                        
                        var firstItem = firstEnumerator.Current;
                        var secondItem = secondEnumerator.Current;
                        
                        if (!IsDeeplyEquals(firstItem, secondItem))
                            return false;
                    }
                    
                    return !secondEnumerator.MoveNext();
                }
                finally
                {
                    if (firstEnumerator is IDisposable firstDisposableEnumerator)
                        firstDisposableEnumerator.Dispose();
                    
                    if (secondEnumerator is IDisposable secondDisposableEnumerator)
                        secondDisposableEnumerator.Dispose();
                }
            }
            
            return first.Equals(second);
        }
    }
}