using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Custom.Extensions
{
    public static class Extensions
    {
        public static void Clamp2D(ref this Vector3 self, Vector2 xClamp, Vector2 yClamp)
        {
            self.x = Mathf.Clamp(self.x, xClamp.x, xClamp.y);
            self.y = Mathf.Clamp(self.y, yClamp.x, yClamp.y);
        }
    }
    public class GenericListComparer<T>
    {
        internal static bool Compare(List<T> firstCollection, List<T> secondCollection)
        {
            return firstCollection.TrueForAll(secondCollection.Contains) &&
                   secondCollection.TrueForAll(firstCollection.Contains);
        }
    }
}