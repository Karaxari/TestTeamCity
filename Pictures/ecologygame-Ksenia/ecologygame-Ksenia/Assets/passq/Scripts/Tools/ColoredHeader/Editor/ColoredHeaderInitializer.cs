using UnityEngine;
using UnityEditor;

namespace ColoredHeader
{
    [InitializeOnLoad]
    public static class ColoredHeaderInitializer
    {
        static GUIStyle fontStyle = null;
        static GUIStyle childFontStyle = null;
        static GUIContent childContent = null;
        static GUIContent headerContent = null;
        static ColoredHeaderInitializer()
        {
            PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, "COLORED_HEADERS");
             
            EditorApplication.hierarchyWindowItemOnGUI += HierarchyWindowItemOnGUI;
        }

        static void InitializeStyles()
        {
            fontStyle = new GUIStyle();
            fontStyle.fontStyle = FontStyle.Bold;
            fontStyle.alignment = TextAnchor.MiddleCenter;
            fontStyle.normal.textColor = Color.white;

            childFontStyle = new GUIStyle(EditorStyles.label);

            childContent = new GUIContent();
            headerContent = new GUIContent();
        }

        static void HierarchyWindowItemOnGUI(int instanceID, Rect selectionRect)
        {
            if (childFontStyle == null || fontStyle == null)
                InitializeStyles();

            GameObject gameObject = EditorUtility.InstanceIDToObject(instanceID) as GameObject;

            if (gameObject == null)
                return;

            ColoredHeader header;

            if (gameObject.TryGetComponent<ColoredHeader>(out header))
            {
                selectionRect.xMax += 15;

                fontStyle.normal.textColor = header.TextColor;

                EditorGUI.DrawRect(selectionRect, header.HeaderColor);

                headerContent.text = gameObject.name.ToUpperInvariant();
                if (header.HeaderIcon != string.Empty)
                    try
                    {
                        headerContent.image = EditorGUIUtility.IconContent(header.HeaderIcon).image;
                    }
                    catch (System.Exception ex)
                    {
                        header.HeaderIcon = string.Empty;
                        headerContent.image = null;
                        EditorUtility.SetDirty(header);
                    }
                else
                    headerContent.image = null;

                EditorGUI.LabelField(selectionRect, headerContent, fontStyle);
                return;
            }

            if (gameObject.GetComponentInParent<ColoredHeader>())
            {
                header = gameObject.GetComponentInParent<ColoredHeader>();

                selectionRect.xMax += 15;

                childFontStyle.normal.textColor = header.ChildTextColor;

                EditorGUI.DrawRect(selectionRect, header.ChildColor);

                childContent.text = gameObject.name;
                childContent.image = EditorGUIUtility.IconContent("d_GameObject Icon").image;

                EditorGUI.LabelField(selectionRect, childContent, childFontStyle);
                return;
            }
        }
    }
}
