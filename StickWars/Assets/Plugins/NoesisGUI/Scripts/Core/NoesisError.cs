using System;
using System.Runtime.InteropServices;

namespace Noesis
{
    public class Error
    {
#if UNITY_EDITOR
        ////////////////////////////////////////////////////////////////////////////////////////////////
        public static void RegisterFunctions(Library lib)
        {
            _registerErrorCallback = lib.Find<RegisterErrorCallbackDelegate>("Noesis_RegisterErrorCallback");
            Noesis_RegisterErrorCallback(Notify);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////
        delegate void RegisterErrorCallbackDelegate(ErrorCallback callback);
        static RegisterErrorCallbackDelegate _registerErrorCallback;
        static void Noesis_RegisterErrorCallback(ErrorCallback callback)
        {
            _registerErrorCallback(callback);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////
        private delegate void ErrorCallback(string desc);
        private static string _pendingError = "";

        ////////////////////////////////////////////////////////////////////////////////////////////////
        [MonoPInvokeCallback(typeof(ErrorCallback))]
        private static void Notify(string desc)
        {
            _pendingError = desc.Replace("Unity/Unity/", "Assets/");
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////
        public static void Check()
        {
            if (_pendingError.Length > 0)
            {
                string message = _pendingError;
                _pendingError = "";

                throw new Exception(message);
            }
        }
#endif
    }
}

