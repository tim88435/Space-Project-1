using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Custom.Interfaces
{
    public interface ITeam
    {
        int Team { get; protected set; }
    }
    public interface ISelectable
    {
        SpriteRenderer spriteRenderer { get; set; }
        public void SetColour(Color colour)
        {
            spriteRenderer.color = colour;
        }
    }
}