using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Reflection;

static public class AutoGetComponentEditor{

    // - - - - - - - - - - - - - - -  EDITOR - - - - - - - - - - - - - - - - - //
    static public string TrimTypeText(this string type)
    {
        type = type.Remove(0, type.IndexOf('$') + 1);
        type = type.Remove(type.LastIndexOf(">"));
        return type;
    }

    static public Type GetElementType(this SerializedProperty property)
    {
        string type;

        if (property.isArray)
        {
            property.arraySize = 1;
            type = property.GetArrayElementAtIndex(0).type;
        }
        else
            type = property.type;
        
        return type.TrimTypeText().GetTypeFromAssembly();
    }


    /// <summary>
    /// 自動判斷variable的型別,取得Children中所有的class
    /// </summary>
    static public void SetObjectValuesGetInChildren(this SerializedObject serializedObject, string variable)
    {
        serializedObject.Update();

        serializedObject.FindProperty(variable).arraySize = 1;
        string type = serializedObject.FindProperty(variable).GetArrayElementAtIndex(0).type.TrimTypeText();
        var compoments = (serializedObject.targetObject as Component).GetComponentsInChildren(type.GetTypeFromAssembly());
        serializedObject.FindProperty(variable).arraySize = compoments.Length;

        //Debug.Log(string.Format("serializedObject:{0} propertyClass:{1}", serializedObject.targetObject, type));
        for (int i = 0; i < compoments.Length; ++i)
            serializedObject.FindProperty(variable).GetArrayElementAtIndex(i).objectReferenceValue = compoments[i];

        serializedObject.ApplyModifiedProperties();
    }

    /// <summary>
    /// 自動判斷variable的型別，並取得Children上的class
    /// </summary>
    static public void SetObjectValueGetInChildren(this SerializedObject serializedObject, string variable)
    {
        var type = serializedObject.FindProperty(variable).type.TrimTypeText();

        //Debug.Log(string.Format("serializedObject:{0} propertyClass:{1}", serializedObject.targetObject, type));
        SetObjectValue(serializedObject
            , variable
            , (serializedObject.targetObject as Component).GetComponentInChildren(type.GetTypeFromAssembly()));
    }

    /// <summary>
    /// 自動判斷variable的型別，並取得Parent上的class
    /// </summary>
    static public void SetObjectValueGetInParent(this SerializedObject serializedObject, string variable)
    {
        var type = serializedObject.FindProperty(variable).type.TrimTypeText();

        //Debug.Log(string.Format("serializedObject:{0} propertyClass:{1}", serializedObject.targetObject, type));
        SetObjectValue(serializedObject
            , variable
            , (serializedObject.targetObject as Component).GetComponentInParent(type.GetTypeFromAssembly()));
    }

    /// <summary>
    /// 自動判斷variable的型別，並取得自身上的class
    /// </summary>
    static public void SetObjectValueGetInSelf(this SerializedObject serializedObject, string variable)
    {
        var type = serializedObject.FindProperty(variable).type.TrimTypeText();
        //Debug.Log(string.Format("serializedObject:{0} propertyClass:{1}" , serializedObject.targetObject, type));

        SetObjectValue(serializedObject
            , variable
            , (serializedObject.targetObject as Component).GetComponent(type));
    }

    /// <summary>
    /// 手動設置要賦予variable的值
    /// </summary>
    static public void SetObjectValue(this SerializedObject serializedObject, string variable, UnityEngine.Object value)
    {
        serializedObject.Update();
        var property = serializedObject.FindProperty(variable);
        if (property == null)
        {
            Debug.LogError(string.Format("@Not found #{0}# variable in #{1}# serializedObject.", variable, serializedObject));
            return;
        }

        if (property.objectReferenceValue == null)
            property.objectReferenceValue = value;

        serializedObject.ApplyModifiedProperties();
    }


    //auto get property
    static public SerializedProperty GetValuesInChildren(this SerializedProperty property)
    {
        return property.SetPropertyValues(
            (property.serializedObject.targetObject as Component)
            .GetComponentsInChildren(property.GetElementType())
            );
    }

    static public SerializedProperty GetValuesInParent(this SerializedProperty property)
    {
        return property.SetPropertyValues(
            (property.serializedObject.targetObject as Component)
            .GetComponentsInParent(property.GetElementType()));
    }

    static public SerializedProperty GetValuesInSelf(this SerializedProperty property)
    {
        return property.SetPropertyValues(
            (property.serializedObject.targetObject as Component)
            .GetComponents(property.GetElementType()));
    }


    static public SerializedProperty SetPropertyValues(this SerializedProperty property, params UnityEngine.Object[] values)
    {
        //property.serializedObject.Update();
        //check array
        //Debug.Log(property.propertyPath);
        string[] variableName = property.propertyPath.Split('.');
        property = property.serializedObject.FindProperty(variableName[0]);

        if (property.isArray)
        {
            property.arraySize = values.Length;

            for (int i = 0; i < values.Length; ++i)
                property.GetArrayElementAtIndex(i).objectReferenceValue = values[i];
        }
        else
        {
            if (property.objectReferenceValue == null && values.Length > 0)
                property.objectReferenceValue = values[0];
            //else
            //    property.objectReferenceValue = null;
        }

        property.serializedObject.ApplyModifiedProperties();
        return property;
    }

}


[CustomPropertyDrawer(typeof(AutoGetComponent))]
public class AutoGetComponentDrawer : PropertyDrawer
{
    override public void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        //Debug.Log("path:" + property.propertyPath);
        var component = (attribute as AutoGetComponent);
        var name = component.gameObjectName;
        //if has name and related
        if (!string.IsNullOrEmpty(name) && component.from >= 0)
        {
            var target = GameObject.Find(name).transform;
            switch ((attribute as AutoGetComponent).from)
            {
                case From.parent:
                    property.SetPropertyValues(target.GetComponentsInParent(property.GetElementType()));
                    break;
                case From.children:
                    property.SetPropertyValues(target.GetComponentsInChildren(property.GetElementType()));
                    break;
                default:
                    property.SetPropertyValues(target.GetComponents(property.GetElementType()));
                    break;
            }
        }
        else if (string.IsNullOrEmpty(name)) //not have name
        {
            switch ((attribute as AutoGetComponent).from)
            {
                case From.parent:
                    property.GetValuesInParent(); break;
                case From.children:
                    property.GetValuesInChildren(); break;
                default:
                    property.GetValuesInSelf(); break;
            }
        }
        else //only name
        {
            property.SetPropertyValues(GameObject.Find(name));
        }

        label = EditorGUI.BeginProperty(position, label, property);
        Rect contentPosition = EditorGUI.PrefixLabel(position, label);
        EditorGUI.indentLevel = 0;
        EditorGUI.PropertyField(contentPosition, property, GUIContent.none);
        EditorGUI.EndProperty();
        
    }

   
    public class AutoGetAssets : PropertyDrawer
    {


    }

}