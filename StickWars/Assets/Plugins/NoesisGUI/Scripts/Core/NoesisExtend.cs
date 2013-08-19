using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Reflection;

namespace Noesis
{
    ////////////////////////////////////////////////////////////////////////////////////////////////
    // Manages Noesis Extensibility
    ////////////////////////////////////////////////////////////////////////////////////////////////
    public partial class Extend
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////
        public static IntPtr New(System.Type type)
        {
            IntPtr typeClassPtr = Extend.GetPtrForType(type);
            IntPtr instance = Noesis_InstantiateExtend(typeClassPtr);

            if (instance == IntPtr.Zero)
            {
                throw new System.Exception(String.Format("Unable to create an instance of '{0}'",
                    type.Name));
            }

            return instance;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////
        public static void Delete(System.Type type, IntPtr instance)
        {
            if (Kernel.IsInitialized())
            {
                if (instance == IntPtr.Zero)
                {
                    throw new System.Exception(String.Format(
                        "Trying to delete a null instance of '{0}'", type.Name));
                }

                IntPtr typeClassPtr = Extend.GetPtrForType(type);
                Noesis_DeleteExtend(typeClassPtr, instance);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////
        public static void Register(System.Type type, IntPtr instance, object obj)
        {
            if (instance == IntPtr.Zero)
            {
                throw new System.Exception(String.Format(
                    "Trying to register a null instance of '{0}'", type.Name));
            }

            IntPtr typeClassPtr = Extend.GetPtrForType(type);
            GCHandle gcHandle = GCHandle.Alloc(obj, GCHandleType.Pinned);
            IntPtr handle = GCHandle.ToIntPtr(gcHandle); 
            Noesis_SetExtendHandle(typeClassPtr, instance, handle);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////
        public static IntPtr TryGetHandle(IntPtr classType, IntPtr instance)
        {
            if (instance == IntPtr.Zero)
            {
                throw new System.Exception("Trying to get handle of a null instance");
            }

            return Noesis_TryGetExtendHandle(classType, instance);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////
        public static void PropertyChanged(System.Type type, IntPtr instance, string propertyName)
        {
            IntPtr classType = Extend.GetPtrForType(type); ;
            IntPtr propertyNamePtr = Marshal.StringToHGlobalAnsi(propertyName);
            Noesis_LaunchChangedEvent(classType, instance, propertyNamePtr);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////
        public static void RegisterCallbacks()
        {
            // register callbacks
            Noesis_RegisterReflectionCallbacks(

                DependencyPropertyChanged,

                OnPostInit,

                CommandCanExecute,
                CommandExecute,

                GetName,
                GetBaseType,
                GetPropertiesCount,
                GetPropertyIndex,
                GetPropertyType,
                GetPropertyInfo,

                GetPropertyValue_Bool,
                GetPropertyValue_Float,
                GetPropertyValue_Int,
                GetPropertyValue_UInt,
                GetPropertyValue_Short,
                GetPropertyValue_UShort,
                GetPropertyValue_String,
                GetPropertyValue_Color,
                GetPropertyValue_Point,
                GetPropertyValue_Rect,
                GetPropertyValue_Size,
                GetPropertyValue_Thickness,
                GetPropertyValue_BaseComponent,

                SetPropertyValue_Bool,
                SetPropertyValue_Float,
                SetPropertyValue_Int,
                SetPropertyValue_UInt,
                SetPropertyValue_Short,
                SetPropertyValue_UShort,
                SetPropertyValue_String,
                SetPropertyValue_Color,
                SetPropertyValue_Point,
                SetPropertyValue_Rect,
                SetPropertyValue_Size,
                SetPropertyValue_Thickness,
                SetPropertyValue_BaseComponent,

                CreateInstance,
                DeleteInstance);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////
        public static void RegisterExtendClasses()
        {
            System.Reflection.Assembly[] assemblies = System.AppDomain.CurrentDomain.GetAssemblies();
            foreach (var A in assemblies)
            {
                System.Type[] types = A.GetTypes();
                foreach (var type in types)
                {
                    if (type.IsSubclassOf(typeof(Noesis.BaseComponent)))
                    {
                        System.Reflection.MethodInfo register = type.GetMethod("Register");
                        if (register != null && register.IsStatic && register.GetParameters().Length == 0)
                        {
                            System.Reflection.MethodInfo extend = FindExtendMethod(type);
                            if (extend != null && extend.IsStatic && extend.GetParameters().Length == 1)
                            {
                                extend.Invoke(null, new object[] { type });
                                register.Invoke(null, new object[] { });
                            }
                            else
                            {
                                Debug.LogWarning("Can't register extended class " + type.Name);
                            }
                        }
                    }
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////
        private static System.Reflection.MethodInfo FindExtendMethod(System.Type type)
        {
            System.Reflection.MethodInfo extend = null;
            System.Type baseType = type.BaseType;
            while (extend == null && baseType != null)
            {
                extend = baseType.GetMethod("Extend");
                baseType = baseType.BaseType;
            }

            return extend;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////
        static System.Collections.Hashtable mTypes = new System.Collections.Hashtable();
        
        ////////////////////////////////////////////////////////////////////////////////////////////////
        public static IntPtr GetPtrForType(System.Type type)
        {
            IntPtr ptrToType = type.TypeHandle.Value;
            
            if (!mTypes.Contains(ptrToType))
            {
                mTypes.Add(ptrToType, type.TypeHandle);
            }
            
            return ptrToType;
        }
        
        ////////////////////////////////////////////////////////////////////////////////////////////////
        private static System.Type GetTypeFromPtr(IntPtr ptrToType)
        {
            return Type.GetTypeFromHandle((System.RuntimeTypeHandle)mTypes[ptrToType]);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////
        private delegate void Callback_DependencyPropertyChanged(IntPtr typeClassPtr, IntPtr instance,
            IntPtr dependencyProperty);

        [MonoPInvokeCallback(typeof(Callback_DependencyPropertyChanged))]
        private static void DependencyPropertyChanged(IntPtr typeClassPtr, IntPtr instance,
            IntPtr dependencyPropertyPtr)
        {
            Type typeClass = GetTypeFromPtr(typeClassPtr);

            GCHandle handleInstance = GCHandle.FromIntPtr(instance);
            System.Object objectInstance = (System.Object)handleInstance.Target;
                
            MethodInfo methodInfo = typeClass.GetMethod("DependencyPropertyChanged");
            if (methodInfo != null)
            {
                DependencyProperty dependencyProperty = 
                    new DependencyProperty(dependencyPropertyPtr, false);
        
                object[] parametersArray = new object[] { dependencyProperty };      
                methodInfo.Invoke(objectInstance, parametersArray);                
            }    
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////
        private delegate void Callback_OnPostInit(IntPtr typeClassPtr, IntPtr instance);

        [MonoPInvokeCallback(typeof(Callback_OnPostInit))]
        private static void OnPostInit(IntPtr typeClassPtr, IntPtr instance)
        {
            Type typeClass = GetTypeFromPtr(typeClassPtr);

            GCHandle handleInstance = GCHandle.FromIntPtr(instance);
            System.Object objectInstance = (System.Object)handleInstance.Target;
                
            MethodInfo methodInfo = typeClass.GetMethod("OnPostInit");
            if (methodInfo != null)
            {
                object[] parametersArray = new object[] { };      
                methodInfo.Invoke(objectInstance, parametersArray);                
            }        
        }
        
        ////////////////////////////////////////////////////////////////////////////////////////////////
        private delegate bool Callback_CommandCanExecute(IntPtr typeClassPtr, IntPtr instance,
            IntPtr param);

        [MonoPInvokeCallback(typeof(Callback_CommandCanExecute))]
        private static bool CommandCanExecute(IntPtr typeClassPtr, IntPtr instance,
            IntPtr paramPtr)
        {
            Type typeClass = GetTypeFromPtr(typeClassPtr);

            GCHandle handleInstance = GCHandle.FromIntPtr(instance);
            System.Object objectInstance = (System.Object)handleInstance.Target;
                
            MethodInfo methodInfo = typeClass.GetMethod("CanExecuteCommand");
            if (methodInfo != null)
            {
                BaseComponent param = 
                    new BaseComponent(paramPtr, false);
        
                object[] parametersArray = new object[] { param };      
                return (bool)methodInfo.Invoke(objectInstance, parametersArray);                
            } 
            
            return false;
        }
        
        ////////////////////////////////////////////////////////////////////////////////////////////////
        private delegate void Callback_CommandExecute(IntPtr typeClassPtr, IntPtr instance,
            IntPtr param);

        [MonoPInvokeCallback(typeof(Callback_CommandExecute))]
        private static void CommandExecute(IntPtr typeClassPtr, IntPtr instance,
            IntPtr paramPtr)
        {
            Type typeClass = GetTypeFromPtr(typeClassPtr);

            GCHandle handleInstance = GCHandle.FromIntPtr(instance);
            System.Object objectInstance = (System.Object)handleInstance.Target;
                
            MethodInfo methodInfo = typeClass.GetMethod("ExecuteCommand");
            if (methodInfo != null)
            {
                BaseComponent param = 
                    new BaseComponent(paramPtr, false);
        
                object[] parametersArray = new object[] { param };      
                methodInfo.Invoke(objectInstance, parametersArray);                
            } 
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////
        private delegate IntPtr Callback_GetName(IntPtr typeClassPtr);

        [MonoPInvokeCallback(typeof(Callback_GetName))]
        private static IntPtr GetName(IntPtr typeClassPtr)
        {
            Type type = GetTypeFromPtr(typeClassPtr);
            string fullName;
            if (type.Namespace != null && type.Namespace.Length > 0)
            {
                fullName = type.Namespace + "." + type.Name;
            }
            else
            {
                fullName = type.Name;
            }
            IntPtr baseNamePtr = Marshal.StringToHGlobalAnsi(fullName);
            return baseNamePtr;
        }
            
        ////////////////////////////////////////////////////////////////////////////////////////////////
        private delegate IntPtr Callback_GetBaseType(IntPtr typeClassPtr);

        [MonoPInvokeCallback(typeof(Callback_GetBaseType))]
        private static IntPtr GetBaseType(IntPtr typeClassPtr)
        {
            Type typeClass = GetTypeFromPtr(typeClassPtr);
            
            return Extend.GetPtrForType(typeClass.BaseType);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////
        private static PropertyInfo[] GetPublicProperties(Type type)
        {
            return type.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////
        private delegate int Callback_GetPropertiesCount(IntPtr typeClassPtr);

        [MonoPInvokeCallback(typeof(Callback_GetPropertiesCount))]
        private static int GetPropertiesCount(IntPtr typeClassPtr)
        {
            Type typeClass = GetTypeFromPtr(typeClassPtr);
            
            PropertyInfo[] properties = GetPublicProperties(typeClass);
        
            return properties.Length;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////
        private delegate int Callback_GetPropertyIndex(IntPtr typeClassPtr, string propName);

        [MonoPInvokeCallback(typeof(Callback_GetPropertyIndex))]
        private static int GetPropertyIndex(IntPtr typeClassPtr, string propName)
        {
            Type typeClass = GetTypeFromPtr(typeClassPtr);

            PropertyInfo[] properties = GetPublicProperties(typeClass);

            int index = 0;
            foreach (PropertyInfo propertyInfo in properties)
            {
                if (propertyInfo.Name == propName)
                {
                    return index;
                }

                ++index;
            }

            return -1;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////
        private enum ReflectionTypes
        {
            Type_Invalid = 0,
            Type_Bool,
            Type_Float,
            Type_Int,
            Type_UInt,
            Type_Short,
            Type_UShort,
            Type_String,
            Type_Color,
            Type_Point,
            Type_Rect,
            Type_Size,
            Type_Thickness,
            Type_BaseComponent
        };

        ////////////////////////////////////////////////////////////////////////////////////////////////
        private static int GetReflectionType(Type type)
        {
            int reflectionType = (int)ReflectionTypes.Type_Invalid;

            if (type == typeof(bool))
            {
                reflectionType = (int)ReflectionTypes.Type_Bool;
            }
            else if (type == typeof(float))
            {
                reflectionType = (int)ReflectionTypes.Type_Float;
            }
            else if (type == typeof(int) || type.IsEnum)
            {
                reflectionType = (int)ReflectionTypes.Type_Int;
            }
            else if (type == typeof(uint))
            {
                reflectionType = (int)ReflectionTypes.Type_UInt;
            }
            else if (type == typeof(short))
            {
                reflectionType = (int)ReflectionTypes.Type_Short;
            }
            else if (type == typeof(ushort))
            {
                reflectionType = (int)ReflectionTypes.Type_UShort;
            }
            else if (type == typeof(string))
            {
                reflectionType = (int)ReflectionTypes.Type_String;
            }
            else if (type == typeof(Color))
            {
                reflectionType = (int)ReflectionTypes.Type_Color;
            }
            else if (type == typeof(Point))
            {
                reflectionType = (int)ReflectionTypes.Type_Point;
            }
            else if (type == typeof(Rect))
            {
                reflectionType = (int)ReflectionTypes.Type_Rect;
            }
            else if (type == typeof(Size))
            {
                reflectionType = (int)ReflectionTypes.Type_Size;
            }
            else if (type == typeof(Thickness))
            {
                reflectionType = (int)ReflectionTypes.Type_Thickness;
            }
            else if (type == typeof(BaseComponent))
            {
                reflectionType = (int)ReflectionTypes.Type_BaseComponent;
            }
            else if (type.IsSubclassOf(typeof(BaseComponent)))
            {
                reflectionType = (int)ReflectionTypes.Type_BaseComponent;
            }

            return reflectionType;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////
        private delegate int Callback_GetPropertyType(IntPtr typeClassPtr, int propertyIndex);

        [MonoPInvokeCallback(typeof(Callback_GetPropertyType))]
        private static int GetPropertyType(IntPtr typeClassPtr, int propertyIndex)
        {
            Type typeClass = GetTypeFromPtr(typeClassPtr);

            PropertyInfo[] properties = GetPublicProperties(typeClass);

            return GetReflectionType(properties[propertyIndex].PropertyType);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////
        private delegate void Callback_GetPropertyInfo(IntPtr typeClassPtr, int propertyIndex,
            ref IntPtr name, [MarshalAs(UnmanagedType.U1)] ref bool read,
            [MarshalAs(UnmanagedType.U1)] ref bool write, ref int type);

        [MonoPInvokeCallback(typeof(Callback_GetPropertyInfo))]
        private static void GetPropertyInfo(IntPtr typeClassPtr, int propertyIndex, ref IntPtr name,
            [MarshalAs(UnmanagedType.U1)] ref bool read, [MarshalAs(UnmanagedType.U1)] ref bool write,
            ref int type)
        {
            Type typeClass = GetTypeFromPtr(typeClassPtr);
            
            PropertyInfo[] properties = GetPublicProperties(typeClass);
            PropertyInfo propertyInfo = properties[propertyIndex];
            
            name = Marshal.StringToHGlobalAnsi(propertyInfo.Name);
            read = propertyInfo.CanRead;
            write = propertyInfo.CanWrite && propertyInfo.GetSetMethod(true).IsPublic;
            type = GetReflectionType(propertyInfo.PropertyType);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////
        [return: MarshalAs(UnmanagedType.U1)]
        private delegate bool Callback_GetPropertyValue_Bool(IntPtr typeClassPtr, int propertyIndex,
            IntPtr instance);

        [MonoPInvokeCallback(typeof(Callback_GetPropertyValue_Bool))]
        [return: MarshalAs(UnmanagedType.U1)]
        private static bool GetPropertyValue_Bool(IntPtr typeClassPtr, int propertyIndex,
            IntPtr instance)
        {
            Type typeClass = GetTypeFromPtr(typeClassPtr);
            
            GCHandle handleInstance = GCHandle.FromIntPtr(instance);
            System.Object objectInstance = (System.Object)handleInstance.Target;

            PropertyInfo[] properties = GetPublicProperties(typeClass);
            PropertyInfo propertyInfo = properties[propertyIndex];

            return (bool)propertyInfo.GetValue(objectInstance, null);
        }

        private delegate float Callback_GetPropertyValue_Float(IntPtr typeClassPtr, int propertyIndex,
            IntPtr instance);

        [MonoPInvokeCallback(typeof(Callback_GetPropertyValue_Float))]
        private static float GetPropertyValue_Float(IntPtr typeClassPtr, int propertyIndex, IntPtr instance)
        {
            Type typeClass = GetTypeFromPtr(typeClassPtr);

            GCHandle handleInstance = GCHandle.FromIntPtr(instance);
            System.Object objectInstance = (System.Object)handleInstance.Target;
    
            PropertyInfo[] properties = GetPublicProperties(typeClass);
            PropertyInfo propertyInfo = properties[propertyIndex];

            return (float)propertyInfo.GetValue(objectInstance, null);
        }

        private delegate int Callback_GetPropertyValue_Int(IntPtr typeClassPtr, int propertyIndex,
            IntPtr instance);

        [MonoPInvokeCallback(typeof(Callback_GetPropertyValue_Int))]
        private static int GetPropertyValue_Int(IntPtr typeClassPtr, int propertyIndex, IntPtr instance)
        {
            Type typeClass = GetTypeFromPtr(typeClassPtr);

            GCHandle handleInstance = GCHandle.FromIntPtr(instance);
            System.Object objectInstance = (System.Object)handleInstance.Target;

            PropertyInfo[] properties = GetPublicProperties(typeClass);
            PropertyInfo propertyInfo = properties[propertyIndex];

            return (int)propertyInfo.GetValue(objectInstance, null);
        }

        private delegate uint Callback_GetPropertyValue_UInt(IntPtr typeClassPtr, int propertyIndex,
            IntPtr instance);

        [MonoPInvokeCallback(typeof(Callback_GetPropertyValue_UInt))]
        private static uint GetPropertyValue_UInt(IntPtr typeClassPtr, int propertyIndex, IntPtr instance)
        {
            Type typeClass = GetTypeFromPtr(typeClassPtr);

            GCHandle handleInstance = GCHandle.FromIntPtr(instance);
            System.Object objectInstance = (System.Object)handleInstance.Target;

            PropertyInfo[] properties = GetPublicProperties(typeClass);
            PropertyInfo propertyInfo = properties[propertyIndex];

            return (uint)propertyInfo.GetValue(objectInstance, null);
        }

        private delegate short Callback_GetPropertyValue_Short(IntPtr typeClassPtr, int propertyIndex,
            IntPtr instance);

        [MonoPInvokeCallback(typeof(Callback_GetPropertyValue_Short))]
        private static short GetPropertyValue_Short(IntPtr typeClassPtr, int propertyIndex, IntPtr instance)
        {
            Type typeClass = GetTypeFromPtr(typeClassPtr);

            GCHandle handleInstance = GCHandle.FromIntPtr(instance);
            System.Object objectInstance = (System.Object)handleInstance.Target;

            PropertyInfo[] properties = GetPublicProperties(typeClass);
            PropertyInfo propertyInfo = properties[propertyIndex];

            return (short)propertyInfo.GetValue(objectInstance, null);
        }

        private delegate ushort Callback_GetPropertyValue_UShort(IntPtr typeClassPtr, int propertyIndex,
            IntPtr instance);

        [MonoPInvokeCallback(typeof(Callback_GetPropertyValue_UShort))]
        private static ushort GetPropertyValue_UShort(IntPtr typeClassPtr, int propertyIndex, IntPtr instance)
        {
            Type typeClass = GetTypeFromPtr(typeClassPtr);

            GCHandle handleInstance = GCHandle.FromIntPtr(instance);
            System.Object objectInstance = (System.Object)handleInstance.Target;

            PropertyInfo[] properties = GetPublicProperties(typeClass);
            PropertyInfo propertyInfo = properties[propertyIndex];

            return (ushort)propertyInfo.GetValue(objectInstance, null);
        }

        private delegate IntPtr Callback_GetPropertyValue_String(IntPtr typeClassPtr, int propertyIndex,
            IntPtr instance);

        [MonoPInvokeCallback(typeof(Callback_GetPropertyValue_String))]
        private static IntPtr GetPropertyValue_String(IntPtr typeClassPtr, int propertyIndex, IntPtr instance)
        {
            Type typeClass = GetTypeFromPtr(typeClassPtr);

            GCHandle handleInstance = GCHandle.FromIntPtr(instance);
            System.Object objectInstance = (System.Object)handleInstance.Target;

            PropertyInfo[] properties = GetPublicProperties(typeClass);
            PropertyInfo propertyInfo = properties[propertyIndex];

            IntPtr stringPtr = Marshal.StringToHGlobalAnsi((string)propertyInfo.GetValue(objectInstance, null));
            return stringPtr;
        }

        private delegate IntPtr Callback_GetPropertyValue_Color(IntPtr typeClassPtr, int propertyIndex,
            IntPtr instance);

        [MonoPInvokeCallback(typeof(Callback_GetPropertyValue_Color))]
        private static IntPtr GetPropertyValue_Color(IntPtr typeClassPtr, int propertyIndex, IntPtr instance)
        {
            Type typeClass = GetTypeFromPtr(typeClassPtr);

            GCHandle handleInstance = GCHandle.FromIntPtr(instance);
            System.Object objectInstance = (System.Object)handleInstance.Target;

            PropertyInfo[] properties = GetPublicProperties(typeClass);
            PropertyInfo propertyInfo = properties[propertyIndex];

            Color color = (Color)propertyInfo.GetValue(objectInstance, null);
            return Color.getCPtr(color).Handle;
        }

        private delegate IntPtr Callback_GetPropertyValue_Point(IntPtr typeClassPtr, int propertyIndex,
            IntPtr instance);

        [MonoPInvokeCallback(typeof(Callback_GetPropertyValue_Point))]
        private static IntPtr GetPropertyValue_Point(IntPtr typeClassPtr, int propertyIndex, IntPtr instance)
        {
            Type typeClass = GetTypeFromPtr(typeClassPtr);

            GCHandle handleInstance = GCHandle.FromIntPtr(instance);
            System.Object objectInstance = (System.Object)handleInstance.Target;

            PropertyInfo[] properties = GetPublicProperties(typeClass);
            PropertyInfo propertyInfo = properties[propertyIndex];

            Point point = (Point)propertyInfo.GetValue(objectInstance, null);
            return Point.getCPtr(point).Handle;
        }

        private delegate IntPtr Callback_GetPropertyValue_Rect(IntPtr typeClassPtr, int propertyIndex,
            IntPtr instance);

        [MonoPInvokeCallback(typeof(Callback_GetPropertyValue_Rect))]
        private static IntPtr GetPropertyValue_Rect(IntPtr typeClassPtr, int propertyIndex, IntPtr instance)
        {
            Type typeClass = GetTypeFromPtr(typeClassPtr);

            GCHandle handleInstance = GCHandle.FromIntPtr(instance);
            System.Object objectInstance = (System.Object)handleInstance.Target;

            PropertyInfo[] properties = GetPublicProperties(typeClass);
            PropertyInfo propertyInfo = properties[propertyIndex];

            Rect rect = (Rect)propertyInfo.GetValue(objectInstance, null);
            return Rect.getCPtr(rect).Handle;
        }

        private delegate IntPtr Callback_GetPropertyValue_Size(IntPtr typeClassPtr, int propertyIndex,
            IntPtr instance);

        [MonoPInvokeCallback(typeof(Callback_GetPropertyValue_Size))]
        private static IntPtr GetPropertyValue_Size(IntPtr typeClassPtr, int propertyIndex, IntPtr instance)
        {
            Type typeClass = GetTypeFromPtr(typeClassPtr);

            GCHandle handleInstance = GCHandle.FromIntPtr(instance);
            System.Object objectInstance = (System.Object)handleInstance.Target;

            PropertyInfo[] properties = GetPublicProperties(typeClass);
            PropertyInfo propertyInfo = properties[propertyIndex];

            Size size = (Size)propertyInfo.GetValue(objectInstance, null);
            return Size.getCPtr(size).Handle;
        }

        private delegate IntPtr Callback_GetPropertyValue_Thickness(IntPtr typeClassPtr, int propertyIndex,
            IntPtr instance);

        [MonoPInvokeCallback(typeof(Callback_GetPropertyValue_Thickness))]
        private static IntPtr GetPropertyValue_Thickness(IntPtr typeClassPtr, int propertyIndex, IntPtr instance)
        {
            Type typeClass = GetTypeFromPtr(typeClassPtr);

            GCHandle handleInstance = GCHandle.FromIntPtr(instance);
            System.Object objectInstance = (System.Object)handleInstance.Target;

            PropertyInfo[] properties = GetPublicProperties(typeClass);
            PropertyInfo propertyInfo = properties[propertyIndex];

            Thickness thickness = (Thickness)propertyInfo.GetValue(objectInstance, null);
            return Thickness.getCPtr(thickness).Handle;
        }

        private delegate IntPtr Callback_GetPropertyValue_BaseComponent(IntPtr typeClassPtr, int propertyIndex,
            IntPtr instance);

        [MonoPInvokeCallback(typeof(Callback_GetPropertyValue_BaseComponent))]
        private static IntPtr GetPropertyValue_BaseComponent(IntPtr typeClassPtr, int propertyIndex, IntPtr instance)
        {
            Type typeClass = GetTypeFromPtr(typeClassPtr);

            GCHandle handleInstance = GCHandle.FromIntPtr(instance);
            System.Object objectInstance = (System.Object)handleInstance.Target;

            PropertyInfo[] properties = GetPublicProperties(typeClass);
            PropertyInfo propertyInfo = properties[propertyIndex];

            BaseComponent baseComponent = (BaseComponent)propertyInfo.GetValue(objectInstance, null);
            return BaseComponent.getCPtr(baseComponent).Handle;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////
        private delegate void Callback_SetPropertyValue_Bool(IntPtr typeClassPtr, int propertyIndex,
            IntPtr instance, [MarshalAs(UnmanagedType.U1)] bool val);

        [MonoPInvokeCallback(typeof(Callback_SetPropertyValue_Bool))]
        private static void SetPropertyValue_Bool(IntPtr typeClassPtr, int propertyIndex,
            IntPtr instance, [MarshalAs(UnmanagedType.U1)] bool val)
        {
            Type typeClass = GetTypeFromPtr(typeClassPtr);

            GCHandle handleInstance = GCHandle.FromIntPtr(instance);
              System.Object objectInstance = (System.Object)handleInstance.Target;
    
            PropertyInfo[] properties = GetPublicProperties(typeClass);
            PropertyInfo propertyInfo = properties[propertyIndex];

            propertyInfo.SetValue(objectInstance, val, null);
        }

        private delegate void Callback_SetPropertyValue_Float(IntPtr typeClassPtr, int propertyIndex,
            IntPtr instance, float val);

        [MonoPInvokeCallback(typeof(Callback_SetPropertyValue_Float))]
        private static void SetPropertyValue_Float(IntPtr typeClassPtr, int propertyIndex,
            IntPtr instance, float val)
        {
            Type typeClass = GetTypeFromPtr(typeClassPtr);

            GCHandle handleInstance = GCHandle.FromIntPtr(instance);
              System.Object objectInstance = (System.Object)handleInstance.Target;
    
            PropertyInfo[] properties = GetPublicProperties(typeClass);
            PropertyInfo propertyInfo = properties[propertyIndex];

            propertyInfo.SetValue(objectInstance, val, null);
        }

        private delegate void Callback_SetPropertyValue_Int(IntPtr typeClassPtr, int propertyIndex,
            IntPtr instance, int val);

        [MonoPInvokeCallback(typeof(Callback_SetPropertyValue_Int))]
        private static void SetPropertyValue_Int(IntPtr typeClassPtr, int propertyIndex,
            IntPtr instance, int val)
        {
            Type typeClass = GetTypeFromPtr(typeClassPtr);

            GCHandle handleInstance = GCHandle.FromIntPtr(instance);
              System.Object objectInstance = (System.Object)handleInstance.Target;
    
            PropertyInfo[] properties = GetPublicProperties(typeClass);
            PropertyInfo propertyInfo = properties[propertyIndex];

            propertyInfo.SetValue(objectInstance, val, null);
        }

        private delegate void Callback_SetPropertyValue_UInt(IntPtr typeClassPtr, int propertyIndex,
            IntPtr instance, uint val);

        [MonoPInvokeCallback(typeof(Callback_SetPropertyValue_UInt))]
        private static void SetPropertyValue_UInt(IntPtr typeClassPtr, int propertyIndex,
            IntPtr instance, uint val)
        {
            Type typeClass = GetTypeFromPtr(typeClassPtr);

            GCHandle handleInstance = GCHandle.FromIntPtr(instance);
              System.Object objectInstance = (System.Object)handleInstance.Target;
    
            PropertyInfo[] properties = GetPublicProperties(typeClass);
            PropertyInfo propertyInfo = properties[propertyIndex];

            propertyInfo.SetValue(objectInstance, val, null);
        }

        private delegate void Callback_SetPropertyValue_Short(IntPtr typeClassPtr, int propertyIndex,
            IntPtr instance, short val);

        [MonoPInvokeCallback(typeof(Callback_SetPropertyValue_Short))]
        private static void SetPropertyValue_Short(IntPtr typeClassPtr, int propertyIndex,
            IntPtr instance, short val)
        {
            Type typeClass = GetTypeFromPtr(typeClassPtr);

            GCHandle handleInstance = GCHandle.FromIntPtr(instance);
              System.Object objectInstance = (System.Object)handleInstance.Target;
    
            PropertyInfo[] properties = GetPublicProperties(typeClass);
            PropertyInfo propertyInfo = properties[propertyIndex];

            propertyInfo.SetValue(objectInstance, val, null);
        }

        private delegate void Callback_SetPropertyValue_UShort(IntPtr typeClassPtr, int propertyIndex,
            IntPtr instance, ushort val);

        [MonoPInvokeCallback(typeof(Callback_SetPropertyValue_UShort))]
        private static void SetPropertyValue_UShort(IntPtr typeClassPtr, int propertyIndex,
            IntPtr instance, ushort val)
        {
            Type typeClass = GetTypeFromPtr(typeClassPtr);

            GCHandle handleInstance = GCHandle.FromIntPtr(instance);
              System.Object objectInstance = (System.Object)handleInstance.Target;
    
            PropertyInfo[] properties = GetPublicProperties(typeClass);
            PropertyInfo propertyInfo = properties[propertyIndex];

            propertyInfo.SetValue(objectInstance, val, null);
        }

        private delegate void Callback_SetPropertyValue_String(IntPtr typeClassPtr, int propertyIndex,
            IntPtr instance, IntPtr val);

        [MonoPInvokeCallback(typeof(Callback_SetPropertyValue_String))]
        private static void SetPropertyValue_String(IntPtr typeClassPtr, int propertyIndex,
            IntPtr instance, IntPtr val)
        {
            Type typeClass = GetTypeFromPtr(typeClassPtr);

            GCHandle handleInstance = GCHandle.FromIntPtr(instance);
              System.Object objectInstance = (System.Object)handleInstance.Target;
    
            PropertyInfo[] properties = GetPublicProperties(typeClass);
            PropertyInfo propertyInfo = properties[propertyIndex];

            string valStr = Marshal.PtrToStringAnsi(val);
            propertyInfo.SetValue(objectInstance, valStr, null);
        }

        private delegate void Callback_SetPropertyValue_Color(IntPtr typeClassPtr, int propertyIndex,
            IntPtr instance, IntPtr val);

        [MonoPInvokeCallback(typeof(Callback_SetPropertyValue_Color))]
        private static void SetPropertyValue_Color(IntPtr typeClassPtr, int propertyIndex,
            IntPtr instance, IntPtr val)
        {
            Type typeClass = GetTypeFromPtr(typeClassPtr);

            GCHandle handleInstance = GCHandle.FromIntPtr(instance);
            System.Object objectInstance = (System.Object)handleInstance.Target;
    
            PropertyInfo[] properties = GetPublicProperties(typeClass);
            PropertyInfo propertyInfo = properties[propertyIndex];

            Color color = new Color(val, false);
            propertyInfo.SetValue(objectInstance, color, null);
        }

        private delegate void Callback_SetPropertyValue_Point(IntPtr typeClassPtr, int propertyIndex,
            IntPtr instance, IntPtr val);

        [MonoPInvokeCallback(typeof(Callback_SetPropertyValue_Point))]
        private static void SetPropertyValue_Point(IntPtr typeClassPtr, int propertyIndex,
            IntPtr instance, IntPtr val)
        {
            Type typeClass = GetTypeFromPtr(typeClassPtr);

            GCHandle handleInstance = GCHandle.FromIntPtr(instance);
            System.Object objectInstance = (System.Object)handleInstance.Target;
    
            PropertyInfo[] properties = GetPublicProperties(typeClass);
            PropertyInfo propertyInfo = properties[propertyIndex];

            Point point = new Point(val, false);
            propertyInfo.SetValue(objectInstance, point, null);
        }

        private delegate void Callback_SetPropertyValue_Rect(IntPtr typeClassPtr, int propertyIndex,
            IntPtr instance, IntPtr val);

        [MonoPInvokeCallback(typeof(Callback_SetPropertyValue_Rect))]
        private static void SetPropertyValue_Rect(IntPtr typeClassPtr, int propertyIndex,
            IntPtr instance, IntPtr val)
        {
            Type typeClass = GetTypeFromPtr(typeClassPtr);

            GCHandle handleInstance = GCHandle.FromIntPtr(instance);
            System.Object objectInstance = (System.Object)handleInstance.Target;
    
            PropertyInfo[] properties = GetPublicProperties(typeClass);
            PropertyInfo propertyInfo = properties[propertyIndex];

            Rect rect = new Rect(val, false);
            propertyInfo.SetValue(objectInstance, rect, null);
        }

        private delegate void Callback_SetPropertyValue_Size(IntPtr typeClassPtr, int propertyIndex,
            IntPtr instance, IntPtr val);

        [MonoPInvokeCallback(typeof(Callback_SetPropertyValue_Size))]
        private static void SetPropertyValue_Size(IntPtr typeClassPtr, int propertyIndex,
            IntPtr instance, IntPtr val)
        {
            Type typeClass = GetTypeFromPtr(typeClassPtr);

            GCHandle handleInstance = GCHandle.FromIntPtr(instance);
            System.Object objectInstance = (System.Object)handleInstance.Target;
    
            PropertyInfo[] properties = GetPublicProperties(typeClass);
            PropertyInfo propertyInfo = properties[propertyIndex];

            Size size = new Size(val, false);
            propertyInfo.SetValue(objectInstance, size, null);
        }

        private delegate void Callback_SetPropertyValue_Thickness(IntPtr typeClassPtr, int propertyIndex,
            IntPtr instance, IntPtr val);

        [MonoPInvokeCallback(typeof(Callback_SetPropertyValue_Thickness))]
        private static void SetPropertyValue_Thickness(IntPtr typeClassPtr, int propertyIndex,
            IntPtr instance, IntPtr val)
        {
            Type typeClass = GetTypeFromPtr(typeClassPtr);

            GCHandle handleInstance = GCHandle.FromIntPtr(instance);
            System.Object objectInstance = (System.Object)handleInstance.Target;
    
            PropertyInfo[] properties = GetPublicProperties(typeClass);
            PropertyInfo propertyInfo = properties[propertyIndex];

            Thickness thickness = new Thickness(val, false);
            propertyInfo.SetValue(objectInstance, thickness, null);
        }

        private delegate void Callback_SetPropertyValue_BaseComponent(IntPtr typeClassPtr, int propertyIndex,
            IntPtr instance, IntPtr val);

        [MonoPInvokeCallback(typeof(Callback_SetPropertyValue_BaseComponent))]
        private static void SetPropertyValue_BaseComponent(IntPtr typeClassPtr, int propertyIndex,
            IntPtr instance, IntPtr val)
        {
            Type typeClass = GetTypeFromPtr(typeClassPtr);

            GCHandle handleInstance = GCHandle.FromIntPtr(instance);
            System.Object objectInstance = (System.Object)handleInstance.Target;
    
            PropertyInfo[] properties = GetPublicProperties(typeClass);
            PropertyInfo propertyInfo = properties[propertyIndex];

            BaseComponent baseComponent = (BaseComponent)Activator.CreateInstance(propertyInfo.PropertyType,
                new object[] { val, false });
            propertyInfo.SetValue(objectInstance, baseComponent, null);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////
        private delegate IntPtr Callback_CreateInstance(IntPtr typeClassPtr, IntPtr cPtr);

        [MonoPInvokeCallback(typeof(Callback_CreateInstance))]
        private static IntPtr CreateInstance(IntPtr typeClassPtr, IntPtr cPtr)
        {
            Type typeClass = GetTypeFromPtr(typeClassPtr);
            
            object instance = Activator.CreateInstance(typeClass, new object[] { cPtr, false });
            
            GCHandle gcHandle = GCHandle.Alloc(instance, GCHandleType.Pinned);
            return GCHandle.ToIntPtr(gcHandle);
        }

        private delegate void Callback_DeleteInstance(IntPtr instance);

        [MonoPInvokeCallback(typeof(Callback_DeleteInstance))]
        private static void DeleteInstance(IntPtr instancePtr)
        {
            GCHandle gcHandle = GCHandle.FromIntPtr(instancePtr);
            gcHandle.Free();
        }
    }
}
