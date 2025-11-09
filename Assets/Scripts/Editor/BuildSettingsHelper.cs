using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Linq;

/// <summary>
/// Editor utility to help manage Build Settings scenes.
/// </summary>
public class BuildSettingsHelper : EditorWindow
{
    [MenuItem("Tools/Build Settings Helper")]
    public static void ShowWindow()
    {
        GetWindow<BuildSettingsHelper>("Build Settings Helper");
    }

    private void OnGUI()
    {
        GUILayout.Label("Build Settings Scenes", EditorStyles.boldLabel);
        GUILayout.Space(10);

        // Display current scenes in build settings
        var scenes = EditorBuildSettings.scenes;

        if (scenes.Length == 0)
        {
            EditorGUILayout.HelpBox("No scenes in Build Settings!", MessageType.Warning);
        }
        else
        {
            for (int i = 0; i < scenes.Length; i++)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label($"{i}: {System.IO.Path.GetFileNameWithoutExtension(scenes[i].path)}");
                GUILayout.Label(scenes[i].enabled ? "✓" : "✗", GUILayout.Width(30));
                EditorGUILayout.EndHorizontal();
            }
        }

        GUILayout.Space(20);

        // Quick add buttons
        if (GUILayout.Button("Add All Scenes in Assets/Scenes"))
        {
            AddAllScenesFromFolder();
        }

        GUILayout.Space(10);

        if (GUILayout.Button("Open Build Settings Window"))
        {
            EditorWindow.GetWindow(System.Type.GetType("UnityEditor.BuildPlayerWindow,UnityEditor"));
        }
    }

    private void AddAllScenesFromFolder()
    {
        string[] guids = AssetDatabase.FindAssets("t:Scene", new[] { "Assets/Scenes" });
        var scenePaths = guids.Select(guid => AssetDatabase.GUIDToAssetPath(guid)).ToArray();

        if (scenePaths.Length == 0)
        {
            Debug.LogWarning("No scenes found in Assets/Scenes");
            return;
        }

        var buildScenes = scenePaths.Select(path => new EditorBuildSettingsScene(path, true)).ToArray();
        EditorBuildSettings.scenes = buildScenes;

        Debug.Log($"✅ Added {buildScenes.Length} scenes to Build Settings:");
        foreach (var scene in buildScenes)
        {
            Debug.Log($"  - {System.IO.Path.GetFileNameWithoutExtension(scene.path)}");
        }
    }
}
