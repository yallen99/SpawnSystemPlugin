﻿using System;
using UnityEditor;
using UnityEditor.Graphs;
using UnityEditorInternal;
using UnityEngine;

namespace SpawningSystem.Editor
{
    public class SpawnerWindow : EditorWindow
    {
        UnityEditor.Editor _spawnerEditor;

        //List's position
        static Rect objectListRect = new Rect(460, 5, 230, 200);
        
        [SerializeField]private GameObject _spawner;
        [SerializeField]private ValuesSync _valuesSync;
        [SerializeField]private SerializedObject _valuesSyncSo = null;
        private ReorderableList _percentList = null; 
        private ReorderableList _objectList = null;

        [SerializeField]private GameObject _tempSpawner;
        [SerializeField]private GameObject _actualSpawner;
        private static GameObject _instance;

        [SerializeField] private float _range;
        [SerializeField] private int _capacity;
        [SerializeField]private bool _fixedTime;
        [SerializeField] private bool _fixedRespawnTime;
        [SerializeField] private bool _placeObjectButton;
        [SerializeField] private bool _abortButton;
        [SerializeField] private bool _newObjectButton;
        
        [SerializeField] private float xPos;
        [SerializeField]private float yPos;
        [SerializeField] private float zPos;
        [SerializeField]private string objName;
        [SerializeField]private bool exiting;
        [SerializeField]private bool objectPlaced;
        [SerializeField]private Vector2 scrollPosition;
        
        private void InstantiateTempObject()
        {
            _tempSpawner = new GameObject("Temporary Spawn");
            _tempSpawner.AddComponent<SpawnerManager>();
            _tempSpawner.AddComponent<ValuesSync>();
            _instance = _tempSpawner;
            _spawner = _tempSpawner;
        }

        public static GameObject GetInstance()
        {
            if (_instance == null)
            {
                _instance = new GameObject("Temporary spawner");
            }
            return _instance;
        }

    
        
        private void OnEnable()
        {
            InstantiateTempObject();

            objName = _spawner.name;
            _valuesSync = _spawner.GetComponent<ValuesSync>();
            _valuesSyncSo = new SerializedObject(_valuesSync);

            GetInstance();

            CreateList();
        }

        private void OnFocus()
        {
            GetInstance();
        }
        
        [MenuItem("Window/Spawn System Editor")]
        //show the window
        static void ShowWindow()
        {
           // GetWindow(typeof(SpawnerWindow));
            GetWindowWithRect(typeof(SpawnerWindow), (new Rect(0, 0, 700, 350)), false, "Spawn System Editor");
        }

        private void OnInspectorUpdate()
        {
            if (!(_spawner.TryGetComponent(out SpawnerManager spawnerManager)))
            {
                bool wrongGameObj = EditorUtility.DisplayDialog("Game Object not valid", "The assigned game object does " + 
                                                                                         "not contain the required script (Spawn Manager). " +
                                                                                         "Please assign a valid game object or create a default one. ", 
                    "Create Default Object",
                    "Assign another object.");
         
                if(wrongGameObj)
                { 
                    DestroyImmediate(_tempSpawner);
                    InstantiateTempObject();
                }
                else
                {
                    _spawner = null;
                }
            }
        }

        private void OnGUI()
        {   
            
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, true, GUILayout.Width(450), GUILayout.Height(245));
            EditorGUILayout.LabelField("Select the target spawn", EditorStyles.boldLabel);
            EditorGUI.BeginChangeCheck();
            _spawner = (GameObject) EditorGUILayout.ObjectField("Target", _spawner, typeof(GameObject), true);
            EditorGUILayout.Space();
            _valuesSync.color = EditorGUILayout.ColorField("Gizmos area color",_valuesSync.color);
            if (EditorGUI.EndChangeCheck())
            {
                _valuesSync = _spawner.GetComponent<ValuesSync>();
                _valuesSyncSo = new SerializedObject(_valuesSync);
                CreateList();
                objName = _spawner.name;
            }
            EditorGUILayout.Space();
            
           //CreatePreview();
           CreateLinkedFields();
           CreateGameObjectFields();
           GUILayout.EndScrollView();
           
           GUILayout.BeginArea(new Rect(30, 240, 400, 150));
           CreateButtons();
           GUILayout.EndArea();
           
           GUILayout.BeginArea(new Rect(440, 255, 250, 150));
            InfoSpawner();
           GUILayout.EndArea();

           
           
            if (_valuesSyncSo != null)
            {
                _valuesSyncSo.Update();
                _objectList.DoList(objectListRect);
                _valuesSyncSo.ApplyModifiedProperties();
            }

            ButtonChecks();
        }
        
        private void OnDestroy()
        {

            if (!objectPlaced)
            {
                exiting = EditorUtility.DisplayDialog("Exit Spawn Editor?", "The unplaced spawn will be lost. " +
                                                                            "Would you like to place it now?",
                    "Place and exit",
                    "Exit without placing");

                if (exiting)
                {
                    PlaceObject();
                }
                else

                {
                    DestroyImmediate(_tempSpawner);
                }

            }
        }

        #region UI Fields

        private void CreateList()
        {
          
            _objectList = new ReorderableList(_valuesSyncSo, _valuesSyncSo.FindProperty("objects"), true, true, true, true);
            _percentList = new ReorderableList(_valuesSyncSo, _valuesSyncSo.FindProperty("percentages"), true, true, true,
                true);

            _objectList.drawHeaderCallback = rect =>
                EditorGUI.LabelField(rect, " Objects          |      Spawn rates (%)");
            
            _objectList.drawElementCallback = (rect, index, active, focused) =>
            {
                rect.y += 2;
                EditorGUI.PropertyField(
                    new Rect(480, rect.y, 130, EditorGUIUtility.singleLineHeight),
                    _objectList.serializedProperty.GetArrayElementAtIndex(index), GUIContent.none);
                EditorGUI.PropertyField(
                    new Rect(650, rect.y, 30, EditorGUIUtility.singleLineHeight),
                    _percentList.serializedProperty.GetArrayElementAtIndex(index), GUIContent.none);
            };

            //if there is 1 object left, disable  -  button
            _objectList.onCanRemoveCallback = (list) => list.count > 1;
            //if there are more than 20 objects, disable + button
            _objectList.onCanAddCallback = (list) => list.count < 10;
            
        }

        private void CreateLinkedFields()
        {   
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("General Settings", EditorStyles.boldLabel);
            _valuesSync.use2Drange = EditorGUILayout.Toggle("2D space / vertical", _valuesSync.use2Drange);
            _valuesSync.range = EditorGUILayout.Slider("Area size: ", _valuesSync.range, 1, 1000);
            EditorGUI.BeginChangeCheck();
            _valuesSync.numberOfObjects = EditorGUILayout.IntField("Number of objects: ", _valuesSync.numberOfObjects);
            if (EditorGUI.EndChangeCheck())
            {
                float area = Mathf.Pow(_valuesSync.range * 2, 2) - Mathf.Pow(2, 2);
                //one third of the area can be populated only to avoid CPU burn
                float maxEnemies = area / 3;
                int maxIntEnemies = Mathf.FloorToInt(maxEnemies);

                if (_valuesSync.numberOfObjects > maxEnemies)
                {
                    bool exceededNumber = EditorUtility.DisplayDialog("Too many objects for this spawn", "The number assigned exceeds the capacity of this spawn." +
                                                                                                         " Spawn capacity is: " + maxIntEnemies.ToString() + ". Please enlarge " +
                                                                                                         "the spawn area or reduce the number of enemies.", 
                        "Continue");

                   
                        _valuesSync.numberOfObjects = maxIntEnemies;
                }
            }
            
            EditorGUILayout.Space();
            
            #region Spawn
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Spawn options",  EditorStyles.boldLabel);
            var valuesSyncMaxTime = _valuesSync.maxTime;
            var valuesSyncMinTime = _valuesSync.minTime;
            _valuesSync.fixedSpawnTime = EditorGUILayout.Toggle("Fixed spawn interval?", _valuesSync.fixedSpawnTime);
            
            EditorGUI.BeginDisabledGroup(!_valuesSync.fixedSpawnTime);
            _valuesSync.spawnTime = EditorGUILayout.FloatField("Time: ",  _valuesSync.spawnTime);
            
            EditorGUI.EndDisabledGroup();
            
            EditorGUI.BeginDisabledGroup(_valuesSync.fixedSpawnTime);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("Range:");
            EditorGUILayout.MinMaxSlider(ref valuesSyncMinTime, ref valuesSyncMaxTime, 0.1f, 60f);
            _valuesSync.minTime = valuesSyncMinTime;
            _valuesSync.maxTime = valuesSyncMaxTime;
            EditorGUILayout.EndVertical();
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("Min time:" + valuesSyncMinTime.ToString());
            EditorGUILayout.LabelField("Max time:" + valuesSyncMaxTime.ToString());
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.Space();
            #endregion

            #region Respawn
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Respawn options", EditorStyles.boldLabel);
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Respawn if objects are destroyed?");
            _valuesSync.respawn =EditorGUILayout.Toggle(_valuesSync.respawn);
           EditorGUILayout.EndHorizontal();
           
            EditorGUI.BeginDisabledGroup(!_valuesSync.respawn);
            
            _valuesSync.deadObjects =
                EditorGUILayout.IntField("Required dead objects:", _valuesSync.deadObjects);
            var valuesRespawnMinTime = _valuesSync.respawnMinTime;
            var valuesRespawnMaxTime = _valuesSync.respawnMaxTime;
            _valuesSync.fixedRespawnTime = EditorGUILayout.Toggle("Fixed respawn interval?", _valuesSync.fixedRespawnTime);
            EditorGUI.BeginDisabledGroup(!_valuesSync.fixedRespawnTime);
            _valuesSync.respawnTime = EditorGUILayout.FloatField("Respawn Time:",  _valuesSync.respawnTime);
            EditorGUI.EndDisabledGroup();
           
            EditorGUI.BeginDisabledGroup(_valuesSync.fixedRespawnTime);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("Range:");
            EditorGUILayout.MinMaxSlider(ref valuesRespawnMinTime, ref valuesRespawnMaxTime, 0.1f, 100f);
            _valuesSync.respawnMinTime = valuesRespawnMinTime;
            _valuesSync.respawnMaxTime = valuesRespawnMaxTime;
            EditorGUILayout.EndVertical();
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("Min time:" + valuesRespawnMinTime.ToString());
            EditorGUILayout.LabelField("Max time:" + valuesRespawnMinTime.ToString());
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
            EditorGUI.EndDisabledGroup();
            
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.Space();
            #endregion
            
            
        }

        private void CreateButtons()
        {
            EditorGUILayout.Space();
           // EditorGUILayout.BeginHorizontal();
            _placeObjectButton = GUILayout.Button("\nPlace Spawner in scene\n");
           // _abortButton = GUILayout.Button("\nAbort Settings\n");
           // EditorGUILayout.EndHorizontal();
            _newObjectButton = GUILayout.Button("\nNew spawner instance\n");
        }

        private void InfoSpawner()
        {
            EditorGUILayout.HelpBox("\nThank you for using Spawn Editor!\n \nPLEASE CLOSE THIS WINDOW BEFORE ENTERING PLAY MODE!\n", MessageType.Warning);
        }

        private void CreateGameObjectFields()
        { 
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Game Object Settings", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Object Position in scene (X, Y, Z)",  EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            xPos = EditorGUILayout.FloatField( xPos);
            yPos = EditorGUILayout.FloatField( yPos);
            zPos = EditorGUILayout.FloatField( zPos);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.LabelField("Object name", EditorStyles.boldLabel);
            objName = EditorGUILayout.TextField( objName);

        }

        #endregion
        
        
        private void PlaceObject()
        {
            _actualSpawner = _tempSpawner;
            _actualSpawner.name = objName;
            _actualSpawner.transform.position = new Vector3(xPos, yPos, zPos);
            _spawner = _actualSpawner;
            _valuesSync = _spawner.GetComponent<ValuesSync>();
            _instance = _actualSpawner;
        }

        private void ButtonChecks()
        {
            if (_placeObjectButton)
            {
                PlaceObject();
                objectPlaced = true;
            }

            if (_abortButton)
            {
                DestroyImmediate(_tempSpawner);
                Close();
            }

            if (_newObjectButton)
            {
                InstantiateTempObject();
            }
        }
        
        
        /*private void CreatePreview()
        {
            //do I need a prev?
            if (_spawner != null)
            {
                if (_spawnerEditor == null)
                
                    
                    _spawnerEditor = UnityEditor.Editor.CreateEditor(_spawner);
                
                    _spawnerEditor.OnInteractivePreviewGUI(GUILayoutUtility.GetRect(0, 100), null);
            }
        }*/
    }
}