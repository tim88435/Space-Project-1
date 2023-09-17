/*
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
namespace InfallibleCode.Completed//TODO: add this to the ships
{
    public class BuildingManager : MonoBehaviour
    {
        [SerializeField] private List<Building> buildings;

        private BuildingUpdateJob _job;
        private NativeArray<Building.Data> _buildingDataArray;

        private void Awake()
        {
            var buildingData = new Building.Data[buildings.Count];
            for (var i = 0; i < buildingData.Length; i++)
            {
                buildingData[i] = new Building.Data(buildings[i]);
            }

            _buildingDataArray = new NativeArray<Building.Data>(buildingData, Allocator.Persistent);

            _job = new BuildingUpdateJob
            {
                BuildingDataArray = _buildingDataArray
            };
        }

        private void Update()
        {
            var jobHandle = _job.Schedule(buildings.Count, 1);
            jobHandle.Complete();
        }

        private void OnDestroy()
        {
            _buildingDataArray.Dispose();
        }
    }
    public struct BuildingUpdateJob : IJobParallelFor
    {
        public NativeArray<Building.Data> BuildingDataArray;

        public void Execute(int index)
        {
            var data = BuildingDataArray[index];
            data.Update();
            BuildingDataArray[index] = data;
        }
    }
    public class Building : MonoBehaviour
    {
        [SerializeField] private int floors;

        public struct Data
        {
            private int _tenants;
            
            public int PowerUsage { get; private set; }

            private Unity.Mathematics.Random _random;

            public Data(Building building)
            {
                _random = new Unity.Mathematics.Random(1);
                _tenants = building.floors * _random.NextInt(20, 500);
                PowerUsage = 0;
            }

            public void Update()
            {
                var random = new Unity.Mathematics.Random(1);
                for (var i = 0; i < _tenants; i++)
                {
                    PowerUsage += random.NextInt(12, 24);
                }
            }
        }
    }
}
*/