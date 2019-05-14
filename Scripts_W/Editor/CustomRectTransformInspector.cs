using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;

[CustomEditor(typeof(RectTransform), true)]
[CanEditMultipleObjects]
public class CustomRectTransformInspector : Editor {

    //Unity's built-in editor
    Editor defaultEditor;
    RectTransform rectTransform;
    protected static bool showWorldSpaceTransform = false; //for foldout

    void OnEnable()
    {
        //When this inspector is created, also create the built-in inspector
        defaultEditor = Editor.CreateEditor(targets, Type.GetType("UnityEditor.RectTransformEditor, UnityEditor"));
        rectTransform = target as RectTransform; 
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

        showWorldSpaceTransform = EditorGUILayout.Foldout(showWorldSpaceTransform, "World Space and Local Position",foldoutStyle);

        if(showWorldSpaceTransform) //if fold out
        {
            GUI.enabled = false; //make the following fields ReadOnly
            rectTransform.position = EditorGUILayout.Vector3Field("Position", rectTransform.position);
            EditorGUILayout.Vector3Field("Rotation", rectTransform.rotation.eulerAngles);
            EditorGUILayout.Vector3Field("Scale", rectTransform.lossyScale);
            EditorGUILayout.Vector3Field("Local Position", rectTransform.localPosition);
            GUI.enabled = true; //stop ReadOnly
        }
    }
}

/*
[CustomEditor(typeof(RectTransform), true)]
public class RectTransformEditor : Editor
{
    
    private Editor editorInstance;
    private Type nativeEditor;
    private MethodInfo onSceneGui;
    private MethodInfo onValidate;
 
    private RectTransform rectTransform;
    protected static bool showWorldSpaceTransform = false; //for foldout

    public override void OnInspectorGUI()
    {
        editorInstance.OnInspectorGUI();
        // Code here

        GUI.enabled = false; //make the following fields ReadOnly
        EditorGUILayout.Vector3Field("Local Position", rectTransform.localPosition);
        GUI.enabled = true; //stop ReadOnly
        
        EditorGUILayout.Space();

        GUIStyle foldoutStyle = new GUIStyle(EditorStyles.foldout);
        foldoutStyle.fontStyle = FontStyle.Bold;

        showWorldSpaceTransform = EditorGUILayout.Foldout(showWorldSpaceTransform, "World Space",foldoutStyle); 

        if(showWorldSpaceTransform) //if fold out
        {
            GUI.enabled = false; //make the following fields ReadOnly
            EditorGUILayout.Vector3Field("World Position", rectTransform.position);
            EditorGUILayout.Vector3Field("World Rotation", rectTransform.rotation.eulerAngles);
            EditorGUILayout.Vector3Field("World Scale", rectTransform.lossyScale);
            GUI.enabled = true; //stop ReadOnly
        }
    }
 
    private void OnEnable()
    {
        if (nativeEditor == null)
            Initialize();
 
        nativeEditor.GetMethod("OnEnable",BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic)?.Invoke(editorInstance, null);
        onSceneGui = nativeEditor.GetMethod("OnSceneGUI", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        onValidate = nativeEditor.GetMethod("OnValidate",BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        rectTransform = target as RectTransform;
    }
 
    private void OnSceneGUI()
    {
        onSceneGui.Invoke(editorInstance, null);
    }
 
    private void OnDisable()
    {
        nativeEditor.GetMethod("OnDisable", BindingFlags.NonPublic | BindingFlags.Instance)?.Invoke(editorInstance, null);
    }
 
    private void Awake()
    {
        Initialize();
        nativeEditor.GetMethod("Awake", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)?.Invoke(editorInstance, null);
    }
 
    private void Initialize()
    {
        nativeEditor = Assembly.GetAssembly(typeof(Editor)).GetType("UnityEditor.RectTransformEditor");
        editorInstance = CreateEditor(target, nativeEditor);
    }
 
    private void OnDestroy()
    {
        nativeEditor.GetMethod("OnDestroy", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)?.Invoke(editorInstance, null);
    }
 
    private void OnValidate()
    {
        if (nativeEditor == null)
            Initialize();
 
        onValidate?.Invoke(editorInstance, null);
    }
 
    private void Reset()
    {
        if (nativeEditor == null)
            Initialize();
 
        nativeEditor.GetMethod("Reset", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)?.Invoke(editorInstance, null);
    }
}
*/