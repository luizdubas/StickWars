using System;
using System.Runtime.InteropServices;

namespace Noesis
{
    /////////////////////////////////////////////////////////////////////////////////////
    /// Imports Renderer functions from Noesis library
    /////////////////////////////////////////////////////////////////////////////////////
    public partial class UIRenderer
    {
#if UNITY_EDITOR
        
        public static void RegisterFunctions(Library lib)
        {
            _notifyDestroyRenderer = lib.Find<NotifyDestroyRendererDelegate>("Noesis_NotifyDestroyRenderer");
            _createRenderer = lib.Find<CreateRendererDelegate>("Noesis_CreateRenderer");
            _setRendererSurfaceSize = lib.Find<SetRendererSurfaceSizeDelegate>("Noesis_RendererSurfaceSize");
            _setRendererAntialiasingMode = lib.Find<SetRendererAntialiasingModeDelegate>("Noesis_RendererAntialiasingMode");
            _setRendererTessMode = lib.Find<SetRendererTessModeDelegate>("Noesis_RendererTessMode");
            _setRendererTessQuality = lib.Find<SetRendererTessQualityDelegate>("Noesis_RendererTessQuality");
            _setRendererFlags = lib.Find<SetRendererFlagsDelegate>("Noesis_RendererFlags");
            _updateRendererTexture = lib.Find<UpdateRendererTextureDelegate>("Noesis_UpdateRendererTexture");
            _updateRenderer = lib.Find<UpdateRendererDelegate>("Noesis_UpdateRenderer");
            _hitTest = lib.Find<HitTestDelegate>("Noesis_HitTest");
            _mouseButtonDown = lib.Find<MouseButtonDownDelegate>("Noesis_MouseButtonDown");
            _mouseButtonUp = lib.Find<MouseButtonUpDelegate>("Noesis_MouseButtonUp");
            _mouseDoubleClick = lib.Find<MouseDoubleClickDelegate>("Noesis_MouseDoubleClick");
            _mouseMove = lib.Find<MouseMoveDelegate>("Noesis_MouseMove");
            _mouseWheel = lib.Find<MouseWheelDelegate>("Noesis_MouseWheel");
            _keyDown = lib.Find<KeyDownDelegate>("Noesis_KeyDown");
            _keyUp = lib.Find<KeyUpDelegate>("Noesis_KeyUp");
            _char = lib.Find<CharDelegate>("Noesis_Char");
        }
        
        ////////////////////////////////////////////////////////////////////////////////////////////////
        delegate void NotifyDestroyRendererDelegate(int rendererId);
        static NotifyDestroyRendererDelegate _notifyDestroyRenderer;
        static void Noesis_NotifyDestroyRenderer(int rendererId)
        {
            _notifyDestroyRenderer(rendererId);
            Error.Check();
        }
        
        ////////////////////////////////////////////////////////////////////////////////////////////////
        delegate int CreateRendererDelegate(IntPtr root);
        static CreateRendererDelegate _createRenderer;
        static int Noesis_CreateRenderer(IntPtr root)
        {
            int ret = _createRenderer(root);
            Error.Check();

            return ret;
        }
        
        ////////////////////////////////////////////////////////////////////////////////////////////////
        delegate void SetRendererSurfaceSizeDelegate(int rendererId, int width, int height,
            float offscreenWidth, float offscreenHeight, int msaa);
        static SetRendererSurfaceSizeDelegate _setRendererSurfaceSize;
        static void Noesis_RendererSurfaceSize(int rendererId, int width, int height,
            float offscreenWidth, float offscreenHeight, int msaa)
        {
            _setRendererSurfaceSize(rendererId, width, height, offscreenWidth, offscreenHeight, msaa);
            Error.Check();
        }
        
        ////////////////////////////////////////////////////////////////////////////////////////////////
        delegate void SetRendererAntialiasingModeDelegate(int rendererId, int mode);
        static SetRendererAntialiasingModeDelegate _setRendererAntialiasingMode;
        static void Noesis_RendererAntialiasingMode(int rendererId, int mode)
        {
            _setRendererAntialiasingMode(rendererId, mode);
            Error.Check();
        }
        
        ////////////////////////////////////////////////////////////////////////////////////////////////
        delegate void SetRendererTessModeDelegate(int rendererId, int mode);
        static SetRendererTessModeDelegate _setRendererTessMode;
        static void Noesis_RendererTessMode(int rendererId, int mode)
        {
            _setRendererTessMode(rendererId, mode);
            Error.Check();
        }
        
        ////////////////////////////////////////////////////////////////////////////////////////////////
        delegate void SetRendererTessQualityDelegate(int rendererId, int quality);
        static SetRendererTessQualityDelegate _setRendererTessQuality;
        static void Noesis_RendererTessQuality(int rendererId, int quality)
        {
            _setRendererTessQuality(rendererId, quality);
            Error.Check();
        }
        
        ////////////////////////////////////////////////////////////////////////////////////////////////
        delegate void SetRendererFlagsDelegate(int rendererId, int flags);
        static SetRendererFlagsDelegate _setRendererFlags;
        static void Noesis_RendererFlags(int rendererId, int flags)
        {
            _setRendererFlags(rendererId, flags);
            Error.Check();
        }
        
        ////////////////////////////////////////////////////////////////////////////////////////////////
        delegate void UpdateRendererTextureDelegate(int rendererId, IntPtr texture, int width,
            int height);
        static UpdateRendererTextureDelegate _updateRendererTexture;
        static void Noesis_UpdateRendererTexture(int rendererId, IntPtr texture, int width, int height)
        {
            _updateRendererTexture(rendererId, texture, width, height);
            Error.Check();
        }
        
        ////////////////////////////////////////////////////////////////////////////////////////////////
        delegate void UpdateRendererDelegate(int rendererId, double timeInSeconds);
        static UpdateRendererDelegate _updateRenderer;
        static void Noesis_UpdateRenderer(int rendererId, double timeInSeconds)
        {
            _updateRenderer(rendererId, timeInSeconds);
            Error.Check();
        }
        
        ////////////////////////////////////////////////////////////////////////////////////////////////
        delegate bool HitTestDelegate(IntPtr root, float x, float y);
        static HitTestDelegate _hitTest;
        static bool Noesis_HitTest(IntPtr root, float x, float y)
        {
            bool hit = _hitTest(root, x, y);
            Error.Check();

            return hit;
        }
        
        ////////////////////////////////////////////////////////////////////////////////////////////////
        delegate void MouseButtonDownDelegate(int rendererId, float x, float y, int button);
        static MouseButtonDownDelegate _mouseButtonDown;
        static void Noesis_MouseButtonDown(int rendererId, float x, float y, int button)
        {
            _mouseButtonDown(rendererId, x, y, button);
            Error.Check();
        }
    
        ////////////////////////////////////////////////////////////////////////////////////////////////
        delegate void MouseButtonUpDelegate(int rendererId, float x, float y, int button);
        static MouseButtonUpDelegate _mouseButtonUp;
        static void Noesis_MouseButtonUp(int rendererId, float x, float y, int button)
        {
            _mouseButtonUp(rendererId, x, y, button);
            Error.Check();
        }
        
        ////////////////////////////////////////////////////////////////////////////////////////////////
        delegate void MouseDoubleClickDelegate(int rendererId, float x, float y, int button);
        static MouseDoubleClickDelegate _mouseDoubleClick;
        static void Noesis_MouseDoubleClick(int rendererId, float x, float y, int button)
        {
            _mouseDoubleClick(rendererId, x, y, button);
            Error.Check();
        }
        
        ////////////////////////////////////////////////////////////////////////////////////////////////
        delegate void MouseMoveDelegate(int rendererId, float x, float y);
        static MouseMoveDelegate _mouseMove;
        static void Noesis_MouseMove(int rendererId, float x, float y)
        {
            _mouseMove(rendererId, x, y);
            Error.Check();
        }
    
        ////////////////////////////////////////////////////////////////////////////////////////////////
        delegate void MouseWheelDelegate(int rendererId, float x, float y, float wheelRotation);
        static MouseWheelDelegate _mouseWheel;
        static void Noesis_MouseWheel(int rendererId, float x, float y, float wheelRotation)
        {
            _mouseWheel(rendererId, x, y, wheelRotation);
            Error.Check();
        }
    
        ////////////////////////////////////////////////////////////////////////////////////////////////
        delegate void KeyDownDelegate(int rendererId, int key);
        static KeyDownDelegate _keyDown;
        static void Noesis_KeyDown(int rendererId, int key)
        {
            _keyDown(rendererId, key);
            Error.Check();
        }
    
        ////////////////////////////////////////////////////////////////////////////////////////////////
        delegate void KeyUpDelegate(int rendererId, int key);
        static KeyUpDelegate _keyUp;
        static void Noesis_KeyUp(int rendererId, int key)
        {
            _keyUp(rendererId, key);
            Error.Check();
        }
    
        ////////////////////////////////////////////////////////////////////////////////////////////////
        delegate void CharDelegate(int rendererId, char ch);
        static CharDelegate _char;
        static void Noesis_Char(int rendererId, char ch)
        {
            _char(rendererId, ch);
            Error.Check();
        }
        
#else
        
        ////////////////////////////////////////////////////////////////////////////////////////////////
        #if UNITY_IPHONE || UNITY_XBOX360
        [DllImport("__Internal", EntryPoint="Noesis_CreateRenderer")]
        #else
        [DllImport("Noesis", EntryPoint = "Noesis_CreateRenderer")]
        #endif
        static extern int Noesis_CreateRenderer(IntPtr root);
        
        ////////////////////////////////////////////////////////////////////////////////////////////////
        #if UNITY_IPHONE || UNITY_XBOX360
        [DllImport("__Internal", EntryPoint="Noesis_NotifyDestroyRenderer")]
        #else
        [DllImport("Noesis", EntryPoint = "Noesis_NotifyDestroyRenderer")]
        #endif
        static extern void Noesis_NotifyDestroyRenderer(int rendererId);
        
        ////////////////////////////////////////////////////////////////////////////////////////////////
        #if UNITY_IPHONE || UNITY_XBOX360
        [DllImport("__Internal", EntryPoint="Noesis_RendererSurfaceSize")]
        #else
        [DllImport("Noesis", EntryPoint = "Noesis_RendererSurfaceSize")]
        #endif
        static extern void Noesis_RendererSurfaceSize(int rendererId, int width, int height,
            float offscreenWidth, float offscreenHeight, int msaa);
        
        ////////////////////////////////////////////////////////////////////////////////////////////////
        #if UNITY_IPHONE || UNITY_XBOX360
        [DllImport("__Internal", EntryPoint="Noesis_RendererAntialiasingMode")]
        #else
        [DllImport("Noesis", EntryPoint = "Noesis_RendererAntialiasingMode")]
        #endif
        static extern void Noesis_RendererAntialiasingMode(int rendererId, int mode);
        
        ////////////////////////////////////////////////////////////////////////////////////////////////
        #if UNITY_IPHONE || UNITY_XBOX360
        [DllImport("__Internal", EntryPoint="Noesis_RendererTessMode")]
        #else
        [DllImport("Noesis", EntryPoint = "Noesis_RendererTessMode")]
        #endif
        static extern void Noesis_RendererTessMode(int rendererId, int mode);
        
        ////////////////////////////////////////////////////////////////////////////////////////////////
        #if UNITY_IPHONE || UNITY_XBOX360
        [DllImport("__Internal", EntryPoint="Noesis_RendererTessQuality")]
        #else
        [DllImport("Noesis", EntryPoint = "Noesis_RendererTessQuality")]
        #endif
        static extern void Noesis_RendererTessQuality(int rendererId, int quality);
        
        ////////////////////////////////////////////////////////////////////////////////////////////////
        #if UNITY_IPHONE || UNITY_XBOX360
        [DllImport("__Internal", EntryPoint="Noesis_RendererFlags")]
        #else
        [DllImport("Noesis", EntryPoint = "Noesis_RendererFlags")]
        #endif
        static extern void Noesis_RendererFlags(int rendererId, int flags);
                
        ////////////////////////////////////////////////////////////////////////////////////////////////
        #if UNITY_IPHONE || UNITY_XBOX360
        [DllImport("__Internal", EntryPoint="Noesis_UpdateRendererTexture")]
        #else
        [DllImport("Noesis", EntryPoint = "Noesis_UpdateRendererTexture")]
        #endif
        static extern void Noesis_UpdateRendererTexture(int rendererId, IntPtr texture, int width, int height);

        ////////////////////////////////////////////////////////////////////////////////////////////////
        #if UNITY_IPHONE || UNITY_XBOX360
        [DllImport("__Internal", EntryPoint="Noesis_UpdateRenderer")]
        #else
        [DllImport("Noesis", EntryPoint = "Noesis_UpdateRenderer")]
        #endif
        static extern void Noesis_UpdateRenderer(int rendererId, double timeInSeconds);
        
        ////////////////////////////////////////////////////////////////////////////////////////////////
        #if UNITY_IPHONE || UNITY_XBOX360
        [DllImport("__Internal", EntryPoint="Noesis_HitTest")]
        #else
        [DllImport("Noesis", EntryPoint = "Noesis_HitTest")]
        #endif
        [return: MarshalAs(UnmanagedType.U1)]
        static extern bool Noesis_HitTest(IntPtr root, float x, float y);
    
        ////////////////////////////////////////////////////////////////////////////////////////////////
        #if UNITY_IPHONE || UNITY_XBOX360
        [DllImport("__Internal", EntryPoint="Noesis_MouseButtonDown")]
        #else
        [DllImport("Noesis", EntryPoint = "Noesis_MouseButtonDown")]
        #endif
        static extern void Noesis_MouseButtonDown(int rendererId, float x, float y, int button);
    
        ////////////////////////////////////////////////////////////////////////////////////////////////
        #if UNITY_IPHONE || UNITY_XBOX360
        [DllImport("__Internal", EntryPoint="Noesis_MouseButtonUp")]
        #else
        [DllImport("Noesis", EntryPoint = "Noesis_MouseButtonUp")]
        #endif
        static extern void Noesis_MouseButtonUp(int rendererId, float x, float y, int button);
        
        ////////////////////////////////////////////////////////////////////////////////////////////////
        #if UNITY_IPHONE || UNITY_XBOX360
        [DllImport("__Internal", EntryPoint="Noesis_MouseDoubleClick")]
        #else
        [DllImport("Noesis", EntryPoint = "Noesis_MouseDoubleClick")]
        #endif
        static extern void Noesis_MouseDoubleClick(int rendererId, float x, float y, int button);
        
        ////////////////////////////////////////////////////////////////////////////////////////////////
        #if UNITY_IPHONE || UNITY_XBOX360
        [DllImport("__Internal", EntryPoint="Noesis_MouseMove")]
        #else
        [DllImport("Noesis", EntryPoint = "Noesis_MouseMove")]
        #endif
        static extern void Noesis_MouseMove(int rendererId, float x, float y);
    
        ////////////////////////////////////////////////////////////////////////////////////////////////
        #if UNITY_IPHONE || UNITY_XBOX360
        [DllImport("__Internal", EntryPoint="Noesis_MouseWheel")]
        #else
        [DllImport("Noesis", EntryPoint = "Noesis_MouseWheel")]
        #endif
        static extern void Noesis_MouseWheel(int rendererId, float x, float y, float wheelRotation);
    
        ////////////////////////////////////////////////////////////////////////////////////////////////
        #if UNITY_IPHONE || UNITY_XBOX360
        [DllImport("__Internal", EntryPoint="Noesis_KeyDown")]
        #else
        [DllImport("Noesis", EntryPoint = "Noesis_KeyDown")]
        #endif
        static extern void Noesis_KeyDown(int rendererId, int key);    
    
        ////////////////////////////////////////////////////////////////////////////////////////////////
        #if UNITY_IPHONE || UNITY_XBOX360
        [DllImport("__Internal", EntryPoint="Noesis_KeyUp")]
        #else
        [DllImport("Noesis", EntryPoint = "Noesis_KeyUp")]
        #endif
        static extern void Noesis_KeyUp(int rendererId, int key);    
    
        ////////////////////////////////////////////////////////////////////////////////////////////////
        #if UNITY_IPHONE || UNITY_XBOX360
        [DllImport("__Internal", EntryPoint="Noesis_Char")]
        #else
        [DllImport("Noesis", EntryPoint = "Noesis_Char")]
        #endif
        static extern void Noesis_Char(int rendererId, char ch);
        
#endif

        ////////////////////////////////////////////////////////////////////////////////////////////////
        #if UNITY_IPHONE
        [DllImport("__Internal", EntryPoint = "UnityRenderEvent")]
        #else
        [DllImport("Noesis", EntryPoint = "UnityRenderEvent")]
        #endif
        private static extern void UnityRenderEvent(int eventId);

    }
}

