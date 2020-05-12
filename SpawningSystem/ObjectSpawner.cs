
//this class was initially used to serialize all the variables in the inspector
//it was later replaced by the editor window functionality
//uncomment for better read

/*
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace SpawningSystem
{
    [ExecuteInEditMode]
    public class ObjectSpawner : MonoBehaviour
    {
        
        //using tooltips, spaces and headers the inspector serialized
        //fields looks more organized and intuitive
        [Header("Enemies & Spawn Rate (%)")]
        [Tooltip("The pool of objects that will spawn")]
        public List<GameObject> objects = new List<GameObject>(1);

        [Tooltip(
            "The spawn rate of the objects. Follows the same order as the objects are put (e.g first object on the above list has the first spawn rate percentage put in this list) ")]
        [Space]
        [Header("The objects correspond to the percentages in the same order.")]
        [Header("The percentages need to sum up 100!")]
        [SerializeField]
        public int[] percentages;
        [Space]
        
        [Header("Area Range")]
        [Tooltip("This is the range of the area in which objects will spawn")]
        [SerializeField]
        public int range;

        [Header("Number of objects")]
        [Tooltip("Sets the maximum number of objects that will spawn")]
        [SerializeField]
        public int numberOfObjects;
        
        [Header("Time between spawns - Seconds")]
        [Tooltip("The minimum and maximum values of time between spawns")]
        [SerializeField]
        public int minTime;
        [SerializeField]
        public int maxTime;

        [Header("Respawn time after an object is destroyed - Seconds")]
        [Tooltip("The time that needs to pass after an object is destroyed to respawn another")]
        [SerializeField]
        public int respawnTime;
        
        [Space]
        [Header("Area Color")]
        [Tooltip("Customize the highlight color in the scene editor that limits the spawn")]
        [SerializeField]
        private Color areaColor;
        

        //getters and setters
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
        
        

        //this region handles methods which create console errors
        //that might occur in case invalid data was introduced in the inspector fields
        
        #region Errors

        //too many enemies
        private void ExceedEnemyNumber()
        {
            if (numberOfObjects >= GetMaxNumberOfSpawns())
            {
                Debug.LogError("Maximum number of enemies for this spawn area exceeded! Please add less than " + GetMaxNumberOfSpawns() + " enemies");
            }
        }

        //negative area size
        private void NegativeArea()
        {
            if (range < 1)
            {
                Debug.LogError("Please use only positive values for the spawning area!");
            }
        }

        //negative number of enemies
        private void NegativeEnemies()
        {
            if (numberOfObjects < 0)
            {
                Debug.LogError("You cannot spawn a negative number of enemies!");
            }
        }

        #endregion
        

        private void Update() 
        {
            //since the script is executed in edit mode, the error methods should be called in Update
           ExceedEnemyNumber();
           NegativeArea();
           NegativeEnemies();
        }
        
        //OnValidate is called each time a value is changed in the inspector
        //this helps keeping track of the arrays sync
        private void OnValidate()
        {
            MaxRange = range;

            //first attempt to sync the objects list with percentages list
            
            /*if (objects.Count - percentages.Count > 0)
            {
                for (int i = 1; i <= Mathf.Abs(objects.Count - percentages.Count); i++)
                {
                    percentages.Add(0);
                }
            } 
            if (objects.Count - percentages.Count < 0)
            {
                for (int i = 1; i <= Mathf.Abs(objects.Count - percentages.Count) ; i++)
                {
                    percentages.Remove(percentages.Last());
                }
            }

            //second attempt to sync the objects list with the percentages array
            if (percentages.Length != objects.Count)
            {
                percentages = new int[objects.Capacity];
            }
        }
        
        
        //this method limits the maximum number of spawns to preserve processing power
        public float GetMaxNumberOfSpawns()
        {
            float area = Mathf.Pow(range * 2, 2) - Mathf.Pow(2, 2);
            float maxEnemies = area / 3;
            return maxEnemies;
        }
        
        // Area color drawn on the scene using Gizmos
        #region Gizmos
        private void OnDrawGizmos()
        {
            Gizmos.color = areaColor;
            Gizmos.DrawWireCube(transform.position, new Vector3 (range * 2,0, range * 2));
        }
        #endregion
    }
}
*/