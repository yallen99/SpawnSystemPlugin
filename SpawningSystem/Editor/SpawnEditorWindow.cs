// this script handles the custom editor window functionality
// the values are directly linked with the ValuesSync script

using System;
using SpawningSystem;
using SpawningSystem.Editor;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace SpawnSystemDll.SpawningSystem.Editor
{
    public class SpawnEditorWindow : EditorWindow
    {
        //the variables are serialized so the data is saved when exiting the play mode
        
        //Reorderable list 
        static Rect objectListRect = new Rect(460, 5, 230, 200);
        [SerializeField]private ValuesSync valuesSync;
        [SerializeField]private SerializedObject valuesSyncSo = null;
        private ReorderableList _percentList = null; 
        private ReorderableList _objectList = null;
       
        //Target instance
        [SerializeField]private GameObject spawner;
        [SerializeField]private GameObject tempSpawner;
        [SerializeField]private GameObject actualSpawner;
        private static GameObject _instance;

        [SerializeField]private float xPos;
        [SerializeField]private float yPos;
        [SerializeField]private float zPos;
        [SerializeField]private string objName;
        [SerializeField]private bool exiting;
        [SerializeField]private bool objectPlaced;
        [SerializeField]private Vector2 scrollPosition;
        [SerializeField]private bool placeObjectButton;
        [SerializeField]private bool abortButton;
        [SerializeField]private bool newObjectButton;

        #region HelpButtons

        private SpawnTutorialWindow _tutorialWindow;
        private SpawnTutorialWindow.States _states;
        private bool _generalHelp;
        private bool _areaHelp;
        private bool _timeHelp;
        

        #endregion
        
        
        #region Singleton
        //singleton pattern was used to:
        //get only one instance of the temporary spawn object
        //open only one Tutorial Window if the user needs help
        private void InstantiateTempObject()
        {
            tempSpawner = new GameObject("Temporary Spawn");
            tempSpawner.AddComponent<SpawnManager>();
            tempSpawner.AddComponent<ValuesSync>();
            _instance = tempSpawner;
            spawner = tempSpawner;
        }

        public static GameObject GetInstance()
        {
            if (_instance == null)
            {
                _instance = new GameObject("Temporary Spawn");
            }
            return _instance;
        }

        //Creating a single tutorial window and focusing on it
        public void GetTutorialInstance(SpawnTutorialWindow.States state)
        {
            if (_tutorialWindow == null)
            {
                _tutorialWindow = CreateWindow<SpawnTutorialWindow>("Spawn System Tutorial");
                _tutorialWindow._states = state;
            }
            else
            {
                FocusWindowIfItsOpen<SpawnTutorialWindow>();
                _tutorialWindow._states = state;
            }
        }
        #endregion
        

        #region Callbacks
        
        private void OnEnable()
        {
            InstantiateTempObject();

            objName = spawner.name;
            valuesSync = spawner.GetComponent<ValuesSync>();
            valuesSyncSo = new SerializedObject(valuesSync);

            GetInstance();
            CreateList();
           
        }

        private void OnFocus()
        {
            GetInstance();
        }

        private void OnInspectorUpdate()
        {
            // Checking if the allocated target contains the required component
            // in this case the SpawnManager script
            if (!(spawner.TryGetComponent(out SpawnManager spawnerManager)))
            {
                bool wrongGameObj = EditorUtility.DisplayDialog("Game Object not valid", "The assigned game object does " + 
                                                                                         "not contain the required script (Spawn Manager). " +
                                                                                         "Please assign a valid game object or create a default one. ", 
                    "Create Default Object",
                    "Assign another object.");
         
                if(wrongGameObj)
                { 
                    DestroyImmediate(tempSpawner);
                    InstantiateTempObject();
                }
                else
                {
                    spawner = null;
                }
            }
        }

        private void OnGUI()
        {   
            //UI positioning and fields 
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, true, GUILayout.Width(450), GUILayout.Height(245));
            //target field
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Select the target spawn", EditorStyles.boldLabel);
            _generalHelp = GUILayout.Button("Need help?", EditorStyles.miniButton);
            GUILayout.EndHorizontal();
            EditorGUILayout.Space();
            EditorGUI.BeginChangeCheck();
            spawner = (GameObject) EditorGUILayout.ObjectField("Target", spawner, typeof(GameObject), true);
            EditorGUILayout.Space();
            //Gizmos color
            valuesSync.color = EditorGUILayout.ColorField("Gizmos area color",valuesSync.color);

            if (EditorGUI.EndChangeCheck())
            {
                valuesSync = spawner.GetComponent<ValuesSync>();
                valuesSyncSo = new SerializedObject(valuesSync);
                CreateList();
                objName = spawner.name;
            }
            EditorGUILayout.Space(); 
            
            //other fields
           CreateLinkedFields();
           CreateGameObjectFields();
           GUILayout.EndScrollView();
           
           //buttons
           GUILayout.BeginArea(new Rect(15, 250, 400, 150));
           CreateButtons();
           GUILayout.EndArea();
           //text box
           GUILayout.BeginArea(new Rect(440, 255, 250, 150));
           InfoSpawner();
           GUILayout.EndArea();

           //updating the reorderable list
           if (valuesSyncSo != null)
            {
                valuesSyncSo.Update();
                _objectList.DoList(objectListRect);
                valuesSyncSo.ApplyModifiedProperties();
            }

            ButtonChecks();
        }

        private void Update()
        {
            ActivateHelpButton();
        }

        private void OnDestroy()
        {
            //when exiting, check if the object created was placed or not
            //and display a prompt message
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
                    DestroyImmediate(tempSpawner);
                }

            }
        }
        #endregion
       
        //Creating the window
        [MenuItem("Window/Spawn System/Editor")]
        static void ShowWindow()
        {
            GetWindowWithRect(typeof(SpawnEditorWindow), (new Rect(0, 0, 700, 350)), false, "Spawn System Editor");
        }
       
        
        #region UI Fields
        private void CreateList()
        {
            //reorderable list functionality
            _objectList = new ReorderableList(valuesSyncSo, valuesSyncSo.FindProperty("objects"), true, true, true, true);
            _percentList = new ReorderableList(valuesSyncSo, valuesSyncSo.FindProperty("percentages"), true, true, true,
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
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Area Settings", EditorStyles.boldLabel);
            _areaHelp = GUILayout.Button("Need help?", EditorStyles.miniButton);
            GUILayout.EndHorizontal();
            EditorGUI.BeginChangeCheck();
            valuesSync.use2Drange = EditorGUILayout.Toggle("Use 2D / vertical space?", valuesSync.use2Drange);
            //2D settings
            EditorGUILayout.LabelField("2D settings", EditorStyles.label);
            EditorGUI.BeginDisabledGroup(!valuesSync.use2Drange);
            valuesSync.xRange2d = EditorGUILayout.Slider("X Size: ", valuesSync.xRange2d, 1, 1000);
            valuesSync.yRange2d = EditorGUILayout.Slider("Y Size: ", valuesSync.yRange2d, 1, 1000);
            EditorGUI.EndDisabledGroup();

            //3D distance
            EditorGUILayout.LabelField("3D settings", EditorStyles.label);
            EditorGUI.BeginDisabledGroup(valuesSync.use2Drange);
            valuesSync.xRange3d = EditorGUILayout.Slider("X Size: ", valuesSync.xRange3d, 1, 1000);
            valuesSync.zRange3d = EditorGUILayout.Slider("Z Size: ", valuesSync.zRange3d, 1, 1000);
            EditorGUI.EndDisabledGroup();
           
            valuesSync.numberOfObjects = EditorGUILayout.IntField("Number of objects: ", valuesSync.numberOfObjects);
            if (EditorGUI.EndChangeCheck())
            {
                //one third of the area can be populated only to avoid CPU burn
                float area;
                if (!valuesSync.use2Drange)
                {
                    area = valuesSync.xRange3d * valuesSync.zRange3d;
                }
                else
                {
                    area = valuesSync.xRange2d * valuesSync.yRange2d;
                }
                float maxEnemies = area / 3;
                int maxIntEnemies = Mathf.FloorToInt(maxEnemies);

                //if the value is over the limits, display a prompt message
                if (valuesSync.numberOfObjects > maxEnemies)
                { 
                    EditorUtility.DisplayDialog("Too many objects for this spawn", "The number assigned exceeds the capacity of this spawn." +
                                                                                 " Spawn capacity is: " + maxIntEnemies.ToString() + ". Please enlarge " +
                                                                                 "the spawn area or reduce the number of enemies.", 
                        "Continue");
                    
                    valuesSync.numberOfObjects = maxIntEnemies;
                }
            }
            
            EditorGUILayout.Space();
            
            //spawn parameters' fields
            #region Spawn
            EditorGUILayout.Space();
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Timings",  EditorStyles.boldLabel);
            _timeHelp = GUILayout.Button("Need Help?", EditorStyles.miniButton);
            GUILayout.EndHorizontal();
            EditorGUILayout.LabelField("Spawn Timings",  EditorStyles.boldLabel);
            var valuesSyncMaxTime = valuesSync.maxTime;
            var valuesSyncMinTime = valuesSync.minTime;
            valuesSync.fixedSpawnTime = EditorGUILayout.Toggle("Fixed spawn interval?", valuesSync.fixedSpawnTime);
            
            EditorGUI.BeginDisabledGroup(!valuesSync.fixedSpawnTime);
            valuesSync.spawnTime = EditorGUILayout.FloatField("Time: ",  valuesSync.spawnTime);
            
            EditorGUI.EndDisabledGroup();
            
            EditorGUI.BeginDisabledGroup(valuesSync.fixedSpawnTime);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("Time Range:");
            EditorGUILayout.MinMaxSlider(ref valuesSyncMinTime, ref valuesSyncMaxTime, 0.1f, 60f);
            valuesSync.minTime = valuesSyncMinTime;
            valuesSync.maxTime = valuesSyncMaxTime;
            EditorGUILayout.EndVertical();
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("Min time:" + valuesSyncMinTime.ToString());
            EditorGUILayout.LabelField("Max time:" + valuesSyncMaxTime.ToString());
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.Space();
            #endregion
            
            //re-spawn parameters' fields
            #region Respawn
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Respawn Options", EditorStyles.boldLabel);
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Respawn if objects are destroyed?");
            valuesSync.respawn =EditorGUILayout.Toggle(valuesSync.respawn);
           EditorGUILayout.EndHorizontal();
           
            EditorGUI.BeginDisabledGroup(!valuesSync.respawn);
            
            valuesSync.deadObjects =
                EditorGUILayout.IntField("Required dead objects:", valuesSync.deadObjects);
            var valuesRespawnMinTime = valuesSync.respawnMinTime;
            var valuesRespawnMaxTime = valuesSync.respawnMaxTime;
            valuesSync.fixedRespawnTime = EditorGUILayout.Toggle("Fixed respawn interval?", valuesSync.fixedRespawnTime);
            EditorGUI.BeginDisabledGroup(!valuesSync.fixedRespawnTime);
            valuesSync.respawnTime = EditorGUILayout.FloatField("Respawn Time:",  valuesSync.respawnTime);
            EditorGUI.EndDisabledGroup();
           
            EditorGUI.BeginDisabledGroup(valuesSync.fixedRespawnTime);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("Time Range:");
            EditorGUILayout.MinMaxSlider(ref valuesRespawnMinTime, ref valuesRespawnMaxTime, 0.1f, 100f);
            valuesSync.respawnMinTime = valuesRespawnMinTime;
            valuesSync.respawnMaxTime = valuesRespawnMaxTime;
            EditorGUILayout.EndVertical();
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("Min time:" + valuesRespawnMinTime.ToString());
            EditorGUILayout.LabelField("Max time:" + valuesRespawnMaxTime.ToString());
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
            EditorGUI.EndDisabledGroup();
            
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.Space();
            #endregion
        }
       
        //info text
        private void InfoSpawner()
        {
            EditorGUILayout.HelpBox("\nThank you for using Spawn Editor!\n \nPLEASE CLOSE THIS WINDOW BEFORE ENTERING PLAY MODE!\n", MessageType.Warning);
        }
        
        //game object name and position for the spawn object 
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

        //button objects and functionality
        #region Big Buttons
        //buttons 
        private GUIStyle _buttonStyle;
        private void ButtonTextStyle()
        {
            _buttonStyle = GUI.skin.GetStyle("button");
            _buttonStyle.alignment = TextAnchor.MiddleCenter;
            _buttonStyle.fontStyle = FontStyle.Bold;
            _buttonStyle.fontSize = 10;
            _buttonStyle.normal.textColor = Color.blue;
        }

        private void CreateButtons()
        {
            ButtonTextStyle();
            placeObjectButton = GUILayout.Button("\nPlace Spawn in scene\n", _buttonStyle);
            newObjectButton = GUILayout.Button("\nNew Spawn instance\n", _buttonStyle);
        }

       
        //transforming the temporary instance into a permanent one
        //and moving it to the desired position
        private void PlaceObject()
        {
            actualSpawner = tempSpawner;
            actualSpawner.name = objName;
            actualSpawner.transform.position = new Vector3(xPos, yPos, zPos);
            spawner = actualSpawner;
            valuesSync = spawner.GetComponent<ValuesSync>();
            _instance = actualSpawner;
        }

        //Button functionality
        private void ButtonChecks()
        {
            if (placeObjectButton)
            {
                PlaceObject();
                objectPlaced = true;
            }

            if (abortButton)
            {
                DestroyImmediate(tempSpawner);
                Close();
            }

            if (newObjectButton)
            {
                InstantiateTempObject();
            }
        }

        #endregion

        #region HelpButtons
        
        //Help buttons functionality is linked with the states in the
        //tutorial window script
        private void ActivateHelpButton()
        {
            if (_areaHelp)
            {
                GetTutorialInstance(SpawnTutorialWindow.States.Area);
            }
            else if (_generalHelp)
            {
                GetTutorialInstance(SpawnTutorialWindow.States.General);
            }
            else if (_timeHelp)
            {
                GetTutorialInstance(SpawnTutorialWindow.States.Time);
            }
        }

        #endregion
        
        
        //initially planned to exist, a preview was no longer
        //necessary since the spawn area could be viewed in the scene editor
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