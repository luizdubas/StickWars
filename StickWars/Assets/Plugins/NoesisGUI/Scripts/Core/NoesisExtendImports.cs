using System;
using System.Runtime.InteropServices;

namespace Noesis
{
    public partial class Extend
    {

    #if UNITY_EDITOR

        ////////////////////////////////////////////////////////////////////////////////////////////////
        public static void RegisterFunctions(Library lib)
        {
            _instantiateExtend = lib.Find<InstantiateExtendDelegate>("Noesis_InstantiateExtend");
            _setExtendHandle = lib.Find<SetExtendHandleDelegate>("Noesis_SetExtendHandle");
            _tryGetExtendHandle = lib.Find<GetExtendHandleDelegate>("Noesis_TryGetExtendHandle");
            _deleteExtend = lib.Find<DeleteExtendDelegate>("Noesis_DeleteExtend");
            _launchChangedEvent = lib.Find<LaunchChangedEventDelegate>("Noesis_LaunchChangedEvent");
            _registerReflectionCallbacks = lib.Find<RegisterReflectionCallbacksDelegate>("Noesis_RegisterReflectionCallbacks");

            DependencyObject.RegisterFunctions(lib);
            DependencyProperty.RegisterFunctions(lib);
            PropertyMetadata.RegisterFunctions(lib);
            UIPropertyMetadata.RegisterFunctions(lib);
            FrameworkPropertyMetadata.RegisterFunctions(lib);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////
        delegate IntPtr InstantiateExtendDelegate(IntPtr classType);
        static InstantiateExtendDelegate _instantiateExtend;
        private static IntPtr Noesis_InstantiateExtend(IntPtr classType)
        {
            IntPtr result = _instantiateExtend(classType);
            Error.Check();
            return result;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////
        delegate void SetExtendHandleDelegate(IntPtr classType, IntPtr instance, IntPtr handle);
        static SetExtendHandleDelegate _setExtendHandle;
        private static void Noesis_SetExtendHandle(IntPtr classType, IntPtr instance, IntPtr handle)
        {
            _setExtendHandle(classType, instance, handle);
            Error.Check();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////
        delegate IntPtr GetExtendHandleDelegate(IntPtr classType, IntPtr instance);
        static GetExtendHandleDelegate _tryGetExtendHandle;
        private static IntPtr Noesis_TryGetExtendHandle(IntPtr classType, IntPtr instance)
        {
            IntPtr result = _tryGetExtendHandle(classType, instance);
            Error.Check();
            return result;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////
        delegate void DeleteExtendDelegate(IntPtr classType, IntPtr instance);
        static DeleteExtendDelegate _deleteExtend;
        private static void Noesis_DeleteExtend(IntPtr classType, IntPtr instance)
        {
            _deleteExtend(classType, instance);
            Error.Check();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////
        delegate void LaunchChangedEventDelegate(IntPtr classType, IntPtr instance, IntPtr propertyName);
        static LaunchChangedEventDelegate _launchChangedEvent;
        private static void Noesis_LaunchChangedEvent(IntPtr classType, IntPtr instance, IntPtr propertyName)
        {
            _launchChangedEvent(classType, instance, propertyName);
            Error.Check();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////
        delegate void RegisterReflectionCallbacksDelegate(
            Callback_DependencyPropertyChanged callback_DependencyPropertyChanged,
            Callback_OnPostInit callback_OnPostInit,
            Callback_CommandCanExecute callback_CommandCanExecute,
            Callback_CommandExecute callback_CommandExecute,
            Callback_GetName callback_GetName,
            Callback_GetBaseType callback_GetBaseType,
            Callback_GetPropertiesCount callback_GetPropertiesCount,
            Callback_GetPropertyIndex callback_GetPropertyIndex,
            Callback_GetPropertyType callback_GetPropertyType,
            Callback_GetPropertyInfo callback_GetPropertyInfo,
            Callback_GetPropertyValue_Bool callback_GetPropertyValue_Bool,
            Callback_GetPropertyValue_Float callback_GetPropertyValue_Float,
            Callback_GetPropertyValue_Int callback_GetPropertyValue_Int,
            Callback_GetPropertyValue_UInt callback_GetPropertyValue_UInt,
            Callback_GetPropertyValue_Short callback_GetPropertyValue_Short,
            Callback_GetPropertyValue_UShort callback_GetPropertyValue_UShort,
            Callback_GetPropertyValue_String callback_GetPropertyValue_String,
            Callback_GetPropertyValue_Color callback_GetPropertyValue_Color,
            Callback_GetPropertyValue_Point callback_GetPropertyValue_Point,
            Callback_GetPropertyValue_Rect callback_GetPropertyValue_Rect,
            Callback_GetPropertyValue_Size callback_GetPropertyValue_Size,
            Callback_GetPropertyValue_Thickness callback_GetPropertyValue_Thickness,
            Callback_GetPropertyValue_BaseComponent callback_GetPropertyValue_BaseComponent,
            Callback_SetPropertyValue_Bool callback_SetPropertyValue_Bool,
            Callback_SetPropertyValue_Float callback_SetPropertyValue_Float,
            Callback_SetPropertyValue_Int callback_SetPropertyValue_Int,
            Callback_SetPropertyValue_UInt callback_SetPropertyValue_UInt,
            Callback_SetPropertyValue_Short callback_SetPropertyValue_Short,
            Callback_SetPropertyValue_UShort callback_SetPropertyValue_UShort,
            Callback_SetPropertyValue_String callback_SetPropertyValue_String,
            Callback_SetPropertyValue_Color callback_SetPropertyValue_Color,
            Callback_SetPropertyValue_Point callback_SetPropertyValue_Point,
            Callback_SetPropertyValue_Rect callback_SetPropertyValue_Rect,
            Callback_SetPropertyValue_Size callback_SetPropertyValue_Size,
            Callback_SetPropertyValue_Thickness callback_SetPropertyValue_Thickness,
            Callback_SetPropertyValue_BaseComponent callback_SetPropertyValue_BaseComponent,
            Callback_CreateInstance callback_CreateInstance,
            Callback_DeleteInstance callback_DeleteInstance);
        
        static RegisterReflectionCallbacksDelegate _registerReflectionCallbacks;
        private static void Noesis_RegisterReflectionCallbacks(
            Callback_DependencyPropertyChanged callback_DependencyPropertyChanged,
            Callback_OnPostInit callback_OnPostInit,
            Callback_CommandCanExecute callback_CommandCanExecute,
            Callback_CommandExecute callback_CommandExecute,
            Callback_GetName callback_GetName,
            Callback_GetBaseType callback_GetBaseType,
            Callback_GetPropertiesCount callback_GetPropertiesCount,
            Callback_GetPropertyIndex callback_GetPropertyIndex,
            Callback_GetPropertyType callback_GetPropertyType,
            Callback_GetPropertyInfo callback_GetPropertyInfo,
            Callback_GetPropertyValue_Bool callback_GetPropertyValue_Bool,
            Callback_GetPropertyValue_Float callback_GetPropertyValue_Float,
            Callback_GetPropertyValue_Int callback_GetPropertyValue_Int,
            Callback_GetPropertyValue_UInt callback_GetPropertyValue_UInt,
            Callback_GetPropertyValue_Short callback_GetPropertyValue_Short,
            Callback_GetPropertyValue_UShort callback_GetPropertyValue_UShort,
            Callback_GetPropertyValue_String callback_GetPropertyValue_String,
            Callback_GetPropertyValue_Color callback_GetPropertyValue_Color,
            Callback_GetPropertyValue_Point callback_GetPropertyValue_Point,
            Callback_GetPropertyValue_Rect callback_GetPropertyValue_Rect,
            Callback_GetPropertyValue_Size callback_GetPropertyValue_Size,
            Callback_GetPropertyValue_Thickness callback_GetPropertyValue_Thickness,
            Callback_GetPropertyValue_BaseComponent callback_GetPropertyValue_BaseComponent,
            Callback_SetPropertyValue_Bool callback_SetPropertyValue_Bool,
            Callback_SetPropertyValue_Float callback_SetPropertyValue_Float,
            Callback_SetPropertyValue_Int callback_SetPropertyValue_Int,
            Callback_SetPropertyValue_UInt callback_SetPropertyValue_UInt,
            Callback_SetPropertyValue_Short callback_SetPropertyValue_Short,
            Callback_SetPropertyValue_UShort callback_SetPropertyValue_UShort,
            Callback_SetPropertyValue_String callback_SetPropertyValue_String,
            Callback_SetPropertyValue_Color callback_SetPropertyValue_Color,
            Callback_SetPropertyValue_Point callback_SetPropertyValue_Point,
            Callback_SetPropertyValue_Rect callback_SetPropertyValue_Rect,
            Callback_SetPropertyValue_Size callback_SetPropertyValue_Size,
            Callback_SetPropertyValue_Thickness callback_SetPropertyValue_Thickness,
            Callback_SetPropertyValue_BaseComponent callback_SetPropertyValue_BaseComponent,
            Callback_CreateInstance callback_CreateInstance,
            Callback_DeleteInstance callback_DeleteInstance)
        {
            _registerReflectionCallbacks(
                callback_DependencyPropertyChanged,
                callback_OnPostInit,
                callback_CommandCanExecute,
                callback_CommandExecute,
                callback_GetName,
                callback_GetBaseType,
                callback_GetPropertiesCount,
                callback_GetPropertyIndex,
                callback_GetPropertyType,
                callback_GetPropertyInfo,
                callback_GetPropertyValue_Bool,
                callback_GetPropertyValue_Float,
                callback_GetPropertyValue_Int,
                callback_GetPropertyValue_UInt,
                callback_GetPropertyValue_Short,
                callback_GetPropertyValue_UShort,
                callback_GetPropertyValue_String,
                callback_GetPropertyValue_Color,
                callback_GetPropertyValue_Point,
                callback_GetPropertyValue_Rect,
                callback_GetPropertyValue_Size,
                callback_GetPropertyValue_Thickness,
                callback_GetPropertyValue_BaseComponent,
                callback_SetPropertyValue_Bool,
                callback_SetPropertyValue_Float,
                callback_SetPropertyValue_Int,
                callback_SetPropertyValue_UInt,
                callback_SetPropertyValue_Short,
                callback_SetPropertyValue_UShort,
                callback_SetPropertyValue_String,
                callback_SetPropertyValue_Color,
                callback_SetPropertyValue_Point,
                callback_SetPropertyValue_Rect,
                callback_SetPropertyValue_Size,
                callback_SetPropertyValue_Thickness,
                callback_SetPropertyValue_BaseComponent,
                callback_CreateInstance,
                callback_DeleteInstance);
            Error.Check();
        }

    #else

        ////////////////////////////////////////////////////////////////////////////////////////////////
        #if UNITY_IPHONE || UNITY_XBOX360
        [DllImport("__Internal", EntryPoint="Noesis_InstantiateExtend")]
        #else
        [DllImport("Noesis", EntryPoint = "Noesis_InstantiateExtend")]
        #endif
        private static extern IntPtr Noesis_InstantiateExtend(IntPtr classType);

        ////////////////////////////////////////////////////////////////////////////////////////////////
        #if UNITY_IPHONE || UNITY_XBOX360
        [DllImport("__Internal", EntryPoint="Noesis_SetExtendHandle")]
        #else
        [DllImport("Noesis", EntryPoint = "Noesis_SetExtendHandle")]
        #endif
        private static extern void Noesis_SetExtendHandle(IntPtr classType, IntPtr instance, IntPtr handle);

        ////////////////////////////////////////////////////////////////////////////////////////////////
        #if UNITY_IPHONE || UNITY_XBOX360
        [DllImport("__Internal", EntryPoint="Noesis_TryGetExtendHandle")]
        #else
        [DllImport("Noesis", EntryPoint = "Noesis_TryGetExtendHandle")]
        #endif
        private static extern IntPtr Noesis_TryGetExtendHandle(IntPtr classType, IntPtr instance);

        ////////////////////////////////////////////////////////////////////////////////////////////////
        #if UNITY_IPHONE || UNITY_XBOX360
        [DllImport("__Internal", EntryPoint="Noesis_DeleteExtend")]
        #else
        [DllImport("Noesis", EntryPoint = "Noesis_DeleteExtend")]
        #endif
        private static extern void Noesis_DeleteExtend(IntPtr classType, IntPtr instance);

        ////////////////////////////////////////////////////////////////////////////////////////////////
        #if UNITY_IPHONE || UNITY_XBOX360
        [DllImport("__Internal", EntryPoint="Noesis_LaunchChangedEvent")]
        #else
        [DllImport("Noesis", EntryPoint = "Noesis_LaunchChangedEvent")]
        #endif
        private static extern void Noesis_LaunchChangedEvent(IntPtr classType, IntPtr instance, IntPtr propertyName);


        ////////////////////////////////////////////////////////////////////////////////////////////////
        #if UNITY_IPHONE || UNITY_XBOX360
        [DllImport("__Internal", EntryPoint="Noesis_RegisterReflectionCallbacks")]
        #else
        [DllImport("Noesis", EntryPoint = "Noesis_RegisterReflectionCallbacks")]
        #endif
        static extern void Noesis_RegisterReflectionCallbacks(
            Callback_DependencyPropertyChanged callback_DependencyPropertyChanged,
            Callback_OnPostInit callback_OnPostInit,
            Callback_CommandCanExecute callback_CommandCanExecute,
            Callback_CommandExecute callback_CommandExecute,
            Callback_GetName callback_GetName,
            Callback_GetBaseType callback_GetBaseType,
            Callback_GetPropertiesCount callback_GetPropertiesCount,
            Callback_GetPropertyIndex callback_GetPropertyIndex,
            Callback_GetPropertyType callback_GetPropertyType,
            Callback_GetPropertyInfo callback_GetPropertyInfo,
            Callback_GetPropertyValue_Bool callback_GetPropertyValue_Bool,
            Callback_GetPropertyValue_Float callback_GetPropertyValue_Float,
            Callback_GetPropertyValue_Int callback_GetPropertyValue_Int,
            Callback_GetPropertyValue_UInt callback_GetPropertyValue_UInt,
            Callback_GetPropertyValue_Short callback_GetPropertyValue_Short,
            Callback_GetPropertyValue_UShort callback_GetPropertyValue_UShort,
            Callback_GetPropertyValue_String callback_GetPropertyValue_String,
            Callback_GetPropertyValue_Color callback_GetPropertyValue_Color,
            Callback_GetPropertyValue_Point callback_GetPropertyValue_Point,
            Callback_GetPropertyValue_Rect callback_GetPropertyValue_Rect,
            Callback_GetPropertyValue_Size callback_GetPropertyValue_Size,
            Callback_GetPropertyValue_Thickness callback_GetPropertyValue_Thickness,
            Callback_GetPropertyValue_BaseComponent callback_GetPropertyValue_BaseComponent,
            Callback_SetPropertyValue_Bool callback_SetPropertyValue_Bool,
            Callback_SetPropertyValue_Float callback_SetPropertyValue_Float,
            Callback_SetPropertyValue_Int callback_SetPropertyValue_Int,
            Callback_SetPropertyValue_UInt callback_SetPropertyValue_UInt,
            Callback_SetPropertyValue_Short callback_SetPropertyValue_Short,
            Callback_SetPropertyValue_UShort callback_SetPropertyValue_UShort,
            Callback_SetPropertyValue_String callback_SetPropertyValue_String,
            Callback_SetPropertyValue_Color callback_SetPropertyValue_Color,
            Callback_SetPropertyValue_Point callback_SetPropertyValue_Point,
            Callback_SetPropertyValue_Rect callback_SetPropertyValue_Rect,
            Callback_SetPropertyValue_Size callback_SetPropertyValue_Size,
            Callback_SetPropertyValue_Thickness callback_SetPropertyValue_Thickness,
            Callback_SetPropertyValue_BaseComponent callback_SetPropertyValue_BaseComponent,
            Callback_CreateInstance callback_CreateInstance,
            Callback_DeleteInstance callback_DeleteInstance);

    #endif

    }
}

