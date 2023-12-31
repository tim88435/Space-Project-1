using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Custom.Interfaces;

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
        public static T AbsHighest<T>(params T[] input) where T : IConvertible
        {
            if (input.Length == 0)
            {
                throw new InvalidOperationException("input is empty");
            }
            T highest = input[0];
            float absHighestFloat = Mathf.Abs(Convert.ToSingle(input[0]));
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
            T lowest = input[0];
            float absLowestFloat = Mathf.Abs(Convert.ToSingle(input[0]));
            //starts from one!!
            for (int i = 1; i < input.Length; i++)
            {
                float current = Mathf.Abs(Convert.ToSingle(input[i]));
                if (absLowestFloat.CompareTo(current) > 0)
                {
                    lowest = input[i];
                    absLowestFloat = current;
                }
            }
            return lowest;
        }
        public static Vector3 ToEuler(this Vector3 input)
        {
            return Quaternion.LookRotation(Vector3.forward, input).eulerAngles;
        }
        //minby for linq is here since we're using .net 7 and not .net 8
        //can remove once .net 8 is out for .net standard (2.2?)
        public static TSource MinBy<TSource, TKey>(this IEnumerable<TSource> source,
        Func<TSource, TKey> selector)
        {
            return source.MinBy(selector, null);
        }
        public static TSource MinBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector, IComparer<TKey> comparer)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (selector == null) throw new ArgumentNullException("selector");
            comparer ??= Comparer<TKey>.Default;

            using (var sourceIterator = source.GetEnumerator())
            {
                if (!sourceIterator.MoveNext())
                {
                    return default;
                    throw new InvalidOperationException("Sequence contains no elements");
                }
                var min = sourceIterator.Current;
                var minKey = selector(min);
                while (sourceIterator.MoveNext())
                {
                    var candidate = sourceIterator.Current;
                    var candidateProjected = selector(candidate);
                    if (comparer.Compare(candidateProjected, minKey) < 0)
                    {
                        min = candidate;
                        minKey = candidateProjected;
                    }
                }
                return min;
            }
        }//minby for linq is here since we're using .net 7 and not .net 8
         //can remove once .net 8 is out for .net standard (2.2?)
        public static TSource MaxBy<TSource, TKey>(this IEnumerable<TSource> source,
        Func<TSource, TKey> selector)
        {
            return source.MaxBy(selector, null);
        }
        public static TSource MaxBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector, IComparer<TKey> comparer)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (selector == null) throw new ArgumentNullException("selector");
            comparer ??= Comparer<TKey>.Default;

            using (var sourceIterator = source.GetEnumerator())
            {
                if (!sourceIterator.MoveNext())
                {
                    return default;
                    throw new InvalidOperationException("Sequence contains no elements");
                }
                var max = sourceIterator.Current;
                var maxKey = selector(max);
                while (sourceIterator.MoveNext())
                {
                    var candidate = sourceIterator.Current;
                    var candidateProjected = selector(candidate);
                    if (comparer.Compare(candidateProjected, maxKey) > 0)
                    {
                        max = candidate;
                        maxKey = candidateProjected;
                    }
                }
                return max;
            }
        }
    }
}