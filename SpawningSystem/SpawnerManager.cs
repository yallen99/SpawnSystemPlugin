﻿using System;
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
        [HideInInspector]public float range;
        [HideInInspector] public bool use2Drange;
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

        /*
        public bool Respawn
        {
            get => respawn;
            set => respawn = value;
        }

        public int DeadObjects
        {
            get => deadObjects;
            set => deadObjects = value;
        }

        public float Range
        {
            get => range;
            set => range = value;
        }

        public int NumberOfObjects
        {
            get => numberOfObjects;
            set => numberOfObjects = value;
        }

        public float MinTime
        {
            get => minTime;
            set => minTime = value;
        }

        public float MaxTime
        {
            get => maxTime;
            set => maxTime = value;
        }

        public float RespawnMinTime
        {
            get => respawnMinTime;
            set => respawnMinTime = value;
        }

        public float RespawnMaxTime
        {
            get => respawnMaxTime;
            set => respawnMaxTime = value;
        }

        public Color Color
        {
            get => color;
            set => color = value;
        }
        */

        /*private void OnEnable()
        {
            objects = new List<GameObject>(1);
            percentages[0] = 0;
        }*/

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
                Gizmos.DrawWireCube(transform.position, new Vector3(range * 2, 0, range * 2));
            }
            else
            {
                Gizmos.DrawWireCube(transform.position, new Vector3(range * 2, range * 2, 0));

            }
        }
        #endregion
    }
    }
    
    public class SpawnerManager : MonoBehaviour
    {
        private int _minEnemyDistance = 1;
        private ValuesSync _valuesSync;
        private float randomTime;
        private float randomRespawnTime;
        private bool _isSpawning;
        private List<GameObject> _currentlySpawnedEnemies = new List<GameObject>();

        private void Start()
        {
            _valuesSync = GetComponent<ValuesSync>();
            StartCoroutine(StartSpawning());
        }


        private void Update()
        {
            if (!_isSpawning && _valuesSync.respawn)
            {
                CheckForDeadObjects();
            }
        }


        private void CheckForDeadObjects()
        {
            for (int i = _currentlySpawnedEnemies.Count -1; i>=0; i--)
            {

                if (_currentlySpawnedEnemies[i] == null) 
                {
                    _valuesSync.numberOfObjects++;
                    _currentlySpawnedEnemies.Remove(_currentlySpawnedEnemies[i]);
                }
                
            }
            
            
        }
        private IEnumerator StartSpawning()
        {
            _isSpawning = true;
             while (_valuesSync.numberOfObjects > 0)
             {
                 _isSpawning = false;
                
                SpawnEnemy(GetEnemyToSpawn());
                _valuesSync.numberOfObjects--;

                if (!_valuesSync.fixedSpawnTime)
                {
                    randomTime = Random.Range((float) _valuesSync.minTime, _valuesSync.maxTime);
                    yield return new WaitForSeconds(randomTime);
                }

                else
                {
                    yield return new WaitForSeconds(_valuesSync.spawnTime);
                }
            }

            /*if (_valuesSync.numberOfObjects <= 0)
            {
                _isSpawning = false;
            }*/
            
            yield return new WaitUntil(() => _valuesSync.numberOfObjects >= _valuesSync.deadObjects);
            if (!_valuesSync.fixedRespawnTime)
            {
                randomRespawnTime = Random.Range((float) _valuesSync.respawnMinTime, _valuesSync.respawnMaxTime);
                yield return new WaitForSeconds(randomRespawnTime);
            }

            else
            {
                yield return  new WaitForSeconds(_valuesSync.respawnTime);
            }

            StartCoroutine(StartSpawning());
        }

        private void SpawnEnemy(GameObject enemy)
        {
            var spawnPos = GetSpawnPosition();
            var child = Instantiate(enemy, spawnPos, Quaternion.identity);
            child.transform.parent = this.transform;
            _currentlySpawnedEnemies.Add(child); 
        }


        #region Check for empty slot

        private Vector3 GetSpawnPosition()
        {
            bool isPositionValid = true;
            Vector3 spawnPos;
            //2D / 3D switch
            if (!_valuesSync.use2Drange)
            {
                do
                {
                    Vector3 distance = new Vector3(GetSpawnDistance(), 0, GetSpawnDistance());
                    spawnPos = transform.position + distance;
                    isPositionValid = IsSpawnPositionValid(spawnPos);
                } while (isPositionValid == false);
            }
            else
            {
                do
                {
                    Vector3 distance = new Vector3(GetSpawnDistance(), GetSpawnDistance(), 0);
                    spawnPos = transform.position + distance;
                    isPositionValid = IsSpawnPositionValid(spawnPos);
                } while (isPositionValid == false);
            }
            return  spawnPos;
        }

        private bool IsSpawnPositionValid(Vector3 spawnPos)
        {
            if (_currentlySpawnedEnemies.Count >= 2)
            {
                foreach (GameObject enemy in _currentlySpawnedEnemies)
                {
                    if (enemy != null)
                    {
                        if (Vector3.Distance(enemy.transform.position, spawnPos) < _minEnemyDistance)
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }


        #endregion
       
        private float GetSpawnDistance()
        {
            int sign = Random.Range(-1, 1);
            if (sign == -1)
            {
                return Random.Range((float) -1, -_valuesSync.range);
            }
            else
            {
                return Random.Range((float) 1, _valuesSync.range);
            }
        }
        
        
        
        private GameObject GetEnemyToSpawn()
        {
            int rarityValue = Random.Range(1, 101);
            
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
        
  
    
  
}
