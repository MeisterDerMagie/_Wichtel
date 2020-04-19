using System;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
using UnityEditor.SceneManagement;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Wichtel.Extensions{
public static class ComponentExtensions
{
    //Caution: Doesn't find components that are on "Don't Destroy on Load"-GameObjects!
    public static List<T> FindAllComponentsOfType<T>() where T : Component
    {
        List<T> components = new List<T>();
        
        //get all root objects
        var allRootObjects = new List<GameObject>();
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            var scene = SceneManager.GetSceneAt(i);
            allRootObjects.AddRange(scene.GetRootGameObjects());
        }
        
        //get all Components in root objects
        foreach (var rootObject in allRootObjects)
        {
            var componentsInChildrenAndOnRootobjectSelf = rootObject.GetComponentsInChildren<T>(true);
            components.AddRange(componentsInChildrenAndOnRootobjectSelf);
        }

        return components;
    }
    
    #if UNITY_EDITOR
    //Schiebt eine Component an den angegebenen Platz. 0 ist immer die Transform-Component

    /// <summary>
    /// Schiebt eine Component an den angegebenen Platz. 0 ist immer die Transform-Component!
    /// </summary>
    /// <param name="_component"></param>
    /// <param name="_index"> Die neue Position der Component auf dem GameObject. Achtung: Muss größer als 0 sein, weil 0 immer die Transform-Component ist! </param>
    public static void MoveComponentAtIndex(this Component _component, int _index)
    {
        if (_component.IsAssetOnDisk()) return;
        if(PrefabUtility.IsPartOfAnyPrefab(_component)) return;

        bool IsPrefabInstance(GameObject go) { return PrefabUtility.GetPrefabParent(go) != null && PrefabUtility.GetPrefabObject(go) != null; }
        Debug.Log(_component.gameObject.name + ": " + IsPrefabInstance(_component.gameObject));
        
        
        List<Component> components = new List<Component>(_component.gameObject.GetComponents<Component>());
        var indexOfThisComponent = components.IndexOf(_component);
        _index = Mathf.Clamp(_index, 1, components.Count);
        
        if (_index < indexOfThisComponent)
        {
            for (int i = indexOfThisComponent; i > 1; i--)
            {
                UnityEditorInternal.ComponentUtility.MoveComponentUp(_component);
            }
        }
        else if (_index > indexOfThisComponent)
        {
            for (int i = indexOfThisComponent; i < _index; i++)
            {
                UnityEditorInternal.ComponentUtility.MoveComponentDown(_component);
            }
        }
    }

    public static bool IsAssetOnDisk(this Component _component)
    {
        return PrefabUtility.IsPartOfPrefabAsset(_component) || IsEditingInPrefabMode(_component);
    }
    
    private static bool IsEditingInPrefabMode(Component _component)
    {
        if (EditorUtility.IsPersistent(_component))
        {
            // if the game object is stored on disk, it is a prefab of some kind, despite not returning true for IsPartOfPrefabAsset =/
            return true;
        }
        else
        {
            // If the GameObject is not persistent let's determine which stage we are in first because getting Prefab info depends on it
            var mainStage = StageUtility.GetMainStageHandle();
            var currentStage = StageUtility.GetStageHandle(_component.gameObject);
            if (currentStage != mainStage)
            {
                var prefabStage = PrefabStageUtility.GetPrefabStage(_component.gameObject);
                if (prefabStage != null)
                {
                    return true;
                }
            }
        }
        return false;
    }
    #endif
}
}