﻿/*using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SpawningSystem
{
    [ExecuteInEditMode]
    public class ObjectSpawner : MonoBehaviour
    {
        //[Header("Enemies & Spawn Rate (%)")]
        //[Tooltip("The pool of objects that will spawn")]
        public List<GameObject> objects = new List<GameObject>(1);
        //[Tooltip("The spawn rate of the objects. Follows the same order as the objects are put (e.g first object on the above list has the first spawn rate percentage put in this list) ")]
        //[Space]
        //[Header("The objects correspond to the percentages in the same order.")]
        //[Header("The percentages need to sum up 100!")]
        //[SerializeField]
        public int[] percentages;
        //[Space]
        
        //[Header("Area Range")]
        //[Tooltip("This is the range of the area in which objects will spawn")]
        //[SerializeField]
        public int range;

        //[Header("Number of objects")]
        //[Tooltip("Sets the maximum number of objects that will spawn")]
        //[SerializeField]
        public int numberOfObjects;
        
        //[Header("Time between spawns - Seconds")]
        //[Tooltip("The minimum and maximum values of time between spawns")]
        //[SerializeField]
        public int minTime;
        //[SerializeField]
        public int maxTime;

        //[Header("Respawn time after an object is destroyed - Seconds")]
        //[Tooltip("The time that needs to pass after an object is destroyed to respawn another")]
        //[SerializeField]
        public int respawnTime;
        
        //[Space]
        //[Header("Area Color")]
        //[Tooltip("Customize the highlight color in the scene editor that limits the spawn")]
        //[SerializeField]
        //private Color areaColor;
        

        public int MinTime
        {
            get => minTime;
        }

        public int MaxTime
        {
            get => maxTime;
        }

        public int MaxRange
        {
            get => range;
            set => range = value;
        }
        public int[] Percentages()
        {
            return percentages;
        }
        
        

        /*#region Errors

        private void ExceedEnemyNumber()
        {
            if (NumberOfObjects >= GetMaxNumberOfSpawns())
            {
                Debug.LogError("Maximum number of enemies for this spawn area exceeded! Please add less than " + GetMaxNumberOfSpawns() + " enemies");
            }
        }

        private void NegativeArea()
        {
            if (range < 1)
            {
                Debug.LogError("Please use only positive values for the spawning area!");
            }
        }

        private void NegativeEnemies()
        {
            if (numberOfObjects < 0)
            {
                Debug.LogError("You cannot spawn a negative number of enemies!");
            }
        }

        #endregion#1#
        

        private void Update() 
        {
           /*ExceedEnemyNumber();
           NegativeArea();
           NegativeEnemies();#1#
        }
        
        private void OnValidate()
        {
            MaxRange = range;

            //type the magic sync here
            /*if (enemies.Count - percentages.Count > 0)
            {
                for (int i = 1; i <= Mathf.Abs(enemies.Count - percentages.Count); i++)
                {
                    percentages.Add(0);
                }
            } 
            if (enemies.Count - percentages.Count < 0)
            {
                for (int i = 1; i <= Mathf.Abs(enemies.Count - percentages.Count) ; i++)
                {
                    percentages.Remove(percentages.Last());
                }
            }
            Debug.Log(enemies.Count - percentages.Count);#1#

            if (percentages.Length != objects.Count)
            {
                percentages = new int[objects.Capacity];
            }
        }
        
        /*public float GetMaxNumberOfSpawns()
        {
            float area = Mathf.Pow(range * 2, 2) - Mathf.Pow(2, 2);
            //one third of the area can be populated only to avoid CPU burn
            float maxEnemies = area / 3;
            return maxEnemies;
        }#1#
        
        /*#region Gizmos
        private void OnDrawGizmos()
        {
            Gizmos.color = areaColor;
            Gizmos.DrawWireCube(transform.position, new Vector3 (range * 2,0, range * 2));
        }
        #endregion#1#
    }
}*/