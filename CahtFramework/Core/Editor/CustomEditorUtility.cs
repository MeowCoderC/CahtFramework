namespace CahtFramework
{
    using System;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;

    public static class CustomEditorUtility
    {
        private static GUIStyle titleStyle;

        private static GUIStyle TitleStyle
        {
            get
            {
                if (titleStyle == null)
                    titleStyle = new GUIStyle("ShurikenModuleTitle")
                    {
                        font          = new GUIStyle(EditorStyles.label).font,
                        fontStyle     = FontStyle.Bold,
                        fontSize      = 14,
                        border        = new RectOffset(15, 7, 4, 4),
                        fixedHeight   = 26f,
                        contentOffset = new Vector2(20f, -2f)
                    };

                return titleStyle;
            }
        }

        public static bool DrawFoldoutTitle(string title, bool isExpanded, float space = 15f)
        {
            EditorGUILayout.Space();

            var rect = GUILayoutUtility.GetRect(16f, TitleStyle.fixedHeight, TitleStyle);
            GUI.Box(rect, title, TitleStyle);

            var currentEvent = Event.current;
            var toggleRect   = new Rect(rect.x + 4f, rect.y + 4f, 13f, 13f);

            if (currentEvent.type == EventType.Repaint)
            {
                EditorStyles.foldout.Draw(toggleRect, false, false, isExpanded, false);
            }
            else if (currentEvent.type == EventType.MouseDown && rect.Contains(currentEvent.mousePosition))
            {
                isExpanded = !isExpanded;
                currentEvent.Use();
            }

            return isExpanded;
        }

        public static bool DrawFoldoutTitle(IDictionary<string, bool> isFoldoutExpandedesByTitle, string title, float space = 15f)
        {
            isFoldoutExpandedesByTitle.TryAdd(title, true);

            isFoldoutExpandedesByTitle[title] = DrawFoldoutTitle(title, isFoldoutExpandedesByTitle[title], space);

            return isFoldoutExpandedesByTitle[title];
        }

        public static void DrawUnderline(float height = 1f)
        {
            var lastRect = GUILayoutUtility.GetLastRect();
            lastRect.y      += height;
            lastRect.height =  height;
            EditorGUI.DrawRect(lastRect, Color.gray);
        }

        public static void EnsureFolderExists(string folderPath)
        {
            if (AssetDatabase.IsValidFolder(folderPath)) return;

            var folders     = folderPath.Split('/');
            var currentPath = folders[0];

            for (var i = 1; i < folders.Length; i++)
            {
                var nextPath = currentPath + "/" + folders[i];
                if (!AssetDatabase.IsValidFolder(nextPath)) AssetDatabase.CreateFolder(currentPath, folders[i]);
                currentPath = nextPath;
            }
        }

        public static void DrawEnumToolbar(SerializedProperty enumProperty)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(enumProperty.displayName);
            enumProperty.enumValueIndex = GUILayout.Toolbar(enumProperty.enumValueIndex, enumProperty.enumDisplayNames);
            EditorGUILayout.EndHorizontal();
        }

        public static void DeepCopySerializeReference(SerializedProperty property)
        {
            if (property.managedReferenceValue == null)
                return;

            property.managedReferenceValue = (property.managedReferenceValue as ICloneable).Clone();
        }

        public static void DeepCopySerializeReferenceArray(SerializedProperty property, string fieldName = "")
        {
            for (var i = 0; i < property.arraySize; i++)
            {
                var elementProperty = property.GetArrayElementAtIndex(i);
                if (!string.IsNullOrEmpty(fieldName))
                    elementProperty = elementProperty.FindPropertyRelative(fieldName);

                if (elementProperty.managedReferenceValue == null)
                    continue;

                elementProperty.managedReferenceValue = (elementProperty.managedReferenceValue as ICloneable).Clone();
            }
        }
    }
}