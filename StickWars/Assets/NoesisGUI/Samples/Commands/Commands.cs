using UnityEngine;
using System.Collections;
using System;
using System.Runtime.InteropServices;


namespace Noesis
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    public class DelegateCommand : Noesis.BaseCommand
    {
        private HandleRef swigCPtr;
        
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private readonly Action action;
        
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public static void Register()
        {
        }
        
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public DelegateCommand(IntPtr cPtr, bool cMemoryOwn)
            : base(cPtr, cMemoryOwn)
        {
            swigCPtr = new HandleRef(this, cPtr);
        }
        
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public DelegateCommand(Action action):
            this(Noesis.Extend.New(typeof(DelegateCommand)), true)
        {
            Noesis.Extend.Register(typeof(DelegateCommand), swigCPtr.Handle, this);
            
            this.action = action;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override void Dispose()
        {
            lock (this)
            {
                if (swigCPtr.Handle != IntPtr.Zero)
                {
                    if (swigCMemOwn)
                    {
                        swigCMemOwn = false;
                        if (Noesis.Kernel.IsInitialized())
                        {
                            Noesis.Extend.Delete(typeof(DelegateCommand), swigCPtr.Handle);
                        }
                    }
                    swigCPtr = new HandleRef(null, IntPtr.Zero);
                }
                GC.SuppressFinalize(this);
                base.Dispose();
            }
        }
        
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool CanExecuteCommand(BaseComponent parameter)
        {
            return true;
        }
        
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public void ExecuteCommand(BaseComponent parameter)
        {
            action();
        }
    }
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    public class ViewModel : SerializableComponent
    {
        private HandleRef swigCPtr;
        
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        string _input = string.Empty;
        public string Input 
        { 
            get
            {
                return _input;
            }
            set
            {
                _input = value; 
            }
        }
        
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private string _output = string.Empty;
        public string Output
        {
            get
            {
                return _output;
            }
            set
            {
                if (_output != value)
                {
                    _output = value;
                    Noesis.Extend.PropertyChanged(typeof(ViewModel), swigCPtr.Handle, "Output");
                }
            }
        }
        
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public DelegateCommand SayHelloCommand 
        { 
            get;
            private set;
        }
        
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public static void Register()
        {
        }
        
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public ViewModel(IntPtr cPtr, bool cMemoryOwn)
            : base(cPtr, cMemoryOwn)
        {
            swigCPtr = new HandleRef(this, cPtr);
            
            SayHelloCommand = new DelegateCommand(SayHello);
        }
        
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public ViewModel():
            this(Noesis.Extend.New(typeof(ViewModel)), true)
        {
            Noesis.Extend.Register(typeof(ViewModel), swigCPtr.Handle, this);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override void Dispose()
        {
            lock (this)
            {
                if (swigCPtr.Handle != IntPtr.Zero)
                {
                    if (swigCMemOwn)
                    {
                        swigCMemOwn = false;
                        if (Noesis.Kernel.IsInitialized())
                        {
                            Noesis.Extend.Delete(typeof(ViewModel), swigCPtr.Handle);
                        }
                    }
                    swigCPtr = new HandleRef(null, IntPtr.Zero);
                }
                GC.SuppressFinalize(this);
                base.Dispose();
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private void SayHello()
        {
            Output = "Hello, " + Input;
        }
    }
}
