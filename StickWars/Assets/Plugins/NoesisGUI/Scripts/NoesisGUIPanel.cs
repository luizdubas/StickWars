using UnityEngine;
using Noesis;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

[AddComponentMenu("NoesisGUI/NoesisGUI Panel")]
public class NoesisGUIPanel : MonoBehaviour
{
    public string _xamlFile;
    public Object _xaml;
    public string _styleFile;
    public Object _style;

    public Vector2 _offscreenSize;

    public AntialiasingMode _antiAliasingMode;
    public TessellationMode _tessellationMode;
    public TessellationQuality _tessellationQuality;
    public RendererFlags _renderFlags;

    public bool _useRealTimeClock;

    ////////////////////////////////////////////////////////////////////////////////////////////////
    private UIRenderer _uiRenderer;

    ////////////////////////////////////////////////////////////////////////////////////////////////
    public T GetRoot<T>() where T : BaseComponent
    {
        if (_uiRenderer != null)
        {
            return _uiRenderer.GetRoot<T>();
        }
        else
        {
            return null;
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////
    void Reset()
    {
        // Called once when component is attached to GameObject for the first time
        _offscreenSize = new Vector2(1, 1);
        _antiAliasingMode = AntialiasingMode.MSAA;
        _tessellationMode = TessellationMode.Threshold;
        _tessellationQuality = TessellationQuality.Medium;
        _renderFlags = RendererFlags.None;
        _useRealTimeClock = false;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////
    void Awake()
    {
        // Create NoesisGUI System
        NoesisGUISystem.Create();

        // Create UI Renderer
        if (NoesisGUISystem.IsInitialized && _xamlFile.Length > 0 && _uiRenderer == null)
        {
            FrameworkElement root = NoesisGUISystem.LoadXaml<FrameworkElement>(_xamlFile);
            if (root != null)
            {
                if (_styleFile != "")
                {
                    ResourceDictionary resources = NoesisGUISystem.LoadXaml<ResourceDictionary>(_styleFile);
                    if (resources != null)
                    {
                        root.GetResources().GetMergedDictionaries().Add(resources);
                    }
                    else
                    {
                        throw new System.Exception("Unable to load style xaml: " + _styleFile);
                    }
                }

                _uiRenderer = new UIRenderer(root, _offscreenSize, gameObject);
            }
            else
            {
                throw new System.Exception("Unable to load xaml: " + _xamlFile);
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////
    void Update()
    {
        if (_uiRenderer != null)
        {
            _uiRenderer.Update(_useRealTimeClock ? Time.realtimeSinceStartup : Time.time,
                _antiAliasingMode, _tessellationMode, _tessellationQuality, _renderFlags);
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////
    void OnWillRenderObject()
    {
        if (_uiRenderer != null)
        {
            _uiRenderer.RenderToTexture();
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////
    void OnPreRender()
    {
        if (_uiRenderer != null)
        {
            _uiRenderer.PreRender();
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////
    void OnPostRender()
    {
        if (_uiRenderer != null)
        {
            _uiRenderer.PostRender();
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////
    void OnGUI()
    {
        if (_uiRenderer != null)
        {
            _uiRenderer.ProcessEvent(UnityEngine.Event.current);
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////
    void OnDestroy()
    {
        DestroyRenderer();
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////
    public void DestroyRenderer()
    {
        // Destroy native UI renderer
        if (_uiRenderer != null)
        {
            _uiRenderer.Destroy();
            _uiRenderer = null;
        }
    }
}
