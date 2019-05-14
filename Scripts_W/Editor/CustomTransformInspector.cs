using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;

[CustomEditor(typeof(Transform), true)]
[CanEditMultipleObjects]
public class CustomTransformInspector : Editor {

    //Unity's built-in editor
    Editor defaultEditor;
    Transform transform;
    protected static bool showWorldSpaceTransform = false; //for foldout

    void OnEnable()
    {
        //When this inspector is created, also create the built-in inspector
        defaultEditor = Editor.CreateEditor(targets, Type.GetType("UnityEditor.TransformInspector, UnityEditor"));
        transform = target as Transform;
    }

    void OnDisable()
    {
        //When OnDisable is called, the default editor we created should be destroyed to avoid memory leakage.
        //Also, make sure to call any required methods like OnDisable
        MethodInfo disableMethod = defaultEditor.GetType().GetMethod("OnDisable", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        if (disableMethod != null)
            disableMethod.Invoke(defaultEditor,null);
        DestroyImmediate(defaultEditor);
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.LabelField("Local Space", EditorStyles.boldLabel);
        defaultEditor.OnInspectorGUI();

        //Show World Space Transform
        EditorGUILayout.Space();

        GUIStyle foldoutStyle = new GUIStyle(EditorStyles.foldout);
        foldoutStyle.fontStyle = FontStyle.Bold;

        showWorldSpaceTransform = EditorGUILayout.Foldout(showWorldSpaceTransform, "World Space",foldoutStyle);

        if(showWorldSpaceTransform) //if fold out
        {
            transform.position = EditorGUILayout.Vector3Field("Position", transform.position);
            GUI.enabled = false; //make the following fields ReadOnly
            EditorGUILayout.Vector3Field("Rotation", transform.rotation.eulerAngles);
            EditorGUILayout.Vector3Field("Scale", transform.lossyScale);

            EditorGUILayout.Vector3Field("Size", GetSizeOfRenderer());

            GUI.enabled = true; //stop ReadOnly
        }
    }

    private Vector3 GetSizeOfRenderer()
    {
        Renderer renderer = transform.GetComponent<Renderer>();

        if(renderer != null)
        {
            return renderer.bounds.size;
        }
        else
        {
            return Vector3.zero;
        }
    }
}