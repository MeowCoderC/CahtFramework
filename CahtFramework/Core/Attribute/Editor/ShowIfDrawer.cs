namespace CahtFramework
{
    using UnityEditor;
    using UnityEngine;

    [CustomPropertyDrawer(typeof(ShowIfAttribute))]
    public class ShowIfDrawer : PropertyDrawer
    {
        private bool ShouldShow(SerializedProperty property)
        {
            var showIf = this.attribute as ShowIfAttribute;

            var conditionPath = property.propertyPath.Replace(property.name, showIf.ConditionFieldName);
            var conditionProp = property.serializedObject.FindProperty(conditionPath);

            if (conditionProp != null && conditionProp.propertyType == SerializedPropertyType.Boolean) return conditionProp.boolValue;

            return true;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (this.ShouldShow(property)) return EditorGUI.GetPropertyHeight(property, label, true);

            return -EditorGUIUtility.standardVerticalSpacing;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (this.ShouldShow(property)) EditorGUI.PropertyField(position, property, label, true);
        }
    }
}