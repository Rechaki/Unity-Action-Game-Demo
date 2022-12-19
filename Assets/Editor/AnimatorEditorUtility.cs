using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using System.IO;

public static class AnimatorEditorUtility
{

    [MenuItem("Assets/Export Animator State", priority = 0)]
    static void ExportAnimatorState() {
        var selectionCount = Selection.objects.Length;
        if (selectionCount == 1)
        {
            if (Selection.objects[0] is AnimatorController)
            {
                var animatorController = Selection.objects[0] as AnimatorController;
                var stateMachine = animatorController.layers[0].stateMachine;
                string path = "Resources/Data/" + animatorController.name + ".txt";
                string content = "";
                var result = new Dictionary<string, string>();
                GetAllStatesAndFullPaths(animatorController.layers[0].stateMachine, null, result);

                foreach (var item in result)
                {
                    if (string.IsNullOrEmpty(content))
                    {
                        content = item.Key;
                    }
                    else
                    {
                        content += "," + item.Key;
                    }
                }

                CreateText(path, content);

            }

        }
    }


    private static void GetAllStatesAndFullPaths(AnimatorStateMachine stateMachine, string parentPath, Dictionary<string, string> result) {
        if (!string.IsNullOrEmpty(parentPath))
        {
            parentPath += ".";
        }
        parentPath += stateMachine.name;

        foreach (var state in stateMachine.states)
        {
            var stateFullPath = $"{parentPath}.{state.state.name}";
            if (result.ContainsKey(state.state.name) == false)
            {
                result.Add(state.state.name, stateFullPath);
            }
        }

        foreach (var subStateMachine in stateMachine.stateMachines)
        {
            GetAllStatesAndFullPaths(subStateMachine.stateMachine, parentPath, result);
        }

    }

    static void CreateText(string path, string content) {
        string fullPath = Application.dataPath + path;
        File.WriteAllText(fullPath, content);
        Debug.Log(fullPath);
        Debug.Log(content);
    }
}