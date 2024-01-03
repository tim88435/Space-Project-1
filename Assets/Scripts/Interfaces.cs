using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Custom.Interfaces
{
    public interface ITeam
    {
        public int TeamID { get; set; }
        public ITeamController teamController
        {
            get
            {
                ITeamController.teamControllers.TryGetValue(TeamID, out ITeamController teamController);
                return teamController;
            }
        }
        public Dictionary<ResourceType, float> Resources
        {
            get
            {
                if (ITeamController.teamControllers.TryGetValue(TeamID, out ITeamController teamController))
                {
                    return teamController.resources;
                }
                return null;
            }
        }
    }
    public interface IColourable : ITeam
    {
        protected internal SpriteRenderer SpriteRenderer { get; set; }
        public void SetColour(Color colour)
        {
            SpriteRenderer.color = colour;
        }
    }
    //TODO: add damage type
    public interface IWeapon
    {
        float DamagePerHit { get; set; }
        float CooldownSeconds { get; set; }
        float NextAttackTime { get; set; }
        float Range { get; set; }
    }
    public interface IDamagable : ITeam//not used much?
    {
        public float MaxHealth { get; set; }
        public float Health { get; set; }
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
        public void SetEdgeAngle(float planetDiameter)
        {
            edgeAngle = Mathf.Asin(transform.lossyScale.x / planetDiameter) * Mathf.Rad2Deg;
        }
        void Place(Planet planet, float angle)
        {
            Vector3 buildingPositionFromPlanet = Quaternion.Euler(Vector3.forward * angle) * Vector3.up * planet.ZoneDistanceFromPlanetCentre(transform.lossyScale.x);
            transform.position = planet.transform.position + buildingPositionFromPlanet;
            transform.rotation = Quaternion.Euler(Vector3.forward * angle);
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
    public interface IHoverableUI : IHoverable, IPointerEnterHandler, IPointerExitHandler
    {
        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            HoverObject.hoveredOver.Add(this);
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            HoverObject.hoveredOver.Remove(this);
        }
    }
    public interface IHoverable
    {
        string Name { get; }
        string Description { get; }
        string GetHoverText()// TODO: test this
        {
            string a = Name == string.Empty ? "" : $"<b>{   Name}</b>";
            a += Description == string.Empty ? "" : $"\n{Description}";
            a = a == string.Empty ? "" : $"   {a}";//three spaces for mouse
            return a;
        }
    }
    public interface ITeamController : ITeam
    {
        public static Dictionary<int, ITeamController> teamControllers = new Dictionary<int, ITeamController>();
        public Dictionary<ResourceType, float> resources { get; set; }
        public static Dictionary<ResourceType, float> GetNewResourceList()
        {
            Dictionary<ResourceType, float> newList = new Dictionary<ResourceType, float>();
            ResourceType[] resourceTypes = UnityEngine.Resources.LoadAll<ResourceType>("GameResources");
            for (int i = 0; i < resourceTypes.Length; i++)
            {
                newList.Add(resourceTypes[i], 0);
            }
            return newList;
        }
    }
}