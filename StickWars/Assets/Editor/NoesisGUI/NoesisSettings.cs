using UnityEditor;
using UnityEngine;
using Noesis;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Globalization;
using System.Reflection;


////////////////////////////////////////////////////////////////////////////////////////////////
public class NoesisSettings : EditorWindow
{
    public static int DX9Platform = 1;
    public static int GLPlatform = 2;
    public static int IOSPlatform = 4;
    public static int AndroidPlatform = 8;

    public static int ActivePlatforms
    {
        get { return ReadSettings(); }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////
    private static string DX9Field = "NoesisDX9";
    private static string GLField = "NoesisGL";
    private static string AndroidField = "NoesisAndroid";
    private static string IOSField = "NoesisIOS";

    private int activePlatforms_;

    ////////////////////////////////////////////////////////////////////////////////////////////
    public static void ClearLog()
    {
        Assembly assembly = Assembly.GetAssembly(typeof(SceneView));
        System.Type type = assembly.GetType("UnityEditorInternal.LogEntries");
        MethodInfo method = type.GetMethod("Clear");
        method.Invoke(new object(), null);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////
    private static int ReadSettings()
    {
        int activePlatforms = 0;

        UnityEngine.RuntimePlatform platform = UnityEngine.Application.platform;
        if (platform == UnityEngine.RuntimePlatform.WindowsEditor)
        {
            if (PlayerPrefs.GetInt(DX9Field, 1) > 0)
            {
                activePlatforms |= DX9Platform;
            }
        }

        if (PlayerPrefs.GetInt(GLField, 1) > 0)
        {
            activePlatforms |= GLPlatform;
        }

        if (PlayerPrefs.GetInt(IOSField, 0) > 0)
        {
            activePlatforms |= IOSPlatform;
        }

        if (PlayerPrefs.GetInt(AndroidField, 0) > 0)
        {
            activePlatforms |= AndroidPlatform;
        }

        return activePlatforms;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////
    private static void WriteSettings(int activePlatforms)
    {
        PlayerPrefs.SetInt(DX9Field, (activePlatforms & DX9Platform) > 0 ? 1 : 0);
        PlayerPrefs.SetInt(GLField, (activePlatforms & GLPlatform) > 0 ? 1 : 0);
        PlayerPrefs.SetInt(IOSField, (activePlatforms & IOSPlatform) > 0 ? 1 : 0);
        PlayerPrefs.SetInt(AndroidField, (activePlatforms & AndroidPlatform) > 0 ? 1 : 0);
        PlayerPrefs.Save();
    }
    
    ////////////////////////////////////////////////////////////////////////////////////////////
    [UnityEditor.MenuItem("Window/NoesisGUI/Settings", false, 30000)]
    static void ShowWindow()
    {
        // Get existing open window or if none, make a new one:
        NoesisSettings window = (NoesisSettings)EditorWindow.GetWindow(typeof(NoesisSettings));
        window.title = "NoesisGUI";

        window.activePlatforms_ = ReadSettings();
    }

    [UnityEditor.MenuItem("Window/NoesisGUI/Unity Tutorial", false, 30011)]
    static void OpenUnityTutorial()
    {
        UnityEngine.Application.OpenURL("file://" + UnityEngine.Application.dataPath +
            "/NoesisGUI/Docs/Gui.Core.Unity3DTutorial.html");
    }

    [UnityEditor.MenuItem("Window/NoesisGUI/Video Tutorial", false, 30012)]
    static void OpenVideoTutorial()
    {
        UnityEngine.Application.OpenURL("https://vimeo.com/65549290");
    }

    [UnityEditor.MenuItem("Window/NoesisGUI/Documentation", false, 30013)]
    static void OpenDocumentation()
    {
        UnityEngine.Application.OpenURL("file://" + UnityEngine.Application.dataPath +
            "/NoesisGUI/index.html");
    }

    [UnityEditor.MenuItem("Window/NoesisGUI/Forums", false, 30014)]
    static void OpenForum()
    {
        UnityEngine.Application.OpenURL("http://forums.noesisengine.com/");
    }
    
    ////////////////////////////////////////////////////////////////////////////////////////////    
    void OnGUI()
    {
        bool isCompiling = EditorApplication.isCompiling;

        GUI.enabled = !isCompiling;

        GUIStyle titleStyle = new GUIStyle(EditorStyles.whiteLabel);
        titleStyle.alignment = TextAnchor.MiddleCenter;
        titleStyle.fontStyle = UnityEngine.FontStyle.Bold;
        titleStyle.fontSize = 12;
        GUILayout.Label("NoesisGUI Settings", titleStyle);
        GUILayout.Label("Build Platforms", EditorStyles.boldLabel);

        int activePlatformsTmp = 0;
        int numActivePlatforms = 0;

        GUILayout.BeginHorizontal();

        UnityEngine.RuntimePlatform platform = UnityEngine.Application.platform;
        if (platform != UnityEngine.RuntimePlatform.WindowsEditor)
        {
            GUI.enabled = false;
        }
        if (GUILayout.Toggle((activePlatforms_ & DX9Platform) > 0, "DX9"))
        {
            activePlatformsTmp |= DX9Platform;
            numActivePlatforms++;
        }
        GUI.enabled = !isCompiling;

        if (GUILayout.Toggle((activePlatforms_ & GLPlatform) > 0, "GL"))
        {
            activePlatformsTmp |= GLPlatform;
            numActivePlatforms++;
        }

        if (GUILayout.Toggle((activePlatforms_ & IOSPlatform) > 0, "iOS"))
        {
            activePlatformsTmp |= IOSPlatform;
            numActivePlatforms++;
        }

        if (GUILayout.Toggle((activePlatforms_ & AndroidPlatform) > 0, "Android"))
        {
            activePlatformsTmp |= AndroidPlatform;
            numActivePlatforms++;
        }
        GUILayout.EndHorizontal();

        int delta = activePlatformsTmp ^ activePlatforms_;
        if (delta > 0)
        {
            activePlatforms_ = activePlatformsTmp;
            WriteSettings(activePlatforms_);
        }

        GUILayout.Space(15.0f);
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        
        if (GUILayout.Button(isCompiling ? "Unity is compiling scripts..." : "Build", GUILayout.MinWidth(100)))
        {
            try
            {
                ClearLog();

                int processNum = 0;
                float totalProcesses = 4.0f + numActivePlatforms;

                float progress = processNum++ / totalProcesses;
                EditorUtility.DisplayProgressBar("Building Resources", "Cleaning DX9", progress);
                Clean("DX9");

                progress = processNum++ / totalProcesses;
                EditorUtility.DisplayProgressBar("Building Resources", "Cleaning GL", progress);
                Clean("GL");

                progress = processNum++ / totalProcesses;
                EditorUtility.DisplayProgressBar("Building Resources", "Cleaning iOS", progress);
                Clean("IOS");

                progress = processNum++ / totalProcesses;
                EditorUtility.DisplayProgressBar("Building Resources", "Cleaning Android", progress);
                Clean("ANDROID");

                if ((activePlatforms_ & DX9Platform) > 0)
                {
                    progress = processNum++ / totalProcesses;
                    EditorUtility.DisplayProgressBar("Building Resources", "Build DX9", progress);
                    Build("DX9");
                }

                if ((activePlatforms_ & GLPlatform) > 0)
                {
                    progress = processNum++ / totalProcesses;
                    EditorUtility.DisplayProgressBar("Building Resources", "Build GL", progress);
                    Build("GL");
                }

                if ((activePlatforms_ & IOSPlatform) > 0)
                {
                    progress = processNum++ / totalProcesses;
                    EditorUtility.DisplayProgressBar("Building Resources", "Build iOS", progress);
                    Build("IOS");
                }

                if ((activePlatforms_ & AndroidPlatform) > 0)
                {
                    progress = processNum++ / totalProcesses;
                    EditorUtility.DisplayProgressBar("Building Resources", "Build Android", progress);
                    Build("ANDROID");
                }

                EditorUtility.DisplayProgressBar("Building Resources", "", 1.0f);
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }

        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUI.enabled = true;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////    
    private static void Clean(string platform)
    {
        using (BuildToolKernel builder = new BuildToolKernel(platform))
        {
            builder.Clean();
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////    
    private static void Build(string platform)
    {
        using (BuildToolKernel builder = new BuildToolKernel(platform))
        {
            builder.Scan();
            builder.Build();
        }
    }
}
