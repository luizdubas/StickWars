using UnityEngine;
using System.Collections;
using System;
using System.Runtime.InteropServices;
using Noesis;

namespace Noesis
{
    
    public class ColorPicker : UserControl 
    {
        private HandleRef swigCPtr;
    
        public static DependencyProperty ColorProperty;
    
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public static void Register()
        {
            // Register the dependency properties
            ColorProperty = DependencyProperty.Register("Color", typeof(SolidColorBrush),
                typeof(ColorPicker), new PropertyMetadata(null));
            
            // Override the source to indicate the user control xaml
            UserControl.SourceProperty.OverrideMetadata(typeof(ColorPicker),
                new PropertyMetadata("Assets/NoesisGUI/Samples/Primitives/ColorPicker.xaml"));
        }
            
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public SolidColorBrush Color
        {
            get { return GetValue<SolidColorBrush>(ColorProperty); }
            set { SetValue<SolidColorBrush>(ColorProperty, value); }
        }
            
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public ColorPicker(IntPtr cPtr, bool cMemoryOwn)
            : base(cPtr, cMemoryOwn)
        {
            swigCPtr = new HandleRef(this, cPtr);
        }
    
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public ColorPicker()
            : this(Noesis.Extend.New(typeof(ColorPicker)), true)
        {
            Noesis.Extend.Register(typeof(ColorPicker), swigCPtr.Handle, this);
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
                            Noesis.Extend.Delete(typeof(ColorPicker), swigCPtr.Handle);
                        }
                    }
                    swigCPtr = new HandleRef(null, IntPtr.Zero);
                }
                GC.SuppressFinalize(this);
                base.Dispose();
            }
        }
            
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public void OnPostInit()
        {
            // Initialize the user control here
            
            // User control elemetns
            this._r = FindName<Slider>("R");
            this._g = FindName<Slider>("G");
            this._b = FindName<Slider>("B");
            this._a = FindName<Slider>("A");
            this._hsv = FindName<HSVControl>("HSVControl");
            
            // Register events
            this._r.ValueChanged += this.OnSliderValueChanged;
            this._g.ValueChanged += this.OnSliderValueChanged;
            this._b.ValueChanged += this.OnSliderValueChanged;
            this._a.ValueChanged += this.OnSliderValueChanged;
            this._hsv.HSVChanged += this.OnHSVChanged;
            
            // Initialize the data members
            this._isUpdatingColor = false;
            this._isUpdatingSliders = false;
            this._changingHSV = false;
        }
            
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public void DependencyPropertyChanged(DependencyProperty prop)
        {
            // Method invoked when a dependency property changes
            
            if (prop == ColorProperty)
            {
                SolidColorBrush newValue = GetValue<SolidColorBrush>(prop);
                OnColorChanged(newValue);
            }
        }
        
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private void OnSliderValueChanged(float oldValue, float newValue)
        {
            if (!this._isUpdatingSliders)
            {
                if (this.Color == null || this.Color.IsFrozen())
                {
                    this._isUpdatingColor = true;
                    this.Color = new SolidColorBrush();
                    this._isUpdatingColor = false;
                }
                
                UpdateColor(this._r.GetValue(), this._g.GetValue(), this._b.GetValue(), this._a.GetValue());
            }
        }
        
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private void OnColorChanged(SolidColorBrush newValue)
        {
            if (!this._isUpdatingColor)
            {
                UpdateSliders(newValue);
            }

            if (!this._changingHSV)
            {
                this._hsv.SetRGBA(newValue.GetColor());
            }
        }
        
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private void UpdateColor(float r, float g, float b, float a)
        {
            this._isUpdatingColor = true;
            this.Color.SetColor(new Noesis.Color(r / 255.0f, g / 255.0f, b / 255.0f, a / 255.0f));

            if (!this._changingHSV)
            {
                this._hsv.SetRGBA(this.Color.GetColor());
            }
            
            this._isUpdatingColor = false;
        }
        
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private void UpdateSliders(SolidColorBrush color)
        {
            this._isUpdatingSliders = true;
            Noesis.Color c = color.GetColor();
            this._r.SetValue(c.GetRedF() * 255.0f);
            this._g.SetValue (c.GetGreenF() * 255.0f);
            this._b.SetValue (c.GetBlueF() * 255.0f);    
            this._a.SetValue (c.GetAlphaF() * 255.0f);
            this._isUpdatingSliders = false;
        }
        
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        void OnHSVChanged()
        {
            this._changingHSV = true;
            this.Color.SetColor(this._hsv.GetRGBA());
            UpdateSliders(this.Color);
            this._changingHSV = false;
        }
            
        Slider _r;
        Slider _g;
        Slider _b;
        Slider _a;
            
        HSVControl _hsv;
            
        bool _isUpdatingColor;
        bool _isUpdatingSliders;
        bool _changingHSV;
    }
    
}
