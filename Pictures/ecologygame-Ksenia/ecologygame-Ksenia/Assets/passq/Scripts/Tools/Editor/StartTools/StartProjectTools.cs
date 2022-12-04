using UnityEngine;
using UnityEditor;
using System.Linq;
#if COLORED_HEADERS
using ColoredHeader;
#endif

namespace StartProjectTool
{
    public class StartProjectTools : EditorWindow
    {
        [MenuItem("passq/Start project tool")]
        public static void Init() { GetWindow<StartProjectTools>("Start project tool", true); }


        private static StartProjectData data = null;
        private static StartProjectData Data { get { if (data == null) { data = GetAllInstances<StartProjectData>().FirstOrDefault(); } return data; } }

        private Editor _editor;

        private void OnGUI()
        {
            if (!_editor)
                _editor = Editor.CreateEditor(this);
            if (_editor)
                _editor.OnInspectorGUI();
        }

        private void OnInspectorUpdate()
        {
            Repaint();
        }
        private static T[] GetAllInstances<T>() where T : ScriptableObject
        {
            string[] guids = AssetDatabase.FindAssets("t:" + typeof(T).Name);
            T[] a = new T[guids.Length];
            for (int i = 0; i < guids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                a[i] = AssetDatabase.LoadAssetAtPath<T>(path);
            }

            return a;
        }

        [CustomEditor(typeof(StartProjectTools), true)]
        public class StartProjectToolEditor : Editor
        {
            private Vector2 directoriesScroll = Vector2.zero;
            private Vector2 hierarchyScroll = Vector2.zero;
            public override void OnInspectorGUI()
            {
                if (Data == null)
                {
                    GUILayout.BeginVertical(EditorStyles.helpBox);

                    GUILayout.Label(EditorGUIUtility.IconContent("Error@2x"));
                    GUILayout.Label("Дата файл отсутствует в проекте");
                    GUILayout.Label("Create/Tools/StartProject/Data");

                    GUILayout.EndVertical();
                    return;
                }

                DrawDirectoryPanel();

                GUILayout.Space(15);

                DrawHierarchyPanel();

                GUILayout.Space(15);

                DrawControlPanel();
            }

            private void DrawDirectoryPanel()
            {
                GUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.MinHeight(100), GUILayout.MaxHeight(300));

                GUILayout.Label("Folders:");

                GUIContent folderNameContent = new GUIContent("Folder name:");

                directoriesScroll = GUILayout.BeginScrollView(directoriesScroll, GUILayout.MaxHeight(150));

                for (int i = 0; i < Data.Directories.Count; i++)
                {
                    DirectoryData directory = Data.Directories[i];

                    GUILayout.BeginHorizontal(EditorStyles.helpBox);

                    GUILayout.Label($"{i}.", GUILayout.Width(25));

                    directory.FolderName = EditorGUILayout.TextField(folderNameContent, directory.FolderName, EditorStyles.textField, GUILayout.Height(25));

                    if (GUILayout.Button(EditorGUIUtility.IconContent("Installed@2x"), GUILayout.Width(25), GUILayout.Height(25)))
                        InstantiateDirectory(directory);

                    if (GUILayout.Button(EditorGUIUtility.IconContent("d_winbtn_mac_close_h@2x"), GUILayout.Width(25), GUILayout.Height(25)))
                    {
                        Data.Directories.RemoveAt(i);
                        EditorUtility.SetDirty(Data);
                        break;
                    }

                    GUILayout.EndHorizontal();
                }

                GUILayout.EndScrollView();

                if (GUILayout.Button(new GUIContent("Setup", EditorGUIUtility.IconContent("Installed@2x").image)))
                {
                    InstantiateDirectories();
                }

                if (GUILayout.Button(new GUIContent("Add", EditorGUIUtility.IconContent("d_winbtn_mac_max_h@2x").image)))
                {
                    Data.Directories.Add(new DirectoryData());
                    EditorUtility.SetDirty(Data);
                }

                GUILayout.EndVertical();
            }

            private void DrawHierarchyPanel()
            {
                EditorGUI.BeginChangeCheck();

                GUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.MinHeight(100), GUILayout.MaxHeight(300));

                GUILayout.Label("Objects:");

                GUIContent folderNameContent = new GUIContent("Object name:");

                hierarchyScroll = GUILayout.BeginScrollView(hierarchyScroll, GUILayout.MaxHeight(150));

                for (int i = 0; i < Data.HierarchyObjects.Count; i++)
                {
                    HierarchyData hierarchy = Data.HierarchyObjects[i];

                    GUILayout.BeginVertical(EditorStyles.helpBox);

                    GUILayout.BeginHorizontal();

                    GUILayout.Label($"{i}.", GUILayout.Width(25));

                    hierarchy.ObjectName = EditorGUILayout.TextField(folderNameContent, hierarchy.ObjectName, EditorStyles.textField, GUILayout.Height(25));

                    if (GUILayout.Button(EditorGUIUtility.IconContent("Installed@2x"),GUILayout.Width(25), GUILayout.Height(25)))
                        InstantiateObject(hierarchy);

                    if (GUILayout.Button(EditorGUIUtility.IconContent("d_winbtn_mac_close_h@2x"), GUILayout.Width(25), GUILayout.Height(25)))
                    {
                        Data.HierarchyObjects.RemoveAt(i);
                        EditorUtility.SetDirty(Data);
                        break;
                    }

                    GUILayout.EndHorizontal();

#if COLORED_HEADERS
                    hierarchy.ColorCollapsed = EditorGUILayout.Foldout(hierarchy.ColorCollapsed, "Color data");

                    if (!hierarchy.ColorCollapsed)
                    {
                        hierarchy.ColorData.HeaderColor = EditorGUILayout.ColorField("Header: ", hierarchy.ColorData.HeaderColor);
                        hierarchy.ColorData.TextColor = EditorGUILayout.ColorField("Text: ", hierarchy.ColorData.TextColor);
                        hierarchy.ColorData.ChildColor = EditorGUILayout.ColorField("Child: ", hierarchy.ColorData.ChildColor);
                        hierarchy.ColorData.ChildTextColor = EditorGUILayout.ColorField("Child text:", hierarchy.ColorData.ChildTextColor);
                        hierarchy.ColorData.HeaderIcon = EditorGUILayout.TextField("Icon: ", hierarchy.ColorData.HeaderIcon);
                    }
#endif

                    GUILayout.EndVertical();

                }

                GUILayout.EndScrollView();

                if (GUILayout.Button(new GUIContent("Setup", EditorGUIUtility.IconContent("Installed@2x").image)))
                {
                    InstantiateObjects();
                }

                if (GUILayout.Button(new GUIContent("Add", EditorGUIUtility.IconContent("d_winbtn_mac_max_h@2x").image)))
                {
                    Data.HierarchyObjects.Add(new HierarchyData());
                    EditorUtility.SetDirty(Data);
                }

                GUILayout.EndVertical();

                if (EditorGUI.EndChangeCheck())
                    EditorUtility.SetDirty(Data);
            }

            private void DrawControlPanel()
            {
                if (GUILayout.Button(new GUIContent("Setup", EditorGUIUtility.IconContent("Installed@2x").image)))
                {
                    InstantiateDirectories();

                    InstantiateObjects();
                }
            }

            private void InstantiateObjects()
            {
                foreach (var item in Data.HierarchyObjects)
                    InstantiateObject(item);
            }

            private void InstantiateObject(HierarchyData data)
            {
                GameObject createdObject = new GameObject(data.ObjectName);

#if COLORED_HEADERS
                ColoredHeader.ColoredHeader header = createdObject.AddComponent<ColoredHeader.ColoredHeader>();

                header.SetupData(data.ColorData.Clone() as ColoredData);
#endif

                EditorUtility.SetDirty(createdObject);
            }

            private void InstantiateDirectories()
            {
                foreach (var item in Data.Directories)
                    InstantiateDirectory(item);

                AssetDatabase.Refresh();
            }

            private void InstantiateDirectory(DirectoryData data, bool withRefresh = false)
            {
                AssetDatabase.CreateFolder("Assets", data.FolderName);

                if (withRefresh)
                    AssetDatabase.Refresh();
            }
        }

        [System.Serializable]
        public class DefaultStringProperty
        {
            [SerializeField]
            public string StringValue;
            [SerializeField]
            public bool BoolValue = true;

            public DefaultStringProperty(string stringValue)
            {
                StringValue = stringValue;
            }
        }
    }
}
