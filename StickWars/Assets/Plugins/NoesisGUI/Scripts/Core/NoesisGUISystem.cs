using UnityEngine;
using System;
using System.Runtime.InteropServices;

////////////////////////////////////////////////////////////////////////////////////////////////
/// NoesisGUI main system
////////////////////////////////////////////////////////////////////////////////////////////////
public class NoesisGUISystem : MonoBehaviour
{
    ////////////////////////////////////////////////////////////////////////////////////////////////
    private static NoesisGUISystem _instance = null;
    private static bool _isCreated = false;
    private static bool _isInitialized = false;

    ////////////////////////////////////////////////////////////////////////////////////////////////
    public static void Create()
    {
        if (!_isCreated)
        {
            GameObject go = new GameObject("NoesisGUISystem");
            _isCreated = true;

            go.AddComponent<NoesisGUISystem>();
            _instance = go.GetComponent<NoesisGUISystem>();
            _isInitialized = _instance != null;
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////
    public static bool IsInitialized { get { return _isInitialized; } }

    ////////////////////////////////////////////////////////////////////////////////////////////////
    public static T LoadXaml<T>(string xamlFile) where T : Noesis.BaseComponent
    {
        return _isInitialized ? _instance.Load<T>(xamlFile) : null;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////
    void Awake()
    {
        Init();
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////
    void Update()
    {
        Noesis_Tick();
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////
    void OnDestroy()
    {
        Shutdown();
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////
    private T Load<T>(string xamlFile) where T : Noesis.BaseComponent
    {
        IntPtr root = IntPtr.Zero;
        Noesis_LoadXAML(ref root, xamlFile);

        if (root == IntPtr.Zero)
        {
            return null;
        }

        Noesis.BaseComponent bc = new Noesis.BaseComponent(root, false);

        return bc.As<T>();
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////
    private void Init()
    {
        HookUnityGraphicsDevice();

#if UNITY_EDITOR
        try
        {
            _library = new Noesis.Library(UnityEngine.Application.dataPath +
                "/Editor/NoesisGUI/BuildTool/Noesis");

            RegisterFunctions(_library);
            Noesis.Error.RegisterFunctions(_library);
            Noesis.Log.RegisterFunctions(_library);
            Noesis.Extend.RegisterFunctions(_library);
            Noesis.UIRenderer.RegisterFunctions(_library);
            Noesis.NoesisGUI_PINVOKE.RegisterFunctions(_library);
#endif
            // In Windows we need to add dll's directory to enable the load of its dependencies
            // This can be removed when Noesis.dll has no dependencies
            // NOTE: Don't move this inside Library code or DoInit because it won't work
            //@{
            UnityEngine.RuntimePlatform platform = UnityEngine.Application.platform;
            if (platform == UnityEngine.RuntimePlatform.WindowsPlayer)
            {
                Noesis.LibraryHelper.SetLoadDirectory(UnityEngine.Application.dataPath + "/Plugins");
            }
            //@}

            DoInit();

#if UNITY_EDITOR
        }
        catch (System.Exception e)
        {
            if (_library != null)
            {
                _library.Dispose();
                _library = null;
            }

            _loadXAML = null;
            _initKernel = null;
            _shutdownKernel = null;
            _tickKernel = null;

            throw e;
        }
#endif

        // To avoid that this GameObject is destroyed when a new scene is loaded
        DontDestroyOnLoad(gameObject);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////
    private void HookUnityGraphicsDevice()
    {
        UnityEngine.RuntimePlatform platform = UnityEngine.Application.platform;
        if (platform == UnityEngine.RuntimePlatform.WindowsEditor ||
            platform == UnityEngine.RuntimePlatform.WindowsPlayer ||
            platform == UnityEngine.RuntimePlatform.OSXEditor ||
            platform == UnityEngine.RuntimePlatform.OSXPlayer)
        {
            // NOTE: We have to make an indirect call to the PInvoke function to avoid that
            //  referenced library gets loaded just by entering current function
            HookUnityGraphicsDeviceDesktop();
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////
    private void HookUnityGraphicsDeviceDesktop()
    {
        UnityInitDevice();
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////
    private void DoInit()
    {
        Noesis.Extend.RegisterCallbacks();

        int deviceType = Noesis_Init(UnityEngine.Application.streamingAssetsPath,
            UnityEngine.Application.dataPath + "/Plugins");

        Noesis.UIRenderer.SetDeviceType(deviceType);
        GL.InvalidateState();

        Noesis.Log.Info(String.Format("Host is Unity v{0}", UnityEngine.Application.unityVersion));

        Noesis.Extend.RegisterExtendClasses();
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////
    private void Shutdown()
    {
        System.Diagnostics.Debug.Assert(_isInitialized);
        _isInitialized = false;

        // Destroy UIRenderer of each GUI panel
        UnityEngine.Object[] objs = UnityEngine.Object.FindObjectsOfType(typeof(NoesisGUIPanel));
        foreach (UnityEngine.Object obj in objs)
        {
            NoesisGUIPanel gui = (NoesisGUIPanel)obj;
            gui.DestroyRenderer();
        }

        // Shutdown Noesis kernel
#if UNITY_EDITOR
        try
        {
#endif
            GC.Collect();
            GC.WaitForPendingFinalizers();

            Noesis_Shutdown();

#if UNITY_EDITOR
        }
        catch (System.Exception e)
        {
            Debug.LogError(e.Message);
        }
        finally
        {
            DisposeLibrary();
        }
#endif
    }


#if UNITY_EDITOR
    ////////////////////////////////////////////////////////////////////////////////////////////////
    private Noesis.Library _library = null;

    ////////////////////////////////////////////////////////////////////////////////////////////////
    private void DisposeLibrary()
    {
        if (_library != null)
        {
            _library.Dispose();
            _library = null;
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////
    private void RegisterFunctions(Noesis.Library lib)
    {
        _loadXAML = lib.Find<LoadXAMLDelegate>("Noesis_LoadXAML");
        _initKernel = lib.Find<InitKernelDelegate>("Noesis_Init");
        _shutdownKernel = lib.Find<ShutdownKernelDelegate>("Noesis_Shutdown");
        _tickKernel = lib.Find<TickKernelDelegate>("Noesis_Tick");
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////
    delegate void LoadXAMLDelegate(ref IntPtr root, string xamlFile);
    private LoadXAMLDelegate _loadXAML = null;
    private void Noesis_LoadXAML(ref IntPtr root, string xamlFile)
    {
        _loadXAML(ref root, xamlFile);
        Noesis.Error.Check();
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////
    delegate int InitKernelDelegate(string dataPath, string pluginsPath);
    private InitKernelDelegate _initKernel = null;
    private int Noesis_Init(string dataPath, string pluginsPath)
    {
        int deviceType = _initKernel(dataPath, pluginsPath);
        Noesis.Error.Check();

        return deviceType;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////
    delegate void ShutdownKernelDelegate();
    private ShutdownKernelDelegate _shutdownKernel = null;
    private void Noesis_Shutdown()
    {
        _shutdownKernel();
        Noesis.Error.Check();
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////
    delegate void TickKernelDelegate();
    private TickKernelDelegate _tickKernel = null;
    private void Noesis_Tick()
    {
        _tickKernel();
        Noesis.Error.Check();
    }
#else
    ////////////////////////////////////////////////////////////////////////////////////////////////
    #if UNITY_IPHONE || UNITY_XBOX360
    [DllImport("__Internal", EntryPoint="Noesis_LoadXAML")]
    #else
    [DllImport("Noesis", EntryPoint = "Noesis_LoadXAML")]
    #endif
    static extern void Noesis_LoadXAML(ref IntPtr root, string xamlFile);

    ////////////////////////////////////////////////////////////////////////////////////////////////
    #if UNITY_IPHONE || UNITY_XBOX360
    [DllImport("__Internal", EntryPoint="Noesis_Init")]
    #else
    [DllImport("Noesis", EntryPoint = "Noesis_Init")]
    #endif
    static extern int Noesis_Init(string dataPath, string pluginsPath);

    ////////////////////////////////////////////////////////////////////////////////////////////////
    #if UNITY_IPHONE || UNITY_XBOX360
    [DllImport("__Internal", EntryPoint="Noesis_Shutdown")]
    #else
    [DllImport("Noesis", EntryPoint = "Noesis_Shutdown")]
    #endif
    static extern void Noesis_Shutdown();

    ////////////////////////////////////////////////////////////////////////////////////////////////
    #if UNITY_IPHONE || UNITY_XBOX360
    [DllImport("__Internal", EntryPoint="Noesis_Tick")]
    #else
    [DllImport("Noesis", EntryPoint = "Noesis_Tick")]
    #endif
    static extern void Noesis_Tick();
#endif

    // Forces loading the library in charge of receiving Unity native events.
    // Note that this is only for non-mobile platforms.
    [DllImport("UnityRenderHook", EntryPoint = "UnityInitDevice")]
    private static extern void UnityInitDevice();
}
