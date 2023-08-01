using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Custom.Interfaces
{
    public interface ITeam
    {
        public int Team { get; set; }
    }
    public interface ISelectable : ITeam
    {
        SpriteRenderer spriteRenderer { get; set; }
        public void SetColour(Color colour)
        {
            spriteRenderer.color = colour;
        }
    }
    //TODO: add damage type
    public interface IWeapon : ITeam
    {
        float DamagePerHit { get; set; }
        float CooldownSeconds { get; set; }
        float NextAttackTime { get; set; }
        float Range { get; set; }
        internal protected bool TryHit(IDamagable damagable)
        {
            if (damagable.Team == Team)
            {
                return false;
            }
            if (Random.value <= damagable.Evasion)
            {
                return false;
            }
            damagable.Health -= DamagePerHit;
            if (damagable.Health <= 0 )
            {
                damagable.Destroy();
            }
            NextAttackTime = CooldownSeconds + Time.time;
            return true;
        }
    }
    public interface IDamagable : ITeam
    {
        /// <summary>
        /// Evasion of a ship between a value of 0 and 1
        /// </summary>
        float Evasion { get; set; }
        float Health { get; set; }
        public void Destroy();
    }
}