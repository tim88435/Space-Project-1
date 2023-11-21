using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Custom.Interfaces;
using UnityEditor;

namespace Custom
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
        public static T AbsHighest<T>(params T[] input) where T : IConvertible
        {
            if (input.Length == 0)
            {
                throw new InvalidOperationException("input is empty");
            }
            T highest = input[0];
            float absHighestFloat = Convert.ToSingle(input[0]);
            //starts from one!!
            for (int i = 1; i < input.Length; i++)
            {
                float current = Mathf.Abs(Convert.ToSingle(input[i]));
                if (absHighestFloat.CompareTo(current) < 0)
                {
                    highest = input[i];
                    absHighestFloat = current;
                }
            }
            return highest;
        }
        public static T AbsLowest<T>(params T[] input) where T : IConvertible
        {
            if (input.Length == 0)
            {
                throw new InvalidOperationException("input is empty");
            }
            T highest = input[0];
            float absHighestFloat = Convert.ToSingle(input[0]);
            //starts from one!!
            for (int i = 1; i < input.Length; i++)
            {
                float current = Mathf.Abs(Convert.ToSingle(input[i]));
                if (absHighestFloat.CompareTo(current) > 0)
                {
                    highest = input[i];
                    absHighestFloat = current;
                }
            }
            return highest;
        }
    }
}