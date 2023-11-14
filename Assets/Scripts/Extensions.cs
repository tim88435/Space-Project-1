using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Custom.Interfaces;
using UnityEditor;
using UnityEditor.VersionControl;

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
        public static List<FlockAgent> SetColour(this List<FlockAgent> flockAgents, Color color)
        {
            for (int i = 0; i < flockAgents.Count; i++)
            {
                ((IColourable)flockAgents[i]).SetColour(color);
            }
            return flockAgents;
        }
        public static IEnumerable<FlockAgent> SetColour(this IEnumerable<FlockAgent> flockAgents, Color color)
        {
            foreach (FlockAgent agent in flockAgents)
            {
                ((IColourable)agent).SetColour(color);
            }
            return flockAgents;
        }
        public static List<FlockAgent> ShowHealthBar(this List<FlockAgent> flockAgents, bool shouldShow)
        {
            for (int i = 0; i < flockAgents.Count; i++)
            {
                flockAgents[i].ShowHealthBar(shouldShow);
            }
            return flockAgents;
        }
        public static IEnumerable<FlockAgent> ShowHealthBar(this IEnumerable<FlockAgent> flockAgents, bool shouldShow)
        {
            foreach (FlockAgent agent in flockAgents)
            {
                agent.ShowHealthBar(shouldShow);
            }
            return flockAgents;
        }
        public static List<T> FindAssetsByType<T>() where T : UnityEngine.Object
        {
            List<T> assets = new List<T>();

            string[] guids = AssetDatabase.FindAssets("t:" + typeof(T).Name);

            for (int i = 0; i < guids.Length; i++)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
                T asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);

                if (asset != null)
                {
                    assets.Add(asset);
                }
            }
            return assets;
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