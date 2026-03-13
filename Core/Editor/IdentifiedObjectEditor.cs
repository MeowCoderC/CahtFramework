namespace CahtFramework
{
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;

    [CustomEditor(typeof(IdentifiedObject), true)]
    public class IdentifiedObjectEditor : Editor
    {
        private SerializedProperty iconProperty;
        private SerializedProperty idProperty;
        private SerializedProperty codeNameProperty;
        private SerializedProperty displayNameProperty;
        private SerializedProperty descriptionProperty;
        private GUIStyle           textAreaStyle;

        private readonly Dictionary<string, bool> isFoldoutExpandedesByTitle = new();

        protected virtual void OnEnable()
        {
            GUIUtility.keyboardControl = 0;

            this.iconProperty        = this.serializedObject.FindProperty("icon");
            this.idProperty          = this.serializedObject.FindProperty("id");
            this.codeNameProperty    = this.serializedObject.FindProperty("codeName");
            this.displayNameProperty = this.serializedObject.FindProperty("displayName");
            this.descriptionProperty = this.serializedObject.FindProperty("description");
        }

        private void StyleSetup()
        {
            if (this.textAreaStyle == null)
            {
                this.textAreaStyle          = new GUIStyle(EditorStyles.textArea);
                this.textAreaStyle.wordWrap = true;
            }
        }

        protected bool DrawFoldoutTitle(string text) { return CustomEditorUtility.DrawFoldoutTitle(this.isFoldoutExpandedesByTitle, text); }

        public override void OnInspectorGUI()
        {
            this.StyleSetup();

            this.serializedObject.Update();

            if (this.DrawFoldoutTitle("Infomation"))
            {
                EditorGUILayout.BeginHorizontal("HelpBox");
                {
                    this.iconProperty.objectReferenceValue = EditorGUILayout.ObjectField(GUIContent.none, this.iconProperty.objectReferenceValue,
                        typeof(Sprite), false, GUILayout.Width(65));

                    EditorGUILayout.BeginVertical();
                    {
                        EditorGUILayout.BeginHorizontal();
                        {
                            GUI.enabled = false;
                            EditorGUILayout.PrefixLabel("ID");
                            EditorGUILayout.PropertyField(this.idProperty, GUIContent.none);
                            GUI.enabled = true;
                        }
                        EditorGUILayout.EndHorizontal();

                        EditorGUI.BeginChangeCheck();
                        var prevCodeName = this.codeNameProperty.stringValue;
                        EditorGUILayout.DelayedTextField(this.codeNameProperty);
                        if (EditorGUI.EndChangeCheck())
                        {
                            var assetPath  = AssetDatabase.GetAssetPath(this.target);
                            var folderPath = assetPath.Substring(0, assetPath.LastIndexOf('/'));
                            var newName    = $"{this.target.GetType().Name.ToUpper()}_{this.codeNameProperty.stringValue}";
                            var newPath    = $"{folderPath}/{newName}.asset";

                            this.serializedObject.ApplyModifiedProperties();

                            var conflictingAsset = AssetDatabase.LoadAssetAtPath<IdentifiedObject>(newPath);

                            if (conflictingAsset != null && conflictingAsset != this.target)
                            {
                                var swap = EditorUtility.DisplayDialog("Name Conflict",
                                    $"The name '{this.codeNameProperty.stringValue}' is already in use by another {this.target.GetType().Name}. Do you want to swap their names?",
                                    "Swap Names", "Cancel");

                                if (swap)
                                {
                                    var tempName = $"TEMP_SWAP_{System.Guid.NewGuid()}";
                                    AssetDatabase.RenameAsset(newPath, tempName);

                                    AssetDatabase.RenameAsset(assetPath, newName);
                                    this.target.name = newName;

                                    var oldName = $"{this.target.GetType().Name.ToUpper()}_{prevCodeName}";
                                    AssetDatabase.RenameAsset($"{folderPath}/{tempName}.asset", oldName);
                                    conflictingAsset.name = oldName;

                                    var soConflict = new SerializedObject(conflictingAsset);
                                    soConflict.FindProperty("codeName").stringValue = prevCodeName;
                                    soConflict.ApplyModifiedProperties();

                                    EditorUtility.SetDirty(conflictingAsset);
                                    AssetDatabase.SaveAssets();
                                    Debug.Log($"[Editor] Successfully swapped names between '{oldName}' and '{newName}'.");
                                }
                                else
                                {
                                    this.codeNameProperty.stringValue = prevCodeName;
                                    this.serializedObject.ApplyModifiedProperties();
                                }
                            }
                            else
                            {
                                var message = AssetDatabase.RenameAsset(assetPath, newName);
                                if (string.IsNullOrEmpty(message))
                                {
                                    this.target.name = newName;
                                }
                                else
                                {
                                    this.codeNameProperty.stringValue = prevCodeName;
                                    this.serializedObject.ApplyModifiedProperties();
                                }
                            }
                        }

                        EditorGUILayout.PropertyField(this.displayNameProperty);
                    }
                    EditorGUILayout.EndVertical();
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginVertical("HelpBox");
                {
                    EditorGUILayout.LabelField("Description");
                    this.descriptionProperty.stringValue = EditorGUILayout.TextArea(this.descriptionProperty.stringValue, this.textAreaStyle, GUILayout.Height(60));
                }
                EditorGUILayout.EndVertical();
            }

            this.serializedObject.ApplyModifiedProperties();
        }
    }
}