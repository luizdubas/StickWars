using UnityEngine;
using System.Collections;
using System;
using System.Runtime.InteropServices;
using Noesis;


namespace Noesis
{
    
public static class ExtensionMethods 
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Clamp the specified val between min and max.
    /// </summary>
    /// <param name='val'>
    /// Value.
    /// </param>
    /// <param name='min'>
    /// Minimum.
    /// </param>
    /// <param name='max'>
    /// Max.
    /// </param>
    /// <typeparam name='T'>
    /// The 1st type parameter.
    /// </typeparam> 
    public static T Clamp<T>(this T val, T min, T max) where T : IComparable<T>
    {
        if (val.CompareTo(min) < 0)
        {
            return min;
        }
        else if(val.CompareTo(max) > 0)
        {
            return max;
        }
        else
        {
            return val;
        }
    }
}

////////////////////////////////////////////////////////////////////////////////////////////////////
/// <summary>
/// HSV control.
/// </summary>
public class HSVControl : UserControl
{
    private HandleRef swigCPtr;
        
    // Dependency properties
    //@{
    public static DependencyProperty AlphaProperty;
    public static DependencyProperty HueProperty;
    public static DependencyProperty SaturationProperty;
    public static DependencyProperty VProperty;
    //@}

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    public static void Register()
    {
        // Register the dependency properties
        AlphaProperty = DependencyProperty.Register("Alpha", typeof(float), typeof(HSVControl),
            new PropertyMetadata(1.0f));
        HueProperty = DependencyProperty.Register("Hue", typeof(float), typeof(HSVControl),
            new PropertyMetadata(180.0f));
        SaturationProperty = DependencyProperty.Register("Saturation", typeof(float), typeof(HSVControl),
            new PropertyMetadata(0.0f));
        VProperty = DependencyProperty.Register("V", typeof(float), typeof(HSVControl),
            new PropertyMetadata(0.0f));
        
        // Override the source to indicate the user control xaml
        UserControl.SourceProperty.OverrideMetadata(typeof(HSVControl),
            new PropertyMetadata("Assets/NoesisGUI/Samples/Primitives/HSVControl.xaml"));
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    public float Alpha
    {
        get { return GetValue<float>(AlphaProperty); }
        set { SetValue<float>(AlphaProperty, value); }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    public float Hue
    {
        get { return GetValue<float>(HueProperty); }
        set { SetValue<float>(HueProperty, value); }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    public float Saturation
    {
        get { return GetValue<float>(SaturationProperty); }
        set { SetValue<float>(SaturationProperty, value); }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    public float V
    {
        get { return GetValue<float>(VProperty); }
        set { SetValue<float>(VProperty, value); }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    public HSVControl(IntPtr cPtr, bool cMemoryOwn)
        : base(cPtr, cMemoryOwn)
    {
        swigCPtr = new HandleRef(this, cPtr);
        isSettingRGBA = false;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    public HSVControl()
        : this(Noesis.Extend.New(typeof(HSVControl)), true)
    {
        Noesis.Extend.Register(typeof(HSVControl), swigCPtr.Handle, this);
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
                        Noesis.Extend.Delete(typeof(HSVControl), swigCPtr.Handle);
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
        this.colorSpectrum = FindName<GradientStop>("ColorSpectrum");
        this.thumbTranslate = FindName<TranslateTransform>("ThumbTransform");
        this.spectrum = FindName<Slider>("Spectrum");
        this.svGrid = FindName<FrameworkElement>("SVGrid");
        
        // Register events
        this.svGrid.MouseLeftButtonDown += this.OnMouseLeftButtonDown;
        this.svGrid.MouseLeftButtonUp += this.OnMouseLeftButtonUp;
        this.svGrid.MouseMove += this.OnMouseMove;
        this.svGrid.SizeChanged += this.OnSizeChanged;
        this.spectrum.ValueChanged += this.OnSliderValueChange;
        
        // Initialize the data members
        this.movingSV = false;
        this.updatingSlider = false;
        this.isSettingRGBA = false;
    }
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    public void DependencyPropertyChanged(DependencyProperty prop)
    {
        // Method invoked when a dependency property changes
            
        if (prop == HueProperty)
        {
            float newValue = GetValue<float>(prop);
            colorSpectrum.SetColor(HSVToColor(newValue, 1, 1));

            if (!updatingSlider)
            {
                spectrum.SetValue(newValue);
            }
        }
        else if (prop == SaturationProperty)
        {
            if (!movingSV)
            {
                Noesis.Size size = this.GetRenderSize();
                SetThumbPosition(new Noesis.Point(V * size.width, 
                    size.height - (GetValue<float>(prop) * size.height)));
            }
        }
        else if (prop == VProperty)
        {
            if (!movingSV)
            {
                Noesis.Size size = this.GetRenderSize();
                SetThumbPosition(new Noesis.Point(GetValue<float>(prop) * size.width, 
                    size.height - (Saturation * size.height)));
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    private static void ColorToHSV(Noesis.Color color, ref float h, ref float s, ref float v)
    {
        float red = color.GetRedF();
        float green = color.GetGreenF();
        float blue  = color.GetBlueF();
        float min = Math.Min(red, Math.Min(green, blue));
        float max = Math.Max(red, Math.Max(green, blue));

        v = max;
        float delta = max - min;

        if (v == 0)
        {
            s = 0;
        }
        else
        {
            s = delta / max;
        }

        if (s == 0)
        {
            h = 0;
        }
        else
        {
            if (red == max)
            {
                h = (green - blue) / delta;
            }
            else if (green == max)
            {
                h = 2 + (blue - red) / delta;
            }
            else // blue == max
            {
                h = 4 + (red - green) / delta;
            }
        }
        h *= 60;
        if (h < 0)
        {
            h += 360;
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    private Noesis.Color HSVToColor(float hue, float saturation, float value)
    {
        float chroma = value * saturation;

        if (hue == 360)
        {
            hue = 0;
        }

        float hueTag = hue / 60;
        float x = chroma * (1 - Math.Abs(hueTag % 2.0f - 1));
        float m = value - chroma;
        switch ((int)hueTag)
        {
            case 0:
            {
                return new Noesis.Color(
                    (int)((chroma + m) * 255 + 0.5), 
                    (int)((x + m) * 255 + 0.5), 
                    (int)(m * 255 + 0.5),
                    255);
            }
            case 1:
            {
                return new Noesis.Color(
                    (int)((x + m) * 255 + 0.5), 
                    (int)((chroma + m) * 255 + 0.5), 
                    (int)(m * 255 + 0.5),
                    255);
            }
            case 2:
            {
                return new Noesis.Color(
                    (int)(m * 255 + 0.5), 
                    (int)((chroma + m) * 255 + 0.5), 
                    (int)((x + m) * 255 + 0.5),
                    255);
            }
            case 3:
            {
                return new Noesis.Color(
                    (int)(m * 255 + 0.5), 
                    (int)((x + m) * 255 + 0.5), 
                    (int)((chroma + m) * 255 + 0.5),
                    255);
            }
            case 4:
            {
                return new Noesis.Color(
                    (int)((x + m) * 255 + 0.5), 
                    (int)(m * 255 + 0.5), 
                    (int)((chroma + m) * 255 + 0.5),
                    255);
            }
            default:
            {
                return new Noesis.Color(
                    (int)((chroma + m) * 255 + 0.5), 
                    (int)(m * 255 + 0.5), 
                    (int)((x + m) * 255 + 0.5),
                    255);
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    public void SetRGBA(Noesis.Color color)
    {
        isSettingRGBA = true;
        float h = 0;
        float s = 0;
        float v = 0;

        ColorToHSV(color, ref h, ref s, ref v);

        Hue = h;
        Saturation = s;
        V = v;
        Alpha = color.GetAlphaF();
        HSVChanged();
        isSettingRGBA = false;
    }
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    private void OnMouseLeftButtonDown(BaseComponent c, MouseButtonEventArgs args)
    {
        Focus();
        svGrid.CaptureMouse();
        
        movingSV = true;
        
        Noesis.Point ctrlPos = svGrid.PointFromScreen(args.position);
        
        this.SetThumbPosition(ctrlPos);
    }
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    private void OnMouseLeftButtonUp(BaseComponent c, MouseButtonEventArgs  args)
    {
        movingSV = false;
        
        svGrid.ReleaseMouseCapture();
    }
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    private void OnMouseMove(BaseComponent c, MouseEventArgs args)
    {
        if (movingSV)
        {
            Noesis.Point ctrlPos = svGrid.PointFromScreen(args.position);
            
            this.SetThumbPosition(ctrlPos);
        }
    }
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    private void OnSizeChanged(BaseComponent c, SizeChangedEventArgs args)
    {
        if (!movingSV)
        {
            Noesis.Size size = args.sizeChangedInfo.newSize;
            this.SetThumbPosition(new Noesis.Point(this.V * size.width, this.Saturation * size.height));
        }
    }
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    private void OnSliderValueChange(float oldValue, float newValue)
    {
        this.updatingSlider = true;
        Hue = newValue;
        
        if (!isSettingRGBA)
        {
            HSVChanged();
        }
        
        updatingSlider = false;
    }
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    private void SetThumbPosition(Noesis.Point pos)
    {
        Noesis.Size size = svGrid.GetRenderSize();
        
        float xPos = ExtensionMethods.Clamp<float>(pos.x, 0.0f, size.width);
        float yPos = ExtensionMethods.Clamp<float>(pos.y, 0.0f, size.height);
        
        float halfWidth = size.width * 0.5f;
        float halfHeight = size.height * 0.5f;
        
        thumbTranslate.SetX (xPos - halfWidth);
        thumbTranslate.SetY (yPos - halfHeight);
        
        if (movingSV)
        {
            V = xPos / size.width;
            Saturation = (size.height - yPos) / size.height;
            HSVChanged();
        }
    }
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    public Noesis.Color GetRGBA()
    {
        Noesis.Color color = HSVToColor(Hue, Saturation, V);
        color.SetAlpha(Alpha);

        return color;
    }

    public delegate void HSVChangedDelegate();
    public event HSVChangedDelegate HSVChanged;
    
    private GradientStop colorSpectrum;
    private TranslateTransform thumbTranslate;
    private Slider spectrum;
    private FrameworkElement svGrid;
    private bool movingSV;
    private bool updatingSlider;
    private bool isSettingRGBA;
}
    
}
