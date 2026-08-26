using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

// Created by Claude. See NamedByInteractionStrategyAttribute.cs for more details.

[CustomPropertyDrawer(typeof(InteractionStrategyHandler))]
[CustomPropertyDrawer(typeof(InteractionResult))]
public class InteractionStrategyLabelDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // 1. Use reflection to find the field marked [NamedByInteractionStrategy]
        //    on whichever struct type this drawer is currently handling.
        string strategyFieldName = GetNamedStrategyFieldName(fieldInfo.FieldType);

        SerializedProperty strategyProp = strategyFieldName != null
            ? property.FindPropertyRelative(strategyFieldName)
            : null;

        // 2. Build the display label
        string newLabel;

        if (strategyProp != null && strategyProp.objectReferenceValue != null)
        {
            // 3. Use the ScriptableObject's name as the label
            newLabel = strategyProp.objectReferenceValue.name;
        }
        else
        {
            newLabel = "Select Strategy...";
        }

        // 4. Draw the property with our custom label
        EditorGUI.PropertyField(position, property, new GUIContent(newLabel), true);
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, label, true);
    }

    /// <summary>
    /// Searches the given type (and its base types) for the first field
    /// decorated with [NamedByInteractionStrategy] and returns its name,
    /// or null if none is found.
    /// </summary>
    private static string GetNamedStrategyFieldName(Type type)
    {
        // Unwrap arrays (T[]) and generic collections (List<T>, etc.)
        if (type.IsArray)
            type = type.GetElementType();
        else if (type.IsGenericType)
            type = type.GetGenericArguments()[0];

        for (Type t = type; t != null && t != typeof(object); t = t.BaseType)
        {
            foreach (FieldInfo field in t.GetFields(
                BindingFlags.Instance |
                BindingFlags.Public |
                BindingFlags.NonPublic))
            {
                if (field.IsDefined(typeof(NamedByInteractionStrategyAttribute), inherit: true))
                    return field.Name;
            }
        }

        return null;
    }
}