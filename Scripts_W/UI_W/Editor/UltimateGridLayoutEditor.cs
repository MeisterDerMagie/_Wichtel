//(c) copyright by Martin M. Klöckener
using System.Collections;
using System.Collections.Generic;
using Sirenix.Serialization;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;

namespace Haferbrei {
[CustomEditor(typeof(UltimateGridLayout))]
[CanEditMultipleObjects]
public class UltimateGridLayoutEditor : Editor
{
    private SerializedProperty m_Padding;
    private SerializedProperty spacing;
    SerializedProperty m_ChildAlignment;
    
    private SerializedProperty fitType;
    private SerializedProperty fillDirection;
    private SerializedProperty fillOriginHorizontal;
    private SerializedProperty fillOriginVertical;
    private SerializedProperty rows;
    private SerializedProperty columns;
    private SerializedProperty sizingX;
    private SerializedProperty sizingY;
    private SerializedProperty fixedCellSize;
    private SerializedProperty cellAspectRatio;
    private SerializedProperty ignoreInactiveChildren;
    private SerializedProperty childForceExpandX, childForceExpandY;
    private SerializedProperty fitContentSizeX, fitContentSizeY;
    
    void OnEnable()
    {
        m_Padding = serializedObject.FindProperty("m_Padding");
        spacing = serializedObject.FindProperty("spacing");
        m_ChildAlignment = serializedObject.FindProperty("m_ChildAlignment");

        fitType = serializedObject.FindProperty("fitType");
        fillDirection = serializedObject.FindProperty("fillDirection");
        fillOriginHorizontal = serializedObject.FindProperty("fillOriginHorizontal");
        fillOriginVertical = serializedObject.FindProperty("fillOriginVertical");
        rows = serializedObject.FindProperty("rows");
        columns = serializedObject.FindProperty("columns");
        sizingX = serializedObject.FindProperty("sizingX");
        sizingY = serializedObject.FindProperty("sizingY");
        fixedCellSize = serializedObject.FindProperty("fixedCellSize");
        cellAspectRatio = serializedObject.FindProperty("cellAspectRatio");
        ignoreInactiveChildren = serializedObject.FindProperty("ignoreInactiveChildren");
        childForceExpandX = serializedObject.FindProperty("childForceExpandX");
        childForceExpandY = serializedObject.FindProperty("childForceExpandY");
        fitContentSizeX = serializedObject.FindProperty("fitContentSizeX");
        fitContentSizeY = serializedObject.FindProperty("fitContentSizeY");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update(); //???
        
        EditorGUILayout.PropertyField(m_Padding, true);
        EditorGUILayout.PropertyField(spacing);
        EditorGUILayout.PropertyField(m_ChildAlignment, true);
        
        EditorGUILayout.PropertyField(fitType);
        EditorGUILayout.PropertyField(fillDirection);
        EditorGUILayout.PropertyField(fillOriginHorizontal);
        EditorGUILayout.PropertyField(fillOriginVertical);
        EditorGUILayout.PropertyField(rows);
        EditorGUILayout.PropertyField(columns);
        EditorGUILayout.PropertyField(sizingX);
        EditorGUILayout.PropertyField(sizingY);
        EditorGUILayout.PropertyField(fixedCellSize);
        EditorGUILayout.PropertyField(cellAspectRatio);
        EditorGUILayout.PropertyField(ignoreInactiveChildren);
        EditorGUILayout.PropertyField(childForceExpandX);
        EditorGUILayout.PropertyField(childForceExpandY);
        EditorGUILayout.PropertyField(fitContentSizeX);
        EditorGUILayout.PropertyField(fitContentSizeY);

        serializedObject.ApplyModifiedProperties();
    }
}
}