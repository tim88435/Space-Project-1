using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Resource
{
    public struct Type
    {
        public string name;
        public float value;
        public static bool operator ==(Type c1, Type c2)
        {
            return c1.name == c2.name;
        }
        public static bool operator !=(Type c1, Type c2)
        {
            return c1.name != c2.name;
        }
    }
    public static Type Rocks = new Type() { name = "Rocks", value = 0 };
    public static Type Durasteel = new Type() { name = "Durasteel", value = 0 };
}
