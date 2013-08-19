using UnityEngine;
using System;
using System.Runtime.InteropServices;

namespace Noesis
{
    public enum AntialiasingMode
    {
        MSAA,
        PPAA
    }

    public enum TessellationMode
    {
        Once,
        Always,
        Threshold
    }

    public enum TessellationQuality
    {
        Low,
        Medium,
        High
    }

    public enum RendererFlags
    {
        None = 0,
        Wireframe = 1,
        ColorBatches = 2,
        ShowMask = 4,
        FlipY = 8
    }

    /////////////////////////////////////////////////////////////////////////////////////
    /// Manages updates, render and input events of a Noesis UI panel
    /////////////////////////////////////////////////////////////////////////////////////
    public partial class UIRenderer
    {
        /////////////////////////////////////////////////////////////////////////////////
        private int _rendererId;
        private IntPtr _root;
        private Vector2 _offscreenSize;
        private Vector3 _mousePos;
        
        private GameObject _target;
        private RenderTexture _texture;
        
        /////////////////////////////////////////////////////////////////////////////////
        public UIRenderer(FrameworkElement root, Vector2 offscreenSize, GameObject target)
        {
            _root = FrameworkElement.getCPtr(root).Handle;
            _offscreenSize = offscreenSize;

            _rendererId = Noesis_CreateRenderer(_root);

            _mousePos = Input.mousePosition;
            
            _target = target;
            _texture = FindTexture();
        }

        /////////////////////////////////////////////////////////////////////////////////
        public void Destroy()
        {
            Noesis_NotifyDestroyRenderer(_rendererId);
        }

        /////////////////////////////////////////////////////////////////////////////////
        public void Update(double timeInSeconds, AntialiasingMode aaMode,
            TessellationMode tessMode, TessellationQuality tessQuality, RendererFlags flags)
        {
            UpdateSettings(aaMode, tessMode, tessQuality, flags);
            UpdateInputs();
            Noesis_UpdateRenderer(_rendererId, timeInSeconds);
        }

        /////////////////////////////////////////////////////////////////////////////////
        enum RenderEvent
        {
            // custom IDs
            PreRender = 2000,
            PostRender = 1000,
            RenderToTexture = 0
        };

        /////////////////////////////////////////////////////////////////////////////////
        public void PreRender()
        {
            IssueRenderEvent(RenderEvent.PreRender);
            if (_isGraphicsDeviceDirectX)
            {
                // (1) Calling GL.IssuePluginEvent() in DirectX in PreRender makes unity
                // scene not rendered. It seems that Unity code of GL.IssuePluginEvent()
                // sets some unexpected states for rendering the scene
                GL.InvalidateState();
            }
        }

        /////////////////////////////////////////////////////////////////////////////////
        public void PostRender()
        {
            IssueRenderEvent(RenderEvent.PostRender);
        }

        /////////////////////////////////////////////////////////////////////////////////
        public void RenderToTexture()
        {
            if (_texture != null)
            {
                // We force creation of texture so we can get the texture native pointer
                if (!_texture.IsCreated())
                {
                    _texture.Create();
                }

                IntPtr texturePtr = _texture.GetNativeTexturePtr();
                if (texturePtr != IntPtr.Zero)
                {
                    Noesis_UpdateRendererTexture(_rendererId, texturePtr,
                        _texture.width, _texture.height);

                    IssueRenderEvent(RenderEvent.RenderToTexture);
                    if (_isGraphicsDeviceDirectX)
                    {
                        // IDEM (1)
                        GL.InvalidateState();
                    }
                }
            }
        }

        /////////////////////////////////////////////////////////////////////////////////
        public T GetRoot<T>() where T : BaseComponent
        {
            IntPtr typeClassPtr = Extend.GetPtrForType(typeof(T));
            
            IntPtr handlePtr = Noesis.Extend.TryGetHandle(typeClassPtr, _root);

            if (handlePtr != System.IntPtr.Zero)
            {
                GCHandle gcHandle = GCHandle.FromIntPtr(handlePtr);
                return (T)gcHandle.Target;
            }
            else
            {
                return (T)Activator.CreateInstance(typeof(T), new object[] { _root, false });                
            }
        }

        /////////////////////////////////////////////////////////////////////////////////
        private bool _shiftPressed = false;
        public void ProcessEvent(UnityEngine.Event ev)
        {
            // Shift key is not correctly notified via ev.type and ev.keyCode
            if (ev.shift)
            {
                if (!_shiftPressed)
                {
                    _shiftPressed = true;
                    Noesis_KeyDown(_rendererId, (int)Key.Shift);
                }
            }
            else
            {
                if (_shiftPressed)
                {
                    _shiftPressed = false;
                    Noesis_KeyUp(_rendererId, (int)Key.Shift);
                }
            }

            switch (ev.type)
            {
                case EventType.MouseDown:
                {
                    Vector2 mouse = MousePosition(ev.mousePosition.x, ev.mousePosition.y);
                    
                    if (HitTest(mouse.x, mouse.y))
                    {
                        ev.Use();
                    }

                    Noesis_MouseButtonDown(_rendererId, (int)mouse.x, (int)mouse.y, ev.button);

                    if (ev.clickCount == 2)
                    {
                        Noesis_MouseDoubleClick(_rendererId, (int)mouse.x, (int)mouse.y, ev.button);
                    }

                    break;
                }
                case EventType.MouseUp:
                {
                    Vector2 mouse = MousePosition(ev.mousePosition.x, ev.mousePosition.y);

                    if (HitTest(mouse.x, mouse.y))
                    {
                        ev.Use();
                    }

                    Noesis_MouseButtonUp(_rendererId, (int)mouse.x, (int)mouse.y, ev.button);

                    break;
                }
                case EventType.KeyDown:
                {
                    if (ev.keyCode != KeyCode.None)
                    {
                        int noesisKeyCode = NoesisKeyCodes.Convert(ev.keyCode);
                        if (noesisKeyCode != 0)
                        {
                            Noesis_KeyDown(_rendererId, noesisKeyCode);
                        }
                    }
                    else if (ev.character != 0)
                    {
                        Noesis_Char(_rendererId, ev.character);
                    }

                    break;
                }
                case EventType.KeyUp:
                {
                    if (ev.keyCode != KeyCode.None)
                    {
                        int noesisKeyCode = NoesisKeyCodes.Convert(ev.keyCode);
                        if (noesisKeyCode != 0)
                        {
                            Noesis_KeyUp(_rendererId, noesisKeyCode);
                        }
                    }

                    break;
                }
            }
        }

        /////////////////////////////////////////////////////////////////////////////////
        private void UpdateSettings(AntialiasingMode aaMode, TessellationMode tessMode,
            TessellationQuality tessQuality, RendererFlags flags)
        {
            // update renderer size
            if (_texture == null)
            {
                Noesis_RendererSurfaceSize(_rendererId, Screen.width, Screen.height,
                    _offscreenSize.x, _offscreenSize.y, QualitySettings.antiAliasing);

                if (_isGraphicsDeviceDirectX)
                {
                    Camera camera = _target.camera != null ? _target.camera : Camera.main;
                    if (camera != null &&
                        camera.actualRenderingPath == RenderingPath.DeferredLighting)
                    {
                        flags |= RendererFlags.FlipY;
                    }
                }
            }
            else // Render to Texture
            {
                System.Diagnostics.Debug.Assert(_texture.width > 0);
                System.Diagnostics.Debug.Assert(_texture.height > 0);

                Noesis_RendererSurfaceSize(_rendererId, _texture.width, _texture.height,
                    _offscreenSize.x, _offscreenSize.y, QualitySettings.antiAliasing);

                if (_isGraphicsDeviceOpenGL)
                {
                    flags |= RendererFlags.FlipY;
                }
            }

            // update renderer settings
            Noesis_RendererAntialiasingMode(_rendererId, (int)aaMode);
            Noesis_RendererTessMode(_rendererId, (int)tessMode);
            Noesis_RendererTessQuality(_rendererId, (int)tessQuality);
            Noesis_RendererFlags(_rendererId, (int)flags);
        }

        /////////////////////////////////////////////////////////////////////////////////
        private void UpdateInputs()
        {
            // mouse move
            if (_mousePos != Input.mousePosition)
            {
                _mousePos = Input.mousePosition;
                Vector2 mouse = MousePosition(_mousePos.x, Screen.height - _mousePos.y);

                Noesis_MouseMove(_rendererId, (int)mouse.x, (int)mouse.y);
            }

            // mouse wheel
            int mouseWheel = (int)(Input.GetAxis("Mouse ScrollWheel") * 10.0f);
            if (mouseWheel != 0)
            {
                Vector2 mouse = MousePosition(_mousePos.x, Screen.height - _mousePos.y);

                Noesis_MouseWheel(_rendererId, (int)mouse.x, (int)mouse.y, mouseWheel);
            }
        }

        /////////////////////////////////////////////////////////////////////////////////
        private Vector2 MousePosition(float x, float y)
        {
            if (_texture == null)
            {
                // Screen coordinates
                //Debug.Log("Mouse(" + x.ToString() + "," + y.ToString());
                return new Vector2(x, y);
            }
            else
            {
                // Object local coordinates
                Ray ray = Camera.main.ScreenPointToRay(new Vector3(x, y, 0));
                
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    Vector3 point = _target.transform.InverseTransformPoint(hit.point);
                    float localX = _texture.width - (point.x + 0.5f) * _texture.width;
                    float localY = (point.y + 0.5f) * _texture.height;

                    //Debug.Log("Mouse(" + localX.ToString() + "," + localY.ToString());
                    return new Vector2(localX, localY);
                }
                
                //Debug.Log("Mouse(-1,-1)");
                return new Vector2(-1, -1);
            }
        }
        
        /////////////////////////////////////////////////////////////////////////////////
        private bool HitTest(float x, float y)
        {
            return Noesis_HitTest(_root, x, y);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////
        private RenderTexture FindTexture()
        {
            // Check if NoesisGUI was attached to a GameObject with a RenderTexture set
            // in the diffuse texture of its main Material
            if (_target.renderer != null && _target.renderer.material != null)
            {
                return _target.renderer.material.mainTexture as RenderTexture;
            }
            else
            {
                return null;
            }
        }

        /////////////////////////////////////////////////////////////////////////////////
        private void IssueRenderEvent(RenderEvent renderEvent)
        {
            // This triggers the Noesis rendering event
            UnityEngine.RuntimePlatform platform = UnityEngine.Application.platform;
            if (platform == UnityEngine.RuntimePlatform.IPhonePlayer ||
                platform == UnityEngine.RuntimePlatform.Android)
            {
                // NOTE: We have to make an indirect call to the PInvoke function to avoid
                //  that referenced library gets loaded just by entering current function
                IssuePluginEventMobile((System.Int32)renderEvent + _rendererId);
            }
            else
            {
                GL.IssuePluginEvent((System.Int32)renderEvent + _rendererId);
            }
        }

        /////////////////////////////////////////////////////////////////////////////////
        private void IssuePluginEventMobile(int eventId)
        {
            UnityRenderEvent(eventId);
        }

        /////////////////////////////////////////////////////////////////////////////////
        enum GfxDeviceRenderer
        {
            OpenGL = 0,              // OpenGL
            D3D9 = 1,                // Direct3D 9
            D3D11 = 2,               // Direct3D 11
            GCM = 3,                 // Sony PlayStation 3 GCM
            Null = 4,                // "null" device (used in batch mode)
            Hollywood = 5,           // Nintendo Wii
            Xenon = 6,               // Xbox 360
            OpenGLES = 7,            // OpenGL ES 1.1
            OpenGLES20Mobile = 8,    // OpenGL ES 2.0 mobile variant
            Molehill = 9,            // Flash 11 Stage3D
            OpenGLES20Desktop = 10   // OpenGL ES 2.0 desktop variant (i.e. NaCl)
        };

        private static bool _isGraphicsDeviceDirectX = false;
        private static bool _isGraphicsDeviceOpenGL = false;

        public static void SetDeviceType(int deviceType)
        {
            GfxDeviceRenderer gfxDeviceRenderer = (GfxDeviceRenderer)deviceType;

            _isGraphicsDeviceDirectX =
                gfxDeviceRenderer == GfxDeviceRenderer.D3D9 ||
                gfxDeviceRenderer == GfxDeviceRenderer.D3D11;

            _isGraphicsDeviceOpenGL =
                gfxDeviceRenderer == GfxDeviceRenderer.OpenGL ||
                gfxDeviceRenderer == GfxDeviceRenderer.OpenGLES ||
                gfxDeviceRenderer == GfxDeviceRenderer.OpenGLES20Mobile ||
                gfxDeviceRenderer == GfxDeviceRenderer.OpenGLES20Desktop;
        }
    }
}
