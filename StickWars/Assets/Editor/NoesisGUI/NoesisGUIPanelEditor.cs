using UnityEditor;
using UnityEngine;
using Noesis;
using System.IO;
using System.Collections.Generic;

[CustomEditor(typeof(NoesisGUIPanel))]
public class NoesisGUIPanelEditor : Editor
{
    ////////////////////////////////////////////////////////////////////////////////////////////////
    public override void OnInspectorGUI()
    {
        NoesisGUIPanel Target = target as NoesisGUIPanel;

        // Xaml File
        EditorGUILayout.Space();
        Object newXaml = EditorGUILayout.ObjectField(new GUIContent("Xaml",
            "Drop here a xaml file that defines the user interface"),
            Target._xaml, typeof(Object), false);

        UpdateXamlPath(Target, newXaml);
        
        // Resources File
        Object newStyle = EditorGUILayout.ObjectField(new GUIContent("Style",
            "Drop here a xaml file that defines a ResourceDictionary with custom styles and resources"),
            Target._style, typeof(Object), false);

        UpdateStylePath(Target, newStyle);
        
        // Renderer Settings
        EditorGUILayout.BeginVertical();

        Target._antiAliasingMode = (AntialiasingMode)EditorGUILayout.EnumPopup(new GUIContent("Antialiasing",
            "Antialiasing Mode: MSAA=Uses hardware multisample, PPA=Propietary GPU accelerated antialiasing algorithm"),
            Target._antiAliasingMode);

        Target._tessellationQuality = (TessellationQuality)EditorGUILayout.EnumPopup(new GUIContent("Quality",
            "Specifies tessellation quality"), Target._tessellationQuality);

        Target._offscreenSize.x = EditorGUILayout.Slider(new GUIContent("Offscreen Width",
            "Specifies offscreen surface width relative to main surface width. Offscreen surface is used for opacity groups and visual brushes. A 0 size disables this feature"),
            Target._offscreenSize.x, 0, 10);
        Target._offscreenSize.y = EditorGUILayout.Slider(new GUIContent("Offscreen Height",
            "Specifies offscreen surface height relative to main surface height. Offscreen surface is used for opacity groups and visual brushes. A 0 size disables this feature"),
            Target._offscreenSize.y, 0, 10);

        Target._useRealTimeClock = EditorGUILayout.Toggle(new GUIContent("Real Time Clock",
            "When enabled, Time.realtimeSinceStartup is used instead of Time.time for animations"),
            Target._useRealTimeClock);

        EditorGUILayout.EndVertical();
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////
    public static string GetXamlPath(string path, string errorMessage)
    {
        if (path == "")
        {
            return "";
        }

        if (System.IO.Path.GetExtension(path) != ".xaml")
        {
            Debug.LogError(errorMessage);
            return "";
        }

        return path;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////
    public static void UpdateXamlPath(NoesisGUIPanel noesisGUI, Object xaml)
    {
        string path = GetXamlPath(AssetDatabase.GetAssetPath(xaml), 
            "Xaml property accepts only .xaml assets");

        if (path != "")
        {
            noesisGUI._xaml = xaml;
            noesisGUI._xamlFile = path;
        }
        else
        {
            noesisGUI._xaml = null;
            noesisGUI._xamlFile = "";
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////
    public static void UpdateStylePath(NoesisGUIPanel noesisGUI, Object style)
    {
        string path = GetXamlPath(AssetDatabase.GetAssetPath(style),
            "Style property accepts only .xaml assets");

        if (path != "")
        {
            noesisGUI._style = style;
            noesisGUI._styleFile = path;
        }
        else
        {
            noesisGUI._style = null;
            noesisGUI._styleFile = "";
        }
    }
}
