using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Custom.Interfaces
{
    public interface ITeam
    {
        public int Team { get; set; }
        public float MaxHealth { get; set; }
        public float Health { get; set; }
    }
    public interface IShip : ITeam
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
    }
    public interface IDamagable : ITeam//not used much?
    {
        /// <summary>
        /// Evasion of a ship between a value of 0 and 1
        /// </summary>
        float Evasion { get; set; }
        public void Destroy();
        public bool TryHitWith(IWeapon weapon)
        {
            weapon.NextAttackTime = weapon.CooldownSeconds + Time.time;
            if (Random.value <= Evasion)
            {
                return false;
            }
            Health -= weapon.DamagePerHit;
            if (Health <= 0)
            {
                Destroy();
            }
            return true;
        }
        public void DealDirectDamage(float damage)
        {
            Health -= damage;
            if (Health <= 0)
            {
                Destroy();
            }
        }
    }
    public interface IPlanetAngle
    {
        float edgeAngle { get; set; }
        public Transform transform { get; }//what? this works????
        public void SetEdgeAngle(float buildingWidth, float planetRadius)
        {
            edgeAngle = Mathf.Asin(buildingWidth / 2 / planetRadius) * Mathf.Rad2Deg;
        }
        public bool IsIntersecting(IPlanetAngle other)
        {
            return IsIntersecting(other.transform.rotation, other.edgeAngle);
        }
        public bool IsIntersecting(Quaternion otherRotation, float otherEdgeAngle)
        {
            return Quaternion.Angle(transform.rotation, otherRotation) < edgeAngle + otherEdgeAngle;
        }
    }
    public interface IHoverable : IPointerEnterHandler, IPointerExitHandler
    {
        string Name { get; }
        string Description { get; }
        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            UIManager.Singleton.hoveredOver.Add(this);
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            UIManager.Singleton.hoveredOver.Remove(this);
        }
        string GetHoverText()
        {
            string a = $"   <b>{Name}</b>\n";//three spaces for mouse
            a += Description;
            return a;
        }
    }
}