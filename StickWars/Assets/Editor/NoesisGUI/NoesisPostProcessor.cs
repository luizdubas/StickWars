using UnityEditor;
using UnityEngine;
using Noesis;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Globalization;


public class NoesisPostProcessor : AssetPostprocessor
{
    ////////////////////////////////////////////////////////////////////////////////////////////
    private static string GenGuid()
    {
        return System.Guid.NewGuid().ToString().Replace("-", "").ToUpper();
    }

    ////////////////////////////////////////////////////////////////////////////////////////////
    static void CreateMakeFile(string asset)
    {
        string importer;

        if (System.IO.Path.GetExtension(asset) == ".xaml")
        {
            importer = "XamlImporter";
        }
        else if (System.IO.Path.GetExtension(asset) == ".font")
        {
            importer = "FontImporter";
        }
        else
        {
            throw new System.Exception(System.String.Format("Unknown resource {0}", asset));
        }

        if (!asset.StartsWith("Assets/"))
        {
            throw new System.Exception(System.String.Format("Unexpected asset {0}", asset));
        }

        System.IO.StreamWriter file = new System.IO.StreamWriter(asset + ".make");
        file.WriteLine("guid = \"" + GenGuid() + "\"");
        file.WriteLine("sources = [\"$SELF\"]");
        file.WriteLine("importer = \"" + importer + "\"");
        file.WriteLine("ImporterOptions =");
        file.WriteLine("{");
        file.WriteLine("}");
        file.Close ();
    }

    ////////////////////////////////////////////////////////////////////////////////////////////
    private static void OnAssetDeleted(string asset, ref bool doScan)
    {
        string assetExt = System.IO.Path.GetExtension(asset);
        if (assetExt == ".make")
        {
            string make = asset;

            string resource = System.IO.Path.GetDirectoryName(make) + "/" +
                System.IO.Path.GetFileNameWithoutExtension(make);
            string resourceExt = System.IO.Path.GetExtension(resource);
            if (resourceExt == ".xaml" || resourceExt == ".font")
            {
                if (File.Exists(resource))
                {
                    // .xaml and .font resources must have its corresponding .make to be used
                    CreateMakeFile(resource);

                    // update resource GUID in the DB
                    doScan = true;
                }
                else
                {
                    // .xaml or .font resource deleted, update DB and remove cache
                    doScan = true;
                }
            }
        }
        else if (assetExt == ".xaml" || assetExt == ".font")
        {
            string resource = asset;

            // .xaml or .font resource deleted, update DB and remove cache
            doScan = true;

            string make = resource + ".make";
            if (File.Exists(make))
            {
                // remove corresponding .make
                File.Delete(make);
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////
    private static void OnAssetAdded(string asset, ref bool doScan, ref bool doBuild)
    {
        string assetExt = System.IO.Path.GetExtension(asset);
        if (assetExt == ".make")
        {
            string make = asset;

            string resource = System.IO.Path.GetDirectoryName(make) + "/" +
                System.IO.Path.GetFileNameWithoutExtension(make);
            string resourceExt = System.IO.Path.GetExtension(resource);
            if (resourceExt == ".xaml" || resourceExt == ".font")
            {
                if (!File.Exists(resource))
                {
                    // .make don't needed if corresponding resource does not exist
                    File.Delete(make);
                }
                else
                {
                    // .xaml or .font resource added
                    doScan = true;
                }
            }
        }
        else if (assetExt == ".xaml" || assetExt == ".font")
        {
            string resource = asset;

            string make = resource + ".make";
            if (!File.Exists(make))
            {
                // .xaml and .font resources must have its corresponding .make to be used
                CreateMakeFile(resource);

                // update resource GUID in the DB
                doScan = true;
            }

            doBuild = true;
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////
    private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets,
        string[] movedAssets, string[] movedFromPath)
    {
        bool doScan = false;
        bool doBuild = false;

        for (int i = 0; i < movedAssets.Length; ++i)
        {
            OnAssetDeleted(movedFromPath[i], ref doScan);
            OnAssetAdded(movedAssets[i], ref doScan, ref doBuild);
        }

        foreach (string asset in deletedAssets)
        {
            OnAssetDeleted(asset, ref doScan);
        }

        foreach (string asset in importedAssets)
        {
            OnAssetAdded(asset, ref doScan, ref doBuild);
        }

        if (doScan || doBuild)
        {
            if (!EditorApplication.isPlaying)
            {
                NoesisSettings.ClearLog();

                if ((NoesisSettings.ActivePlatforms & NoesisSettings.DX9Platform) > 0)
                {
                    Build("DX9", doScan, doBuild);
                }
                if ((NoesisSettings.ActivePlatforms & NoesisSettings.GLPlatform) > 0)
                {
                    Build("GL", doScan, doBuild);
                }
                if ((NoesisSettings.ActivePlatforms & NoesisSettings.IOSPlatform) > 0)
                {
                    Build("IOS", doScan, doBuild);
                }
                if ((NoesisSettings.ActivePlatforms & NoesisSettings.AndroidPlatform) > 0)
                {
                    Build("ANDROID", doScan, doBuild);
                }

                UpdateNoesisGUIPaths();
            }
            else
            {
                Debug.LogWarning("Can't build NoesisGUI resources while playing. " +
                    "Please build them manually using NoesisGUI Settings Build button");
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////
    private static void Build(string platform, bool doScan, bool doBuild)
    {
        using (BuildToolKernel builder = new BuildToolKernel(platform))
        {
            if (doScan)
            {
                builder.Scan();
            }

            if (doBuild)
            {
                builder.Build();
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////
    private static void UpdateNoesisGUIPaths()
    {
        UnityEngine.Object[] objs = UnityEngine.Object.FindObjectsOfType(typeof(NoesisGUIPanel));

        foreach (UnityEngine.Object obj in objs)
        {
            NoesisGUIPanel noesisGUI = (NoesisGUIPanel)obj;

            NoesisGUIPanelEditor.UpdateXamlPath(noesisGUI, noesisGUI._xaml);
            NoesisGUIPanelEditor.UpdateStylePath(noesisGUI, noesisGUI._style);
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////
    static NoesisPostProcessor()
    {
        UnityEditor.EditorApplication.update += OnEditorUpdate;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////
    static void OnEditorUpdate()
    {
        if (UnityEditor.EditorApplication.isPlaying &&
            UnityEditor.EditorApplication.isCompiling)
        {
            // Call Noesis shutdown
            UnityEngine.Object.DestroyImmediate(GameObject.Find("NoesisGUISystem"));

            // Warn user
            UnityEditor.EditorUtility.DisplayDialog("Warning",
                "Modifying scripts while playing a scene is not supported by NoesisGUI",
                "Stop Player");

            // Stop playing
            UnityEditor.EditorApplication.ExecuteMenuItem("Edit/Play");
        }
    }
}
