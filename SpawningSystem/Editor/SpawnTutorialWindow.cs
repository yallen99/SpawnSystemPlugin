using System;
using UnityEditor;
using UnityEngine;
using System.IO;

namespace SpawningSystem.Editor
{
    [Serializable]
    public class SpawnTutorialWindow : EditorWindow
    {
        private Vector2 _scrollView;
        private GUIStyle _titleStyle;
        private GUIStyle _subtitleStyle;
        private GUIStyle _buttonStyle;
        private GUIStyle _descriptionStyle;

        #region Texts
        public string _defaultText ;
        public string _generalText;
        public string _objectText;
        public string _areaText;
        public string _timeText;
        public string _useText;
        #endregion
 

        private bool _generalButton;
        private bool _objectInfoButton;
        private bool _areaSizesButton;
        private bool _useIdeasButton;
        private bool _timeButton;

        //States responsible with the message display
        //used in both this and the SpawnEditorWindow script
        public enum States
        {
            Default,
            General,
            Object,
            Area,
            Time,
            Use
        }

        public States _states;
        
        [MenuItem("Window/Spawn System/Tutorial")]
        static void ShowWindow()
        {
            GetWindowWithRect(typeof(SpawnTutorialWindow), (new Rect(0, 0, 700, 350)), false, "Spawn System Tutorial");
        }

        private void OnGUI()
        {
            //styles
            ApplyStyles();
            
            //title
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Welcome to Spawn System Tutorial!", _titleStyle);
      
           

            //Left area -> Buttons
            GUILayout.BeginArea(new Rect(30, 50, 300, 350));
            _scrollView = GUILayout.BeginScrollView(_scrollView, false, false, GUILayout.Width(300), GUILayout.Height(280));

            EditorGUILayout.LabelField("Select one of the buttons below to \nfind out how to use the tool", _subtitleStyle);
            //buttons
            GUILayout.Space(20);
            _generalButton = GUILayout.Button("\nGeneral Information\n", _buttonStyle);
            GUILayout.Space(20);
            _objectInfoButton = GUILayout.Button("\nHow do I assign objects?\n", _buttonStyle);
            GUILayout.Space(20);
            _areaSizesButton = GUILayout.Button("\nHow do I set the area size?\n", _buttonStyle);
            GUILayout.Space(20);
            _timeButton =  GUILayout.Button("\nHow do I set the timings?\n", _buttonStyle);
            GUILayout.Space(20);
            _useIdeasButton = GUILayout.Button("\nWhere can I use the tool?\n", _buttonStyle);
           
           
            GUILayout.EndScrollView();
            GUILayout.EndArea();
            
            
            //Right Area -> Info
            GUILayout.BeginArea(new Rect(360, 50, 320, 30));
            EditorGUILayout.LabelField("Information", _subtitleStyle);
            GUILayout.EndArea();
            DisplayMessage();


            
        }

        //Custom styles applied to text
        #region GUI TextStyles

        public void ApplyStyles()
        {
            TitleStyle();
            ButtonTextStyle();
            SubtitleStyle();
            DescriptionTextStyle();
        }
        private void TitleStyle()
        {
            _titleStyle = GUI.skin.GetStyle("tutorialTitle");
            _titleStyle.alignment = TextAnchor.MiddleCenter;
            _titleStyle.fontStyle = FontStyle.Bold;
            _titleStyle.fontSize = 16;
            _titleStyle.normal.textColor = Color.black;
        }

        private void SubtitleStyle()
        {
            _subtitleStyle = GUI.skin.GetStyle("subtitle");
            _subtitleStyle.alignment = TextAnchor.UpperCenter;
            _subtitleStyle.fontStyle = FontStyle.Bold;
            _subtitleStyle.fontSize = 12;
            _subtitleStyle.normal.textColor = Color.black;
        }

        private void ButtonTextStyle()
        {
            _buttonStyle = GUI.skin.GetStyle("button");
            _buttonStyle.alignment = TextAnchor.MiddleCenter;
            _buttonStyle.fontStyle = FontStyle.Bold;
            _buttonStyle.fontSize = 10;
            _buttonStyle.normal.textColor = Color.blue;

        }

        private void DescriptionTextStyle()
        {
            _descriptionStyle = GUI.skin.GetStyle("label");
            _descriptionStyle.alignment = TextAnchor.UpperCenter;
            _descriptionStyle.fontSize = 12;
            _descriptionStyle.normal.textColor = Color.black;
        }

    
        
        #endregion

        //State Machine coordinating the text display
        //this is linked with the Spawn Editor window 
        #region Information Texts
        private void MessageContainer(String info)
        {
            GetTexts();
            GUILayout.BeginArea(new Rect(340, 80, 350, 260),EditorStyles.helpBox);
            EditorGUILayout.LabelField(info,_descriptionStyle, GUILayout.ExpandHeight(true));
            GUILayout.EndArea();
        }
        
        private void DisplayMessage()
        {
            switch (_states)
            {
                case States.Default:
                    MessageContainer(_defaultText);
                    break;
                case States.General:
                    MessageContainer(_generalText);
                    break;
                case States.Object:
                    MessageContainer(_objectText);
                    break;
                case States.Area:
                    MessageContainer(_areaText);
                    break;
                case States.Time:
                   MessageContainer(_timeText);
                   break;
                case States.Use:
                    MessageContainer(_useText);
                    break;
            }
        }

        private void ChangeState()
        {
            if (_generalButton)
            {
                _states = States.General;
            }
            else if (_objectInfoButton)
            {
                _states = States.Object;
            }
            else if (_timeButton)
            {
                _states = States.Time;
            }
            else if (_areaSizesButton)
            {
                _states = States.Area;
            }
            else if (_useIdeasButton)
            {
                _states = States.Use;
            }
        }

        private void Awake()
        {
            _states = States.Default;
        }

        //Info Texts
        private void GetTexts()
        {
             _defaultText = " \n\n\n\n\n\n\nPress a button to start learning about \nSpawn System Editor";
             _generalText = "< General >\n\n" +
                            "Spawn System editor allows you to customize\n" +
                            "your own random object generator. When opened,\n" +
                            "the window will automatically create a temporary\n" +
                            "target object to point at.\n" +
                            "\nDO NOT DELETE THIS OBJECT WHILE EDITING IT!\n" +
                            "You can place it permanently after you finished\n" +
                            " working with it. Create and edit as many Spawn\n" +
                            " instances as you want by simply changing the\n" +
                            "Target Field.\n" +
                            "\n| --------------------------------------- |\n" +
                              "|   CLOSE THE SPAWN WINDOW     |\n" +
                              "|   BEFORE PLAYING THE GAME       |\n" +
                              "| --------------------------------------- |\n";
             _areaText = "\n\n< Area size >\n\n" +
                         "The area size is split between X & Z on 3D\n" +
                         "space and X & Y on 2D space. The objects will\n" +
                         "spawn within the designated area which you\n" +
                         "can visualize in the Scene Editor.\n\n" +
                         "The number of objects spawned DEPENDS DIRECTLY\n" +
                         "on the area size. The bigger the area size,\n" +
                         "the more objects can be spawned. Please note\n" +
                         "that you cannot spawn more than 1/3 of your\n" +
                         "area size.\n\n";
             _timeText = "\n\n< Timings >\n\n" +
                         "The editor allows the customization of any\n" +
                         "time parameter used. Set your own time \n" +
                         "between spawns and the time to wait before re-\n" +
                         "spawning. You can choose a random value for\n" +
                         "diversity or opt for a fixed time.  \n\n" +
                         "If you choose to re-spawn the destroyed\n" +
                         "objects, you can choose after how many objects\n " +
                         "you want the re-spawning to start by specifying\n" +
                         "the value in the [Required dead objects] field.\n\n";
             _useText = "\n< Use it in your way! >\n\n" +
                        "The Spawn system has been designed to fulfill\n" +
                        "any spawning needs of a game developer, from\n" +
                        "spawning enemies, resources to lights and \n" +
                        "particle effects. The mose common usages are:\n" +
                        "\nENEMY SPAWN\n" +
                        "RESOURCE GENERATION\n\n" +
                        "The Spawn can fit both 3D and 2D games. To\n" +
                        "trigger the spawn, the objects needs to be \n" +
                        "active, which allows you to trigger the spawn\n" +
                        "at any point by simply setting active or\n" +
                        "inactive the object.\n";
             _objectText = "\n\n< Objects and Rarities >\n\n" +
                           "To assign an object, drag and drop a prefab\n" +
                           "or a game object into the list in the right.\n" +
                           "Any game object can be instantiated.\n\n" +
                           "The percentages reflect the rate of spawn of\n" +
                           "each assigned object. \n\n" +
                           "Keep in mind that if you leave the Rarity\n" +
                           "to 0, the object will never spawn, while if\n " +
                           "it's set to 100, it will spawn exclusively.\n\n" +
                           "-> THE SPAWN RATES NEED TO SUM UP 100 ! <-\n\n";

        }
        
        private void Update()
        {
            ChangeState();
        }
        
        #endregion

    }

}