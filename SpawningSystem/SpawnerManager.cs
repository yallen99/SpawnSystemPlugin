
//This file contains 2 scripts for the ease of access
//the first script is holding the variables
//the second is holding the spawn functionality

//The reason the 2 classes are separated is that
//the variables script needs to be executed in edit mode

using System;
using System.Collections;
using System.Collections.Generic;
using SpawningSystem;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SpawningSystem
{
    [ExecuteInEditMode]
    [Serializable]
     public class ValuesSync : MonoBehaviour
    {
        [HideInInspector]public List<GameObject> objects = new List<GameObject>(3); 
        [HideInInspector]public int[] percentages;
        [HideInInspector] public float xRange3d;
        [HideInInspector] public float zRange3d;
        [HideInInspector] public bool use2Drange;
        [HideInInspector] public float xRange2d;
        [HideInInspector] public float yRange2d;
        [HideInInspector]public int numberOfObjects;
        [HideInInspector]public float minTime;
        [HideInInspector]public float maxTime;
        [HideInInspector]public float spawnTime;
        [HideInInspector]public bool fixedSpawnTime;
        [HideInInspector]public float respawnMinTime;
        [HideInInspector]public float respawnMaxTime;
        [HideInInspector]public float respawnTime;
        [HideInInspector]public bool fixedRespawnTime;
        [HideInInspector]public int deadObjects;
        [HideInInspector]public bool respawn;
        [HideInInspector]public Color color = Color.yellow;

        
        // OnValidate and Gizmos methods which were previously used in
        // the ObjectSpawner script, that is no more in use
        private void OnValidate()
        {
            if ( percentages != null && percentages.Length != objects.Count)
            {
                percentages = new int[objects.Capacity];
            }
        }
        
        #region Gizmos
        private void OnDrawGizmos()
        {
            Gizmos.color = color;
            if (!use2Drange)
            {
                Gizmos.DrawWireCube(transform.position, new Vector3(xRange3d * 2, 0, zRange3d * 2));
            }
            else
            {
                Gizmos.DrawWireCube(transform.position, new Vector3(xRange2d * 2, yRange2d * 2, 0));

            }
        }
        #endregion
    }
}
    
    public class SpawnerManager : MonoBehaviour
    {
        private int _minObjectDistance = 1;
        private ValuesSync _valuesSync;
        private float _randomTime;
        private float _randomRespawnTime;
        private bool _isSpawning;
        private List<GameObject> _currentlySpawnedObjects = new List<GameObject>();

        private void Start()
        {
            _valuesSync = GetComponent<ValuesSync>();
            StartCoroutine(StartSpawning());
        }


        #region Re-spawn functionality
        private void Update()
        {
            if (!_isSpawning && _valuesSync.respawn)
            {
                CheckForDeadObjects();
            }
        }

        // this method checks if any of the objects were destroyed
        // and adds them again to be re-spawned 
        private void CheckForDeadObjects()
        {
            for (int i = _currentlySpawnedObjects.Count -1; i>=0; i--)
            {

                if (_currentlySpawnedObjects[i] == null) 
                {
                    _valuesSync.numberOfObjects++;
                    _currentlySpawnedObjects.Remove(_currentlySpawnedObjects[i]);
                }
                
            }
        }
        #endregion
       
        
        //the fundamental coroutine which directs the spawning
        //  according to the time set
        private IEnumerator StartSpawning()
        {
            // spawning
             _isSpawning = true;
             while (_valuesSync.numberOfObjects > 0)
             {
                 _isSpawning = false;
                
                SpawnEnemy(GetEnemyToSpawn());
                _valuesSync.numberOfObjects--;

                if (!_valuesSync.fixedSpawnTime)
                {
                    _randomTime = Random.Range(_valuesSync.minTime, _valuesSync.maxTime);
                    yield return new WaitForSeconds(_randomTime);
                }

                else
                {
                    yield return new WaitForSeconds(_valuesSync.spawnTime);
                }
            }
             
            // re-spawning (if applies)
            yield return new WaitUntil(() => _valuesSync.numberOfObjects >= _valuesSync.deadObjects);
            if (!_valuesSync.fixedRespawnTime)
            {
                _randomRespawnTime = Random.Range(_valuesSync.respawnMinTime, _valuesSync.respawnMaxTime);
                yield return new WaitForSeconds(_randomRespawnTime);
            }

            else
            {
                yield return  new WaitForSeconds(_valuesSync.respawnTime);
            }
            
            // the coroutine needs to be recursive to spawn all the objects
            StartCoroutine(StartSpawning());
        }

        // instantiating the object as a child of the spawn object
        // at a valid position
        private void SpawnEnemy(GameObject enemy)
        {
            var spawnPos = GetSpawnPosition();
            var child = Instantiate(enemy, spawnPos, Quaternion.identity);
            child.name = enemy.name;
            child.transform.parent = this.transform;
            _currentlySpawnedObjects.Add(child); 
        }

        // the segment rarity system based on which the object is extracted to spawn
        private GameObject GetEnemyToSpawn()
        {
            int rarityValue = Random.Range(1, 101);
            
            // initially the system was hardcoded due to testing and research purposes
            // the segments were [1,60], [60,90], [90,95] and [95,100]
            
            /*if (rarityValue >=1 && rarityValue <60)
            {
                return editor.enemies[0];
            }
        
            if (rarityValue >=60 && rarityValue <90)
            {
                return editor.enemies[1];
            }

            if (rarityValue >=90 && rarityValue <95)
            {
                return editor.enemies[2];
            }
        
            if(rarityValue >=95 && rarityValue <100)
            {
                return editor.enemies[3];
            }

            return editor.enemies[0];*/

            
            //later implementation, flexible and based on the percentages values
            //it assigns the object
            int[] segments = new int [_valuesSync.percentages.Length];
            for (int j = 0; j < segments.Length; j++)
            {
                for (int q = 0; q <= j; q++)
                {
                    segments[j] += _valuesSync.percentages[q];
                }
            }

            for (int i = 0; i < segments.Length; i++)
            {
                if ( rarityValue <= segments[i])
                {
                    return _valuesSync.objects[i];
                }
            }
            return _valuesSync.objects[0];
        }
        
        
        // this section focuses on getting a valid place to spawn the object
        #region Check for empty slot
       
        //the positions were split between
        private Vector3 GetSpawnPosition()
        {
            bool isPositionValid = true;
            Vector3 spawnPos;
            //2D / 3D switch
            if (!_valuesSync.use2Drange)
            {
                do
                {
                    Vector3 distance = new Vector3(Get3dSpawnDistanceX(), 0, Get3dSpawnDistanceZ());
                    spawnPos = transform.position + distance;
                    isPositionValid = IsSpawnPositionValid(spawnPos);
                } while (isPositionValid == false);
            }
            else
            {
                do
                {
                    Vector3 distance = new Vector3(Get2dSpawnDistanceX(), Get2dSpawnDistanceY(), 0);
                    spawnPos = transform.position + distance;
                    isPositionValid = IsSpawnPositionValid(spawnPos);
                } while (isPositionValid == false);
            }
            return  spawnPos;
        }

        // applying the distance between objects
        private bool IsSpawnPositionValid(Vector3 spawnPos)
        {
            if (_currentlySpawnedObjects.Count >= 2)
            {
                foreach (GameObject obj in _currentlySpawnedObjects)
                {
                    if (obj != null)
                    {
                        if (Vector3.Distance(obj.transform.position, spawnPos) < _minObjectDistance)
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }
        #endregion

        
        //this section is creating the width and length of the area
        // on both 3D and 2D dimensions according to the sign ( +, -)
        #region Area Dimensions
        private float Get3dSpawnDistanceX()
        {
            int sign = Random.Range(-1, 1);
            if (sign == -1)
            {
                return Random.Range((float) -1, -_valuesSync.xRange3d);
            }
            return Random.Range((float) 1, _valuesSync.xRange3d);
        }

        private float Get3dSpawnDistanceZ()
        {
            int sign = Random.Range(-1, 1);
            if (sign == -1)
            {
                return Random.Range((float) -1, -_valuesSync.zRange3d);
            }
            return Random.Range((float) 1, _valuesSync.zRange3d);
        }

        
        private float Get2dSpawnDistanceX()
        {
            int sign = Random.Range(-1, 1);
            if (sign == -1)
            {
                return Random.Range((float) -1, -_valuesSync.xRange2d);
            }
            return Random.Range((float) 1, _valuesSync.xRange2d);
        }
        
        private float Get2dSpawnDistanceY()
        {
            int sign = Random.Range(-1, 1);
            if (sign == -1)
            { 
                return Random.Range((float) -1, -_valuesSync.yRange2d);
            }
            return Random.Range((float) 1, _valuesSync.yRange2d);
        }
        #endregion

        
        
       
        
  
    
  
}
