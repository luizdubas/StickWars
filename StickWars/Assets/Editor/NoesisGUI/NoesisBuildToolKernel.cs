using UnityEditor;
using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Reflection;

namespace Noesis
{
    ////////////////////////////////////////////////////////////////////////////////////////////////
    public partial class BuildToolKernel: IDisposable
    {
        private Library library_ = null;
        private static string platform_ = null;

        private delegate void LogCallback(int severity, string message);
        private delegate void RegisterLogCallbackDelegate(LogCallback callback);
        private RegisterLogCallbackDelegate registerLogCallback_ = null;

        private delegate void InitKernelDelegate(string platform, string assetsPath, string streamingAssetsPath);
        private InitKernelDelegate initKernel_ = null;

        private delegate void ShutdownKernelDelegate();
        private ShutdownKernelDelegate shutdownKernel_ = null;

        private delegate void ScanDelegate();
        private ScanDelegate scan_ = null;

        private delegate void BuildDelegate();
        private BuildDelegate build_ = null;

        private delegate void RebuildDelegate();
        private BuildDelegate rebuild_ = null;

        private delegate void CleanDelegate();
        private CleanDelegate clean_ = null;

        public BuildToolKernel(string platform)
        {
            library_ = new Library(UnityEngine.Application.dataPath + "/Editor/NoesisGUI/BuildTool/Noesis");
            platform_ = platform;

            try
            {
                registerLogCallback_ = library_.Find<RegisterLogCallbackDelegate>("Noesis_RegisterLogCallback");
                initKernel_ = library_.Find<InitKernelDelegate>("Noesis_InitBuildTool");
                shutdownKernel_ = library_.Find<ShutdownKernelDelegate>("Noesis_ShutdownBuildTool");
                scan_ = library_.Find<ScanDelegate>("Noesis_Scan");
                build_ = library_.Find<BuildDelegate>("Noesis_Build");
                rebuild_ = library_.Find<BuildDelegate>("Noesis_Rebuild");
                clean_ = library_.Find<CleanDelegate>("Noesis_Clean");

                registerLogCallback_(OnLog);

                Error.RegisterFunctions(library_);
                Log.RegisterFunctions(library_);
                Extend.RegisterFunctions(library_);
                NoesisGUI_PINVOKE.RegisterExtendFunctions(library_);
                Extend.RegisterCallbacks();

                initKernel_(platform, UnityEngine.Application.dataPath, UnityEngine.Application.streamingAssetsPath);
                Error.Check();

                Log.Info(String.Format("Host is Unity v{0}", UnityEngine.Application.unityVersion));

                Extend.RegisterExtendClasses();
            }
            catch(Exception e)
            {
                if (shutdownKernel_ != null)
                {
                    shutdownKernel_();
                }

                registerLogCallback_ = null;
                initKernel_ = null;
                shutdownKernel_ = null;
                scan_ = null;
                build_ = null;
                rebuild_ = null;
                clean_ = null;

                library_.Dispose();
                throw e;
            }
        }

        ~BuildToolKernel()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (shutdownKernel_ != null)
            {
                System.GC.Collect();
                System.GC.WaitForPendingFinalizers();
                shutdownKernel_();

                registerLogCallback_ = null;
                initKernel_ = null;
                shutdownKernel_ = null;
                scan_ = null;
                build_ = null;
                rebuild_ = null;
                clean_ = null;

                library_.Dispose();
            }

            System.GC.SuppressFinalize(this);
        }
        
        ////////////////////////////////////////////////////////////////////////////////////////////////
        public void Scan()
        {
            Log.Info(String.Format("SCAN {0}", platform_));
            scan_();
            Error.Check();
        }
        
        ////////////////////////////////////////////////////////////////////////////////////////////////
        public void Clean()
        {
            Log.Info(String.Format("CLEAN {0}", platform_));
            clean_();
            Error.Check();
        }
        
        ////////////////////////////////////////////////////////////////////////////////////////////////
        public void Build()
        {
            Log.Info(String.Format("BUILD {0}", platform_));
            build_();
            Error.Check();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////
        public void Rebuild()
        {
            Log.Info(String.Format("REBUILD {0}", platform_));
            rebuild_();
            Error.Check();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////
        private static string assetBeingProcessed_;

        ////////////////////////////////////////////////////////////////////////////////////////////////
        [MonoPInvokeCallback (typeof(LogCallback))]
        private static void OnLog(int severity, string message)
        {
            message = message.Replace("Unity/Unity/", "Assets/");

            switch (severity)
            {
                case 0: // Critical
                { 
                    Debug.LogError(String.Format("[{0}] {1}\n{2}", platform_, assetBeingProcessed_, message));
                    break;
                }
                case 10: // Warning
                {
                    Debug.LogWarning(String.Format("[{0}] {1}\n{2}", platform_, assetBeingProcessed_, message));
                    break;
                }
                case 20: // Info
                {
                    if (message.StartsWith("> Building "))
                    {
                        assetBeingProcessed_ = message.Substring(11);
                    }
                    break;
                }
                case 30: // Debug
                {
                    break;
                }
            }
        }
    }
}
