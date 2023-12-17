using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(MinMaxSlider))]
public class MinMaxSliderDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (property.propertyType == SerializedPropertyType.Vector2)
        {
            Vector2 range = property.vector2Value;
            float min = range.x;
            float max = range.y;
            var attr = attribute as MinMaxSlider;

            EditorGUI.BeginChangeCheck();
            label.text = string.Format("{0} {1}",label.text,property.vector2Value.ToString());
            EditorGUI.MinMaxSlider(label, position, ref min, ref max, attr.min, attr.max);

            if (EditorGUI.EndChangeCheck())
            {
                range.x = min;
                range.y = max;
                property.vector2Value = range;

            }
        }
        else {
            EditorGUI.LabelField(position, label, "Use only with Vector2");
        }
    }
}