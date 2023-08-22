using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Custom.Interfaces;

namespace Custom.Extensions
{
    public static class Extensions
    {
        public static void Clamp2D(ref this Vector3 self, Vector2 xClamp, Vector2 yClamp)
        {
            self.x = Mathf.Clamp(self.x, xClamp.x, xClamp.y);
            self.y = Mathf.Clamp(self.y, yClamp.x, yClamp.y);
        }
        public static Vector3 Flatten2D(this Vector3 self)
        {
            self.z = 0;
            return self;
        }
        public static void SetColour(this List<FlockAgent> flockAgents, Color color)
        {
            for (int i = 0; i < flockAgents.Count; i++)
            {
                ((IShip)flockAgents[i]).SetColour(color);
            }
        }
        public static void SetColour(this IEnumerable<FlockAgent> flockAgents, Color color)
        {
            foreach (FlockAgent agent in flockAgents)
            {
                ((IShip)agent).SetColour(color);
            }
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